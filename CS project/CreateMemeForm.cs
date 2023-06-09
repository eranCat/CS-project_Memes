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
    public partial class CreateMemeForm : Form , DoubleClickMemeEventListener
    {
        bool generateOnEdit = true;//TODO save user's settings with checkbox 
        private List<Image> LoadedImages { get; set; }
        public MemesListEditor EditForm { get; private set; }

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
                pb.Maximum = memesRes.Count;
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
                Image img = await GraphicsHelper.DownloadImageFromUrlAsync(meme.Url);
                LoadedImages.Add(img);

                progressEvent.Report(i+1);
            }
        }

        private async void generateMeme_ClickAsync(object sender, EventArgs e)
        {
            var meme = await GenerateMeme();
            if (meme != null)
            {
                ShowMeme(meme,false);
            }
        }

        private Meme currentMeme = null;

        private async Task<GeneratedMeme> GenerateMeme()
        {
            Meme selectedMeme = currentMeme;
            if (selectedMeme == null) {
                if (listViewMemes.SelectedItems.Count > 0)
                {
                    selectedMeme = (Meme)listViewMemes.SelectedItems[0].Tag;
                }
                else
                {
                    MessageBox.Show("Select an image");
                    return null;
                }
            }

            string id = selectedMeme.Id;
            string name = selectedMeme.Name;
            string url = "url";
            string text1 = textBox1.Text;
            string text2 = textBox2.Text;

            if(checkBox1.Checked)
            {
                text1 = String.Concat(text1.Reverse());
            }
            if(checkBox2.Checked)
            {
                text2 = String.Concat(text2.Reverse());
            }

            RadioButton checkedRBtn = typePanel.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            GeneratedMeme newMeme;
            long uid = -1;

            if (selectedMeme is GeneratedMeme)
            {
                newMeme = (GeneratedMeme)selectedMeme;
                newMeme.Text1 = text1;
                newMeme.Text2 = text2;
                uid = newMeme.Uid;
            }
            else
            {
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
                        return null;
                }
            }

            try
            {
                newMeme = await MemeAPI.Instance.CreateMemeAsync(newMeme);
                if (uid != -1)
                    newMeme.Uid = uid;
                currentMeme = newMeme;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }

            return newMeme;
        }

        private void ShowMeme(Meme m, bool fillFields)
        {
            if (m is GeneratedMeme)
            {
                GeneratedMeme gm = (GeneratedMeme)m;
                if (fillFields)
                {
                    //detach and reattach event handlers to not update when updating texts programatically
                    textBox1.TextChanged -= textBox1_Changed;
                    textBox1.Text = gm.Text1;
                    textBox1.TextChanged += textBox1_Changed;

                    textBox2.TextChanged -= textBox2_Changed;
                    textBox2.Text = gm.Text2;
                    textBox2.TextChanged += textBox2_Changed;
                }

                if (gm is FunnyMeme)
                {
                    rBtnFunny.Checked = true;
                }
                else if (gm is SadMeme)
                {
                    rBtnSad.Checked = true;
                }
            }
            
            if (m.Url != null)
            {
                try
                {
                    pBox_meme.LoadAsync(m.Url);
                }
                catch (WebException we)
                {
                    MessageBox.Show(we.Message);
                }
            }
            else
                Console.WriteLine("image url returned null");

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
            //TODO make sure that when editing ,
            //the currect meme uid stays the same after loading meme from file
            if (currentMeme != null)
            {
                if (currentMeme is GeneratedMeme m)
                {
                    SaveMeme(m);
                }
                else MessageBox.Show("Not a generated meme");
            }
            else
            {
                MessageBox.Show("Create a meme first");
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
                int res = -1;
                try
                {
                    res = LocalDB.Instance.SaveToList(m,filename);
                    //LocalDB.Instance.SaveData(m,filename);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return;
                }
                switch (res)
                {
                    case 0:
                        MessageBox.Show("Added successfully");
                        break;
                    case 1:
                        MessageBox.Show("Updated successfully");
                        break;

                    default:
                        MessageBox.Show("Couldn't update file!");
                        break;
                }
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = AskFileLocation();
            if (path == null)
            {
                return;
            }

            var listOfMemes = LocalDB.Instance.LoadListFromFile(path);
            if (listOfMemes.Count == 0)
            {
                MessageBox.Show("No memes to load");
                return;
            }
            if (this.EditForm == null)
            {
                this.EditForm = new MemesListEditor();
                this.EditForm.DblClickMemeEventListener = this;
            }

            this.EditForm.populate(listOfMemes, path);
            this.EditForm.ShowDialog();
        }

        private string AskFileLocation()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"d:\",
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
        private void textBox1_Changed(object sender, EventArgs e)
        {
            updateFromAsync(textBox1);
        }
        private void textBox2_Changed(object sender, EventArgs e)
        {
            updateFromAsync(textBox2);
        }

        private async Task updateFromAsync(TextBox textBox)
        {
            if (!generateOnEdit) return;
            if (textBox.Text.Length > 0)
            {
                var newMeme = await GenerateMeme();
                if (newMeme != null)
                {
                    ShowMeme(newMeme,false);
                }
            }
        }

        private void listViewMemes_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listViewMemes.SelectedIndices.Count > 0)
            {
                var index = listViewMemes.SelectedIndices[0];
                Image selectedImage = LoadedImages[index];
                pBox_meme.Image = selectedImage;
                currentMeme = (Meme)listViewMemes.SelectedItems[0].Tag;
            }
        }

        public void OnDoubleClickMeme(GeneratedMeme meme, int index)
        {
            MemeAPI.Instance.CurrentMeme = meme;
            currentMeme = meme;
            ShowMeme(meme,true);

            var item = listViewMemes.FindItemWithText(meme.Name);
            
            if (item != null)
            {
                listViewMemes.SelectedItems.Clear();
                //detach and reattach to not trigger the event
                listViewMemes.ItemSelectionChanged -= listViewMemes_ItemSelectionChanged;
                item.Selected = true;
                listViewMemes.EnsureVisible(item.Index);
                listViewMemes.ItemSelectionChanged += listViewMemes_ItemSelectionChanged;
            }
        }

        private async void checkBox1_CheckedChangedAsync(object sender, EventArgs e)
        {
            GeneratedMeme m = await GenerateMeme();
            if(m!=null)ShowMeme(m,false);
        }

        private async void checkBox2_CheckedChangedAsync(object sender, EventArgs e)
        {
            GeneratedMeme m = await GenerateMeme();
            if (m != null) ShowMeme(m, false);
        }
    }
}
