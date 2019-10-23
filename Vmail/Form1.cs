using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenPop.Pop3;

namespace Vmail
{
    public partial class Form1 : Form
    {
        private List<user_c> user_list;

        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        public void loadMailList()
        {
            user_list = constants.getMailList(this, tabControl1);
        }

        public void loadProfiles()
        {
            foreach (var x in user_list)
            {
                DownloadMessages(x.server, x.port, x.ssl, x.user, x.password, x.table);
            }
        }

        public List<user_c> getUserList()
        {
            return user_list;
        }

        public void load()
        {
            loadMailList();
            loadProfiles();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            load();
        }

        public void DownloadMessages(string server, int port, bool ssl, string user, string password, DataGridView table)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (sender, e) =>
            {
                try
                {
                    FetchMessages(server, port, ssl, user, password, 10);         
                }
                catch (Exception err)
                {
                    MessageBox.Show("[Error] error de conexión <?>. \n\n" + err.ToString(), constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            bg.RunWorkerCompleted += (sender, e) =>
            {
                BackgroundWorker _bg = new BackgroundWorker();
                _bg.DoWork += (_sender, _e) =>
                {
                    using (localDBEntities context = new localDBEntities())
                    {
                        byte[] read = constants.imageToByte(Properties.Resources.check_icon);
                        byte[] attachment = constants.imageToByte(Properties.Resources.attachment_icon);
                        var t = from x in context.messages
                                select new
                                {
                                    Leido = x.read == true ? read : null,
                                    Adjuntos = x.attachment == true ? attachment : null,
                                    De = x.sender,
                                    Asunto = x.subject,
                                    Contenido = x.content_view,
                                    Fecha = x.date
                                };
                        if(table.InvokeRequired)
                        {
                            table.Invoke((MethodInvoker)delegate
                            {
                                table.DataSource = null;
                                table.DataSource = t.ToList();
                            });
                        }                      
                    }
                };
                _bg.RunWorkerCompleted += (_sender, _e) =>
                {
                    pictureBox1.Visible = false;
                    label1.Text = "Listo.";
                };
                if(!_bg.IsBusy)
                {
                    label1.Text = "Cargando Mensajes...";
                    _bg.RunWorkerAsync();
                }              
            };
            if(!bg.IsBusy)
            {
                label1.Text = "Buscando Mensajes...";
                pictureBox1.Visible = true;
                bg.RunWorkerAsync();
            }        
        }

        public void FetchMessages(string hostname, int port, bool useSsl, string username, string password, int lim = 0)
        {           
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password, AuthenticationMethod.UsernameAndPassword);
                int messageCount = client.GetMessageCount();
                using (localDBEntities context = new localDBEntities())
                {
                    OpenPop.Mime.Message m;
                    for (int i = messageCount; i > 0; i--)
                    {
                        if (i <= messageCount - lim)
                        {
                            break;
                        }

                        //--------------------->
                        m = client.GetMessage(i);
                        var t = new message()
                        {
                            messageID = m.Headers.MessageId,
                            sender = m.Headers.From.DisplayName + " - <" + m.Headers.From.Address + ">",
                            subject = m.Headers.Subject,
                            content_view = m.Headers.ContentDescription,
                            date = m.Headers.DateSent.ToShortDateString(),
                            read = false,
                            attachment = m.FindAllAttachments().Count > 0 ? true : false
                        };
                        context.messages.Add(t);
                    }
                    context.SaveChanges();
                }
            }
        }

        private void añadirUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new user().ShowDialog(this);
        }
    }
}
