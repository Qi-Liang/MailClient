using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MailClient_beta_v1._1.SourceFile;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailClient_beta_v1._1.Pages
{
    /// <summary>
    /// Interaction logic for MultipleAccountManagement.xaml
    /// </summary>
    public partial class MultipleAccountManagement : UserControl
    {
        public MultipleAccountManagement()
        {
            InitializeComponent();
            string DirectoryPath = @"MailClient";
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(DirectoryPath);
            System.IO.DirectoryInfo[] ds = d.GetDirectories("*.*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (System.IO.DirectoryInfo var in ds)
            {
                //路径全称
                //var.FullName; 
                //仅文件名称
                ListBox1.Items.Add(var.Name);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string Username = 邮箱账号.Text;

            string Password = 密码内容.Password;

            SocketLogin Login_Test = new SocketLogin();
            string ServerName = Login_Test.HostNameReg(Username);
            if (ServerName == null)
            {
                return;
            }
            int Port = 110;
            bool CheckBoxState = false;
            if (Login_Test.Login(ServerName, Port, Username, Password, CheckBoxState))
            {
                //登录成功后显示提示消息
                //将用户名密码保存在本地，下次登录直接调用
                bool status = false;
                邮箱账号.Clear();
                密码内容.Clear();
                string DirectoryPath = @"MailClient";
                System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(DirectoryPath);
                System.IO.DirectoryInfo[] ds = d.GetDirectories("*.*", System.IO.SearchOption.TopDirectoryOnly);
                foreach (System.IO.DirectoryInfo var in ds)
                {
                    if (Username == var.Name)
                    {
                        status = true;
                        break;
                    }
                }
                Login_Test.FileWrite(Username, Password);
                if (status == false)
                {
                    ListBox1.Items.Add(Username);
                    MessageBox.Show("添加账户成功！");
                    return;
                }
                else
                {
                    MessageBox.Show("该账户已存在！");
                    return;
                }

            }
            else
            {
                MessageBox.Show(Login_Test.exception_p.Message);
                return;
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string DirectoryPath = @"MailClient\\"+ListBox1.SelectedItem.ToString();
                System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(DirectoryPath);
                d.Delete(true);
                ListBox1.Items.RemoveAt(ListBox1.Items.IndexOf(ListBox1.SelectedItem));
            }
            catch
            {
                MessageBox.Show("未选中要删除的账户！");
            }


        }

        private void SwitchAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginWindow.NowUsername =  ListBox1.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("未选中要切换的账户！");
            }
        }
    }
}
