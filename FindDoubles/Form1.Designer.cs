namespace FindDoubles
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Dirs = new System.Windows.Forms.DataGridView();
            this.detail = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.log = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.PathText = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.min_mb = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.error = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Dirs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_mb)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(707, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Поиск";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(12, 25);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(689, 67);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "d:\\[MAIN_MEDIA]\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Папки поиска";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 504);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "label3";
            // 
            // Dirs
            // 
            this.Dirs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dirs.Location = new System.Drawing.Point(12, 98);
            this.Dirs.Name = "Dirs";
            this.Dirs.Size = new System.Drawing.Size(770, 182);
            this.Dirs.TabIndex = 9;
            this.Dirs.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Dirs_CellMouseClick);
            this.Dirs.SelectionChanged += new System.EventHandler(this.Dirs_SelectionChanged);
            this.Dirs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Dirs_MouseDoubleClick);
            // 
            // detail
            // 
            this.detail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.detail.Location = new System.Drawing.Point(12, 286);
            this.detail.Name = "detail";
            this.detail.Size = new System.Drawing.Size(770, 193);
            this.detail.TabIndex = 10;
            this.detail.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.detail_MouseDoubleClick);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(788, 96);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Удалить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.Location = new System.Drawing.Point(869, 98);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.log.Size = new System.Drawing.Size(565, 182);
            this.log.TabIndex = 12;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(788, 286);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "Удалить";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // PathText
            // 
            this.PathText.Location = new System.Drawing.Point(15, 546);
            this.PathText.Name = "PathText";
            this.PathText.Size = new System.Drawing.Size(686, 20);
            this.PathText.TabIndex = 14;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(707, 543);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 15;
            this.button4.Text = "Удалить";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // min_mb
            // 
            this.min_mb.DecimalPlaces = 1;
            this.min_mb.Location = new System.Drawing.Point(869, 543);
            this.min_mb.Name = "min_mb";
            this.min_mb.Size = new System.Drawing.Size(65, 20);
            this.min_mb.TabIndex = 16;
            this.min_mb.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(940, 545);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "mb";
            // 
            // error
            // 
            this.error.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.error.Location = new System.Drawing.Point(869, 297);
            this.error.Multiline = true;
            this.error.Name = "error";
            this.error.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.error.Size = new System.Drawing.Size(565, 182);
            this.error.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1446, 588);
            this.Controls.Add(this.error);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.min_mb);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.PathText);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.log);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.detail);
            this.Controls.Add(this.Dirs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Dirs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.min_mb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView Dirs;
        private System.Windows.Forms.DataGridView detail;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox log;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox PathText;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.NumericUpDown min_mb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox error;
    }
}

