using FastColoredTextBoxNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public partial class UserControl1 : UserControl
    {
        private bool _isUpdating = false;
        private Timer _debounceTimer;
        private string _currentImagePath = null;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<NutritionModel> BatchList { get; set; } = new List<NutritionModel> { new NutritionModel() };

        public UserControl1()
        {
            InitializeComponent();

            _debounceTimer = new Timer();
            _debounceTimer.Interval = 500;
            _debounceTimer.Tick += DebounceTimer_Tick;

            RegisterUIEvents(this);
            codeEditor.TextChanged += CodeEditor_TextChanged;

            // Đăng ký sự kiện đổi Tab
            tabControlProducts.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            button1.Click += button1_Click;

            // Đăng ký context menu cho tabControlProducts (chuột phải -> Tạo sản phẩm)
            ContextMenuStrip tabContextMenu = new ContextMenuStrip();
            ToolStripMenuItem miCreateProduct = new ToolStripMenuItem("Tạo sản phẩm");
            miCreateProduct.Click += MiCreateProduct_Click;
            tabContextMenu.Items.Add(miCreateProduct);
            tabControlProducts.ContextMenuStrip = tabContextMenu;
            tabControlProducts.MouseDown += TabControlProducts_MouseDown;
        }

        private void TabControlProducts_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControlProducts.TabCount; i++)
                {
                    if (tabControlProducts.GetTabRect(i).Contains(e.Location))
                    {
                        tabControlProducts.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void MiCreateProduct_Click(object sender, EventArgs e)
        {
            var newModel = new NutritionModel();
            BatchList.Add(newModel);
            UpdateTabs();
            tabControlProducts.SelectedIndex = BatchList.Count - 1;
            JsonMapper.MapModelToUI(this, newModel);
            codeEditor.Text = JsonConvert.SerializeObject(BatchList, Formatting.Indented);
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            Ultility.SetPlaceholder(txtProductName, Constants.PlaceHolderForTxtProductName);
            Ultility.SetPlaceholder(txtServingUnit, Constants.PlaceHolderForTxtServingUnit);
            SetCodeEditor(codeEditor);

            UpdateTabs();
            if (BatchList.Count > 0)
            {
                JsonMapper.MapModelToUI(this, BatchList[0]);
            }
            codeEditor.Text = JsonConvert.SerializeObject(BatchList, Formatting.Indented);
        }

        private void UpdateTabs()
        {
            int oldIndex = tabControlProducts.SelectedIndex;
            tabControlProducts.TabPages.Clear();
            for (int i = 0; i < BatchList.Count; i++)
            {
                string title = string.IsNullOrWhiteSpace(BatchList[i].ProductName) ? $"Sản phẩm {i + 1}" : BatchList[i].ProductName;
                tabControlProducts.TabPages.Add(title);
            }
            if (oldIndex >= 0 && oldIndex < tabControlProducts.TabPages.Count)
            {
                tabControlProducts.SelectedIndex = oldIndex;
            }
            else if (tabControlProducts.TabPages.Count > 0)
            {
                tabControlProducts.SelectedIndex = 0;
            }
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

        private void UI_Changed(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            int index = tabControlProducts.SelectedIndex;
            if (index >= 0 && index < BatchList.Count)
            {
                JsonMapper.MapUIToModel(this, BatchList[index]);
                string tabTitle = string.IsNullOrWhiteSpace(BatchList[index].ProductName) ? $"Sản phẩm {index + 1}" : BatchList[index].ProductName;
                if (tabControlProducts.TabPages[index].Text != tabTitle)
                {
                    tabControlProducts.TabPages[index].Text = tabTitle;
                }
                codeEditor.Text = JsonConvert.SerializeObject(BatchList, Formatting.Indented);
            }

            _debounceTimer.Stop();
            _debounceTimer.Start();

            _isUpdating = false;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                // Thà để trống/gray chứ không giữ hình ảnh cũ của tab trước
                if (picProductImage.Image != null)
                {
                    picProductImage.Image.Dispose();
                    picProductImage.Image = null;
                }
                lblError.Visible = false;

                int index = tabControlProducts.SelectedIndex;
                // Đảm bảo không vượt quá chỉ số mảng
                if (index >= 0 && index < BatchList.Count)
                {
                    JsonMapper.MapModelToUI(this, BatchList[index]);
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                List<NutritionModel> data = null;
                string text = codeEditor.Text?.Trim() ?? "";

                // Strip markdown code block fences if present (e.g. ```json ... ``` or ``` ... ```)
                if (text.StartsWith("```"))
                {
                    int firstNewline = text.IndexOf('\n');
                    if (firstNewline > 0)
                    {
                        text = text.Substring(firstNewline + 1).Trim();
                    }
                    if (text.EndsWith("```"))
                    {
                        text = text.Substring(0, text.Length - 3).Trim();
                    }
                }

                if (text.StartsWith("{"))
                {
                    var singleModel = JsonConvert.DeserializeObject<NutritionModel>(text, _jsonSettings);
                    if (singleModel != null) data = new List<NutritionModel> { singleModel };
                }
                else if (text.StartsWith("["))
                {
                    data = JsonConvert.DeserializeObject<List<NutritionModel>>(text, _jsonSettings);
                }

                if (data != null && data.Count > 0)
                {
                    BatchList = data;
                    UpdateTabs();
                    int index = tabControlProducts.SelectedIndex;
                    if (index >= 0 && index < BatchList.Count)
                    {
                        JsonMapper.MapModelToUI(this, BatchList[index]);
                    }
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
            catch { }
            finally
            {
                _isUpdating = false;
            }
        }

        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            int index = tabControlProducts.SelectedIndex;
            if (index < 0 || index >= BatchList.Count) return;
            var model = BatchList[index];
            if (string.IsNullOrWhiteSpace(model.ProductName) || model.ProductName == Constants.PlaceHolderForTxtProductName)
            {
                if (picProductImage.Image != null)
                {
                    picProductImage.Image.Dispose();
                    picProductImage.Image = null;
                }
                lblError.Visible = false;
                return;
            }

            try
            {
                string newImagePath = await new Python().RunGeneratorAsync(model);
                if (!string.IsNullOrEmpty(newImagePath) && File.Exists(newImagePath))
                {
                    lblError.Visible = false;
                    if (picProductImage.Image != null) { picProductImage.Image.Dispose(); picProductImage.Image = null; }

                    using (var fs = new FileStream(newImagePath, FileMode.Open, FileAccess.Read))
                    {
                        picProductImage.Image = Image.FromStream(fs);
                    }
                    _currentImagePath = newImagePath;
                }
                else
                {
                    if (picProductImage.Image != null) { picProductImage.Image.Dispose(); picProductImage.Image = null; }
                    lblError.Visible = true;
                    lblError.Text = "Lỗi render: Không thể tạo hình ảnh.";
                }
            }
            catch (Exception ex)
            {
                // CẤM ĐỂ LẠI CÁI CŨ BẰNG MỌI GIÁ
                if (picProductImage.Image != null) { picProductImage.Image.Dispose(); picProductImage.Image = null; }
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
                MessageBox.Show("Không có ảnh để xuất. Vui lòng tạo ảnh trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string path = null;
            if (SaveImage.IsRegistryKeyExists())
            {
                path = SaveImage.ReadRegistryKeys();
            }
            if (string.IsNullOrEmpty(path))
            {
                path = SaveImage.SelectFolder();
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                SaveImage.SaveOrUpdateRegistryKey(path);
            }

            string productName = "SanPham";
            if (BatchList != null && tabControlProducts.SelectedIndex >= 0 && tabControlProducts.SelectedIndex < BatchList.Count)
            {
                var prod = BatchList[tabControlProducts.SelectedIndex];
                if (!string.IsNullOrWhiteSpace(prod.ProductName) && prod.ProductName != Constants.PlaceHolderForTxtProductName)
                {
                    productName = prod.ProductName;
                }
            }

            string filePath = Path.Combine(path, SaveImage.BuildFileName(productName));
            SaveImage.SaveImageToFile(picProductImage, filePath);
            if (BatchList != null && tabControlProducts.SelectedIndex >= 0 && tabControlProducts.SelectedIndex < BatchList.Count)
            {
                MetadataHelper.SaveMetadata(filePath, BatchList[tabControlProducts.SelectedIndex]);
            }
            MessageBox.Show($"Đã xuất ảnh thành công ra thư mục:\n{path}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtProductName.Text) && txtProductName.Text != Constants.PlaceHolderForTxtProductName)
            {
                txtNameFile.Text = SaveImage.BuildFileName(txtProductName.Text);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (BatchList == null || BatchList.Count == 0)
            {
                MessageBox.Show("Không có sản phẩm nào để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Kiểm tra xem tất cả các tab có điền đủ 4 ô đầu hay không (tên sp, serving size, serving, calo)
            for (int i = 0; i < BatchList.Count; i++)
            {
                var model = BatchList[i];
                bool hasName = !string.IsNullOrWhiteSpace(model.ProductName) && model.ProductName != Constants.PlaceHolderForTxtProductName;
                bool hasServingWeight = model.ServingWeight > 0;
                bool hasServingCount = model.ServingCount > 0;

                if (!hasName || !hasServingWeight || !hasServingCount || model.Calories <= 0)
                {
                    string tabTitle = string.IsNullOrWhiteSpace(model.ProductName) ? $"Sản phẩm {i + 1}" : model.ProductName;
                    MessageBox.Show($"Tab '{tabTitle}' (Sản phẩm {i + 1}) không có đủ 4 thông tin đầu (Tên sản phẩm, Serving size, Serving, Calo). Vui lòng điền đầy đủ trước khi xuất toàn bộ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string path = null;
            if (SaveImage.IsRegistryKeyExists())
            {
                path = SaveImage.ReadRegistryKeys();
            }
            if (string.IsNullOrEmpty(path))
            {
                path = SaveImage.SelectFolder();
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                SaveImage.SaveOrUpdateRegistryKey(path);
            }
            int successCount = 0;
            try
            {
                for (int i = 0; i < BatchList.Count; i++)
                {
                    var model = BatchList[i];
                    string prodName = string.IsNullOrWhiteSpace(model.ProductName) ? $"SanPham_{i + 1}" : model.ProductName;
                    string fileName = SaveImage.BuildFileName(prodName);
                    string targetPath = Path.Combine(path, fileName);

                    await new Python().RunGeneratorAsync(model, targetPath);
                    if (File.Exists(targetPath))
                    {
                        successCount++;
                    }
                }

                MessageBox.Show($"Đã xuất thành công {successCount}/{BatchList.Count} sản phẩm ra thư mục:\n{path}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất toàn bộ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}