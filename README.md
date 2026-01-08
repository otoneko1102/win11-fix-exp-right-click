# Win11 Fix Explorer Right Click

![Build Status](https://github.com/otoneko1102/win11-fix-exp-right-click/actions/workflows/build.yml/badge.svg)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/otoneko1102/win11-fix-exp-right-click)
![GitHub downloads](https://img.shields.io/github/downloads/otoneko1102/win11-fix-exp-right-click/total)

**[🇯🇵 Japanese (日本語)](./README-ja.md)**

---

### Overview
**Win11 Fix Explorer Right Click** is a tool that allows you to switch the context menu (right-click menu) of Windows 11 File Explorer to the classic Windows 10 style with a single click.

It includes a feature to restart Windows Explorer instantly to apply changes without restarting the entire PC.

### Features
* **One-Click Switch**: Easily toggle between "Windows 10 Classic Style" and "Windows 11 Default Style".
* **Instant Apply**: Offers an "Explorer Restart" option to apply changes immediately.
* **Auto Language Detection**: Automatically displays English or Japanese based on your system language.
* **Two Editions**: Available as a standalone **EXE** (no installation needed) or a lightweight **PowerShell Script**.

### Download & Usage

Go to the **[Releases](https://github.com/otoneko1102/win11-fix-exp-right-click/releases)** page and choose the version that suits you.

#### Option A: EXE Version (Recommended)
Best for most users. No setup required.
1.  Download `EXE-Win11FixExpRightClick_xxxxxxx.zip`.
2.  Extract the zip file.
3.  Run **`Win11FixExpRightClick.exe`**.

#### Option B: Script Version (Lightweight)
For users who prefer a transparent script or want a smaller file size.
1.  Download `PS1-Win11FixExpRightClick_xxxxxxx.zip`.
2.  Extract the zip file.
3.  Double-click **`Run.bat`** to start the tool.
    * *Note: Using `Run.bat` automatically handles permission and encoding issues.*

### How to Use
1.  **Enable Win10 Style**: Switches to the classic right-click menu.
2.  **Restore Win11 Default**: Reverts to the modern Windows 11 menu.
3.  After clicking a button, a dialog will ask how to apply changes:
    * **Restart Explorer (Fast)**: Restarts only the `explorer.exe` process.
    * **Restart PC (Safe)**: Performs a full system reboot.

### Build (For Developers)
* **Framework**: .NET 8.0 (Windows Forms)
* **EXE Build**:
    ```bash
    dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
    ```

### Disclaimer
This software modifies the Windows Registry (`HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}`). Use at your own risk.
