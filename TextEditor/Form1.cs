using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        [Serializable]
        public class Order
        {
            public bool WordWrap { get; set; }
            public bool StatusBar { get; set; }
            public string FileName { get; set; }
            public float ZoomFactor;

            public Order()
            {
                FileName = string.Empty;
                ZoomFactor = 1f;
            }

            public Order(bool wordWrap, bool statusBar, string fileName, float zoomFactor)
            {
                WordWrap = wordWrap;
                StatusBar = statusBar;
                FileName = fileName;
                ZoomFactor = zoomFactor;
            }
        }

        private readonly string SettingsFileName = @"settings.xml";

        private string FileName = string.Empty;
        private StringBuilder savedText = new StringBuilder();

        private const double minimumFontSize = 0.9, maximumFontSize = 2;

        public Form1()
        {
            InitializeComponent();

            if (File.Exists(SettingsFileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Order));
                try
                {
                    using (Stream stream = File.OpenRead(SettingsFileName))
                    {
                        var ob = serializer.Deserialize(stream);
                        if (ob is Order order)
                        {
                            textBox.WordWrap = order.WordWrap;
                            statusBarToolStripMenuItem.Checked = order.StatusBar;
                            statusStrip1.Visible = order.StatusBar;
                            textBox.ZoomFactor = order.ZoomFactor;
                            
                            if (!string.IsNullOrEmpty(order.FileName) && File.Exists(order.FileName))
                            {
                                FileName = order.FileName;
                                stream.Dispose();
                                LoadData(order.FileName);
                            }
                        }
                        else
                        {
                            stream.Dispose();
                            File.Delete(SettingsFileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    File.Delete(SettingsFileName);
                    this.Close();
                }
            }

            this.MinimumSize = new Size(512, 256+128);

            textBox.ContextMenuStrip = textBoxContextMenuStrip;

            UpdateTextBoxContextMenu();
            UpdateToolStripMenu();
            UpdateStatusBar();
            UpdateTitle();
        }

        #region Form1 actions
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (NonSaveWarning(sender, e))
                    e.Cancel = true;
            }
        }

        #endregion

        #region menu strip
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NonSaveWarning(sender, e))
                return;
            textBox.Text = string.Empty;
            savedText.Clear();
            FileName = string.Empty;
            UpdateStatusBar();
            UpdateTitle();
            SaveSetting();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NonSaveWarning(sender, e))
                return;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Rich text file (*.rtf)|*.rtf|Text file (*.txt)|*.txt|All files(*.*)|*.*";
            dialog.DefaultExt = ".rtf";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadData(dialog.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                SaveData(FileName);
                SaveSetting();
            }
            UpdateToolStripMenu();
            UpdateTitle();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (true)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Rich text file (*.rtf)|*.rtf|Text file (*.txt)|*.txt|All files(*.*)|*.*";
                dialog.DefaultExt = ".rtf";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(dialog.FileName);
                    if (ext != ".rtf")
                    {
                        var res = MessageBox.Show($"When saving a file in '{ext}' format, you may lose some data. Are you sure you want to continue?", "Possible data loss", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                        if (res == DialogResult.Yes)
                        {
                            SaveData(dialog.FileName);
                            break;
                        }
                        else if (res == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                    else
                    {
                        SaveData(dialog.FileName);
                        break;
                    }
                }
            }
            UpdateToolStripMenu();
            UpdateTitle();
            SaveSetting();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NonSaveWarning(sender, e))
                return;
            this.Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox.CanUndo)
                textBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox.CanRedo)
                textBox.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste(DataFormats.GetFormat(DataFormats.Text));
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FindForm(textBox);
            form.Show();
        }

        private void makeUpercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.SelectedText))
                return;
            textBox.SelectedText = textBox.SelectedText.ToUpper();
        }

        private void makeLowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.SelectedText))
                return;
            textBox.SelectedText = textBox.SelectedText.ToLower();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.SelectedText))
                return;
            var dialog = new FontDialog();
            dialog.FontMustExist = true;
            dialog.Font = textBox.Font;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.SelectionFont = dialog.Font;
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.SelectedText))
                return;
            var dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.SelectionColor = dialog.Color;
            }
        }

        private void resetStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.SelectedText))
                return;
            textBox.SelectionColor = textBox.ForeColor;
            textBox.SelectionFont = textBox.Font;
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.WordWrap = wordWrapToolStripMenuItem.Checked;
            SaveSetting();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox.ZoomFactor < maximumFontSize)
                textBox.ZoomFactor += 0.1f;
            UpdateStatusBar();
            SaveSetting();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox.ZoomFactor > minimumFontSize)
                textBox.ZoomFactor -= 0.1f;
            UpdateStatusBar();
            SaveSetting();
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = statusBarToolStripMenuItem.Checked;
            SaveSetting();
        }

        #endregion

        #region context menu strip
        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            undoToolStripMenuItem_Click(sender, e);
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            redoToolStripMenuItem_Click(sender, e);
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutToolStripMenuItem_Click(sender, e);
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyToolStripMenuItem_Click(sender, e);
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pasteToolStripMenuItem_Click(sender, e);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.SelectAll();
        }

        private void makeUpercaseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            makeUpercaseToolStripMenuItem_Click(sender, e);
        }

        private void makeLowercaseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            makeLowercaseToolStripMenuItem_Click(sender, e);
        }

        private void fontToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fontToolStripMenuItem_Click(sender, e);
        }

        private void colorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            colorToolStripMenuItem_Click(sender, e);
        }

        private void resetStyleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            resetStyleToolStripMenuItem_Click(sender, e);
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.SelectedText))
                return;
            try
            {
                var url = $"http://google.com/search?q={textBox.SelectedText}".Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region textBox
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (Clipboard.ContainsText())// prevent cases when the user tries to insert something other than text (for example, graphics)
                    textBox.Paste(DataFormats.GetFormat(DataFormats.Text));
                e.Handled = true;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            UpdateToolStripMenu();
            UpdateTitle();

            SaveSetting();
        }

        private void textBox_SelectionChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
            UpdateToolStripMenu();
            UpdateTextBoxContextMenu();
        }
        #endregion

        private void UpdateToolStripMenu()
        {
            bool value = !string.IsNullOrEmpty(textBox.SelectedText);

            cutToolStripMenuItem.Enabled = value;
            copyToolStripMenuItem.Enabled = value;
            pasteToolStripMenuItem.Enabled = Clipboard.ContainsText();

            wordWrapToolStripMenuItem.Checked = textBox.WordWrap;

            redoToolStripMenuItem.Enabled = textBox.CanRedo;
            undoToolStripMenuItem.Enabled = textBox.CanUndo;

            transformationsToolStripMenuItem.Enabled = value;
            styleToolStripMenuItem.Enabled = value;
        }

        private void UpdateStatusBar()
        {
            int line = textBox.GetLineFromCharIndex(textBox.SelectionStart);
            int column = textBox.SelectionStart - textBox.GetFirstCharIndexFromLine(line);

            lnCol_toolStripStatusLabel.Text = $"Ln {line + 1} Col {column + 1}";
            zoom_toolStripStatusLabel.Text = $"{Math.Round(textBox.ZoomFactor * 100)}%";
        }

        private void UpdateTextBoxContextMenu()
        {
            bool value = !string.IsNullOrEmpty(textBox.SelectedText);

            undoToolStripMenuItem1.Enabled = textBox.CanUndo;
            redoToolStripMenuItem1.Enabled = textBox.CanRedo;
            cutToolStripMenuItem1.Enabled = value;
            copyToolStripMenuItem1.Enabled = value;
            pasteToolStripMenuItem1.Enabled = Clipboard.ContainsText();
            selectAllToolStripMenuItem.Enabled = !string.IsNullOrEmpty(textBox.Text);
            transformationsToolStripMenuItem1.Enabled = value;
            styleToolStripMenuItem1.Enabled = value;
            searchToolStripMenuItem.Enabled = value;
        }

        private void UpdateTitle()
        {
            string title = string.Empty;
            if (!IsSaved())
                title += '*';
            if (string.IsNullOrEmpty(FileName))
                title += $"Untitled - {Application.ProductName}";
            else
                title += $"{GetFileName()} - {Application.ProductName}";
            this.Text = title;
        }

        private bool NonSaveWarning(object sender, EventArgs e)
        {
            if (!IsSaved())
            {
                var res = MessageBox.Show($"Do you want to save changes to {GetFileName()}?", GetFileName(), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                }
                else if (res == DialogResult.Cancel)
                    return true;
            }
            return false;
        }

        private string GetFileName()
        {
            if (string.IsNullOrEmpty(FileName))
                return "Untitled";
            return Path.GetFileNameWithoutExtension(FileName);
        }

        private void SaveSetting()
        {
            var order = new Order(textBox.WordWrap, statusBarToolStripMenuItem.Checked, FileName, textBox.ZoomFactor);
            XmlSerializer serializer = new XmlSerializer(typeof(Order));
            using(Stream stream = File.Create(SettingsFileName))
            {
                serializer.Serialize(stream, order);
            }
        }

        private void SaveData(string fileName)
        {
            try
            {
                using (File.Create(fileName)) { }
                if (Path.GetExtension(fileName) == ".rtf")
                {
                    textBox.SaveFile(fileName, RichTextBoxStreamType.RichText);
                }
                else
                {
                    textBox.SaveFile(fileName, RichTextBoxStreamType.PlainText);
                }
                savedText.Clear();
                savedText.Append(textBox.Text);
                if (FileName != fileName)
                    FileName = fileName;
            }
            catch (Exception ex)
            {
                if(File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsSaved()
        {
            if (string.IsNullOrEmpty(FileName) && string.IsNullOrEmpty(textBox.Text))
                return true;
            if (string.IsNullOrEmpty(FileName))
                return false;
            return savedText.ToString() == textBox.Text;
        }

        private void LoadData(string path)
        {
            try
            {
                if (Path.GetExtension(path) == ".rtf")
                {
                    textBox.LoadFile(path, RichTextBoxStreamType.RichText);
                }
                else 
                {
                    textBox.LoadFile(path, RichTextBoxStreamType.PlainText);
                }
                savedText.Clear();
                savedText.Append(textBox.Text);
                FileName = path;
                UpdateStatusBar();
                UpdateTitle();
                SaveSetting();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}