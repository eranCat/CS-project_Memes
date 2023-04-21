using CS_project.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_project
{
    public partial class CreateMemeForm : Form
    {
        bool generateOnEdit = true;//TODO save user's settigns with checkbox 

        public CreateMemeForm()
        {
            InitializeComponent();
            setFormSizePercentage(.8,.8);
        }

        private void setFormSizePercentage(double wPercent,double hPercent)
        {
            Rectangle screen = Screen.FromPoint(Cursor.Position).WorkingArea;
            int w = (int)(screen.Width * wPercent);
            int h = (int)(screen.Height * hPercent);
            
            this.Size = new Size(w, h);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            generateBtn.Enabled = true;

            try
            {
                await MemeAPI.Instance.LoadPouplarMemes(randomise: true);
            }
            catch (Exception error)
            {
                Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show(error.Message);
                }));
                return;
            }
            // Update UI after the task is done
            Invoke((MethodInvoker)(() =>
            {
                // Update UI controls here
                Dictionary<string, string> id_names = MemeAPI.Instance.getMappedMemes();
                comboBox1.DataSource = new BindingSource(id_names, null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
            }));
        }

        private void generateMeme_Click(object sender, EventArgs e)
        {
            GenerateMeme();
        }

        private async Task GenerateMeme()
        {
            KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>)comboBox1.SelectedItem;
            string id = selectedItem.Key;
            string name = selectedItem.Value;
            string url = "url";
            string text1 = textBox1.Text;
            string text2 = textBox2.Text;
            string text3 = textBox3.Text;
            string text4 = textBox4.Text;

            RadioButton checkedRBtn = typePanel.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            //MemeType memeType = MemeType.Funny;
            GeneratedMeme newMeme;
            switch (checkedRBtn?.Name)
            {
                case "rBtnFunny":
                    newMeme = new FunnyMeme(id, name, url, text1, text2, text3, text4);
                    break;
                case "rBtnSad":
                    newMeme = new SadMeme(id, name, url, text1, text2, text3, text4);
                    break;
                default:
                    MessageBox.Show("Choose a type!");
                    return;
            }

            try
            {
                await MemeAPI.Instance.CreateMemeAsync(newMeme);
            }
            catch (Exception err)
            {
                Invoke((MethodInvoker)(() => MessageBox.Show(err.Message)));
                return;
            }

            ShowMeme(newMeme);
        }

        private void ShowMeme(Meme m, bool fillFields = false)
        {
            if (m == null) return;

            if (m is GeneratedMeme)
            {
                GeneratedMeme gm = (GeneratedMeme)m;
                if (fillFields)
                {
                    textBox1.Text = gm.Text1;
                    textBox2.Text = gm.Text2;
                    textBox3.Text = gm.Text3;
                    textBox4.Text = gm.Text4;
                    comboBox1.SelectedIndex = comboBox1.FindStringExact(gm.Name);
                }
                if (gm.Url != null)
                {
                    pBox_meme.Load(gm.Url);
                }
                else
                    Console.WriteLine("image url returned null");

                if (gm is FunnyMeme)
                {
                    rBtnFunny.Checked = true;
                }
                else if (gm is SadMeme)
                {
                    rBtnSad.Checked = true;
                }
            }
            else if (m.Url != null)
            {
                pBox_meme.Load(m.Url);
            }

        }

        private void comboBox1_SelectionIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox1.SelectedIndex;
            ShowMeme(MemeAPI.Instance.Memes[i]);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            MemeAPI.clearResources();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Save");
            GeneratedMeme m = MemeAPI.Instance.CurrentMeme;
            if (m == null)
            {
                MessageBox.Show("Create a meme first");
            }
            else
            {
                try
                {
                    LocalDB.Instance.SaveData(m);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return;
                }
                MessageBox.Show("Saved successfully");
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Load");
            GeneratedMeme meme = LocalDB.Instance.OpenFromFile();
            //MemeAPI.Instance.CurrentMeme;
            if (meme != null)
            {
                ShowMeme(meme, true);
                MemeAPI.Instance.CurrentMeme = meme;
            }
            else
            {
                MessageBox.Show("No saved data found!");
            }
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            string selectedMemeName = ((KeyValuePair<string, string>)comboBox1.SelectedItem).Value;
            saveDialog.FileName = selectedMemeName;
            saveDialog.DefaultExt = "jpg";
            saveDialog.Filter = "JPG Image | *.jpg";
            saveDialog.ValidateNames = true;
            DialogResult dialogResult = saveDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                int width = (pBox_meme.Width);
                int height = (pBox_meme.Height);
                Bitmap bitmap = new Bitmap(width, height);

                pBox_meme.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));
                bitmap.Save(saveDialog.FileName);
            }
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            updateFrom(textBox1);
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            updateFrom(textBox2);
        }

        private void updateFrom(TextBox textBox)
        {
            if (!generateOnEdit) return;
            if (textBox.Text.Length > 0)
                GenerateMeme();
        }
    }
}
