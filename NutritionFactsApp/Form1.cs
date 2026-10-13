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
        public static Task PythonInitTask { get; private set; }

        public Form1()
        {
            InitializeComponent();
            PythonInitTask = Task.Run(() => InitializePython());

            this.Width = 1000;
            this.Height = 600;

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
            // BỎ Task.Run ra để kiểm tra xem nó có bị lỗi ngay lúc mở App không
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string pythonDir = Path.Combine(baseDir, "python-3.8");
                string dllPath = Path.Combine(pythonDir, "python38.dll");

                // THÔNG BÁO CỤ THỂ NẾU SAI ĐƯỜNG DẪN
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
                // Hiển thị lỗi ra MessageBox để bạn biết ngay tại sao không chạy
                MessageBox.Show("Lỗi khởi tạo Python: " + ex.ToString(), "Error");
            }
        }
        private void manageFactsBtnNav_Click(object sender, EventArgs e)
        {
            SwitchPageToCreatePage(false);
        }

        private void createFactsBtnNav_Click(object sender, EventArgs e)
        {
            SwitchPageToCreatePage(true);
        }

        private void toNewDictToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = SaveImage.SelectFolder();
            if (!string.IsNullOrEmpty(path))
            {
                SaveImage.SaveOrUpdateRegistryKey(path);
                MessageBox.Show($"Đã cập nhật thư mục mặc định thành:\n{path}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
