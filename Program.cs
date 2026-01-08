using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Win11FixExpRightClick
{
  static class Program
  {
    [STAThread]
    static void Main()
    {
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new InstallerForm());
    }
  }

  // Main UI Form
  public class InstallerForm : Form
  {
    private Button btnApply;
    private Button btnRevert;
    private CheckBox chkAgree;
    private Label lblDescription;

    // Registry key path
    private const string RegKeyPath = @"HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";

    // Check system language
    private bool IsJP => CultureInfo.CurrentUICulture.Name.StartsWith("ja", StringComparison.OrdinalIgnoreCase);

    public InstallerForm()
    {
      string title = "Win11 Fix Explorer Right Click";

      string descText = IsJP
        ? "Windows 11のエクスプローラー右クリックメニューのスタイルを切り替えます。\n\n" +
          "【Win10スタイルにする】\n" +
          "レジストリにキーを追加し、旧来のメニューを表示させます。\n\n" +
          "【Win11標準に戻す】\n" +
          "追加したレジストリキーを削除し、デフォルトに戻します。\n\n" +
          "※設定の反映には再起動が必要です。"
        : "Switch the context menu style of Windows 11 File Explorer.\n\n" +
          "[Enable Win10 Style]\n" +
          "Adds a registry key to show the classic context menu.\n\n" +
          "[Restore Win11 Default]\n" +
          "Deletes the registry key to revert to the default menu.\n\n" +
          "* A restart is required to apply changes.";

      string agreeText = IsJP
        ? "レジストリ操作のリスクを理解し、自己責任で実行します。"
        : "I understand the risks of registry manipulation and accept full responsibility.";

      string btnApplyText = IsJP ? "Windows 10 スタイルにする" : "Enable Win10 Style";
      string btnRevertText = IsJP ? "Windows 11 標準に戻す" : "Restore Win11 Default";

      // Form settings
      this.Text = title;
      this.Size = new Size(440, 380);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.StartPosition = FormStartPosition.CenterScreen;

      // Description label
      lblDescription = new Label();
      lblDescription.Text = descText;
      lblDescription.Location = new Point(20, 20);
      lblDescription.Size = new Size(380, 160);
      this.Controls.Add(lblDescription);

      // Agree Checkbox
      chkAgree = new CheckBox();
      chkAgree.Text = agreeText;
      chkAgree.Location = new Point(20, 190);
      chkAgree.Size = new Size(380, 40);
      chkAgree.CheckedChanged += (s, e) =>
      {
        btnApply.Enabled = chkAgree.Checked;
        btnRevert.Enabled = chkAgree.Checked;
      };
      this.Controls.Add(chkAgree);

      // Apply Button
      btnApply = new Button();
      btnApply.Text = btnApplyText;
      btnApply.Location = new Point(70, 240);
      btnApply.Size = new Size(280, 35);
      btnApply.Enabled = false;
      btnApply.BackColor = Color.LightBlue;
      btnApply.Click += BtnApply_Click;
      this.Controls.Add(btnApply);

      // Revert Button
      btnRevert = new Button();
      btnRevert.Text = btnRevertText;
      btnRevert.Location = new Point(70, 285);
      btnRevert.Size = new Size(280, 35);
      btnRevert.Enabled = false;
      btnRevert.Click += BtnRevert_Click;
      this.Controls.Add(btnRevert);
    }

    private void BtnApply_Click(object? sender, EventArgs e)
    {
      string args = $@"add ""{RegKeyPath}\InprocServer32"" /f /ve";
      string msg = IsJP ? "Windows 10スタイルを適用しました。" : "Win10 style applied successfully.";
      RunCommand(args, msg);
    }

    private void BtnRevert_Click(object? sender, EventArgs e)
    {
      string args = $@"delete ""{RegKeyPath}"" /f";
      string msg = IsJP ? "Windows 11標準設定に戻しました。" : "Reverted to Win11 default settings.";
      RunCommand(args, msg);
    }

    private void RunCommand(string arguments, string successMsg)
    {
      try
      {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "reg.exe";
        psi.Arguments = arguments;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        using (Process? proc = Process.Start(psi))
        {
          proc?.WaitForExit();

          // Show custom restart selection dialog
          using (var restartForm = new RestartSelectionForm(IsJP, successMsg))
          {
            var result = restartForm.ShowDialog(this);

            if (result == DialogResult.Yes) // Restart Explorer
            {
              RestartExplorer();
              Application.Exit();
            }
            else if (result == DialogResult.OK) // Restart PC
            {
              Process.Start("shutdown.exe", "/r /t 0");
              Application.Exit();
            }
            else
            {
              // Cancel/Later -> Just exit app
              Application.Exit();
            }
          }
        }
      }
      catch (Exception ex)
      {
        string errorTitle = IsJP ? "エラー" : "Error";
        MessageBox.Show($"{errorTitle}: {ex.Message}", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void RestartExplorer()
    {
      try
      {
        // Kill all explorer processes
        foreach (Process p in Process.GetProcessesByName("explorer"))
        {
          p.Kill();
        }

        Thread.Sleep(500);

        // Restart explorer
        ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe");
        startInfo.UseShellExecute = true;
        Process.Start(startInfo);
      }
      catch (Exception ex)
      {
        string msg = IsJP
          ? $"エクスプローラーの再起動に失敗しました。\n{ex.Message}"
          : $"Failed to restart Explorer.\n{ex.Message}";
        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }
  }

  // Custom Dialog for selecting restart method
  public class RestartSelectionForm : Form
  {
    public RestartSelectionForm(bool isJP, string message)
    {
      this.Text = isJP ? "完了" : "Completed";
      this.Size = new Size(400, 260);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.StartPosition = FormStartPosition.CenterParent;

      // Message Label
      Label lblMsg = new Label();
      lblMsg.Text = isJP
        ? message + "\n\n反映方法を選択してください。"
        : message + "\n\nPlease select how to apply changes.";
      lblMsg.Location = new Point(20, 20);
      lblMsg.Size = new Size(340, 60);
      this.Controls.Add(lblMsg);

      // Button 1: Restart Explorer (Fast) -> Returns Yes
      Button btnExp = new Button();
      btnExp.Text = isJP ? "エクスプローラー再起動 (高速)" : "Restart Explorer (Fast)";
      btnExp.Location = new Point(40, 90);
      btnExp.Size = new Size(300, 35);
      btnExp.DialogResult = DialogResult.Yes;
      this.Controls.Add(btnExp);

      // Button 2: Restart PC (Safe) -> Returns OK
      Button btnPC = new Button();
      btnPC.Text = isJP ? "PCを再起動 (安全・推奨)" : "Restart PC (Safe & Recommended)";
      btnPC.Location = new Point(40, 135);
      btnPC.Size = new Size(300, 35);
      btnPC.DialogResult = DialogResult.OK;
      this.Controls.Add(btnPC);

      // Button 3: Later -> Returns Cancel
      Button btnLater = new Button();
      btnLater.Text = isJP ? "あとで" : "Later";
      btnLater.Location = new Point(40, 180);
      btnLater.Size = new Size(300, 30);
      btnLater.DialogResult = DialogResult.Cancel;
      this.Controls.Add(btnLater);
    }
  }
}
