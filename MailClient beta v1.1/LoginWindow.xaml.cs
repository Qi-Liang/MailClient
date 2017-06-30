using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MailClient_beta_v1._1.SourceFile;
using System.Configuration;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailClient_beta_v1._1
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static bool CheckBoxState;
        public static string CheckBoxState2;
        public static string NowUsername;
        public LoginWindow()
        {
            InitializeComponent();
            CheckBoxState2 = ConfigurationManager.AppSettings["CheckBoxState2"];
            if(CheckBoxState2.Equals("true"))
            {
            textBox.Text = ConfigurationManager.AppSettings["Username"];
            passwordBox.Password = ConfigurationManager.AppSettings["Password"];
            }
            CheckBoxState2 = "false";
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            string Username = textBox.Text;
            NowUsername = Username;
            string Password = passwordBox.Password;

            SocketLogin Login_Test = new SocketLogin();
            string ServerName = Login_Test.HostNameReg(Username);
            if (ServerName == null)
            {
                return;
            }
            int Port;
            if (CheckBoxState)
            {
                Port = 995;
            }
            else
            {
                Port = 110;
            }
            if (Login_Test.Login(ServerName, Port, Username, Password, CheckBoxState))
            {
                //登录成功后显示提示消息
                //将用户名密码保存在本地，下次登录直接调用
                Login_Test.FileWrite(Username, Password);
                this.Hide();
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["Username"].Value = Username;
                cfa.AppSettings.Settings["Password"].Value = Password;
                cfa.AppSettings.Settings["CheckBoxState2"].Value = CheckBoxState2;
                cfa.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                MainWindow MW = new MainWindow();
                MW.Show();
                this.Close();
                return;
            }
            else
            {
                MessageBox.Show(Login_Test.exception_p.Message);
                return;
            }
        }

        private void 登陆选项_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxState = true;
        }

        private void 登陆选项_unChecked(object sender, RoutedEventArgs e)
        {
            CheckBoxState = false;
        }

        private void Remember_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxState2 = "true";
        }
        private void Remember_unChecked(object sender, RoutedEventArgs e)
        {
            CheckBoxState2 = "false";
        }

    }
}