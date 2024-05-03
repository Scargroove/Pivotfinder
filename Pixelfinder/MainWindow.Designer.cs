namespace Pixelfinder
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonDeleteListItem = new System.Windows.Forms.Button();
            this.buttonStartPixelfind = new System.Windows.Forms.Button();
            this.numericUpDownSpriteWidth = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSpriteHeight = new System.Windows.Forms.NumericUpDown();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonSelectPixelColor = new System.Windows.Forms.Button();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.pictureBoxPickedColor = new System.Windows.Forms.PictureBox();
            this.groupBoxSpriteSize = new System.Windows.Forms.GroupBox();
            this.labelSpriteSizeY = new System.Windows.Forms.Label();
            this.labelSpriteSizeX = new System.Windows.Forms.Label();
            this.checkBoxRemovePixel = new System.Windows.Forms.CheckBox();
            this.buttonAddListItem = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBoxOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPickedColor)).BeginInit();
            this.groupBoxSpriteSize.SuspendLayout();
            this.SuspendLayout();
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
            this.listBox.Size = new System.Drawing.Size(360, 121);
            this.listBox.TabIndex = 1;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
            this.listBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_DragEnter);
            this.listBox.DragLeave += new System.EventHandler(this.listBox_DragLeave);
            // 
            // buttonDeleteListItem
            // 
            this.buttonDeleteListItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteListItem.Location = new System.Drawing.Point(383, 51);
            this.buttonDeleteListItem.Name = "buttonDeleteListItem";
            this.buttonDeleteListItem.Size = new System.Drawing.Size(186, 23);
            this.buttonDeleteListItem.TabIndex = 2;
            this.buttonDeleteListItem.Text = "delete spritesheet";
            this.buttonDeleteListItem.UseVisualStyleBackColor = true;
            this.buttonDeleteListItem.Click += new System.EventHandler(this.buttonDeleteListItem_Click);
            // 
            // buttonStartPixelfind
            // 
            this.buttonStartPixelfind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartPixelfind.Location = new System.Drawing.Point(383, 264);
            this.buttonStartPixelfind.Name = "buttonStartPixelfind";
            this.buttonStartPixelfind.Size = new System.Drawing.Size(186, 49);
            this.buttonStartPixelfind.TabIndex = 3;
            this.buttonStartPixelfind.Text = "start";
            this.buttonStartPixelfind.UseVisualStyleBackColor = true;
            this.buttonStartPixelfind.Click += new System.EventHandler(this.startPixelfind_Click);
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
            this.numericUpDownSpriteWidth.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownSpriteWidth.TabIndex = 4;
            this.numericUpDownSpriteWidth.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
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
            this.numericUpDownSpriteHeight.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownSpriteHeight.TabIndex = 5;
            this.numericUpDownSpriteHeight.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Location = new System.Drawing.Point(12, 145);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(360, 202);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.PictureBox_DragDrop);
            this.pictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.PictureBox_DragEnter);
            this.pictureBox.DragLeave += new System.EventHandler(this.PictureBox_DragLeave);
            // 
            // buttonSelectPixelColor
            // 
            this.buttonSelectPixelColor.Location = new System.Drawing.Point(6, 74);
            this.buttonSelectPixelColor.Name = "buttonSelectPixelColor";
            this.buttonSelectPixelColor.Size = new System.Drawing.Size(120, 23);
            this.buttonSelectPixelColor.TabIndex = 6;
            this.buttonSelectPixelColor.Text = "change pixel color";
            this.buttonSelectPixelColor.UseVisualStyleBackColor = true;
            this.buttonSelectPixelColor.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOptions.Controls.Add(this.pictureBoxPickedColor);
            this.groupBoxOptions.Controls.Add(this.groupBoxSpriteSize);
            this.groupBoxOptions.Controls.Add(this.checkBoxRemovePixel);
            this.groupBoxOptions.Controls.Add(this.buttonSelectPixelColor);
            this.groupBoxOptions.Location = new System.Drawing.Point(383, 80);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(186, 178);
            this.groupBoxOptions.TabIndex = 7;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "options";
            // 
            // pictureBoxPickedColor
            // 
            this.pictureBoxPickedColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxPickedColor.Location = new System.Drawing.Point(132, 76);
            this.pictureBoxPickedColor.Name = "pictureBoxPickedColor";
            this.pictureBoxPickedColor.Size = new System.Drawing.Size(19, 19);
            this.pictureBoxPickedColor.TabIndex = 10;
            this.pictureBoxPickedColor.TabStop = false;
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
            // labelSpriteSizeY
            // 
            this.labelSpriteSizeY.AutoSize = true;
            this.labelSpriteSizeY.Location = new System.Drawing.Point(86, 21);
            this.labelSpriteSizeY.Name = "labelSpriteSizeY";
            this.labelSpriteSizeY.Size = new System.Drawing.Size(12, 13);
            this.labelSpriteSizeY.TabIndex = 7;
            this.labelSpriteSizeY.Text = "y";
            // 
            // labelSpriteSizeX
            // 
            this.labelSpriteSizeX.AutoSize = true;
            this.labelSpriteSizeX.Location = new System.Drawing.Point(9, 21);
            this.labelSpriteSizeX.Name = "labelSpriteSizeX";
            this.labelSpriteSizeX.Size = new System.Drawing.Size(12, 13);
            this.labelSpriteSizeX.TabIndex = 6;
            this.labelSpriteSizeX.Text = "x";
            // 
            // checkBoxRemovePixel
            // 
            this.checkBoxRemovePixel.AutoSize = true;
            this.checkBoxRemovePixel.Location = new System.Drawing.Point(6, 103);
            this.checkBoxRemovePixel.Name = "checkBoxRemovePixel";
            this.checkBoxRemovePixel.Size = new System.Drawing.Size(85, 17);
            this.checkBoxRemovePixel.TabIndex = 7;
            this.checkBoxRemovePixel.Text = "remove pixel";
            this.checkBoxRemovePixel.UseVisualStyleBackColor = true;
            this.checkBoxRemovePixel.CheckStateChanged += new System.EventHandler(this.CheckBoxRemovePixel_CheckStateChanged);
            // 
            // buttonAddListItem
            // 
            this.buttonAddListItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddListItem.Location = new System.Drawing.Point(383, 12);
            this.buttonAddListItem.Name = "buttonAddListItem";
            this.buttonAddListItem.Size = new System.Drawing.Size(186, 33);
            this.buttonAddListItem.TabIndex = 8;
            this.buttonAddListItem.Text = "add spritesheet";
            this.buttonAddListItem.UseVisualStyleBackColor = true;
            this.buttonAddListItem.Click += new System.EventHandler(this.buttonAddListItem_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.Location = new System.Drawing.Point(383, 319);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(186, 32);
            this.buttonExit.TabIndex = 9;
            this.buttonExit.Text = "exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 359);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonAddListItem);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonStartPixelfind);
            this.Controls.Add(this.buttonDeleteListItem);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.pictureBox);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(598, 398);
            this.Name = "MainWindow";
            this.Text = "Pixelfinder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.DragLeave += new System.EventHandler(this.Form1_DragLeave);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPickedColor)).EndInit();
            this.groupBoxSpriteSize.ResumeLayout(false);
            this.groupBoxSpriteSize.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button buttonDeleteListItem;
        private System.Windows.Forms.Button buttonStartPixelfind;
        private System.Windows.Forms.NumericUpDown numericUpDownSpriteWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownSpriteHeight;
        private System.Windows.Forms.Button buttonSelectPixelColor;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Button buttonAddListItem;
        private System.Windows.Forms.CheckBox checkBoxRemovePixel;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.GroupBox groupBoxSpriteSize;
        private System.Windows.Forms.Label labelSpriteSizeY;
        private System.Windows.Forms.Label labelSpriteSizeX;
        private System.Windows.Forms.PictureBox pictureBoxPickedColor;
        private System.Windows.Forms.ImageList imageList1;
    }
}

