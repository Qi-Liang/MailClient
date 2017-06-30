using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using MailClient_beta_v1._1;
using MailClient_beta_v1._1.SourceFile;
using System.Text.RegularExpressions;

namespace MailClient_beta_v1._1.Pages
{
    /// <summary>
    /// Interaction logic for ReceiveMailSettings.xaml
    /// </summary>
    public partial class ReceiveMailSettings : UserControl
    {
        private Receive Receive_ = new Receive();
        public Receive Receive_pro
        {
            get
            {
                return Receive_;
            }
            private set
            {
                Receive_ = value;
            }
        }

        private sealed class parameter
        {
            public static bool complete_download;
            public static bool delete;
            public static int num = 0;
        }
        public ReceiveMailSettings()
        {
            InitializeComponent();
        }

        private void 立即从服务器更新邮件到本地客户端_Click(object sender, RoutedEventArgs e)
        {
            ReceiveProgressBar.Visibility = Visibility.Visible;
            //添加按钮点击事件处理程序↓

            //-----------------------------
            ReceiveProgressBar.Visibility = Visibility.Hidden;
        }

        private void 收件规则保存_Click(object sender, RoutedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername + "\\";
            string data = Receive.var.Setting + "\r\n" + "complete_download=";
            if (parameter.complete_download)
                data = data + "true\r\n";
            else
                data = data + "flase\r\n";
            data = data + "delete=";
            if (parameter.delete)
                data = data + "true\r\n";
            else
                data = data + "flase\r\n";
            Receive_pro.Write(path, "res.ini", data);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (competle.SelectedIndex == 0)
            {
                parameter.complete_download = true;
            }
            else if (competle.SelectedIndex == 1)
            {
                parameter.complete_download = false;
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            if (delete.SelectedIndex == 0)
            {
                parameter.delete = false;
            }
            else if (delete.SelectedIndex == 1)
            {
                parameter.delete = true;
            }
        }

        private void 立即从服务器更新邮件到本地客户端_Click_1(object sender, RoutedEventArgs e)
        {
            string username, passwd, path, hostnanme, res;
            int port;
            bool complete, del;

            path = "MailClient\\" + LoginWindow.NowUsername + "\\";
            username = File.ReadAllText(path + "users\\" + "Username.txt");
            username = username.Substring(0, username.Length - 2);
            passwd = File.ReadAllText(path + "users\\" + "Password.txt");
            passwd = passwd.Substring(0, passwd.Length - 2);
            if (!File.Exists(path + "res.ini"))
            {
                FileStream myFs = new FileStream(path + "res.ini", FileMode.Create);
                StreamWriter mySw = new StreamWriter(myFs);
                mySw.Write("[Setting]\r\ncomplete_download=true\r\ndelete=flase");
                mySw.Close();
                myFs.Close();
            }
            res = File.ReadAllText(path + "res.ini");
            Regex charset = new Regex(@"(?<=complete_download=).+?(?=\r\n)");
            Match charset_match = charset.Match(res);// match charset
            if (Convert.ToString(charset_match) == "true")
                complete = true;
            else
                complete = false;

            Regex charset_2 = new Regex(@"(?<=delete=).+?(?=\r\n)");
            Match charset_match_2 = charset_2.Match(res);// match charset
            if (Convert.ToString(charset_match_2) == "true")
                del = true;
            else
                del = false;

            hostnanme = HostNameReg(username);
            if (LoginWindow.CheckBoxState)
                port = 995;
            else
                port = 110;
            Receive_pro.connect(hostnanme, port, LoginWindow.CheckBoxState);
            Receive_pro.login(username, passwd, ref parameter.num, LoginWindow.CheckBoxState);
            ReceiveProgressBar.Maximum = parameter.num;
            ReceiveProgressBar.Value = 0;
            double value = 0;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(ReceiveProgressBar.SetValue);
            do
            {
                value += 1;
                Dispatcher.Invoke(updatePbDelegate,
    System.Windows.Threading.DispatcherPriority.Background,
    new object[] { ProgressBar.ValueProperty, value });
            }
            while (ReceiveProgressBar.Value != ReceiveProgressBar.Maximum);
            Receive_pro.download(username, parameter.num, LoginWindow.CheckBoxState, complete, del);
            Receive_pro.CloseSocket();
            
            //Receive_pro.Write(path, "number.txt", Convert.ToString(parameter.num));
        }
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        public static string HostNameReg(string username)
        {
            //匹配@及之后的内容
            string pattern = @"@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            Match MatchText;
            string value;
            string HostName;
            try
            {
                MatchText = reg.Match(username);
                value = MatchText.Value;
                //去除@
                HostName = value.Remove(0, 1);
                HostName = string.Concat("pop.", HostName);
                return HostName;
            }
            catch
            {
                MessageBox.Show("Host is illegal !" + "\nError Message:" + "Please check the username!");
                return null;
            }

        }
    }
}

