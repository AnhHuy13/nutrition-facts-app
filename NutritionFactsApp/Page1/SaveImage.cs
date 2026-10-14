using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public class SaveImage
    {
        private const string RegistrySubKey = @"Software\NutritionFactsApp";
        private const string ValueName = "PathDefault";
        private static string _cachedPathDefault = null;

        public static void SaveImageToFile(PictureBox image, string filePath)
        {
            try
            {
                if (image.Image == null)
                {
                    MessageBox.Show("Không có ảnh để lưu.");
                    return;
                }

                string dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                image.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu ảnh: " + ex.Message);
            }
        }

        public static void AddRegistryKeys(string pathDefault)
        {
            SaveOrUpdateRegistryKey(pathDefault);
        }

        public static void ChangeRegistryKeys(string pathDefault)
        {
            SaveOrUpdateRegistryKey(pathDefault);
        }

        public static void SaveOrUpdateRegistryKey(string pathDefault)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pathDefault))
                {
                    return;
                }

                if (!Directory.Exists(pathDefault))
                {
                    Directory.CreateDirectory(pathDefault);
                }

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistrySubKey))
                {
                    if (key != null)
                    {
                        key.SetValue("NutritionFactsApp", "NutritionFactsApp", RegistryValueKind.String);
                        key.SetValue(ValueName, pathDefault, RegistryValueKind.String);
                        _cachedPathDefault = pathDefault;
                        Console.WriteLine("Registry keys saved successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra khi lưu registry: {ex.Message}");
            }
        }

        public static string ReadRegistryKeys()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistrySubKey))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(ValueName);
                        if (value != null)
                        {
                            string path = value.ToString();
                            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                            {
                                _cachedPathDefault = path;
                                return path;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra khi đọc dữ liệu registry: {ex.Message}");
            }
            return null;
        }

        public static bool IsRegistryKeyExists()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistrySubKey))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(ValueName);
                        if (value != null)
                        {
                            string path = value.ToString();
                            bool exists = !string.IsNullOrEmpty(path) && Directory.Exists(path);
                            if (exists) _cachedPathDefault = path;
                            return exists;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra khi kiểm tra registry: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra xem giá trị PathDefault trong Windows Registry có bị thay đổi (so với lần đọc/lưu gần nhất) hay không.
        /// </summary>
        public static bool HasRegistryChanged()
        {
            try
            {
                string currentRegistryPath = null;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistrySubKey))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(ValueName);
                        if (value != null)
                        {
                            currentRegistryPath = value.ToString();
                        }
                    }
                }

                bool changed = (currentRegistryPath != _cachedPathDefault);
                return changed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi kiểm tra thay đổi registry: " + ex.Message);
                return false;
            }
        }

        public static string SelectFolder()
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = "Chọn thư mục lưu mặc định";
                folderBrowser.ShowNewFolderButton = true;
                folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowser.SelectedPath;
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        if (!Directory.Exists(selectedPath))
                        {
                            Directory.CreateDirectory(selectedPath);
                        }
                        SaveOrUpdateRegistryKey(selectedPath);
                        return selectedPath;
                    }
                }
            }
            return null;
        }

        public static string BuildFileName(string productName)
        {
            string cleanName = productName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                cleanName = cleanName.Replace(c, '_');
            }
            string dateStr = DateTime.Now.ToString("dd-MM-yyyy");
            return $"{cleanName} - {dateStr}.jpg";
        }
    }
}
