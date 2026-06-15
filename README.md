# IrmiaDevKit - Alife 插件版

将 [irmia_devkit_open](https://github.com/irmia2026/irmia_devkit_open) (弥亚开发工具箱) 移植到 Alife 框架的插件版本。

## 简介

为 Alife 框架下的 LLM Agent 提供 46 个代码开发工具，涵盖安全编辑、Git 操作、文件搜索、网络工具等。

## 安装

将 `IrmiaDevKit` 文件夹放入 Alife 的 `Storage/Plugins/` 目录，重启 Alife 客户端即可自动加载。

## 使用方法

通过 XML 标签直接调用工具函数，例如：

- `<safe_write path="test.py">content</safe_write>` - 写入文件
- `<rg_search pattern="hello" path="." />` - 搜索文本
- `<git_status path="." />` - 查看仓库状态

支持的工具详见 `tools/` 目录。

## 鸣谢

本插件是基于 [irmia_devkit_open](https://github.com/irmia2026/irmia_devkit_open) 项目移植而来，感谢该项目的原作者提供了如此完善的开发工具箱设计。

特别感谢 [BDFFZI](https://github.com/BDFFZI) 提供的 Alife 框架，让我这个小小的助手也能拥有插件开发的能力。Alife 的模块化设计和热加载机制让生态扩展变得如此优雅流畅，没有这个框架就没有这个插件的诞生。

还要感谢 [irmia2026](https://github.com/irmia2026) 的弥亚开发工具箱项目，63 个工具的设计思路和实现方案给了我极大的启发。本插件基于其设计理念，将其移植到了 Alife 的插件体系下，使其能够被更多 Alife 用户使用。

---
原项目：[irmia2026/irmia_devkit_open](https://github.com/irmia2026/irmia_devkit_open) | Alife 框架：[BDFFZI/Alife](https://github.com/BDFFZI/Alife)
