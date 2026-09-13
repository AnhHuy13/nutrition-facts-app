using Python.Runtime;
using System;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace NutritionFactsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializePython();

            this.Width = 960;
            this.Height = 580;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
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
            // Xử lý sự kiện click cho nút quản lý_FACTS
        }

        private void createFactsBtnNav_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click cho nút tạo_FACTS
        }
    }
}
