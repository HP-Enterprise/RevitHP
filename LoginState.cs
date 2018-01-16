using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class LoginState : INotifyPropertyChanged
    {     
     
        private string _rolename;
        public string RoleName
        {
            get { return _rolename; }
            set {
                if (this.RoleName != value)
                {
                    this.RoleName = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("RoleName"));
                }
               }
        }
        public LoginState(string roleName)
        {
            this._rolename=roleName;
        }

        public static string rolename;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
