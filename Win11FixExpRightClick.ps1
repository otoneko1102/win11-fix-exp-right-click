<#
.SYNOPSIS
  Windows 11 Fix Explorer Right Click (GUI Version)
.DESCRIPTION
  Switches the Windows 11 context menu to the classic Windows 10 style using a GUI.
  Equivalent to the EXE version.
#>

# --- 1. Admin Privilege Check ---
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
  try {
    Start-Process powershell -Verb RunAs -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""
    exit
  }
  catch {
    Write-Warning "Administrator privileges are required."
    exit
  }
}

# --- 2. Load Assemblies ---
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

# --- 3. Language Detection ---
$isJP = [System.Globalization.CultureInfo]::CurrentUICulture.Name.StartsWith("ja")

# --- 4. Logic Functions ---

function Restart-Explorer {
  try {
    # Kill explorer
    Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
    # Start explorer
    Start-Process explorer.exe
  }
  catch {
    $msg = if ($isJP) { "エクスプローラーの再起動に失敗しました。`n$($_.Exception.Message)" } else { "Failed to restart Explorer.`n$($_.Exception.Message)" }
    [System.Windows.Forms.MessageBox]::Show($msg, "Error", 'OK', 'Warning')
  }
}

function Show-RestartSelectionDialog {
  param($parentForm, $successMsg)

  $dialog = New-Object System.Windows.Forms.Form
  $dialog.Text = if ($isJP) { "完了" } else { "Completed" }
  $dialog.Size = New-Object System.Drawing.Size(400, 280)
  $dialog.FormBorderStyle = 'FixedDialog'
  $dialog.MaximizeBox = $false
  $dialog.StartPosition = 'CenterParent'

  $lbl = New-Object System.Windows.Forms.Label
  $lbl.Text = if ($isJP) { "$successMsg`n`n反映方法を選択してください。" } else { "$successMsg`n`nPlease select how to apply changes." }
  $lbl.Location = New-Object System.Drawing.Point(20, 20)
  $lbl.Size = New-Object System.Drawing.Size(340, 60)
  $dialog.Controls.Add($lbl)

  # Button 1: Restart Explorer
  $btnExp = New-Object System.Windows.Forms.Button
  $btnExp.Text = if ($isJP) { "エクスプローラー再起動 (高速)" } else { "Restart Explorer (Fast)" }
  $btnExp.Location = New-Object System.Drawing.Point(40, 90)
  $btnExp.Size = New-Object System.Drawing.Size(300, 35)
  $btnExp.DialogResult = 'Yes'
  $dialog.Controls.Add($btnExp)

  # Button 2: Restart PC
  $btnPC = New-Object System.Windows.Forms.Button
  $btnPC.Text = if ($isJP) { "PCを再起動 (安全・推奨)" } else { "Restart PC (Safe & Recommended)" }
  $btnPC.Location = New-Object System.Drawing.Point(40, 135)
  $btnPC.Size = New-Object System.Drawing.Size(300, 35)
  $btnPC.DialogResult = 'OK'
  $dialog.Controls.Add($btnPC)

  # Button 3: Later
  $btnLater = New-Object System.Windows.Forms.Button
  $btnLater.Text = if ($isJP) { "あとで" } else { "Later" }
  $btnLater.Location = New-Object System.Drawing.Point(40, 180)
  $btnLater.Size = New-Object System.Drawing.Size(300, 30)
  $btnLater.DialogResult = 'Cancel'
  $dialog.Controls.Add($btnLater)

  $result = $dialog.ShowDialog($parentForm)

  if ($result -eq 'Yes') {
    Restart-Explorer
    $parentForm.Close()
  }
  elseif ($result -eq 'OK') {
    Start-Process "shutdown.exe" -ArgumentList "/r /t 0"
    $parentForm.Close()
  }
  else {
    $parentForm.Close()
  }
  $dialog.Dispose()
}

function Run-RegistryCommand {
  param($regArgs, $msg)
  try {
    $p = Start-Process reg.exe -ArgumentList $regArgs -NoNewWindow -Wait -PassThru
    if ($p.ExitCode -eq 0) {
      Show-RestartSelectionDialog -parentForm $mainForm -successMsg $msg
    }
  }
  catch {
    [System.Windows.Forms.MessageBox]::Show("Error: $($_.Exception.Message)", "Error", 'OK', 'Error')
  }
}

# --- 5. Build Main GUI ---

$mainForm = New-Object System.Windows.Forms.Form
$title = if ($isJP) { "Win11 右クリックメニュー切替" } else { "Win11 Fix Explorer Right Click" }
$mainForm.Text = $title
$mainForm.Size = New-Object System.Drawing.Size(440, 380)
$mainForm.FormBorderStyle = 'FixedDialog'
$mainForm.MaximizeBox = $false
$mainForm.StartPosition = 'CenterScreen'

# Description
$descText = if ($isJP) {
  "Windows 11のエクスプローラー右クリックメニューのスタイルを切り替えます。`n`n" +
  "【Win10スタイルにする】`n" +
  "レジストリにキーを追加し、旧来のメニューを表示させます。`n`n" +
  "【Win11標準に戻す】`n" +
  "追加したレジストリキーを削除し、デフォルトに戻します。`n`n" +
  "※設定の反映には再起動が必要です。"
} else {
  "Switch the context menu style of Windows 11 File Explorer.`n`n" +
  "[Enable Win10 Style]`n" +
  "Adds a registry key to show the classic context menu.`n`n" +
  "[Restore Win11 Default]`n" +
  "Deletes the registry key to revert to the default menu.`n`n" +
  "* A restart is required to apply changes."
}
$lblDesc = New-Object System.Windows.Forms.Label
$lblDesc.Text = $descText
$lblDesc.Location = New-Object System.Drawing.Point(20, 20)
$lblDesc.Size = New-Object System.Drawing.Size(380, 160)
$mainForm.Controls.Add($lblDesc)

# Checkbox
$agreeText = if ($isJP) { "レジストリ操作のリスクを理解し、自己責任で実行します。" } else { "I understand the risks of registry manipulation and accept full responsibility." }
$chkAgree = New-Object System.Windows.Forms.CheckBox
$chkAgree.Text = $agreeText
$chkAgree.Location = New-Object System.Drawing.Point(20, 190)
$chkAgree.Size = New-Object System.Drawing.Size(380, 40)
$mainForm.Controls.Add($chkAgree)

# Apply Button
$btnApply = New-Object System.Windows.Forms.Button
$btnApply.Text = if ($isJP) { "Windows 10 スタイルにする" } else { "Enable Win10 Style" }
$btnApply.Location = New-Object System.Drawing.Point(70, 240)
$btnApply.Size = New-Object System.Drawing.Size(280, 35)
$btnApply.Enabled = $false
$btnApply.BackColor = [System.Drawing.Color]::LightBlue
$mainForm.Controls.Add($btnApply)

# Revert Button
$btnRevert = New-Object System.Windows.Forms.Button
$btnRevert.Text = if ($isJP) { "Windows 11 標準に戻す" } else { "Restore Win11 Default" }
$btnRevert.Location = New-Object System.Drawing.Point(70, 285)
$btnRevert.Size = New-Object System.Drawing.Size(280, 35)
$btnRevert.Enabled = $false
$mainForm.Controls.Add($btnRevert)

# Events
$chkAgree.Add_CheckedChanged({
  $btnApply.Enabled = $chkAgree.Checked
  $btnRevert.Enabled = $chkAgree.Checked
})

$RegKeyPath = "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}"

$btnApply.Add_Click({
  $a = "add `"$RegKeyPath\InprocServer32`" /f /ve"
  $m = if ($isJP) { "Windows 10スタイルを適用しました。" } else { "Win10 style applied successfully." }
  Run-RegistryCommand -regArgs $a -msg $m
})

$btnRevert.Add_Click({
  $a = "delete `"$RegKeyPath`" /f"
  $m = if ($isJP) { "Windows 11標準設定に戻しました。" } else { "Reverted to Win11 default settings." }
  Run-RegistryCommand -regArgs $a -msg $m
})

# Show
[void]$mainForm.ShowDialog()
