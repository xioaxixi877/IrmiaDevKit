using Alife.Framework;
using Alife.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading;

namespace Alife.Plugin.AlifePluginIrmiaDevKit;

[Module("IrmiaDevKit", description: "IrmiaDevKit - 47个Python开发工具，ExecTool+ListTools双接口")]
public class IrmiaDevKitModule : InteractiveModule<IrmiaDevKitModule>
{
    public override async Task AwakeAsync(AwakeContext context)
    {
        var handler = new XmlHandler(this);
        handler.RegisterHandlerWithoutDocument("exec", ExecTool);
        handler.RegisterHandlerWithoutDocument("list", ListTools);
        Logger.RegisterModule(this);
    }

    string FindPython()
    {
        var paths = new[] { "python3", "python" };
        foreach (var p in paths)
        {
            try
            {
                var psi = new ProcessStartInfo(p, "--version");
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;
                using var proc = Process.Start(psi);
                if (proc != null) return p;
            }
            catch { }
        }
        return "python";
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("执行一个Python工具: args=[工具名] [参数...]")]
    public string ExecTool(string args)
    {
        var parts = args.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "用法: exec 工具名 [参数]";
        var name = parts[0];
        var toolArgs = parts.Length > 1 ? parts[1] : "";

        var toolsDir = Path.Combine(AlifeDirectory.StorageDirectory, "Skills", "irmia_devkit_open", "tools");
        if (!Directory.Exists(toolsDir))
        {
            toolsDir = Path.Combine(Environment.CurrentDirectory, "tools");
            if (!Directory.Exists(toolsDir)) return "tools目录不存在";
        }

        var script = Path.Combine(toolsDir, name + ".py");
        if (!File.Exists(script)) return "工具 " + name + ".py 不存在";

        var py = FindPython();
        var psi = new ProcessStartInfo(py, "\"" + script + "\" " + toolArgs);
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.CreateNoWindow = true;
        psi.StandardOutputEncoding = Encoding.UTF8;
        psi.StandardErrorEncoding = Encoding.UTF8;
        using var proc = Process.Start(psi);
        if (proc == null) return "启动失败";
        proc.WaitForExit(60000);
        var output = proc.StandardOutput.ReadToEnd().Trim();
        var error = proc.StandardError.ReadToEnd().Trim();
        if (!string.IsNullOrEmpty(output)) return output;
        return error;
    }

    [XmlFunction(FunctionMode.OneShot)]
    [Description("列出所有可用的Python工具")]
    public string ListTools(string args)
    {
        var toolsDir = Path.Combine(AlifeDirectory.StorageDirectory, "Skills", "irmia_devkit_open", "tools");
        if (!Directory.Exists(toolsDir))
        {
            toolsDir = Path.Combine(Environment.CurrentDirectory, "tools");
            if (!Directory.Exists(toolsDir)) return "tools目录不存在";
        }
        var tools = Directory.GetFiles(toolsDir, "*.py")
            .Select(Path.GetFileNameWithoutExtension)
            .OrderBy(x => x)
            .ToList();
        return "可用工具(" + tools.Count + "个): " + string.Join(", ", tools);
    }
}
