using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public class SaveImage
    {
        public static void SaveImageToFile(PictureBox image, string filePath)
        {
            try
            {
                if (image.Image == null)
                {
                    MessageBox.Show("Không có ảnh để lưu.");
                    return;
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
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NutritionFactsApp"))
                {
                    if (key != null)
                    {   
                        key.SetValue("NutritionFactsApp", "NutritionFactsApp");                      // REG_SZ (String)
                        key.SetValue("PathDefault", pathDefault, RegistryValueKind.String); // REG_SZ (String)

                        Console.WriteLine("Registry keys added successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra khi viết dữ liệu: {ex.Message}");
            }
        }

        public static void ChangeRegistryKeys(string pathDefault)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NutritionFactsApp", writable: true))
                {
                    if (key != null)
                    {
                        key.SetValue("PathDefault", pathDefault, RegistryValueKind.String); // REG_SZ (String)
                        Console.WriteLine("Registry keys changed successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khóa đăng ký để thay đổi.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra khi thay đổi dữ liệu: {ex.Message}");
            }
        }

        public static string ReadRegistryKeys()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NutritionFactsApp"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("PathDefault");
                        if (value != null)
                        {
                            return value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy giá trị PathDefault trong khóa đăng ký.");
                            return null;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khóa đăng ký.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra khi đọc dữ liệu: {ex.Message}");
                return null;
            }
        }

        public static bool IsRegistryKeyExists()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NutritionFactsApp"))
                {
                    if (key == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra khi kiểm tra dữ liệu: {ex.Message}");
            }
            return false;
        }

        public static string SelectFolder()
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = "Đặt/đổi thư mục mặc định";
                folderBrowser.ShowNewFolderButton = true;
                folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowser.SelectedPath;
                    return selectedPath;
                }
            }
            return null;
        }

        public static string BuildFileName(string productName)
        {
            string ProductName = Ultility.RemoveUnicode(productName);
            ProductName = ProductName.Replace(" ", "_");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{ProductName}_{timestamp}.jpg";
        }
    }
}
