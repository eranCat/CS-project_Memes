using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CS_project
{
    public partial class CreateMemeForm : Form
    {
        public CreateMemeForm()
        {
            InitializeComponent();
            
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            generateBtn.Enabled = true;
            
            try
            {
                await MemeAPI.Instance.LoadPouplarMemes();
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
                Dictionary<string,string> id_names = MemeAPI.Instance.getMappedMemes();
                comboBox1.DataSource = new BindingSource(id_names,null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
            }));
        }

        private async void generateMeme_Click(object sender, EventArgs e)
        {
            string id = ( (KeyValuePair<string,string>) comboBox1.SelectedItem).Key;
            string text1 = textBox1.Text;
            string text2 = textBox2.Text;
            string text3 = textBox3.Text;
            string text4 = textBox4.Text;

            GeneratedMeme newMeme = new GeneratedMeme(id, text1, text2,text3,text4);

            try
            {
                await MemeAPI.Instance.CreateMemeAsync(newMeme);
            }
            catch (Exception err)
            {
                Invoke((MethodInvoker)(() => MessageBox.Show(err.Message)));
                return;
            }

            Debug.WriteLine(newMeme.ImgUrl);
            showMeme(newMeme);
        }

        private void showMeme(Meme m)
        {
            if (m == null) return;

            if (m is GeneratedMeme)
            {
                GeneratedMeme gm = (GeneratedMeme)m;
                if (gm.ImgUrl != null)
                {
                    pBox_meme.Load(gm.ImgUrl);
                }
                else
                    Console.WriteLine("image url returned null");
            }
            else if(m.Url!=null)
            {
                pBox_meme.Load(m.Url);
            }
            
        }

        private void comboBox1_SelectionIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.Text);
            int i = comboBox1.SelectedIndex;
            showMeme(MemeAPI.Instance.Memes[i]);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            MemeAPI.clearResources();
        }

        private void linkLabelUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GeneratedMeme m = MemeAPI.Instance.CurrentMeme;

            string url = m?.ImgUrl;
            if (url == null)
            {
                MessageBox.Show("Not generated yet");
                return;
            }
            try
            {
                VisitLink(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void VisitLink(string link)
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            linkLabelUrl.LinkVisited = true;
            //Call the Process.Start method to open the default browser
            //with a URL:
            System.Diagnostics.Process.Start(link);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }
    }
}
