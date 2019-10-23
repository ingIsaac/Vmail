using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Vmail
{
    public static class constants
    {
        private static string xmlfile = Application.StartupPath + "//Files//Config.xml";
        public static string msg_box_caption = "Vmail - Notificación";

        public static int stringToInt(string n)
        {
            try
            {
                return int.Parse(n);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static Byte[] imageToByte(Image image)
        {
            if (image != null)
            {
                Byte[] r;
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    r = ms.ToArray();
                    ms.Close();
                    ms.Dispose();
                };
                if (r.Length > 0)
                {
                    return r;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static List<user_c> getMailList(Form form, TabControl tab)
        {
            List<user_c> o = new List<user_c>();
            try
            {
                XDocument opciones_xml = XDocument.Load(xmlfile);

                var tc = from x in opciones_xml.Descendants("Config") select x;

                foreach (TabPage z in tab.TabPages)
                {
                    tab.TabPages.Remove(z);
                }
                //---------------------------------------------------->

                foreach (XElement v in tc)
                {
                    foreach(XElement n in v.Elements())
                    {                        
                        TabPage tp = new TabPage(n.Element("Email").Value);
                        DataGridView dg = new DataGridView();
                        dg.Dock = DockStyle.Fill;
                        tab.TabPages.Add(tp);
                        tp.Controls.Add(dg);
                        o.Add(new user_c(n.Element("Server").Value, stringToInt(n.Element("Port").Value), n.Element("Ssl").Value == "true" ? true : false, n.Element("Email").Value, n.Element("Password").Value, dg));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(form, "[Error] no se puede obtener datos del archivo config.xml. \n\n" + e.ToString(), msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return o;
        }


        public static user_c getMail(Form form, string email)
        {
            user_c t = null;
            try
            {
                XDocument opciones_xml = XDocument.Load(xmlfile);

                var n = opciones_xml.Descendants("Config").Elements("User").Where(x => x.Element("Email").Value == email).FirstOrDefault();

                if(n != null)
                {
                    t = new user_c(n.Element("Server").Value, stringToInt(n.Element("Port").Value), n.Element("Ssl").Value == "true" ? true : false, n.Element("Email").Value, n.Element("Password").Value, null);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(form, "[Error] no se puede obtener datos del archivo config.xml. \n\n" + e.ToString(), msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return t;
        }

        public static void deleteMail(Form form, string email)
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(xmlfile);
                opciones_xml.Descendants("Config").Elements("User").Where(x => x.Element("Email").Value == email).FirstOrDefault().Remove();
                opciones_xml.Save(xmlfile);
                MessageBox.Show(form, "Se ha eliminado la configuración del usuario.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(form, "[Error] no se puede borrar los datos del archivo config.xml. \n\n" + e.ToString(), msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SetNewMail(Form form, string server, int port, bool ssl, string email, string password)
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(xmlfile);
                var n = opciones_xml.Descendants("Config").Elements("User").Where(x => x.Element("Email").Value == email).FirstOrDefault();

                if(n != null)
                {
                    n.Element("Password").Value = password;
                    n.Element("Server").Value = server;
                    n.Element("Port").Value = password;
                    n.Element("Ssl").Value = ssl ? "true" : "false";
                }
                else
                {
                    var m = opciones_xml.Descendants("Config").FirstOrDefault();
                    if(m != null)
                    {
                        var t = new XElement("User");
                        m.Add(t);
                        t.Add(new XElement("Server", server));
                        t.Add(new XElement("Port", port));
                        t.Add(new XElement("Ssl", ssl ? "true" : "false"));
                        t.Add(new XElement("Email", email));
                        t.Add(new XElement("Password", password));
                    }
                }
                opciones_xml.Save(xmlfile);
                MessageBox.Show(form, "Se ha guardado la configuración del usuario.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(form, "[Error] no se puede añadir datos del archivo config.xml. \n\n" + e.ToString(), msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
