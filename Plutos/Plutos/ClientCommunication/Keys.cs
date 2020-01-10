using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plutos
{
    class Keys
    {
        public byte[] aesKey { get; set; }
        public byte[] IV { get; set; }

        public Keys(string aesKey, string IV)
        {
            this.aesKey = Encoding.UTF8.GetBytes(aesKey);
            this.IV = Encoding.UTF8.GetBytes(IV);
        }
    }
}
