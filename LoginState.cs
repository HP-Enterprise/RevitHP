using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class LoginState
    {     
        public string ACCESS_TOKEN {get;set;}
        private string _rolename;
        public string RoleName
        {
            get { return _rolename; }
            set { _rolename = value; }
        }
        public LoginState(string roleName)
        {
            this._rolename=roleName;
        }
    }
}
