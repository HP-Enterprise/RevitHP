using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    class LoginVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_strErrorMsg = String.Empty;
        public string ErrorMsg
        {
            get { return m_strErrorMsg; }
            set
            {
                m_strErrorMsg = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ErrorMsg"));
            }
        }
    }
}
