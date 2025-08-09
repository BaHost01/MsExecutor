using CloudyApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VelocityAPI;
namespace MsExecutor
{
    public partial class MsExecutor : Form
    {
        public MsExecutor()
        {
            InitializeComponent();
        }



        private readonly VelAPI Velocity = new VelAPI();

        private void Attachwithapi()
        {
            var proc = Process.GetProcessesByName("RobloxPlayerBeta").FirstOrDefault();
            if (proc == null) { }
            bool Cloudy = File.ReadAllText("settings.txt").Contains("Cloudy = True");
            bool Vel = File.ReadAllText("settings.txt").Contains("Velocity = True");
            if (Cloudy)
            {
                if (CloudyApi.Api.misc.isRobloxOpen())
                {
                    LogToConsole("Roblox Has been Detected", LogType.INFO);
                    if (!CloudyApi.Api.External.IsInjected()) { CloudyApi.Api.External.inject(); LogToConsole("Injecting Into Roblox!", LogType.Success); }
                    else { LogToConsole("Already Injected!", LogType.Warning); }
                }
                else
                {
                    LogToConsole("Roblox not detected!", LogType.ERROR);
                }
            }
            if (Vel)
            {

                if (proc != null)
                {

                    LogToConsole("Roblox Has been Detected", LogType.INFO);
                    if (!Velocity.IsAttached(proc.Id)) { Velocity.Attach(proc.Id); LogToConsole("Injecting Into Roblox!", LogType.Success); }
                    else { LogToConsole("Already Injected!", LogType.Warning); }
                }
            }
        }
        private void Minimize()
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void Exit()
        {
            System.Windows.Forms.Application.Exit();
        }
        private async Task CleaWhiteitor()
        {
            await webView21.ExecuteScriptAsync("SetText('')");
        }

        private async Task SaveTextAsLua()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save File";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (webView21.CoreWebView2 != null)
                    {
                        var result = await webView21.ExecuteScriptAsync("GetText();");
                        if (!string.IsNullOrEmpty(result) && result.Length > 1)
                        {
                            string text = result;
                            if (text.StartsWith("\"") && text.EndsWith("\""))
                            {
                                text = text.Substring(1, text.Length - 2);
                                text = text.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
                            }
                            File.WriteAllText(saveFileDialog.FileName, text);
                        }
                    }
                }
            }
        }
        private async void LoadMonaco()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.Navigate(System.Windows.Forms.Application.StartupPath + "\\bin\\MonacoWithTabs\\monaco.html");
        }

        private async Task LoadTextFromFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";
                openFileDialog.Title = "Open File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileContent = File.ReadAllText(openFileDialog.FileName);
                    if (webView21.CoreWebView2 != null)
                    {
                        string jsSafe = fileContent.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", "").Replace("\n", "\\n");
                        await webView21.ExecuteScriptAsync($"SetText('{jsSafe}');");
                    }
                }
            }
        }


        private async Task Velocityexecute()
        {
            if (webView21.CoreWebView2 != null)
            {
                var result = await webView21.ExecuteScriptAsync("GetText();");
                if (!string.IsNullOrEmpty(result) && result.Length > 1)
                {
                    string script = result;
                    if (script.StartsWith("\"") && script.EndsWith("\""))
                    {
                        script = script.Substring(1, script.Length - 2);
                        script = script.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
                    }

                    Velocity.Execute(script);
                }

            }
        }
        private async Task CloudyExecute()
        {
            if (webView21.CoreWebView2 != null)
            {
                var result = await webView21.ExecuteScriptAsync("GetText();");
                if (!string.IsNullOrEmpty(result) && result.Length > 1)
                {
                    string script = result;
                    if (script.StartsWith("\"") && script.EndsWith("\""))
                    {
                        script = script.Substring(1, script.Length - 2);
                        script = script.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
                    }

                    CloudyApi.Api.External.execute(script);
                }
            }


        }
        private async void Execute()
        {
            var proc = Process.GetProcessesByName("RobloxPlayerBeta").FirstOrDefault();
            if (proc == null) { }
            bool Cloudy = File.ReadAllText("settings.txt").Contains("Cloudy = True");
            bool Vel = File.ReadAllText("settings.txt").Contains("Velocity = True");
            if (Vel)
            {
                if (proc != null)
                {

                    LogToConsole("Roblox Has been Detected", LogType.INFO);
                    if (!Velocity.IsAttached(proc.Id)) { await Velocityexecute(); }
                    else { LogToConsole("Already Injected!", LogType.ERROR); }
                }
                else
                {
                    LogToConsole("Roblox not detected!", LogType.ERROR);
                }
            }
            if (Cloudy)
            {
                if (CloudyApi.Api.misc.isRobloxOpen())
                {
                    LogToConsole("Roblox Has been Detected", LogType.INFO);
                    if (CloudyApi.Api.External.IsInjected()) { await CloudyExecute(); }
                    else { LogToConsole("Already Injected!", LogType.ERROR); }
                }
                else
                {
                    LogToConsole("Roblox not detected!", LogType.ERROR);
                }
            }
        }

        private async void Startup()
        {
            Api.misc.disableSecurity();
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.Navigate(System.Windows.Forms.Application.StartupPath + "\\bin\\MonacoWithTabs\\monaco.html");
            if (!File.Exists("settings.txt"))
            {
                MessageBox.Show("Creating Settings File");
                using (File.Create("settings.txt")) { }
                File.WriteAllText("settings.txt", "Velocity = True, Cloudy = False");
            }
            LoadMonaco();

            try
            {
                string Exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string dir = Path.GetDirectoryName(Exe);
                var files = Directory.GetFiles(dir);
                string Executorn = Path.GetFileName(Exe);
                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);
                    if (filename.Equals(Executorn, StringComparison.OrdinalIgnoreCase))
                        continue;
                    else if (filename.Equals("currentVersion.txt", StringComparison.OrdinalIgnoreCase))
                        continue;
                    else if (filename.Equals("settings.txt", StringComparison.OrdinalIgnoreCase))
                        continue;
                    try
                    {
                        File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.Hidden);
                    }
                    catch { MessageBox.Show("Error on File configuration"); }
                }
            }
            catch { MessageBox.Show("Error on fetching files"); }
        }
        public enum LogType
        {
            INFO,
            ERROR,
            Warning,
            Success
        }

        public static class ConsoleLogger
        {
            public static Color GetLogColor(LogType type)
            {
                if (type == LogType.INFO)
                    return Color.White;
                if (type == LogType.ERROR)
                    return Color.Red;
                if (type == LogType.Warning)
                    return Color.Yellow;
                if (type == LogType.Success)
                    return Color.Green;

                return Color.White;
            }
        }

        private void LogToConsole(string message, LogType type)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            Color logColor = ConsoleLogger.GetLogColor(type);

            consoleBox.SelectionStart = consoleBox.TextLength;
            consoleBox.SelectionLength = 0;


            consoleBox.SelectionColor = logColor;
            consoleBox.AppendText($"[ {type} ]");


            consoleBox.SelectionColor = Color.White;
            consoleBox.AppendText($" [{timestamp}] ");


            consoleBox.SelectionColor = logColor;
            consoleBox.AppendText($"{message}\n");

            consoleBox.ScrollToCaret();
        }
        private void vlcheck()
        {
            var proc = Process.GetProcessesByName("RobloxPlayerBeta").FirstOrDefault();
            if (proc == null) { }
            if (proc != null)
            {
                if (Velocity.IsAttached(proc.Id)) { VelocityLabel.Text = "Velocity: Injected!"; VelocityLabel.ForeColor = Color.Lime; }
                else { VelocityLabel.Text = "Velocity: Not Injected!"; VelocityLabel.ForeColor = Color.Red; }
            }
            else { VelocityLabel.Text = "Velocity: Not Injected!"; VelocityLabel.ForeColor = Color.Red; }
        }
        private void Ccheck()
        {
            if (CloudyApi.Api.External.IsInjected()) { CloudyLabel.Text = "Cloudy: Injected!"; CloudyLabel.ForeColor = Color.Lime; }
            else { CloudyLabel.Text = "Cloudy: Not Injected!"; CloudyLabel.ForeColor = Color.Red; }
        }
        
        private void MsExecutor_Load(object sender, EventArgs e)
        {
            Startup();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Execute_Click(object sender, EventArgs e)
        {
            Execute();
        }

        private void Attach_Click(object sender, EventArgs e)
        {
            Attachwithapi();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {

            Minimize();
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void VelocitySwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (VelocitySwitch.Checked)
            {
                CloudySwitch.Checked = false;
                File.WriteAllText("settings.txt", "Cloudy = False, Velocity = True");

            }
        }

        private void CloudySwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (CloudySwitch.Checked)
            {
                VelocitySwitch.Checked = false;
                File.WriteAllText("settings.txt", "Cloudy = True, Velocity = False");

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Ccheck();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            vlcheck();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
