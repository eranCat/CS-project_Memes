﻿using CS_project.CS_project;
using CS_project.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace CS_project
{
    public partial class CreateMemeForm : Form
    {
        bool generateOnEdit = true;//TODO save user's settigns with checkbox 
        private List<Image> LoadedImages { get; set; }

        public CreateMemeForm()
        {
            InitializeComponent();
            setFormSizePercentage(.8, .8);
        }

        private void setFormSizePercentage(double wPercent, double hPercent)
        {
            Rectangle screen = Screen.FromPoint(Cursor.Position).WorkingArea;
            int w = (int)(screen.Width * wPercent);
            int h = (int)(screen.Height * hPercent);

            this.Size = new Size(w, h);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Progress<int> progress = new Progress<int>();

            ProgressBar pb = progressBar1;

            progress.ProgressChanged += (s, step) =>
            {
                if (step != -1)
                {
                    if (step >= pb.Maximum)
                    {
                        pb.Hide();
                    }
                    else
                    {
                        pb.Value = step;
                        if (!pb.Visible)
                        {
                            pb.Show();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Issue with loading");
                    pb.Hide();
                }
            };
            List<Meme> memesRes = null;
            try
            {
                memesRes = await MemeAPI.Instance.LoadPouplarMemes(progress);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                pb.Hide();
            }

            if (memesRes != null)
            {
                await LoadImagesAsync(memesRes, progress);
                fillListView(memesRes);
                pb.Value = pb.Maximum;
                generateBtn.Enabled = true;
            }
        }

        private void fillListView(List<Meme> memes)
        {
            ImageList images = new ImageList();
            images.ImageSize = new Size(100, 100);
            images.ColorDepth = ColorDepth.Depth32Bit;

            ListViewItem item;
            for (int i = 0; i < memes.Count; i++)
            {
                Meme meme = memes[i];
                
                var thumb = GraphicsHelper.FixedSize(
                    LoadedImages[i],
                    images.ImageSize.Width,
                    images.ImageSize.Height,
                    BackColor);

                images.Images.Add(thumb);
                item = new ListViewItem(meme.Name, i)
                {
                    Tag = meme
                };
                listViewMemes.Items.Add(item);
            }

            listViewMemes.LargeImageList = images;

            listViewMemes.Items[0].Selected = true;
        }

        private async Task LoadImagesAsync(List<Meme> memes, IProgress<int> progressEvent)
        {
            LoadedImages = new List<Image>(memes.Count);
            for (int i = 0; i < memes.Count; i++)
            {
                Meme meme = memes[i];
                Image img = await DownloadImageFromUrlAsync(meme.Url);
                LoadedImages.Add(img);

                progressEvent.Report(i+1);
            }
        }

        public async Task<Image> DownloadImageFromUrlAsync(string imageUrl)
        {
            Image image = null;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                WebResponse webResponse = await webRequest.GetResponseAsync();

                Stream stream = webResponse.GetResponseStream();

                image = Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

            return image;
        }

        private void generateMeme_Click(object sender, EventArgs e)
        {
            GenerateMeme();
        }

        private async void GenerateMeme()
        {
            if (listViewMemes.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select an image");
                return;
            }
            Meme meme;

            if (MemeAPI.Instance.CurrentMeme != null)
            {
                meme = MemeAPI.Instance.CurrentMeme;
            }
            else
            {
                meme = (Meme)listViewMemes.SelectedItems[0].Tag;
            }

            string id = meme.Id;
            string name = meme.Name;
            string url = "url";
            string text1 = textBox1.Text;
            string text2 = textBox2.Text;

            RadioButton checkedRBtn = typePanel.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            GeneratedMeme newMeme;
            switch (checkedRBtn?.Name)
            {
                case "rBtnFunny":
                    newMeme = new FunnyMeme(id, name, url, text1, text2);
                    break;
                case "rBtnSad":
                    newMeme = new SadMeme(id, name, url, text1, text2);
                    break;
                default:
                    MessageBox.Show("Choose a type!");
                    return;
            }

            try
            {
                newMeme = await MemeAPI.Instance.CreateMemeAsync(newMeme);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
            if (newMeme != null)
            {
                ShowMeme(newMeme);
            }
        }

        private void ShowMeme(Meme m, bool fillFields = false)
        {
            if (m is GeneratedMeme)
            {
                GeneratedMeme gm = (GeneratedMeme)m;
                if (fillFields)
                {
                    textBox1.Text = gm.Text1;
                    textBox2.Text = gm.Text2;
                    //comboBox1.SelectedIndex = comboBox1.FindStringExact(gm.Name);
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            MemeAPI.clearResources();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneratedMeme m = MemeAPI.Instance.CurrentMeme;
            if (m == null)
            {
                MessageBox.Show("Create a meme first");
            }
            else
            {
                SaveMeme(m);
            }
        }

        private static void SaveMeme(GeneratedMeme m)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON|*.json";
            saveFileDialog.Title = "Save meme json";
            saveFileDialog.FileName = m.Name.Trim() + ".json";
            var result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    LocalDB.Instance.SaveData(m,filename);
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
            string path = AskFileLocation();
            //MessageBox.Show("Load");
            GeneratedMeme meme;
            if (path != null)
                meme = LocalDB.Instance.OpenFromFile(path);
            else
                meme = LocalDB.Instance.OpenFromFile(); 

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

        private string AskFileLocation()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "json",
                Filter = "JSON files (*.json)|*.json",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            return openFileDialog1.ShowDialog() == DialogResult.OK ? openFileDialog1.FileName : null;
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            string selectedMemeName = MemeAPI.Instance.CurrentMeme?.Name ?? "My meme";
            //string selectedMemeName = ((KeyValuePair<string, string>)comboBox1.SelectedItem).Value;
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

        private void listViewMemes_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listViewMemes.SelectedIndices.Count > 0)
            {
                var index = listViewMemes.SelectedIndices[0];
                Image selectedImage = LoadedImages[index];
                pBox_meme.Image = selectedImage;
            }
        }
    }
}
