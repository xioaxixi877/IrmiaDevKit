# IrmiaDevKit

将弥亚开发工具箱（irmia_devkit_open）和KiraAI GitHub工具移植为Alife框架的插件版本。

## 用途

本插件为Alife桌面助手提供46个Python工具和完整的GitHub API操作能力，涵盖以下功能：

- **文件操作**：编辑、删除、哈希、比较、补丁、压缩解压
- **搜索工具**：正则搜索、ES搜索、文本过滤
- **代码工具**：语法检查、代码图、符号重命名、依赖扫描、lint运行
- **Git工具**：Git操作、版本日志
- **数据库工具**：数据库查询
- **HTTP工具**：HTTP请求、文件下载、HTML提取
- **系统工具**：进程列表、端口检查、系统快照、磁盘信息
- **数据处理**：JSON查询、CSV工具、diff、时间工具、UUID生成
- **开发工具**：项目初始化、配置文件diff、日志解析
- **GitHub工具**：搜索仓库/代码/用户、获取文件/issue/PR、创建仓库/文件/issue/PR/分支、star仓库

所有工具通过XmlFunctionCaller注册为Alife框架可调用的功能，无需手动编写脚本。

## 鸣谢

- [BDFFZI](https://github.com/BDFFZI) — Alife框架
- [irmia2026](https://github.com/irmia2026) — irmia_devkit_open原项目
