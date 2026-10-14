using Python.Runtime;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializePython();
            userControl1.UpdatePathLabel();

            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new System.Drawing.Icon(iconPath);
                }
            }
            catch { }

            this.Width = userControl1.Width + 25;
            this.Height = userControl1.Height + 75;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void SwitchPageToCreatePage(bool isCreatePage)
        {
            if (isCreatePage)
            {
                userControl1.BringToFront();
            }
            else
            {
                // userControl2.BringToFront();
            }
        }

        private void InitializePython()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string pythonDir = Path.Combine(baseDir, "python-3.8");
                string dllPath = Path.Combine(pythonDir, "python38.dll");

                if (!File.Exists(dllPath))
                {
                    MessageBox.Show("Không tìm thấy file DLL tại: " + dllPath, "Lỗi Python");
                    return;
                }

                string path = Environment.GetEnvironmentVariable("PATH");
                Environment.SetEnvironmentVariable("PATH", pythonDir + ";" + path);

                Runtime.PythonDLL = dllPath;
                PythonEngine.PythonHome = pythonDir;
                PythonEngine.Initialize();

                System.Diagnostics.Debug.WriteLine("PYTHON KHỞI TẠO XONG!");

                PythonEngine.BeginAllowThreads();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo Python: " + ex.ToString(), "Error");
            }
        }
        private void createFactsBtnNav_Click(object sender, EventArgs e)
        {
            SwitchPageToCreatePage(true);
        }

        private void manageFactsBtnNav_Click(object sender, EventArgs e)
        {
            SwitchPageToCreatePage(false);
        }

        private void toNewDictToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = SaveImage.SelectFolder();
            if (!string.IsNullOrEmpty(path))
            {
                SaveImage.SaveOrUpdateRegistryKey(path);
                userControl1.UpdatePathLabel();
                MessageBox.Show($"Đã cập nhật thư mục mặc định thành:\n{path}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
