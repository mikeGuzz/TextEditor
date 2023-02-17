namespace TextEditor
{
    partial class FindForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.findWhat_textBox = new System.Windows.Forms.TextBox();
            this.find_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.down_radioButton = new System.Windows.Forms.RadioButton();
            this.up_radioButton = new System.Windows.Forms.RadioButton();
            this.matchCase_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find what:";
            // 
            // findWhat_textBox
            // 
            this.findWhat_textBox.Location = new System.Drawing.Point(12, 27);
            this.findWhat_textBox.Name = "findWhat_textBox";
            this.findWhat_textBox.Size = new System.Drawing.Size(291, 23);
            this.findWhat_textBox.TabIndex = 1;
            this.findWhat_textBox.TextChanged += new System.EventHandler(this.findWhat_textBox_TextChanged);
            // 
            // find_button
            // 
            this.find_button.Location = new System.Drawing.Point(147, 128);
            this.find_button.Name = "find_button";
            this.find_button.Size = new System.Drawing.Size(75, 23);
            this.find_button.TabIndex = 2;
            this.find_button.Text = "Find";
            this.find_button.UseVisualStyleBackColor = true;
            this.find_button.Click += new System.EventHandler(this.find_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(228, 128);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 3;
            this.cancel_button.Text = "Cancel";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.down_radioButton);
            this.groupBox1.Controls.Add(this.up_radioButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(111, 48);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Direction";
            // 
            // down_radioButton
            // 
            this.down_radioButton.AutoSize = true;
            this.down_radioButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.down_radioButton.Location = new System.Drawing.Point(52, 22);
            this.down_radioButton.Name = "down_radioButton";
            this.down_radioButton.Size = new System.Drawing.Size(56, 19);
            this.down_radioButton.TabIndex = 1;
            this.down_radioButton.TabStop = true;
            this.down_radioButton.Text = "Down";
            this.down_radioButton.UseVisualStyleBackColor = true;
            // 
            // up_radioButton
            // 
            this.up_radioButton.AutoSize = true;
            this.up_radioButton.Checked = true;
            this.up_radioButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.up_radioButton.Location = new System.Drawing.Point(6, 22);
            this.up_radioButton.Name = "up_radioButton";
            this.up_radioButton.Size = new System.Drawing.Size(40, 19);
            this.up_radioButton.TabIndex = 0;
            this.up_radioButton.TabStop = true;
            this.up_radioButton.Text = "Up";
            this.up_radioButton.UseVisualStyleBackColor = true;
            // 
            // matchCase_checkBox
            // 
            this.matchCase_checkBox.AutoSize = true;
            this.matchCase_checkBox.Location = new System.Drawing.Point(129, 60);
            this.matchCase_checkBox.Name = "matchCase_checkBox";
            this.matchCase_checkBox.Size = new System.Drawing.Size(86, 19);
            this.matchCase_checkBox.TabIndex = 5;
            this.matchCase_checkBox.Text = "Match case";
            this.matchCase_checkBox.UseVisualStyleBackColor = true;
            this.matchCase_checkBox.CheckedChanged += new System.EventHandler(this.matchCase_checkBox_CheckedChanged);
            // 
            // FindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 159);
            this.Controls.Add(this.matchCase_checkBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.find_button);
            this.Controls.Add(this.findWhat_textBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.Text = "Find";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox findWhat_textBox;
        private Button find_button;
        private Button cancel_button;
        private GroupBox groupBox1;
        private RadioButton down_radioButton;
        private RadioButton up_radioButton;
        private CheckBox matchCase_checkBox;
    }
}