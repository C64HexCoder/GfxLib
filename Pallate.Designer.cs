using System.Windows.Forms;

namespace GfxLib
{
    partial class Pallate
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.colorPanel = new System.Windows.Forms.Panel();
            this.RedLbl = new System.Windows.Forms.Label();
            this.GreenLbl = new System.Windows.Forms.Label();
            this.BlueLbl = new System.Windows.Forms.Label();
            this.redNum = new System.Windows.Forms.NumericUpDown();
            this.greenNum = new System.Windows.Forms.NumericUpDown();
            this.blueNum = new System.Windows.Forms.NumericUpDown();
            this.selectedColorPb = new System.Windows.Forms.PictureBox();
            this.RGBPanel = new System.Windows.Forms.Panel();
            this.AlphaLbl = new System.Windows.Forms.Label();
            this.AlphaNum = new System.Windows.Forms.NumericUpDown();
            this.idxNum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.redNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedColorPb)).BeginInit();
            this.RGBPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlphaNum)).BeginInit();
            this.SuspendLayout();
            // 
            // colorPanel
            // 
            this.colorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.colorPanel.Location = new System.Drawing.Point(0, 0);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(497, 510);
            this.colorPanel.TabIndex = 0;
            this.colorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.colorPanel_Paint);
            this.colorPanel.Resize += new System.EventHandler(this.colorPanel_Resize);
            // 
            // RedLbl
            // 
            this.RedLbl.AutoSize = true;
            this.RedLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RedLbl.ForeColor = System.Drawing.Color.Red;
            this.RedLbl.Location = new System.Drawing.Point(165, 7);
            this.RedLbl.Name = "RedLbl";
            this.RedLbl.Size = new System.Drawing.Size(45, 25);
            this.RedLbl.TabIndex = 2;
            this.RedLbl.Text = "Red";
            this.RedLbl.Visible = false;
            // 
            // GreenLbl
            // 
            this.GreenLbl.AutoSize = true;
            this.GreenLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.GreenLbl.ForeColor = System.Drawing.Color.Lime;
            this.GreenLbl.Location = new System.Drawing.Point(272, 7);
            this.GreenLbl.Name = "GreenLbl";
            this.GreenLbl.Size = new System.Drawing.Size(63, 25);
            this.GreenLbl.TabIndex = 3;
            this.GreenLbl.Text = "Green";
            this.GreenLbl.Visible = false;
            // 
            // BlueLbl
            // 
            this.BlueLbl.AutoSize = true;
            this.BlueLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BlueLbl.ForeColor = System.Drawing.Color.Blue;
            this.BlueLbl.Location = new System.Drawing.Point(398, 7);
            this.BlueLbl.Name = "BlueLbl";
            this.BlueLbl.Size = new System.Drawing.Size(50, 25);
            this.BlueLbl.TabIndex = 4;
            this.BlueLbl.Text = "Blue";
            this.BlueLbl.Visible = false;
            // 
            // redNum
            // 
            this.redNum.Location = new System.Drawing.Point(142, 35);
            this.redNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.redNum.Name = "redNum";
            this.redNum.Size = new System.Drawing.Size(100, 31);
            this.redNum.TabIndex = 5;
            this.redNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // greenNum
            // 
            this.greenNum.Location = new System.Drawing.Point(262, 35);
            this.greenNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.greenNum.Name = "greenNum";
            this.greenNum.Size = new System.Drawing.Size(100, 31);
            this.greenNum.TabIndex = 6;
            this.greenNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // blueNum
            // 
            this.blueNum.Location = new System.Drawing.Point(379, 35);
            this.blueNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blueNum.Name = "blueNum";
            this.blueNum.Size = new System.Drawing.Size(100, 31);
            this.blueNum.TabIndex = 7;
            this.blueNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // selectedColorPb
            // 
            this.selectedColorPb.Location = new System.Drawing.Point(3, 517);
            this.selectedColorPb.Name = "selectedColorPb";
            this.selectedColorPb.Size = new System.Drawing.Size(494, 52);
            this.selectedColorPb.TabIndex = 8;
            this.selectedColorPb.TabStop = false;
            this.selectedColorPb.Visible = false;
            this.selectedColorPb.Resize += new System.EventHandler(this.selectedColorPb_Resize);
            // 
            // RGBPanel
            // 
            this.RGBPanel.Controls.Add(this.AlphaLbl);
            this.RGBPanel.Controls.Add(this.AlphaNum);
            this.RGBPanel.Controls.Add(this.greenNum);
            this.RGBPanel.Controls.Add(this.RedLbl);
            this.RGBPanel.Controls.Add(this.blueNum);
            this.RGBPanel.Controls.Add(this.GreenLbl);
            this.RGBPanel.Controls.Add(this.BlueLbl);
            this.RGBPanel.Controls.Add(this.redNum);
            this.RGBPanel.Location = new System.Drawing.Point(4, 577);
            this.RGBPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.RGBPanel.Name = "RGBPanel";
            this.RGBPanel.Size = new System.Drawing.Size(491, 97);
            this.RGBPanel.TabIndex = 9;
            this.RGBPanel.Resize += new System.EventHandler(this.RGBPanel_Resize);
            // 
            // AlphaLbl
            // 
            this.AlphaLbl.AutoSize = true;
            this.AlphaLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AlphaLbl.Location = new System.Drawing.Point(40, 7);
            this.AlphaLbl.Name = "AlphaLbl";
            this.AlphaLbl.Size = new System.Drawing.Size(51, 25);
            this.AlphaLbl.TabIndex = 9;
            this.AlphaLbl.Text = "Alpa";
            // 
            // AlphaNum
            // 
            this.AlphaNum.Location = new System.Drawing.Point(16, 35);
            this.AlphaNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.AlphaNum.Name = "AlphaNum";
            this.AlphaNum.Size = new System.Drawing.Size(100, 31);
            this.AlphaNum.TabIndex = 8;
            this.AlphaNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // idxNum
            // 
            this.idxNum.AutoSize = true;
            this.idxNum.BackColor = System.Drawing.Color.Transparent;
            this.idxNum.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.idxNum.Location = new System.Drawing.Point(231, 517);
            this.idxNum.Name = "idxNum";
            this.idxNum.Size = new System.Drawing.Size(35, 41);
            this.idxNum.TabIndex = 10;
            this.idxNum.Text = "0";
            // 
            // Pallate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.idxNum);
            this.Controls.Add(this.RGBPanel);
            this.Controls.Add(this.selectedColorPb);
            this.Controls.Add(this.colorPanel);
            this.Name = "Pallate";
            this.Size = new System.Drawing.Size(500, 679);
            this.Enter += new System.EventHandler(this.Pallate_Enter);
            this.Resize += new System.EventHandler(this.Pallate_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.redNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedColorPb)).EndInit();
            this.RGBPanel.ResumeLayout(false);
            this.RGBPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlphaNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel colorPanel;
        private Label RedLbl;
        private Label GreenLbl;
        private Label BlueLbl;
        private NumericUpDown redNum;
        private NumericUpDown greenNum;
        private NumericUpDown blueNum;
        private PictureBox selectedColorPb;
        private Panel RGBPanel;
        private Label idxNum;
        private Label AlphaLbl;
        private NumericUpDown AlphaNum;
    }
}