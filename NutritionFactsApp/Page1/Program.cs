using System;
using System.Windows.Forms;
using AutoUpdaterDotNET;

namespace NutritionFactsApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Form1 MainForm { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Tự động kiểm tra cập nhật (Thay đường dẫn URL đến file update.xml trên GitHub của bạn)
            AutoUpdater.Start("https://raw.githubusercontent.com/username/NutritionFactsApp/main/update.xml");

            MainForm = new Form1();
            Application.Run(MainForm);
        }
    }
}
