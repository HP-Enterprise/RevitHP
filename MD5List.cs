using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    class MD5List
    {
        public MD5List(string md5)
        {
            MD5 = md5;
        }
        public MD5List()
        {
           
        }
        public string md5 { get; set; }
     
        public string MD5
        {
            get { return md5; }
            set
            {
                if (this.MD5 != value)
                {
                    this.md5 = value;
                }
            }
        }
    }
}
