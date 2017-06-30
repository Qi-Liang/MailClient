using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MailClient_beta_v1._1.SourceFile
{
    public class Messager : INotifyPropertyChanged
    {
        private string msgStr1;
        public string MsgStr1
        {
            get { return msgStr1; }
            set
            {
                msgStr1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MsgStr1"));
            }
        }

        private string msgStr2;
        public string MsgStr2
        {
            get { return msgStr2; }
            set
            {
                msgStr2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MsgStr2"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public Messager()
        {
            MsgStr1 = "";
            MsgStr2 = "";
        }

    }

    public class GlobalUse
    {
        public static Messager Notification { get; set; }
        static GlobalUse()
        {
            Notification = new Messager();
        }
    }
}
