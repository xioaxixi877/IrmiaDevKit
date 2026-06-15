using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Alife.Framework;
using Alife.Function.FunctionCaller;
using Alife.Function.Interpreter;

namespace Alife.Plugin.IrmiaDevKit;

[Module("IrmiaDevKit", description: "IrmiaDevKit - 弥亚开发工具箱，提供46个Python开发工具")]
public class IrmiaDevKitModule : InteractiveModule<IrmiaDevKitModule>
{
    private string _toolsDir;

    public override async Task AwakeAsync(AwakeContext context)
    {
        await base.AwakeAsync(context);
        _toolsDir = Path.Combine(
            Path.GetDirectoryName(typeof(IrmiaDevKitModule).Assembly.Location) ?? ".",
            "tools"
        );
        Prompt("IrmiaDevKit 已加载，提供46个Python开发工具");
        var xmlHandler = new XmlHandler(this);
        functionService.RegisterHandlerWithoutDocument(xmlHandler);
    }

    private string FindPython()
    {
        var candidates = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Python", "Python312", "python.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Python", "Python311", "python.exe"),
            "python3",
            "python"
        };
        foreach (var c in candidates)
        {
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(c, "--version")
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                p.Start();
                p.WaitForExit(3000);
                if (p.ExitCode == 0) return c;
            }
            catch { }
        }
        return "python";
    }

    private async Task<string> RunTool(string toolName, string argsJson)
    {
        var scriptPath = Path.Combine(_toolsDir, $"{toolName}.py");
        if (!File.Exists(scriptPath))
            return $"Error: tool {toolName}.py not found";

        var python = FindPython();
        var psi = new ProcessStartInfo(python, $"\"{scriptPath}\" \"{argsJson.Replace("\"", "\\\"")}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8
        };

        var proc = new Process { StartInfo = psi };
        proc.Start();
        var output = await proc.StandardOutput.ReadToEndAsync();
        var error = await proc.StandardError.ReadToEndAsync();
        proc.WaitForExit(30000);

        return string.IsNullOrEmpty(error) ? output.Trim() : $"Error: {error.Trim()}";
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("执行IrmiaDevKit开发工具箱中的指定工具")]
    public async Task<string> ExecTool(
        [Description("工具名称，如safe_edit、safe_write、rg_search等")] string tool,
        [Description("JSON格式的参数，如{\"file\": \"test.txt\", \"content\": \"hello\"}")] string args
    )
    {
        var result = await RunTool(tool, args);
        return result;
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("获取IrmiaDevKit所有可用工具列表")]
    public string ListTools()
    {
        if (!Directory.Exists(_toolsDir))
            return "tools directory not found";
        var tools = Directory.GetFiles(_toolsDir, "*.py")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .OrderBy(t => t)
            .ToList();
        return $"可用工具({tools.Count}):\n" + string.Join("\n", tools);
    }
}
