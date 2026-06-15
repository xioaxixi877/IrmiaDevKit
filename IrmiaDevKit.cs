using Alife.Framework.Core;
using Alife.Framework.Handler;
using Alife.Shared.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Alife.Plugin.AlifePluginIrmiaDevKit;

[Module("AlifePluginIrmiaDevKit", description: "Alife.Plugin.IrmiaDevKit - 47个Python开发工具")]
public sealed class AlifePluginIrmiaDevKitModule : InteractiveModule<AlifePluginIrmiaDevKitModule>
{
    private static readonly string toolsDir = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Alife.Plugin.IrmiaDevKit", "tools"
    );
    private readonly JsonSerializerOptions _jsonOpts = new() { WriteIndented = false };

    private string FindPython()
    {
        foreach (var candidate in new[] { "python3", "python" })
        {
            try
            {
                var p = Process.Start(new ProcessStartInfo(candidate, "--version")
                {
                    RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
                });
                p?.WaitForExit(2000);
                if (p?.ExitCode == 0) return candidate;
            }
            catch { }
        }
        return "python";
    }

    private string RunTool(string toolName, string args)
    {
        var py = FindPython();
        var script = Path.Combine(toolsDir, toolName + ".py");
        if (!File.Exists(script)) return "{\"error\":\"tool " + toolName + " not found\"}";
        var psi = new ProcessStartInfo(py, "\"" + script + "\" " + args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8
        };
        using var proc = Process.Start(psi);
        var stdout = proc?.StandardOutput.ReadToEnd() ?? "";
        var stderr = proc?.StandardError.ReadToEnd() ?? "";
        proc?.WaitForExit(15000);
        var output = string.IsNullOrEmpty(stderr) ? stdout : stdout + "\\nSTDERR: " + stderr;
        return string.IsNullOrEmpty(output) ? "{}" : output;
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("List all available dev tools")]
    public async Task ListTools(XmlContext ctx)
    {
        if (!Directory.Exists(toolsDir))
        {
            await ctx.OutputAsync("{\"tools\":[],\"error\":\"tools dir not found:" + toolsDir + "\"}");
            return;
        }
        var tools = Directory.GetFiles(toolsDir, "*.py")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .OrderBy(t => t)
            .ToList();
        var result = JsonSerializer.Serialize(new { tools, count = tools.Count }, _jsonOpts);
        await ctx.OutputAsync(result);
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("Execute a dev tool with given arguments")]
    public async Task ExecTool(XmlContext ctx,
        [Description("tool name without .py")] string tool,
        [Description("arguments passed to the tool")] string args = "")
    {
        var result = RunTool(tool, args);
        await ctx.OutputAsync(result);
    }
}
