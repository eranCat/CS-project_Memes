using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace CS_project
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\User\source\repos\CS-project_Memes\CS project\DB\LoginDB.mdf"";Integrated Security=True;Connect Timeout=30");
            sqlcon.Open();
            string username = textBox1.Text.Trim();
            string pass = textBox2.Text.Trim();
            string query = $"Select *from [Table] where username = '{username}'and password='{pass}'";
            SqlCommand comd = new SqlCommand(query, sqlcon);
            comd.Parameters.AddWithValue("@username", textBox1.Text);
            comd.Parameters.AddWithValue("@password", textBox2.Text);
            SqlDataReader reader = null;
            reader = comd.ExecuteReader();
            //SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
            //DataTable dtbl = new DataTable();
            //sda.Fill(dtbl);
            if (reader.Read() == true)
            {
                this.Hide();
                CreateMemeForm objmain = new CreateMemeForm();
                objmain.Show();
            }
            else
            {
                MessageBox.Show("Error Username of Password");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
