using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestAppWpf
{
    class User : INotifyPropertyChanged
    {
        private int index;
        private string userName;
        private string organization;
        private string ip;
        private string sessionId;
        private DateTime loginTime;
        private DateTime logoutTime;
        private byte endCode;

        public int Index { get 
            { 
                return index; 
            } 
            set 
            {
                index = value;
                OnPropertyChanged("NumOrder"); 
            } 
        }

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string Organization
        {
            get
            {
                return organization;
            }
            set
            {
                organization = value;
                OnPropertyChanged("Organization");
            }
        }

        public string Ip
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
                OnPropertyChanged("Ip");
            }
        }

        public string SessionId
        {
            get
            {
                return sessionId;
            }
            set
            {
                sessionId = value;
                OnPropertyChanged("Id");
            }
        }

        public DateTime LoginTime
        {
            get
            {
                return loginTime;
            }
            set
            {
                loginTime = value;
                OnPropertyChanged("LoginTime");
            }
        }

        public DateTime LogoutTime
        {
            get
            {
                return logoutTime;
            }
            set
            {
                logoutTime = value;
                OnPropertyChanged("LogoutTime");
            }
        }

        public byte EndCode 
        { 
            get
            {
                return endCode;
            }

            set
            {
                endCode = value;
                OnPropertyChanged("EndCode");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
