namespace TextCheckAndInserter
{
    partial class TextCheckAndInserterForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tBoxTargetTextFilepath = new System.Windows.Forms.TextBox();
            this.bTargetTextFilepath = new System.Windows.Forms.Button();
            this.tBoxTargetText = new System.Windows.Forms.TextBox();
            this.LBoxExtention = new System.Windows.Forms.ListBox();
            this.tBoxExtenstion = new System.Windows.Forms.TextBox();
            this.tBoxStatus = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.bDirectory = new System.Windows.Forms.Button();
            this.tBoxSetDirectory = new System.Windows.Forms.TextBox();
            this.bExcute = new System.Windows.Forms.Button();
            this.tBoxCompareLine = new System.Windows.Forms.TextBox();
            this.tBoxInserLine = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tBoxTargetTextFilepath
            // 
            this.tBoxTargetTextFilepath.AllowDrop = true;
            this.tBoxTargetTextFilepath.Location = new System.Drawing.Point(12, 12);
            this.tBoxTargetTextFilepath.Name = "tBoxTargetTextFilepath";
            this.tBoxTargetTextFilepath.Size = new System.Drawing.Size(457, 22);
            this.tBoxTargetTextFilepath.TabIndex = 0;
            this.tBoxTargetTextFilepath.DragDrop += new System.Windows.Forms.DragEventHandler(this.tBoxTargetTextFilepath_DragDrop);
            this.tBoxTargetTextFilepath.DragEnter += new System.Windows.Forms.DragEventHandler(this.tBoxTargetTextFilepath_DragEnter);
            // 
            // bTargetTextFilepath
            // 
            this.bTargetTextFilepath.Location = new System.Drawing.Point(475, 10);
            this.bTargetTextFilepath.Name = "bTargetTextFilepath";
            this.bTargetTextFilepath.Size = new System.Drawing.Size(75, 23);
            this.bTargetTextFilepath.TabIndex = 1;
            this.bTargetTextFilepath.Text = "TargetText";
            this.bTargetTextFilepath.UseVisualStyleBackColor = true;
            this.bTargetTextFilepath.Click += new System.EventHandler(this.bTargetTextFilepath_Click);
            // 
            // tBoxTargetText
            // 
            this.tBoxTargetText.Enabled = false;
            this.tBoxTargetText.Location = new System.Drawing.Point(12, 191);
            this.tBoxTargetText.Multiline = true;
            this.tBoxTargetText.Name = "tBoxTargetText";
            this.tBoxTargetText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tBoxTargetText.Size = new System.Drawing.Size(326, 407);
            this.tBoxTargetText.TabIndex = 2;
            this.tBoxTargetText.WordWrap = false;
            // 
            // LBoxExtention
            // 
            this.LBoxExtention.FormattingEnabled = true;
            this.LBoxExtention.ItemHeight = 12;
            this.LBoxExtention.Location = new System.Drawing.Point(12, 69);
            this.LBoxExtention.Name = "LBoxExtention";
            this.LBoxExtention.Size = new System.Drawing.Size(185, 88);
            this.LBoxExtention.TabIndex = 4;
            this.LBoxExtention.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LBoxExtention_MouseDoubleClick);
            // 
            // tBoxExtenstion
            // 
            this.tBoxExtenstion.Location = new System.Drawing.Point(12, 163);
            this.tBoxExtenstion.Name = "tBoxExtenstion";
            this.tBoxExtenstion.Size = new System.Drawing.Size(185, 22);
            this.tBoxExtenstion.TabIndex = 5;
            this.tBoxExtenstion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tBoxExtenstion_KeyPress);
            // 
            // tBoxStatus
            // 
            this.tBoxStatus.Location = new System.Drawing.Point(344, 69);
            this.tBoxStatus.Multiline = true;
            this.tBoxStatus.Name = "tBoxStatus";
            this.tBoxStatus.ReadOnly = true;
            this.tBoxStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tBoxStatus.Size = new System.Drawing.Size(340, 529);
            this.tBoxStatus.TabIndex = 6;
            this.tBoxStatus.WordWrap = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // bDirectory
            // 
            this.bDirectory.Location = new System.Drawing.Point(475, 40);
            this.bDirectory.Name = "bDirectory";
            this.bDirectory.Size = new System.Drawing.Size(75, 23);
            this.bDirectory.TabIndex = 7;
            this.bDirectory.Text = "SetDirectory";
            this.bDirectory.UseVisualStyleBackColor = true;
            this.bDirectory.Click += new System.EventHandler(this.bDirectory_Click);
            // 
            // tBoxSetDirectory
            // 
            this.tBoxSetDirectory.AllowDrop = true;
            this.tBoxSetDirectory.Location = new System.Drawing.Point(12, 40);
            this.tBoxSetDirectory.Name = "tBoxSetDirectory";
            this.tBoxSetDirectory.Size = new System.Drawing.Size(457, 22);
            this.tBoxSetDirectory.TabIndex = 8;
            // 
            // bExcute
            // 
            this.bExcute.Location = new System.Drawing.Point(556, 12);
            this.bExcute.Name = "bExcute";
            this.bExcute.Size = new System.Drawing.Size(119, 51);
            this.bExcute.TabIndex = 9;
            this.bExcute.Text = "Excute";
            this.bExcute.UseVisualStyleBackColor = true;
            this.bExcute.Click += new System.EventHandler(this.bExcute_Click);
            // 
            // tBoxCompareLine
            // 
            this.tBoxCompareLine.Location = new System.Drawing.Point(203, 82);
            this.tBoxCompareLine.Name = "tBoxCompareLine";
            this.tBoxCompareLine.Size = new System.Drawing.Size(135, 22);
            this.tBoxCompareLine.TabIndex = 10;
            // 
            // tBoxInserLine
            // 
            this.tBoxInserLine.Location = new System.Drawing.Point(203, 110);
            this.tBoxInserLine.Name = "tBoxInserLine";
            this.tBoxInserLine.Size = new System.Drawing.Size(135, 22);
            this.tBoxInserLine.TabIndex = 11;
            // 
            // TextCheckAndInserterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 605);
            this.Controls.Add(this.tBoxInserLine);
            this.Controls.Add(this.tBoxCompareLine);
            this.Controls.Add(this.bExcute);
            this.Controls.Add(this.tBoxSetDirectory);
            this.Controls.Add(this.bDirectory);
            this.Controls.Add(this.tBoxStatus);
            this.Controls.Add(this.tBoxExtenstion);
            this.Controls.Add(this.LBoxExtention);
            this.Controls.Add(this.tBoxTargetText);
            this.Controls.Add(this.bTargetTextFilepath);
            this.Controls.Add(this.tBoxTargetTextFilepath);
            this.KeyPreview = true;
            this.Name = "TextCheckAndInserterForm";
            this.Text = "TextCheckAndInserter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextCheckAndInserterForm_FormClosing);
            this.Load += new System.EventHandler(this.TextCheckAndInserterForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextCheckAndInserterForm_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tBoxTargetTextFilepath;
        private System.Windows.Forms.Button bTargetTextFilepath;
        private System.Windows.Forms.TextBox tBoxTargetText;
        private System.Windows.Forms.ListBox LBoxExtention;
        private System.Windows.Forms.TextBox tBoxExtenstion;
        private System.Windows.Forms.TextBox tBoxStatus;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button bDirectory;
        private System.Windows.Forms.TextBox tBoxSetDirectory;
        private System.Windows.Forms.Button bExcute;
        private System.Windows.Forms.TextBox tBoxCompareLine;
        private System.Windows.Forms.TextBox tBoxInserLine;
    }
}

