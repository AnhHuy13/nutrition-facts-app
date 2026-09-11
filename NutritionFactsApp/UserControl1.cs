using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Python.Runtime;

namespace NutritionFactsApp
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }


        private void UserControl1_Load(object sender, EventArgs e)
        {
            SetPlaceholder(txtProductName, "Nhập tên sản phẩm...");
            SetPlaceholder(txtServingUnit, "VD: bịch, phần, gói, hộp,...");

            SetCodeEditor(codeEditor);
        }




        public void SetCodeEditor(FastColoredTextBox codeEditor)
        {
            codeEditor.Language = Language.Custom;  // For Python
            codeEditor.AutoCompleteBrackets = true;
            codeEditor.CommentPrefix = "#";
            codeEditor.Range.tb.LeftBracket = '(';  // Bracket config doesn't work in PythonSyntax.xml
            codeEditor.Range.tb.RightBracket = ')';
            codeEditor.Range.tb.LeftBracket2 = '[';
            codeEditor.Range.tb.RightBracket2 = ']';

            try
            {
                string tempXmlPath = Path.Combine(Path.GetTempPath(), "PythonSyntax_temp.xml");
                string xmlContent = System.Text.Encoding.UTF8.GetString(Properties.Resources.PythonSyntax);
                File.WriteAllText(tempXmlPath, xmlContent, System.Text.Encoding.UTF8);
                codeEditor.DescriptionFile = tempXmlPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp cấu hình ngôn ngữ: {ex.Message}", "Lỗi Resource", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            codeEditor.OnTextChanged();
        }





        public void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.Text = placeholderText;
            textBox.ForeColor = Color.Gray;
            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };
            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }



        public NutritionModel GetInfo()
        {
            return new NutritionModel
            {
                // Thông tin chung
                ProductName = txtProductName.Text.Trim(),
                ServingUnit = txtServingUnit.Text.Trim(),
                Code = codeEditor.Text.Trim(),

                ServingCount = nudServingCount.Value,
                ServingWeight = nudServingWeight.Value,
                Calories = nudCalories.Value,

                // Thông tin Đa lượng (Chất béo)
                TotalFat = nudTotalFat.Value,
                SaturatedFat = nudSaturatedFat.Value,
                TransFat = nudTransFat.Value,

                // Thông tin Đa lượng (Cholesterol & Sodium)
                Cholesterol = nudCholesterol.Value,
                Sodium = nudSodium.Value,

                // Thông tin Đa lượng (Carb, Đường, Đạm)
                TotalCarbohydrate = nudTotalCarbs.Value,
                DietaryFiber = nudDietaryFiber.Value,
                TotalSugars = nudTotalSugars.Value,
                AddedSugars = nudAddedSugars.Value,
                Protein = nudProtein.Value,

                // Thông tin Vi lượng (Vitamin & Khoáng chất)
                VitaminD = nudVitaminD.Value,
                Calcium = nudCalcium.Value,
                Iron = nudIron.Value,
                Potassium = nudPotassium.Value
            };
        }
    }
}