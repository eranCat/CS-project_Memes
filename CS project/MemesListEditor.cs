using CS_project.CS_project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_project
{
    public partial class MemesListEditor : Form
    {
        private List<GeneratedMeme> generatedMemes;
        private List<Image> images;

        public MemesListEditor()
        {
            InitializeComponent();
        }

        public async void populate(List<GeneratedMeme> generatedMemes)
        {
            this.generatedMemes = generatedMemes;
            images = await LoadImagesAsync(generatedMemes.Select(meme => meme.Url).ToList());

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(200, 200);
            imageList.ColorDepth = ColorDepth.Depth32Bit;

            for (int i = 0; i < generatedMemes.Count; i++)
            {
                GeneratedMeme m = generatedMemes[i];
                var item = new ListViewItem(m.Name, i);
                item.Tag = m;

                listView1.Items.Add(item);

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

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
