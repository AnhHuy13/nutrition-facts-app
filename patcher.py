import os

files_to_update = {
    "NutritionFactsApp/Python.cs": r'''using Python.Runtime;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NutritionFactsApp
{
    internal class Python
    {
        public async Task RunGeneratorAsync(NutritionModel model)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (Py.GIL())
                    {
                        string json = JsonConvert.SerializeObject(model);
                        dynamic scope = Py.CreateScope();
                        scope.Set("data_json", json);
                        
                        string script = @"
import json
from nutrition_facts.generator import NutritionFactsGenerator
from nutrition_facts.models import NutritionData

# Load data từ C#
data = json.loads(data_json)

# Logic render
try:
    # generator = NutritionFactsGenerator()
    # generator.generate(NutritionData(**data), 'output/preview.png')
    pass
except Exception as e:
    print(f'Error in Python: {e}')
";
                        scope.Exec(script);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi Python: " + ex.Message);
                }
            });
        }
    }
}''',

    "NutritionFactsApp/UserControl1.cs": r'''using FastColoredTextBoxNS;
using System;
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
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            SetPlaceholder(txtProductName, "Nhập tên sản phẩm...");
            SetPlaceholder(txtServingUnit, "VD: bịch, phần, gói, hộp,...");
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

        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();
            NutritionModel model = GetInfo();
            
            if (!string.IsNullOrWhiteSpace(model.ProductName))
            {
                await new Python().RunGeneratorAsync(model);
                OnDataChanged?.Invoke(this, EventArgs.Empty);
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
                ServingUnit = txtServingUnit.Text == "VD: bịch, phần, gói, hộp,..." ? "" : txtServingUnit.Text.Trim(),
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
    }
}'''
}

def update():
    for path, content in files_to_update.items():
        # Đảm bảo đường dẫn tồn tại
        if os.path.exists(os.path.dirname(path)):
            with open(path, 'w', encoding='utf-8') as f:
                f.write(content)
            print(f"Thành công: {path}")
        else:
            print(f"Lỗi: Thư mục không tồn tại {path}")

if __name__ == "__main__":
    update()