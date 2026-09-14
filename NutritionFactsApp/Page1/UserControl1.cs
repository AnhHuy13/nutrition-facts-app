using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using Newtonsoft.Json;

namespace NutritionFactsApp
{
    public partial class UserControl1 : UserControl
    {
        private bool _isUpdating = false;
        private Timer _debounceTimer;
        private string _currentImagePath = null;
        public NutritionModel CurrentModel { get; set; } = new NutritionModel();

        public UserControl1()
        {
            InitializeComponent();

            // 1. Khởi tạo Timer cho Preview
            _debounceTimer = new Timer();
            _debounceTimer.Interval = 500;
            _debounceTimer.Tick += DebounceTimer_Tick;

            // 2. Gán sự kiện cho mọi Control
            RegisterUIEvents(this);
            codeEditor.TextChanged += CodeEditor_TextChanged;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            Ultility.SetPlaceholder(txtProductName, Constants.PlaceHolderForTxtProductName);
            Ultility.SetPlaceholder(txtServingUnit, Constants.PlaceHolderForTxtServingUnit);
            SetCodeEditor(codeEditor);
        }

        private void RegisterUIEvents(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox t) t.TextChanged += UI_Changed;
                else if (c is NumericUpDown n) n.ValueChanged += UI_Changed;

                if (c.HasChildren) RegisterUIEvents(c);
            }
        }

        // --- CÁC HÀM HỖ TRỢ ---

        public void SetCodeEditor(FastColoredTextBox editor)
        {
            editor.Language = Language.Custom;
            editor.AutoCompleteBrackets = true;
            editor.CommentPrefix = "#";
            try
            {
                string tempXmlPath = Path.Combine(Path.GetTempPath(), "PythonSyntax_temp.xml");
                if (Properties.Resources.PythonSyntax != null)
                {
                    string xmlContent = System.Text.Encoding.UTF8.GetString(Properties.Resources.PythonSyntax);
                    File.WriteAllText(tempXmlPath, xmlContent, System.Text.Encoding.UTF8);
                    editor.DescriptionFile = tempXmlPath;
                }
            }
            catch { }
            editor.OnTextChanged();
        }

        // --- ĐỒNG BỘ DỮ LIỆU ---

        private void UI_Changed(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            JsonMapper.MapUIToModel(this, CurrentModel);
            codeEditor.Text = JsonConvert.SerializeObject(CurrentModel, Formatting.Indented);

            _debounceTimer.Stop();
            _debounceTimer.Start();

            _isUpdating = false;
        }

        private void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                var data = JsonConvert.DeserializeObject<NutritionModel>(codeEditor.Text);
                if (data != null)
                {
                    CurrentModel = data;
                    JsonMapper.MapModelToUI(this, CurrentModel);

                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
            catch { }

            _isUpdating = false;
        }

        // --- PREVIEW RENDER ---

        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            if (string.IsNullOrWhiteSpace(CurrentModel.ProductName)) return;

            try
            {
                string newImagePath = await new Python().RunGeneratorAsync(CurrentModel);

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
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = "Lỗi render: " + ex.Message;
            }
        }

        private void picProductImage_Click(object sender, EventArgs e) { }

        private void picProductImage_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, picProductImage.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void copyPromptsBtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Constants.PromptsAI);
            Ultility.SetStatusBarText("Đã copy prompts AI vào clipboard.");
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            if (picProductImage.Image == null)
            {
                MessageBox.Show("Chưa có ảnh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (SaveImage.IsRegistryKeyExists())
            {
                try
                {
                    SaveImage.SaveImageToFile(picProductImage, SaveImage.ReadRegistryKeys());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    string path = SaveImage.SelectFolder();
                    if (!string.IsNullOrEmpty(path))
                    {
                        SaveImage.SaveImageToFile(picProductImage, path);
                    }
                    else
                    {
                        //abort
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        } 

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtProductName.Text) && txtProductName.Text != Constants.PlaceHolderForTxtProductName)
            {
                Console.WriteLine("txtProductName_TextChanged: " + txtProductName.Text);
                txtNameFile.Text = SaveImage.BuildFileName(txtProductName.Text);
            }
            else
            {
                txtNameFile.Text = "";
                //do nothing
            }
        }
    }
}