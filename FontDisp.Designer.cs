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
            this.charAsciiTxt.Location = new System.Drawing.Point(43, 46);
            this.charAsciiTxt.Name = "charAsciiTxt";
            this.charAsciiTxt.Size = new System.Drawing.Size(43, 46);
            this.charAsciiTxt.TabIndex = 1;
            this.charAsciiTxt.Text = "0";
            // 
            // imageGrid
            // 
            this.imageGrid.BackgroundColor = System.Drawing.Color.Transparent;
            this.imageGrid.CellBorder = true;
            this.imageGrid.CellSizing = GfxLib.ImageGrid.CellSize.Fixed;
            this.imageGrid.CellsOnX = 16;
            this.imageGrid.CellsOnY = 16;
            this.imageGrid.CellWIdthHeight = 8;
            this.imageGrid.DisplayMode = GfxLib.ImageGrid.DisplayModes.Mono;
            this.imageGrid.DrawColor = System.Drawing.Color.Blue;
            this.imageGrid.EnableDrawing = true;
            this.imageGrid.GridImage = null;
            this.imageGrid.KeepAspectRatio = true;
            this.imageGrid.LineWidth = 2;
            this.imageGrid.Location = new System.Drawing.Point(128, 3);
            this.imageGrid.Name = "imageGrid";
            this.imageGrid.ResizeToFitCells = GfxLib.ImageGrid.ResizeTypes.None;
            this.imageGrid.Size = new System.Drawing.Size(130, 130);
            this.imageGrid.TabIndex = 0;
            // 
            // FontDisp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.charAsciiTxt);
            this.Controls.Add(this.imageGrid);
            this.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Name = "FontDisp";
            this.Size = new System.Drawing.Size(261, 136);
            this.Load += new System.EventHandler(this.FontDisp_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FontDisp_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageGrid imageGrid;
        public System.Windows.Forms.Label charAsciiTxt;
    }
}
