using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TextEditor
{
    public partial class FindForm : Form
    {
        public static readonly string SettingsFilePath = @"findFormSetting.xml";

        private RichTextBox textBox;

        public FindForm(RichTextBox textBox)
        {
            if (textBox != null)
                this.textBox = textBox;
            else
                throw new ArgumentNullException(nameof(textBox));

            InitializeComponent();

            if (!string.IsNullOrEmpty(textBox.SelectedText))
            {
                findWhat_textBox.Text = textBox.SelectedText;
                findWhat_textBox.SelectAll();
            }

            #region load data
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(bool));
                    using (Stream stream = File.OpenRead(SettingsFilePath))
                    {
                        if (serializer.Deserialize(stream) is bool value)
                        {
                            stream.Dispose();
                            matchCase_checkBox.Checked = value;
                        }
                        else
                        {
                            stream.Dispose();
                            File.Delete(SettingsFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            #endregion

            UpdateFindButtonState();
        }

        private void find_button_Click(object sender, EventArgs e)
        {
            int index = -1;

            if (matchCase_checkBox.Checked)
            {
                index = up_radioButton.Checked ? textBox.Text.IndexOf(findWhat_textBox.Text) : textBox.Text.LastIndexOf(findWhat_textBox.Text);
            }
            else
            {
                StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
                index = up_radioButton.Checked ? textBox.Text.IndexOf(findWhat_textBox.Text, comparison) : textBox.Text.LastIndexOf(findWhat_textBox.Text, comparison);
            }

            if (index == -1)
            {
                MessageBox.Show($"Cannot find {'"' + findWhat_textBox.Text + '"'}", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                textBox.Select(index, findWhat_textBox.Text.Length);
                textBox.ScrollToCaret();
            }
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void matchCase_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            #region save data
            XmlSerializer serializer = new XmlSerializer(typeof(bool));
            using(Stream stream = File.Create(SettingsFilePath))
            {
                serializer.Serialize(stream, matchCase_checkBox.Checked);
            }
            #endregion
        }

        private void findWhat_textBox_TextChanged(object sender, EventArgs e)
        {
            UpdateFindButtonState();
        }

        private void UpdateFindButtonState()
        {
            if (!string.IsNullOrEmpty(findWhat_textBox.Text))
                find_button.Enabled = true;
            else
                find_button.Enabled = false;
        }
    }
}
