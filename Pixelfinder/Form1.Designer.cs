namespace Pixelfinder
{
    partial class Form1
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonDeleteListItem = new System.Windows.Forms.Button();
            this.startPixelfind = new System.Windows.Forms.Button();
            this.numericUpDownSpriteWidth = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSpriteHeight = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(12, 343);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(362, 95);
            this.listBox.TabIndex = 1;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
            this.listBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_DragEnter);
            this.listBox.DragLeave += new System.EventHandler(this.listBox_DragLeave);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BackgroundImage = global::Pixelfinder.Properties.Resources.drag_and_drop;
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Location = new System.Drawing.Point(508, 213);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(280, 225);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.PictureBox_DragDrop);
            this.pictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.PictureBox_DragEnter);
            this.pictureBox.DragLeave += new System.EventHandler(this.pictureBox_DragLeave);
            // 
            // buttonDeleteListItem
            // 
            this.buttonDeleteListItem.Location = new System.Drawing.Point(381, 343);
            this.buttonDeleteListItem.Name = "buttonDeleteListItem";
            this.buttonDeleteListItem.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteListItem.TabIndex = 2;
            this.buttonDeleteListItem.Text = "delete";
            this.buttonDeleteListItem.UseVisualStyleBackColor = true;
            this.buttonDeleteListItem.Click += new System.EventHandler(this.buttonDeleteListItem_Click);
            // 
            // startPixelfind
            // 
            this.startPixelfind.Location = new System.Drawing.Point(381, 280);
            this.startPixelfind.Name = "startPixelfind";
            this.startPixelfind.Size = new System.Drawing.Size(75, 23);
            this.startPixelfind.TabIndex = 3;
            this.startPixelfind.Text = "start";
            this.startPixelfind.UseVisualStyleBackColor = true;
            this.startPixelfind.Click += new System.EventHandler(this.startPixelfind_Click);
            // 
            // numericUpDownSpriteWidth
            // 
            this.numericUpDownSpriteWidth.Location = new System.Drawing.Point(508, 76);
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
            this.numericUpDownSpriteWidth.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownSpriteWidth.TabIndex = 4;
            this.numericUpDownSpriteWidth.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // numericUpDownSpriteHeight
            // 
            this.numericUpDownSpriteHeight.Location = new System.Drawing.Point(508, 134);
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
            this.numericUpDownSpriteHeight.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownSpriteHeight.TabIndex = 5;
            this.numericUpDownSpriteHeight.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.numericUpDownSpriteHeight);
            this.Controls.Add(this.numericUpDownSpriteWidth);
            this.Controls.Add(this.startPixelfind);
            this.Controls.Add(this.buttonDeleteListItem);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.pictureBox);
            this.Name = "Form1";
            this.Text = "Pixlelfinder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.DragLeave += new System.EventHandler(this.Form1_DragLeave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpriteHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button buttonDeleteListItem;
        private System.Windows.Forms.Button startPixelfind;
        private System.Windows.Forms.NumericUpDown numericUpDownSpriteWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownSpriteHeight;
    }
}

