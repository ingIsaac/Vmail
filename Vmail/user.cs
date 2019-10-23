using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vmail
{
    public partial class user : Form
    {
        public user()
        {
            InitializeComponent();
            loadUsers();
        }

        private void loadUsers()
        {
            comboBox1.Items.Clear();
            foreach(var u in ((Form1)Application.OpenForms["form1"]).getUserList())
            {
                comboBox1.Items.Add(u.user);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string server = textBox1.Text;
            int port = constants.stringToInt(textBox2.Text);
            bool ssl = checkBox1.Checked;
            string user = textBox4.Text;
            string password = textBox5.Text;

            if(server != string.Empty)
            {
                if(port > 0)
                {
                    if(user != string.Empty)
                    {
                        if(password != string.Empty)
                        {
                            constants.SetNewMail(this, server, port, ssl, user, password);
                            reload();
                        }
                        else
                        {
                            MessageBox.Show(this, "[Error]: necesitas añadir una contraseña.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error]: necesitas añadir un usuario.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(this, "[Error]: necesitas un puerto válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "[Error]: necesitas añadir un servidor.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            user_c t = constants.getMail(this, comboBox1.Text);
            if(t != null)
            {
                button2.Visible = true;
                textBox1.Text = t.server;
                textBox2.Text = t.port.ToString();
                checkBox1.Checked = t.ssl ? true : false;
                textBox4.Text = t.user;
                textBox5.Text = t.password;
            }
        }

        private void reload()
        {
            ((Form1)Application.OpenForms["form1"]).load();
            loadUsers();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text != string.Empty)
            {
                constants.deleteMail(this, comboBox1.Text);
                reload();
            }
        }
    }
}
