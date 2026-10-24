# AppDataCleaner

**AppDataCleaner** 是一个简易的工具，用于扫描和清理 Windows 系统中 `C:\Users\[Your Username]\AppData` 文件夹内的应用程序数据。众所周知，各大应用喜欢在 C 盘拉屎，卸载了数据也不会消失，本工具帮助你消灭垃圾。

## 主要功能

- **灵活扫描**：可选择 `Local`、`LocalLow`、`Roaming`、`ProgramData` 中的一个或多个目录。
- **空间统计**：实时展示扫描项目数、总占用空间以及当前选中项的大小。
- **搜索与排序**：按路径、大小或备注实时筛选，点击表头即可切换排序方式。
- **安全清理**：支持移至回收站或永久删除，批量操作前会确认，并在结束后汇总失败项。
- **文件备注**：使用 `appname.json` 保存各文件夹的说明（内置 400+ 条常见目录说明），可在界面中直接编辑；留空保存即可清除备注。
- **快捷操作**：
  - `F5` 开始扫描，`Esc` 取消扫描
  - `Ctrl+F` 搜索，`Ctrl+O` 打开位置，`Ctrl+C` 复制路径
  - `Delete` 移至回收站，`Shift+Delete` 永久删除，`Enter` 打开选中项
- **自动更新检测**：启动时自动检查 GitHub Releases，发现新版本会在标题栏提示，点击即可打开下载页面；也可点击版本号手动检查。
- **现代界面**：浅色主题、圆角卡片、自绘按钮与勾选列表，布局清晰易读。

> 建议以管理员身份运行，以便扫描和清理更多受保护目录。运行日志位于 `%LOCALAPPDATA%\AppDataCleaner\cleaner.log`。

## 使用方法

点击 [这里](https://github.com/Gwenep/AppDataCleaner/releases) 访问发布页面并下载最新版本。

下载后解压，双击 `AppDataCleaner.exe` 运行。

![app](https://raw.githubusercontent.com/Gwenep/AppDataCleaner/main/img/app.png)

## 贡献

欢迎任何形式的贡献！你可以通过提交 issues 或 pull requests 来帮助我们改进 AppDataCleaner。
