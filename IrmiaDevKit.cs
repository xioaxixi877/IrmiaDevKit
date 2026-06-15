using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Alife.Platform;

namespace IrmiaDevKit
{
    [Module("IrmiaDevKit", description: "IrmiaDevKit - 弥亚开发工具箱，提供46个Python开发工具")]
    public class IrmiaDevKitModule : InteractiveModule<IrmiaDevKitModule>
    {
        private readonly Dictionary<string, Func<JObject, Task<string>>> _tools = new();
        private string _toolsDir;
        private string _pythonExe = "python";

        public override async Task AwakeAsync()
        {
            _toolsDir = Path.Combine(GetPluginDirectory(), "tools");
            Directory.CreateDirectory(_toolsDir);
            InitializeTools();
            RegisterXmlFunctions();
        }

        private void InitializeTools()
        {
            RegisterTool("safe_edit", SafeEdit);
            RegisterTool("safe_write", SafeWrite);
            RegisterTool("rg_search", RgSearch);
            RegisterTool("shell_exec", ShellExec);
            RegisterTool("git_smart", GitSmart);
            RegisterTool("file_remove", FileRemove);
            RegisterTool("file_zip", FileZip);
            RegisterTool("dir_tree", DirTree);
            RegisterTool("dir_list", DirList);
            RegisterTool("disk_info", DiskInfo);
            RegisterTool("syntax_check", SyntaxCheck);
            RegisterTool("file_diff", FileDiff);
            RegisterTool("file_hash", FileHash);
            RegisterTool("file_patch", FilePatch);
            RegisterTool("multi_edit", MultiEdit);
            RegisterTool("symbol_rename", SymbolRename);
            RegisterTool("codegraph", CodeGraph);
            RegisterTool("dep_scan", DepScan);
            RegisterTool("http_get", HttpGet);
            RegisterTool("http_download", HttpDownload);
            RegisterTool("db_query", DbQuery);
            RegisterTool("es_search", EsSearch);
            RegisterTool("json_query", JsonQuery);
            RegisterTool("csv_utils", CsvUtils);
            RegisterTool("text_filter", TextFilter);
            RegisterTool("html_extract", HtmlExtract);
            RegisterTool("md_strip", MdStrip);
            RegisterTool("encode_utils", EncodeUtils);
            RegisterTool("time_utils", TimeUtils);
            RegisterTool("semver", Semver);
            RegisterTool("uuid_gen", UuidGen);
            RegisterTool("port_check", PortCheck);
            RegisterTool("proc_list", ProcList);
            RegisterTool("sys_snapshot", SysSnapshot);
            RegisterTool("config_diff", ConfigDiff);
            RegisterTool("diff_strings", DiffStrings);
            RegisterTool("lint_runner", LintRunner);
            RegisterTool("test_runner", TestRunner);
            RegisterTool("project_init", ProjectInit);
            RegisterTool("git_changelog", GitChangelog);
            RegisterTool("gh_cli", GhCli);
            RegisterTool("op_log", OpLog);
            RegisterTool("tool_stats", ToolStats);
            RegisterTool("safe_query", SafeQuery);
        }

        private void RegisterTool(string name, Func<JObject, Task<string>> handler)
        {
            _tools[name] = handler;
        }

        private void RegisterXmlFunctions()
        {
            foreach (var kv in _tools)
            {
                RegisterTool(kv.Key, kv.Value);
            }
        }

        private string GetPluginDirectory()
        {
            return Path.GetDirectoryName(new StackTrace(true).GetFrame(0).GetFileName()) ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        private string RunPythonTool(string toolName, string argsJson)
        {
            var scriptPath = Path.Combine(_toolsDir, $"{toolName}.py");
            var psi = new ProcessStartInfo
            {
                FileName = _pythonExe,
                Arguments = $"\"{scriptPath}\" \"{argsJson.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Timeout = 30000
            };
            using var proc = Process.Start(psi);
            proc.WaitForExit(30000);
            var stdout = proc.StandardOutput.ReadToEnd();
            var stderr = proc.StandardError.ReadToEnd();
            if (proc.ExitCode != 0) throw new Exception($"Tool {toolName} failed: {stderr}");
            return stdout;
        }

        private async Task<string> SafeEdit(JObject args) => RunPythonTool("safe_edit", args.ToString());
        private async Task<string> SafeWrite(JObject args) => RunPythonTool("safe_write", args.ToString());
        private async Task<string> RgSearch(JObject args) => RunPythonTool("rg_search", args.ToString());
        private async Task<string> ShellExec(JObject args) => RunPythonTool("shell_exec", args.ToString());
        private async Task<string> GitSmart(JObject args) => RunPythonTool("git_smart", args.ToString());
        private async Task<string> FileRemove(JObject args) => RunPythonTool("file_remove", args.ToString());
        private async Task<string> FileZip(JObject args) => RunPythonTool("file_zip", args.ToString());
        private async Task<string> DirTree(JObject args) => RunPythonTool("dir_tree", args.ToString());
        private async Task<string> DirList(JObject args) => RunPythonTool("dir_list", args.ToString());
        private async Task<string> DiskInfo(JObject args) => RunPythonTool("disk_info", args.ToString());
        private async Task<string> SyntaxCheck(JObject args) => RunPythonTool("syntax_check", args.ToString());
        private async Task<string> FileDiff(JObject args) => RunPythonTool("file_diff", args.ToString());
        private async Task<string> FileHash(JObject args) => RunPythonTool("file_hash", args.ToString());
        private async Task<string> FilePatch(JObject args) => RunPythonTool("file_patch", args.ToString());
        private async Task<string> MultiEdit(JObject args) => RunPythonTool("multi_edit", args.ToString());
        private async Task<string> SymbolRename(JObject args) => RunPythonTool("symbol_rename", args.ToString());
        private async Task<string> CodeGraph(JObject args) => RunPythonTool("codegraph", args.ToString());
        private async Task<string> DepScan(JObject args) => RunPythonTool("dep_scan", args.ToString());
        private async Task<string> HttpGet(JObject args) => RunPythonTool("http_get", args.ToString());
        private async Task<string> HttpDownload(JObject args) => RunPythonTool("http_download", args.ToString());
        private async Task<string> DbQuery(JObject args) => RunPythonTool("db_query", args.ToString());
        private async Task<string> EsSearch(JObject args) => RunPythonTool("es_search", args.ToString());
        private async Task<string> JsonQuery(JObject args) => RunPythonTool("json_query", args.ToString());
        private async Task<string> CsvUtils(JObject args) => RunPythonTool("csv_utils", args.ToString());
        private async Task<string> TextFilter(JObject args) => RunPythonTool("text_filter", args.ToString());
        private async Task<string> HtmlExtract(JObject args) => RunPythonTool("html_extract", args.ToString());
        private async Task<string> MdStrip(JObject args) => RunPythonTool("md_strip", args.ToString());
        private async Task<string> EncodeUtils(JObject args) => RunPythonTool("encode_utils", args.ToString());
        private async Task<string> TimeUtils(JObject args) => RunPythonTool("time_utils", args.ToString());
        private async Task<string> Semver(JObject args) => RunPythonTool("semver", args.ToString());
        private async Task<string> UuidGen(JObject args) => RunPythonTool("uuid_gen", args.ToString());
        private async Task<string> PortCheck(JObject args) => RunPythonTool("port_check", args.ToString());
        private async Task<string> ProcList(JObject args) => RunPythonTool("proc_list", args.ToString());
        private async Task<string> SysSnapshot(JObject args) => RunPythonTool("sys_snapshot", args.ToString());
        private async Task<string> ConfigDiff(JObject args) => RunPythonTool("config_diff", args.ToString());
        private async Task<string> DiffStrings(JObject args) => RunPythonTool("diff_strings", args.ToString());
        private async Task<string> LintRunner(JObject args) => RunPythonTool("lint_runner", args.ToString());
        private async Task<string> TestRunner(JObject args) => RunPythonTool("test_runner", args.ToString());
        private async Task<string> ProjectInit(JObject args) => RunPythonTool("project_init", args.ToString());
        private async Task<string> GitChangelog(JObject args) => RunPythonTool("git_changelog", args.ToString());
        private async Task<string> GhCli(JObject args) => RunPythonTool("gh_cli", args.ToString());
        private async Task<string> OpLog(JObject args) => RunPythonTool("op_log", args.ToString());
        private async Task<string> ToolStats(JObject args) => RunPythonTool("tool_stats", args.ToString());
        private async Task<string> SafeQuery(JObject args) => RunPythonTool("db_query", args.ToString());
    }
}
