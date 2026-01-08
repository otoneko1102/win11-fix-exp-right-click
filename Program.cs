using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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

  public class InstallerForm : Form
  {
    private Button btnApply;
    private Button btnRevert;
    private CheckBox chkAgree;
    private Label lblDescription;

    // registry key
    private const string RegKeyPath = @"HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";

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
          "※設定の反映にはPCの再起動が必要です。"
        : "Switch the context menu style of Windows 11 File Explorer.\n\n" +
          "[Enable Win10 Style]\n" +
          "Adds a registry key to show the classic context menu.\n\n" +
          "[Restore Win11 Default]\n" +
          "Deletes the registry key to revert to the default menu.\n\n" +
          "* A system restart is required to apply changes.";

      string agreeText = IsJP
        ? "レジストリ操作のリスクを理解し、自己責任で実行します。"
        : "I understand the risks of registry manipulation and accept full responsibility.";

      string btnApplyText = IsJP ? "Windows 10 スタイルにする" : "Enable Win10 Style";
      string btnRevertText = IsJP ? "Windows 11 標準に戻す" : "Restore Win11 Default";

      // Form
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

      // Check box
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

      // Apply
      btnApply = new Button();
      btnApply.Text = btnApplyText;
      btnApply.Location = new Point(70, 240);
      btnApply.Size = new Size(280, 35);
      btnApply.Enabled = false;
      btnApply.BackColor = Color.LightBlue;
      btnApply.Click += BtnApply_Click;
      this.Controls.Add(btnApply);

      // Back
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

          string restartMsg = IsJP
            ? $"{successMsg}\n反映するには再起動が必要です。\n\n今すぐ再起動しますか？"
            : $"{successMsg}\nA restart is required to apply changes.\n\nRestart now?";

          string title = IsJP ? "完了" : "Completed";

          DialogResult result = MessageBox.Show(
            restartMsg,
            title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

          if (result == DialogResult.Yes)
          {
            Process.Start("shutdown.exe", "/r /t 0");
          }

          Application.Exit();
        }
      }
      catch (Exception ex)
      {
        string errorTitle = IsJP ? "エラー" : "Error";
        MessageBox.Show($"{errorTitle}: {ex.Message}", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
