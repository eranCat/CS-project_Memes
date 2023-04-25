using CS_project.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            generateBtn.Enabled = true;

            try
            {
                await MemeAPI.Instance.LoadPouplarMemes(randomise: false);
            }
            catch (Exception error)
            {
                Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show(error.Message);
                }));
                return;
            }
            
            LoadImages(MemeAPI.Instance.Memes);
            fillListView(MemeAPI.Instance.Memes);
        }

        private void fillListView(List<Meme> memes)
        {
            ImageList images = new ImageList();
            ListViewItem item;
            images.ImageSize = new Size(100, 100);
            for (int i = 0; i < memes.Count; i++)
            {
                Meme meme = memes[i];
                var imageFromUrl = LoadedImages[i];
                images.Images.Add(imageFromUrl);
                item = new ListViewItem(meme.Name, i);
                item.Tag = meme;
                listViewMemes.Items.Add(item);
            }

            listViewMemes.LargeImageList = images;

            listViewMemes.Items[0].Selected = true;
        }

        private void LoadImages(List<Meme> memes)
        {
            LoadedImages = new List<Image>(memes.Count);
            foreach (var meme in memes)
            {
                Image img = DownloadImageFromUrl(meme.Url);
                LoadedImages.Add(img);
            }
        }

        public Image DownloadImageFromUrl(string imageUrl)
        {
            Image image = null;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                WebResponse webResponse = webRequest.GetResponse();

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
            Meme meme = (Meme)listViewMemes.SelectedItems[0].Tag;
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
