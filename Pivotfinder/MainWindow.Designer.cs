namespace Pivotfinder
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonDeleteListItem = new System.Windows.Forms.Button();
            this.buttonStartPixelfind = new System.Windows.Forms.Button();
            this.groupBoxSpriteSize = new System.Windows.Forms.GroupBox();
            this.numericUpDownSpriteHeight = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSpriteWidth = new System.Windows.Forms.NumericUpDown();
            this.labelSpriteSizeX = new System.Windows.Forms.Label();
            this.labelSpriteSizeY = new System.Windows.Forms.Label();
            this.groupBoxPixelOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxRemovePivot = new System.Windows.Forms.CheckBox();
            this.buttonSelectPivotColor = new System.Windows.Forms.Button();
            this.checkBoxFindPivots = new System.Windows.Forms.CheckBox();
            this.groupBoxAlpha = new System.Windows.Forms.GroupBox();
            this.checkBoxChangeAlpha = new System.Windows.Forms.CheckBox();
            this.checkBoxRemoveAlpha = new System.Windows.Forms.CheckBox();
            this.numericUpDownAlphaTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownAlphaFrom = new System.Windows.Forms.NumericUpDown();
            this.labelAlphaFrom = new System.Windows.Forms.Label();
            this.buttonAlphaToColor = new System.Windows.Forms.Button();
            this.labelAlphaTo = new System.Windows.Forms.Label();
            this.checkBoxSetNewAlphaColor = new System.Windows.Forms.CheckBox();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.buttonAddListItem = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonPivotToSpriteSheet = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBoxSpriteSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteWidth)).BeginInit();
            this.groupBoxPixelOptions.SuspendLayout();
            this.groupBoxAlpha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAlphaTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAlphaFrom)).BeginInit();
            this.groupBoxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BackgroundImage = global::Pivotfinder.Properties.Resources.icon;
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Location = new System.Drawing.Point(12, 145);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(360, 341);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.PictureBox_DragDrop);
            this.pictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.PictureBox_DragEnter);
            this.pictureBox.DragLeave += new System.EventHandler(this.PictureBox_DragLeave);
            // 
            // listBox
            // 
            this.listBox.AllowDrop = true;
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.HorizontalScrollbar = true;
            this.listBox.Location = new System.Drawing.Point(12, 12);
            this.listBox.MinimumSize = new System.Drawing.Size(250, 56);
            this.listBox.Name = "listBox";
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox.Size = new System.Drawing.Size(360, 121);
            this.listBox.TabIndex = 0;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
            this.listBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_DragEnter);
            this.listBox.DragLeave += new System.EventHandler(this.listBox_DragLeave);
            // 
            // buttonDeleteListItem
            // 
            this.buttonDeleteListItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteListItem.Location = new System.Drawing.Point(384, 51);
            this.buttonDeleteListItem.Name = "buttonDeleteListItem";
            this.buttonDeleteListItem.Size = new System.Drawing.Size(186, 24);
            this.buttonDeleteListItem.TabIndex = 2;
            this.buttonDeleteListItem.Text = "remove sprites";
            this.buttonDeleteListItem.UseVisualStyleBackColor = true;
            this.buttonDeleteListItem.Click += new System.EventHandler(this.buttonDeleteListItem_Click);
            // 
            // buttonStartPixelfind
            // 
            this.buttonStartPixelfind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartPixelfind.Location = new System.Drawing.Point(384, 381);
            this.buttonStartPixelfind.Name = "buttonStartPixelfind";
            this.buttonStartPixelfind.Size = new System.Drawing.Size(186, 34);
            this.buttonStartPixelfind.TabIndex = 11;
            this.buttonStartPixelfind.Text = "start";
            this.buttonStartPixelfind.UseVisualStyleBackColor = true;
            this.buttonStartPixelfind.Click += new System.EventHandler(this.startOperation_Click);
            // 
            // groupBoxSpriteSize
            // 
            this.groupBoxSpriteSize.Controls.Add(this.labelSpriteSizeY);
            this.groupBoxSpriteSize.Controls.Add(this.labelSpriteSizeX);
            this.groupBoxSpriteSize.Controls.Add(this.numericUpDownSpriteWidth);
            this.groupBoxSpriteSize.Controls.Add(this.numericUpDownSpriteHeight);
            this.groupBoxSpriteSize.Location = new System.Drawing.Point(6, 19);
            this.groupBoxSpriteSize.Name = "groupBoxSpriteSize";
            this.groupBoxSpriteSize.Size = new System.Drawing.Size(174, 49);
            this.groupBoxSpriteSize.TabIndex = 10;
            this.groupBoxSpriteSize.TabStop = false;
            this.groupBoxSpriteSize.Text = "spritesize";
            // 
            // numericUpDownSpriteHeight
            // 
            this.numericUpDownSpriteHeight.Location = new System.Drawing.Point(104, 19);
            this.numericUpDownSpriteHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownSpriteHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSpriteHeight.Name = "numericUpDownSpriteHeight";
            this.numericUpDownSpriteHeight.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownSpriteHeight.TabIndex = 4;
            this.numericUpDownSpriteHeight.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // numericUpDownSpriteWidth
            // 
            this.numericUpDownSpriteWidth.Location = new System.Drawing.Point(27, 19);
            this.numericUpDownSpriteWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownSpriteWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSpriteWidth.Name = "numericUpDownSpriteWidth";
            this.numericUpDownSpriteWidth.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownSpriteWidth.TabIndex = 3;
            this.numericUpDownSpriteWidth.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // labelSpriteSizeX
            // 
            this.labelSpriteSizeX.AutoSize = true;
            this.labelSpriteSizeX.Location = new System.Drawing.Point(9, 21);
            this.labelSpriteSizeX.Name = "labelSpriteSizeX";
            this.labelSpriteSizeX.Size = new System.Drawing.Size(15, 13);
            this.labelSpriteSizeX.TabIndex = 6;
            this.labelSpriteSizeX.Text = "x:";
            // 
            // labelSpriteSizeY
            // 
            this.labelSpriteSizeY.AutoSize = true;
            this.labelSpriteSizeY.Location = new System.Drawing.Point(86, 21);
            this.labelSpriteSizeY.Name = "labelSpriteSizeY";
            this.labelSpriteSizeY.Size = new System.Drawing.Size(15, 13);
            this.labelSpriteSizeY.TabIndex = 7;
            this.labelSpriteSizeY.Text = "y:";
            // 
            // groupBoxPixelOptions
            // 
            this.groupBoxPixelOptions.Controls.Add(this.checkBoxFindPivots);
            this.groupBoxPixelOptions.Controls.Add(this.buttonSelectPivotColor);
            this.groupBoxPixelOptions.Controls.Add(this.checkBoxRemovePivot);
            this.groupBoxPixelOptions.Location = new System.Drawing.Point(6, 74);
            this.groupBoxPixelOptions.Name = "groupBoxPixelOptions";
            this.groupBoxPixelOptions.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBoxPixelOptions.Size = new System.Drawing.Size(174, 94);
            this.groupBoxPixelOptions.TabIndex = 12;
            this.groupBoxPixelOptions.TabStop = false;
            this.groupBoxPixelOptions.Text = "pivots";
            // 
            // checkBoxRemovePivot
            // 
            this.checkBoxRemovePivot.AutoSize = true;
            this.checkBoxRemovePivot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxRemovePivot.Location = new System.Drawing.Point(6, 68);
            this.checkBoxRemovePivot.Name = "checkBoxRemovePivot";
            this.checkBoxRemovePivot.Size = new System.Drawing.Size(92, 17);
            this.checkBoxRemovePivot.TabIndex = 7;
            this.checkBoxRemovePivot.Text = "remove pivots";
            this.checkBoxRemovePivot.UseVisualStyleBackColor = true;
            this.checkBoxRemovePivot.CheckStateChanged += new System.EventHandler(this.checkBoxRemovePixel_CheckStateChanged);
            // 
            // buttonSelectPivotColor
            // 
            this.buttonSelectPivotColor.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.buttonSelectPivotColor.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonSelectPivotColor.Location = new System.Drawing.Point(6, 17);
            this.buttonSelectPivotColor.Name = "buttonSelectPivotColor";
            this.buttonSelectPivotColor.Size = new System.Drawing.Size(162, 24);
            this.buttonSelectPivotColor.TabIndex = 5;
            this.buttonSelectPivotColor.Text = "set pivot color";
            this.buttonSelectPivotColor.UseVisualStyleBackColor = true;
            this.buttonSelectPivotColor.Click += new System.EventHandler(this.buttonSelectPixelColor_Click);
            // 
            // checkBoxFindPivots
            // 
            this.checkBoxFindPivots.AutoSize = true;
            this.checkBoxFindPivots.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxFindPivots.Location = new System.Drawing.Point(6, 46);
            this.checkBoxFindPivots.Name = "checkBoxFindPivots";
            this.checkBoxFindPivots.Size = new System.Drawing.Size(125, 17);
            this.checkBoxFindPivots.TabIndex = 6;
            this.checkBoxFindPivots.Text = "save pivots to textfile";
            this.checkBoxFindPivots.UseVisualStyleBackColor = true;
            this.checkBoxFindPivots.CheckedChanged += new System.EventHandler(this.checkBoxFindCoordinates_CheckedChanged);
            // 
            // groupBoxAlpha
            // 
            this.groupBoxAlpha.Controls.Add(this.checkBoxSetNewAlphaColor);
            this.groupBoxAlpha.Controls.Add(this.labelAlphaTo);
            this.groupBoxAlpha.Controls.Add(this.buttonAlphaToColor);
            this.groupBoxAlpha.Controls.Add(this.labelAlphaFrom);
            this.groupBoxAlpha.Controls.Add(this.numericUpDownAlphaFrom);
            this.groupBoxAlpha.Controls.Add(this.numericUpDownAlphaTo);
            this.groupBoxAlpha.Controls.Add(this.checkBoxRemoveAlpha);
            this.groupBoxAlpha.Controls.Add(this.checkBoxChangeAlpha);
            this.groupBoxAlpha.Location = new System.Drawing.Point(6, 174);
            this.groupBoxAlpha.Name = "groupBoxAlpha";
            this.groupBoxAlpha.Size = new System.Drawing.Size(174, 115);
            this.groupBoxAlpha.TabIndex = 14;
            this.groupBoxAlpha.TabStop = false;
            this.groupBoxAlpha.Text = "set alpha";
            // 
            // checkBoxChangeAlpha
            // 
            this.checkBoxChangeAlpha.AutoSize = true;
            this.checkBoxChangeAlpha.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxChangeAlpha.Location = new System.Drawing.Point(6, 63);
            this.checkBoxChangeAlpha.Name = "checkBoxChangeAlpha";
            this.checkBoxChangeAlpha.Size = new System.Drawing.Size(95, 17);
            this.checkBoxChangeAlpha.TabIndex = 8;
            this.checkBoxChangeAlpha.Text = "to fully opaque";
            this.checkBoxChangeAlpha.UseVisualStyleBackColor = true;
            this.checkBoxChangeAlpha.CheckedChanged += new System.EventHandler(this.checkBoxChangeAlpha_CheckedChanged);
            // 
            // checkBoxRemoveAlpha
            // 
            this.checkBoxRemoveAlpha.AutoSize = true;
            this.checkBoxRemoveAlpha.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxRemoveAlpha.Location = new System.Drawing.Point(6, 40);
            this.checkBoxRemoveAlpha.Name = "checkBoxRemoveAlpha";
            this.checkBoxRemoveAlpha.Size = new System.Drawing.Size(112, 17);
            this.checkBoxRemoveAlpha.TabIndex = 10;
            this.checkBoxRemoveAlpha.Text = "to fully transparent";
            this.checkBoxRemoveAlpha.UseVisualStyleBackColor = true;
            this.checkBoxRemoveAlpha.CheckedChanged += new System.EventHandler(this.checkBoxRemoveAlpha_CheckedChanged);
            // 
            // numericUpDownAlphaTo
            // 
            this.numericUpDownAlphaTo.Location = new System.Drawing.Point(114, 16);
            this.numericUpDownAlphaTo.Maximum = new decimal(new int[] {
            254,
            0,
            0,
            0});
            this.numericUpDownAlphaTo.Name = "numericUpDownAlphaTo";
            this.numericUpDownAlphaTo.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownAlphaTo.TabIndex = 12;
            // 
            // numericUpDownAlphaFrom
            // 
            this.numericUpDownAlphaFrom.Location = new System.Drawing.Point(37, 16);
            this.numericUpDownAlphaFrom.Maximum = new decimal(new int[] {
            254,
            0,
            0,
            0});
            this.numericUpDownAlphaFrom.Name = "numericUpDownAlphaFrom";
            this.numericUpDownAlphaFrom.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownAlphaFrom.TabIndex = 11;
            // 
            // labelAlphaFrom
            // 
            this.labelAlphaFrom.AutoSize = true;
            this.labelAlphaFrom.Location = new System.Drawing.Point(6, 18);
            this.labelAlphaFrom.Name = "labelAlphaFrom";
            this.labelAlphaFrom.Size = new System.Drawing.Size(30, 13);
            this.labelAlphaFrom.TabIndex = 13;
            this.labelAlphaFrom.Text = "from:";
            // 
            // buttonAlphaToColor
            // 
            this.buttonAlphaToColor.Location = new System.Drawing.Point(27, 81);
            this.buttonAlphaToColor.Name = "buttonAlphaToColor";
            this.buttonAlphaToColor.Size = new System.Drawing.Size(141, 23);
            this.buttonAlphaToColor.TabIndex = 16;
            this.buttonAlphaToColor.Text = "set new color";
            this.buttonAlphaToColor.UseVisualStyleBackColor = true;
            this.buttonAlphaToColor.Click += new System.EventHandler(this.buttonAlphaToColor_Click);
            // 
            // labelAlphaTo
            // 
            this.labelAlphaTo.AutoSize = true;
            this.labelAlphaTo.Location = new System.Drawing.Point(89, 18);
            this.labelAlphaTo.Name = "labelAlphaTo";
            this.labelAlphaTo.Size = new System.Drawing.Size(19, 13);
            this.labelAlphaTo.TabIndex = 14;
            this.labelAlphaTo.Text = "to:";
            // 
            // checkBoxSetNewAlphaColor
            // 
            this.checkBoxSetNewAlphaColor.AutoSize = true;
            this.checkBoxSetNewAlphaColor.Location = new System.Drawing.Point(6, 86);
            this.checkBoxSetNewAlphaColor.Name = "checkBoxSetNewAlphaColor";
            this.checkBoxSetNewAlphaColor.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSetNewAlphaColor.TabIndex = 17;
            this.checkBoxSetNewAlphaColor.UseVisualStyleBackColor = true;
            this.checkBoxSetNewAlphaColor.CheckedChanged += new System.EventHandler(this.checkBoxSetNewAlphaColor_CheckedChanged);
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOptions.Controls.Add(this.groupBoxAlpha);
            this.groupBoxOptions.Controls.Add(this.groupBoxPixelOptions);
            this.groupBoxOptions.Controls.Add(this.groupBoxSpriteSize);
            this.groupBoxOptions.Location = new System.Drawing.Point(384, 80);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(186, 295);
            this.groupBoxOptions.TabIndex = 7;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "options";
            // 
            // buttonAddListItem
            // 
            this.buttonAddListItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddListItem.Location = new System.Drawing.Point(384, 12);
            this.buttonAddListItem.Name = "buttonAddListItem";
            this.buttonAddListItem.Size = new System.Drawing.Size(186, 34);
            this.buttonAddListItem.TabIndex = 1;
            this.buttonAddListItem.Text = "add sprites";
            this.buttonAddListItem.UseVisualStyleBackColor = true;
            this.buttonAddListItem.Click += new System.EventHandler(this.buttonAddListItem_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.Location = new System.Drawing.Point(384, 459);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(186, 24);
            this.buttonExit.TabIndex = 13;
            this.buttonExit.Text = "exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonPivotToSpriteSheet
            // 
            this.buttonPivotToSpriteSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPivotToSpriteSheet.Location = new System.Drawing.Point(384, 420);
            this.buttonPivotToSpriteSheet.Name = "buttonPivotToSpriteSheet";
            this.buttonPivotToSpriteSheet.Size = new System.Drawing.Size(186, 34);
            this.buttonPivotToSpriteSheet.TabIndex = 12;
            this.buttonPivotToSpriteSheet.Text = "textfile to sprite";
            this.buttonPivotToSpriteSheet.UseVisualStyleBackColor = true;
            this.buttonPivotToSpriteSheet.Click += new System.EventHandler(this.buttonCoordinatesToSpriteSheet_Click);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 493);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(582, 18);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 14;
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 511);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonPivotToSpriteSheet);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonAddListItem);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonStartPixelfind);
            this.Controls.Add(this.buttonDeleteListItem);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.pictureBox);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(598, 550);
            this.Name = "MainWindow";
            this.Text = "Pivotfinder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.DragLeave += new System.EventHandler(this.Form1_DragLeave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBoxSpriteSize.ResumeLayout(false);
            this.groupBoxSpriteSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteWidth)).EndInit();
            this.groupBoxPixelOptions.ResumeLayout(false);
            this.groupBoxPixelOptions.PerformLayout();
            this.groupBoxAlpha.ResumeLayout(false);
            this.groupBoxAlpha.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAlphaTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAlphaFrom)).EndInit();
            this.groupBoxOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button buttonDeleteListItem;
        private System.Windows.Forms.Button buttonStartPixelfind;
        private System.Windows.Forms.GroupBox groupBoxSpriteSize;
        private System.Windows.Forms.Label labelSpriteSizeY;
        private System.Windows.Forms.Label labelSpriteSizeX;
        private System.Windows.Forms.NumericUpDown numericUpDownSpriteWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownSpriteHeight;
        private System.Windows.Forms.GroupBox groupBoxPixelOptions;
        private System.Windows.Forms.CheckBox checkBoxFindPivots;
        private System.Windows.Forms.Button buttonSelectPivotColor;
        private System.Windows.Forms.CheckBox checkBoxRemovePivot;
        private System.Windows.Forms.GroupBox groupBoxAlpha;
        private System.Windows.Forms.CheckBox checkBoxSetNewAlphaColor;
        private System.Windows.Forms.Label labelAlphaTo;
        private System.Windows.Forms.Button buttonAlphaToColor;
        private System.Windows.Forms.Label labelAlphaFrom;
        private System.Windows.Forms.NumericUpDown numericUpDownAlphaFrom;
        private System.Windows.Forms.NumericUpDown numericUpDownAlphaTo;
        private System.Windows.Forms.CheckBox checkBoxRemoveAlpha;
        private System.Windows.Forms.CheckBox checkBoxChangeAlpha;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Button buttonAddListItem;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonPivotToSpriteSheet;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

