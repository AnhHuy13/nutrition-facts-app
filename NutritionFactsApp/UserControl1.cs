using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public partial class UserControl1 : UserControl
    {
        public event EventHandler OnDataChanged;
        private Timer debounceTimer;

        public UserControl1()
        {
            InitializeComponent();
            AttachChangeEvents(this.Controls);
            
            debounceTimer = new Timer();
            debounceTimer.Interval = 500;
            debounceTimer.Tick += DebounceTimer_Tick;
           
            lblError.Visible = true;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            SetPlaceholder(txtProductName, "Nhập tên sản phẩm...");
            SetPlaceholder(txtServingUnit, "VD: bịch, phần, gói, hộp,... (KHÔNG BẮT BUỘC)");
            SetCodeEditor(codeEditor);
        }









        private void AttachChangeEvents(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl.Name == "codeEditor") continue;
                if (ctrl is TextBox txt) txt.TextChanged += AnyControl_ValueChanged;
                else if (ctrl is NumericUpDown nud) nud.ValueChanged += AnyControl_ValueChanged;
                
                if (ctrl.HasChildren) AttachChangeEvents(ctrl.Controls);
            }
        }

        private void AnyControl_ValueChanged(object sender, EventArgs e)
        {
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private string _currentImagePath = null;

        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {

            /*
            if (string.IsNullOrEmpty(txtProductName.Text) || txtProductName.Text == "Nhập tên sản phẩm..."
                || string.IsNullOrEmpty(txtServingUnit.Text) || txtServingUnit.Text == "VD: bịch, phần, gói, hộp,... (KHÔNG BẮT BUỘC)"
                || nudServingCount.Value == 0 || nudServingWeight.Value == 0 || nudCalories.Value == 0)
            {
                List<string> required = new List<string>();
                if (string.IsNullOrEmpty(txtProductName.Text) || txtProductName.Text == "Nhập tên sản phẩm..."){
                    required.Add("Tên sản phẩm");
                }
                if (string.IsNullOrEmpty(txtServingUnit.Text) || txtServingUnit.Text == "VD: bịch, phần, gói, hộp,... (KHÔNG BẮT BUỘC)")
                {
                    required.Add("Đơn vị gọi tên khẩu phần");
                }
                if (nudServingCount.Value == 0)
                {
                    required.Add("Số lượng Serving");
                }
                if (nudServingWeight.Value == 0)
                {
                    required.Add("Trọng lượng 1 serving (g)");
                }
                if (nudCalories.Value == 0)
                {
                    required.Add("Calories");
                }

                for (int i = 0; i < required.Count; i++)
                {
                    try
                    {
                        lblError.Text = $"Vui lòng nhập đầy đủ \n{string.Join(", \n\n", required)}";
                    }
                    catch
                    {
                        
                    }
                }

                return;
            }    

            */

            debounceTimer.Stop();
            NutritionModel model = GetInfo();

            if (string.IsNullOrWhiteSpace(model.ProductName)) return;

            string newImagePath = await new Python().RunGeneratorAsync(model);

            if (File.Exists(newImagePath))
            {
                lblError.Visible = false;
                if (picProductImage.Image != null)
                {
                    picProductImage.Image.Dispose();
                    picProductImage.Image = null;
                }

                using (var fs = new FileStream(newImagePath, FileMode.Open, FileAccess.Read))
                {
                    picProductImage.Image = Image.FromStream(fs);
                }

                if (_currentImagePath != null && File.Exists(_currentImagePath))
                {
                    try { File.Delete(_currentImagePath); } catch { }
                }

                _currentImagePath = newImagePath;
            }
        }

        public void SetCodeEditor(FastColoredTextBox codeEditor)
        {
            codeEditor.Language = Language.Custom;
            codeEditor.AutoCompleteBrackets = true;
            codeEditor.CommentPrefix = "#";
            try
            {
                string tempXmlPath = Path.Combine(Path.GetTempPath(), "PythonSyntax_temp.xml");
                string xmlContent = System.Text.Encoding.UTF8.GetString(Properties.Resources.PythonSyntax);
                File.WriteAllText(tempXmlPath, xmlContent, System.Text.Encoding.UTF8);
                codeEditor.DescriptionFile = tempXmlPath;
            }
            catch { }
            codeEditor.OnTextChanged();
        }

        public void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.Text = placeholderText;
            textBox.ForeColor = Color.Gray;
            textBox.Enter += (s, e) => { if (textBox.Text == placeholderText) { textBox.Text = ""; textBox.ForeColor = Color.Black; } };
            textBox.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(textBox.Text)) { textBox.Text = placeholderText; textBox.ForeColor = Color.Gray; } };
        }

        public NutritionModel GetInfo()
        {
            return new NutritionModel
            {
                ProductName = txtProductName.Text == "Nhập tên sản phẩm..." ? "" : txtProductName.Text.Trim(),
                ServingUnit = txtServingUnit.Text == "VD: bịch, phần, gói, hộp,... (KHÔNG BẮT BUỘC)" ? "" : txtServingUnit.Text.Trim(),
                Code = codeEditor.Text.Trim(),
                ServingCount = nudServingCount.Value,
                ServingWeight = nudServingWeight.Value,
                Calories = nudCalories.Value,
                TotalFat = nudTotalFat.Value,
                SaturatedFat = nudSaturatedFat.Value,
                TransFat = nudTransFat.Value,
                Cholesterol = nudCholesterol.Value,
                Sodium = nudSodium.Value,
                TotalCarbohydrate = nudTotalCarbs.Value,
                DietaryFiber = nudDietaryFiber.Value,
                TotalSugars = nudTotalSugars.Value,
                AddedSugars = nudAddedSugars.Value,
                Protein = nudProtein.Value,
                VitaminD = nudVitaminD.Value,
                Calcium = nudCalcium.Value,
                Iron = nudIron.Value,
                Potassium = nudPotassium.Value
            };
        }

        private void picProductImage_Click(object sender, EventArgs e) { }

        private void picProductImage_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, picProductImage.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }
    }
}