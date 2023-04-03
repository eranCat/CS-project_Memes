using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CS_project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MemeAPI.Instance.Memes.Count > 0)
            {
                Meme m = MemeAPI.Instance.getRandomMeme();
                showMeme(m);
            }
            else
            {
                MessageBox.Show("Memes list is empty!");
            }
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            loadBtn.Enabled = false;
            try
            {
                if (MemeAPI.Instance.LoadPouplarMemes())
                {
                    //MessageBox.Show("Loaded!");
                    button1.Enabled = true;
                    string[] names = MemeAPI.Instance.Memes.Select(m=>m.Name).ToArray();
                    comboBox1.DataSource = names;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            loadBtn.Enabled = true;
        }

        private void showMeme(Meme m)
        {
            if (m == null) return;

            pBox_meme.Load(m.Url);
            label1.Text = m.Text0;
        }

        private void comboBox1_SelectionIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.Text);
            int i = comboBox1.SelectedIndex;
            showMeme(MemeAPI.Instance.Memes[i]);
        }
    }
}
