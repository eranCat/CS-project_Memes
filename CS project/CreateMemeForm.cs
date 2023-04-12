﻿using CS_project.DataModels;
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
                Dictionary<string, string> id_names = MemeAPI.Instance.getMappedMemes();
                comboBox1.DataSource = new BindingSource(id_names, null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
            }));
        }

        private async void generateMeme_Click(object sender, EventArgs e)
        {
            KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>)comboBox1.SelectedItem;
            string id = selectedItem.Key;
            string name = selectedItem.Value;
            string url = "url";//TODO set url
            string text1 = textBox1.Text;
            string text2 = textBox2.Text;
            string text3 = textBox3.Text;
            string text4 = textBox4.Text;

            GeneratedMeme newMeme = new GeneratedMeme(id, name, url, text1, text2, text3, text4);

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
                if (gm.ImgUrl != null)
                {
                    pBox_meme.Load(gm.ImgUrl);
                }
                else
                    Console.WriteLine("image url returned null");
            }
            else if (m.Url != null)
            {
                pBox_meme.Load(m.Url);
            }

        }

        private void comboBox1_SelectionIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.Text);
            int i = comboBox1.SelectedIndex;
            ShowMeme(MemeAPI.Instance.Memes[i]);
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
            Process.Start(link);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Save");
            GeneratedMeme m = MemeAPI.Instance.CurrentMeme;
            if (m == null)
            {
                MessageBox.Show("Create a meme first");
            }
            else
            {
                LocalDB.Instance.SaveData(m);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
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
    }
}
