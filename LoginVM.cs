using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Windows.Threading;

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

        public string UserName { get; set; }

        public SecureString Pwd { private get; set; }

        public void Login(Action action)
        {
            if (String.IsNullOrWhiteSpace(UserName))
            {
                this.ErrorMsg = "请填写用户名";
                return;
            }

            if (Pwd == null || Pwd.Length == 0)
            {
                this.ErrorMsg = "请填写密码";
                return;
            }

            var dispatcher = Dispatcher.CurrentDispatcher;

            var actLogin = new Action(() => {
                try
                {
                    RevitBiz.Instance.Login(this.UserName, this.Pwd);
                    if (action != null)
                    {
                        action.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    dispatcher.Invoke(new Action(() =>
                    {
                        ErrorMsg = ex.Message;
                    }));
                }
            });
            actLogin.BeginInvoke(null, null);
        }
    }
}
