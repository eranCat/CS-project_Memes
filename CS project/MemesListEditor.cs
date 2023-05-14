using CS_project.CS_project;
using CS_project.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_project
{
    public partial class MemesListEditor : Form
    {
        private List<GeneratedMeme> generatedMemes;
        private List<Image> images;
        private DoubleClickMemeEventListener dblClickMemeEventListener;
        private string currentPath;

        public DoubleClickMemeEventListener DblClickMemeEventListener { get => dblClickMemeEventListener; set => dblClickMemeEventListener = value; }

        public MemesListEditor()
        {
            InitializeComponent();
        }

        public async void populate(List<GeneratedMeme> generatedMemes, string path)
        {
            this.generatedMemes = generatedMemes;
            this.currentPath = path;
            listView1.Items.Clear();

            for (int i = 0; i < generatedMemes.Count; i++)
            {
                GeneratedMeme m = generatedMemes[i];
                var item = new ListViewItem(m.Name, i);
                item.Tag = m;
                listView1.Items.Add(item);
            }

            images = await LoadImagesAsync(generatedMemes.Select(meme => meme.Url).ToList());

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(100, 100);
            imageList.ColorDepth = ColorDepth.Depth32Bit;

            for (int i = 0; i < generatedMemes.Count; i++)
            {
                GeneratedMeme m = generatedMemes[i];
                var item = listView1.Items[i];

                var thumb = GraphicsHelper.FixedSize(
                    images[i],
                    imageList.ImageSize.Width,
                    imageList.ImageSize.Height,
                    listView1.BackColor);

                imageList.Images.Add(thumb);
            }

            listView1.LargeImageList = imageList;
        }

        private async Task<List<Image>> LoadImagesAsync(List<string> urls)
        {
            List<Image> images = new List<Image>();
            foreach (var url in urls)
            {
                var img = await GraphicsHelper.DownloadImageFromUrlAsync(url);
                images.Add(img);
            }

            return images;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Delete items from list and from file
            var items = listView1.SelectedItems;
            
            foreach (var item in items)
            {
                var meme = (GeneratedMeme)((ListViewItem)item).Tag;
                generatedMemes.Remove(meme);
                listView1.Items.Remove((ListViewItem)item); ;
            }

            LocalDB.Instance.SaveMemes(generatedMemes, currentPath);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var listItem = listView1.SelectedItems[0];
                var index = listItem.Index;
                var meme = listItem.Tag as GeneratedMeme;

                this.dblClickMemeEventListener?.OnDoubleClickMeme(meme, index);
                Close();
            }
        }
    }

    public interface DoubleClickMemeEventListener
    {
        void OnDoubleClickMeme(GeneratedMeme meme, int index);
    }
}
