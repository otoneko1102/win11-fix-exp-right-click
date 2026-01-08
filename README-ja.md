# Win11 Fix Explorer Right Click

![Build Status](https://github.com/otoneko1102/win11-fix-exp-right-click/actions/workflows/build.yml/badge.svg)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/otoneko1102/win11-fix-exp-right-click)
![GitHub downloads](https://img.shields.io/github/downloads/otoneko1102/win11-fix-exp-right-click/total)

**[🇺🇸 English](./README.md)**

---

### 概要
**Win11 Fix Explorer Right Click** は、Windows 11のエクスプローラーの右クリックメニューを、ワンクリックでWindows 10以前の「クラシックスタイル」に戻すことができるツールです。

PC全体を再起動することなく、エクスプローラーのみを再起動して設定を即座に反映させる機能を搭載しています。

### 特徴
* **ワンクリック切替**: 「Windows 10スタイル」と「Windows 11標準スタイル」を簡単に切り替え可能。
* **高速反映**: 「エクスプローラーの再起動」機能により、PCの再起動を待たずに設定を反映できます。
* **自動言語判定**: 日本語・英語を自動で切り替えます。
* **2つのエディション**: 面倒な設定不要の **EXE版** と、軽量な **PowerShellスクリプト版** を用意しています。

### ダウンロードと使い方

**[Releases](https://github.com/otoneko1102/win11-fix-exp-right-click/releases)** ページから、お好みのバージョンをダウンロードしてください。

#### A. EXE版（推奨）
設定不要ですぐに使いたい方向けです。
1.  `EXE-Win11FixExpRightClick_xxxxxxx.zip` をダウンロードします。
2.  ファイルを解凍（展開）します。
3.  **`Win11FixExpRightClick.exe`** を実行してください。

#### B. スクリプト版（軽量）
中身を確認したい方や、ファイルサイズを小さく抑えたい方向けです。
1.  `PS1-Win11FixExpRightClick_xxxxxxx.zip` をダウンロードします。
2.  ファイルを解凍（展開）します。
3.  フォルダ内の **`Run.bat`** をダブルクリックして実行してください。
    * *※同梱の `Run.bat` を使うことで、セキュリティ制限や文字化けを回避して起動できます。*

### 操作方法
1.  **Windows 10 スタイルにする**: クラシックな右クリックメニューを有効にします。
2.  **Windows 11 標準に戻す**: Windows 11のモダンなメニューに戻します。
3.  ボタンを押すと、反映方法を選択するダイアログが表示されます。
    * **エクスプローラー再起動 (高速)**: `explorer.exe` のみを再起動します（作業中のフォルダは閉じられます）。
    * **PCを再起動 (安全・推奨)**: PC全体を再起動します。

### 開発者向け情報
* **フレームワーク**: .NET 8.0 (Windows Forms)
* **ビルドコマンド**:
    ```bash
    dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
    ```

### 免責事項
本ソフトウェアはWindowsのレジストリ (`HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}`) を変更します。利用は自己責任で行ってください。本ツールを使用したことによるシステムの不具合や損害について、作者は一切の責任を負いません。
