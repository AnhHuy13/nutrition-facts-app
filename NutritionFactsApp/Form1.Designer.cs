namespace NutritionFactsApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.manageFactsBtnNav = new System.Windows.Forms.Button();
            this.createFactsBtnNav = new System.Windows.Forms.Button();
            this.userControl11 = new NutritionFactsApp.UserControl1();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toNewDictToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.manageFactsBtnNav);
            this.panel2.Controls.Add(this.createFactsBtnNav);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(197, 597);
            this.panel2.TabIndex = 1;
            // 
            // manageFactsBtnNav
            // 
            this.manageFactsBtnNav.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.manageFactsBtnNav.Location = new System.Drawing.Point(3, 176);
            this.manageFactsBtnNav.Name = "manageFactsBtnNav";
            this.manageFactsBtnNav.Size = new System.Drawing.Size(194, 71);
            this.manageFactsBtnNav.TabIndex = 1;
            this.manageFactsBtnNav.Text = "Quản lý nhãn";
            this.manageFactsBtnNav.UseVisualStyleBackColor = true;
            this.manageFactsBtnNav.Click += new System.EventHandler(this.manageFactsBtnNav_Click);
            // 
            // createFactsBtnNav
            // 
            this.createFactsBtnNav.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.createFactsBtnNav.Location = new System.Drawing.Point(3, 107);
            this.createFactsBtnNav.Name = "createFactsBtnNav";
            this.createFactsBtnNav.Size = new System.Drawing.Size(194, 71);
            this.createFactsBtnNav.TabIndex = 0;
            this.createFactsBtnNav.Text = "Tạo nhãn";
            this.createFactsBtnNav.UseVisualStyleBackColor = true;
            this.createFactsBtnNav.Click += new System.EventHandler(this.createFactsBtnNav_Click);
            // 
            // userControl11
            // 
            this.userControl11.Location = new System.Drawing.Point(204, 9);
            this.userControl11.Name = "userControl11";
            this.userControl11.Size = new System.Drawing.Size(1270, 777);
            this.userControl11.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1481, 33);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toNewDictToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toNewDictToolStripMenuItem
            // 
            this.toNewDictToolStripMenuItem.Name = "toNewDictToolStripMenuItem";
            this.toNewDictToolStripMenuItem.Size = new System.Drawing.Size(358, 34);
            this.toNewDictToolStripMenuItem.Text = "Chuyển thư mục mặc định mới";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1481, 630);
            this.Controls.Add(this.userControl11);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.panel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button manageFactsBtnNav;
        private System.Windows.Forms.Button createFactsBtnNav;
        private UserControl1 userControl11;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toNewDictToolStripMenuItem;
    }
}

