using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vmail
{
    public class user_c
    {
        public string server { get; set; }
        public int port { get; set; }
        public bool ssl { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public DataGridView table { get; set; }

        public user_c(string server, int port, bool ssl, string user, string password, DataGridView table)
        {
            this.server = server;
            this.port = port;
            this.ssl = ssl;
            this.user = user;
            this.password = password;
            this.table = table;
        }
    }
}
