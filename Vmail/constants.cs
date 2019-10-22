using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace Vmail
{
    public static class constants
    {
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


        public static void setNewMail()
        {

        }
    }
}
