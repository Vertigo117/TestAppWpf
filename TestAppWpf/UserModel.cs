using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestAppWpf
{
    public class User
    {
        public int Index { get; set; }

        public string UserName { get; set; }

        public string Organization { get; set; }

        public string Ip { get; set; }

        public string SessionId { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime LogoutTime { get; set; }

        public byte EndCode { get; set; }

    }
}
