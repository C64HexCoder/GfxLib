namespace GfxLib
{
    partial class FontDisp
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
            this.charAsciiTxt = new System.Windows.Forms.Label();
            this.imageGrid = new GfxLib.ImageGrid();
            this.SuspendLayout();
            // 
            // charAsciiTxt
            // 
            this.charAsciiTxt.AutoSize = true;
            this.charAsciiTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.charAsciiTxt.Location = new System.Drawing.Point(45, 42);
            this.charAsciiTxt.Name = "charAsciiTxt";
            this.charAsciiTxt.Size = new System.Drawing.Size(43, 46);
            this.charAsciiTxt.TabIndex = 1;
            this.charAsciiTxt.Text = "0";
            // 
            // imageGrid
            // 
            this.imageGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imageGrid.BackgroundColor = System.Drawing.Color.Transparent;
            this.imageGrid.CellBorder = true;
            this.imageGrid.CellSizing = GfxLib.ImageGrid.CellSize.Fixed;
            this.imageGrid.CellsOnX = 10;
            this.imageGrid.CellsOnY = 15;
            this.imageGrid.CellWIdthHeight = 8;
            this.imageGrid.DisplayMode = GfxLib.ImageGrid.DisplayModes.Mono;
            this.imageGrid.DrawColor = System.Drawing.Color.Blue;
            this.imageGrid.EnableDrawing = true;
            this.imageGrid.GridImage = null;
            this.imageGrid.KeepAspectRatio = true;
            this.imageGrid.LineWidth = 2;
            this.imageGrid.Location = new System.Drawing.Point(156, 2);
            this.imageGrid.Margin = new System.Windows.Forms.Padding(2);
            this.imageGrid.Name = "imageGrid";
            this.imageGrid.ResizeToFitCells = GfxLib.ImageGrid.ResizeTypes.None;
            this.imageGrid.Size = new System.Drawing.Size(82, 122);
            this.imageGrid.TabIndex = 0;
            this.imageGrid.Resize += new System.EventHandler(this.imageGrid_Resize);
            // 
            // FontDisp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.charAsciiTxt);
            this.Controls.Add(this.imageGrid);
            this.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Name = "FontDisp";
            this.Size = new System.Drawing.Size(241, 132);
            this.Click += new System.EventHandler(this.FontDisp_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageGrid imageGrid;
        public System.Windows.Forms.Label charAsciiTxt;
    }
}
