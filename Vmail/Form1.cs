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
        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            DownloadMessages(dataGridView1);
        }

        public void DownloadMessages(DataGridView table)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (sender, e) =>
            {
                try
                {
                    FetchMessages("Outlook.Office365.com", 995, true, "ing_isaacsolis@hotmail.com", "software456sr", 10);         
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            };
            bg.RunWorkerCompleted += (sender, e) =>
            {
                pictureBox1.Visible = false;
                label1.Text = "Listo";
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
                                Fecha  = x.date
                            };
                    table.DataSource = null;
                    table.DataSource = t.ToList();             
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

    }
}
