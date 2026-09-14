using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public static class Ultility
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        private const int EM_SETCUEBANNER = 0x1501;






        public static void SetStatusBarText(string text)
        {
            Form1 mainForm = Program.MainForm;

            if (mainForm != null)
            {
                mainForm.toolStripStatusLabel.Text = text;

                Timer timer = new Timer();
                timer.Interval = 3000;

                timer.Tick += (s, args) =>
                {
                    mainForm.toolStripStatusLabel.Text = "";
                    timer.Stop();
                    timer.Dispose();
                };

                timer.Start();
            }
        }

        public static string RemoveUnicode(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ"};

            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y"};

            StringBuilder sb = new StringBuilder(text);

            for (int i = 0; i < arr1.Length; i++)
            {
                sb.Replace(arr1[i], arr2[i]);
                sb.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }

            return sb.ToString();
        }

        public static void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            if (textBox != null && textBox.IsHandleCreated)
            {
                SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, placeholderText);
            }
            else if (textBox != null)
            {
                textBox.HandleCreated += (s, e) =>
                {
                    SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, placeholderText);
                };
            }
        }

    }
}
