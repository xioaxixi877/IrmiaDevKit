using Alife.Framework;
using Alife.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Alife.Plugin.AlifePluginIrmiaDevKit;

[Module("AlifePluginIrmiaDevKit", description: "IrmiaDevKit - 47个Python开发工具")]
public class AlifePluginIrmiaDevKitModule : InteractiveModule<AlifePluginIrmiaDevKitModule>
{
    private static readonly string toolsDir = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Plugins", "Alife.Plugin.IrmiaDevKit", "tools");

    private string FindPython()
    {
        foreach (var path in new[] { "python", "python3", "C:\Python312\python.exe", "C:\Users\Ha1hai\AppData\Local\Programs\Python\Python312\python.exe" })
        {
            try
            {
                using var proc = Process.Start(new ProcessStartInfo(path, "--version") { RedirectStandardOutput = true, CreateNoWindow = true });
                if (proc != null) { proc.WaitForExit(2000); if (proc.ExitCode == 0) return path; }
            }
            catch { }
        }
        return "python";
    }

    private string RunTool(string name, string args)
    {
        var py = FindPython();
        var script = Path.Combine(toolsDir, name + ".py");
        if (!File.Exists(script)) return $"工具 {name}.py 不存在";
        var psi = new ProcessStartInfo(py, $""{script}" {args}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8
        };
        using var proc = Process.Start(psi);
        proc.WaitForExit(60000);
        var output = proc.StandardOutput.ReadToEnd().Trim();
        var error = proc.StandardError.ReadToEnd().Trim();
        return string.IsNullOrEmpty(output) ? error : output;
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("执行IrmiaDevKit工具，参数: toolName=工具名称 toolArgs=传给py的参数")]
    public string ExecTool(string toolName, string toolArgs = "")
    {
        try
        {
            if (!Directory.Exists(toolsDir)) return $"tools目录不存在: {toolsDir}";
            return RunTool(toolName, toolArgs);
        }
        catch (Exception ex) { return $"执行失败: {ex.Message}"; }
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("列出所有可用工具(47个Python脚本)")]
    public string ListTools()
    {
        try
        {
            if (!Directory.Exists(toolsDir)) return $"tools目录不存在: {toolsDir}";
            var tools = Directory.GetFiles(toolsDir, "*.py").Select(Path.GetFileNameWithoutExtension).OrderBy(x => x).ToList();
            return $"共 {tools.Count} 个工具:
{string.Join("
", tools)}";
        }
        catch (Exception ex) { return $"获取工具列表失败: {ex.Message}"; }
    }
}
