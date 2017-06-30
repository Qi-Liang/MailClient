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
using MailClient_beta_v1._1;
using System.IO;
using MailClient_beta_v1._1.SourceFile;

namespace MailClient_beta_v1._1.Pages
{
    /// <summary>
    /// Interaction logic for MailManage.xaml
    /// </summary>
    public partial class MailManage : UserControl
    {
        public MailManage()
        {
            InitializeComponent();
        }

        private void 删除本地邮件_Click(object sender, RoutedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";

            foreach (ListBoxItem i in delete.SelectedItems)
            {
                //delete.Items.Remove(i);
                del(path + mail_path + i.Tag.ToString() + ".eml");
                del(path + mail_path + i.Tag.ToString() + ".html");
            }
            reload();
        }

        private static bool del(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            reload();
        }

        public void reload()
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            string DirectoryPathnow_ = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\";

            delete.Items.Clear();
            if (Directory.Exists(DirectoryPathnow_))
            {
                System.IO.DirectoryInfo dir_ = new System.IO.DirectoryInfo(DirectoryPathnow_);
                FileInfo[] mails_ = dir_.GetFiles("*.eml");
                foreach (var tmpname in mails_)
                {
                    string tmp = tmpname.ToString();
                    string name = tmp.Substring(0, tmp.Length - 4);
                    ListBoxItem temp = new ListBoxItem();
                    temp.Tag = Convert.ToString(name);
                    temp.Content = Receive.regex_title_Sender(File.ReadAllText(path + mail_path + "\\" + Convert.ToString(name) + ".eml"));
                    temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + "\\" + Convert.ToString(name) + ".eml"));
                    delete.Items.Add(temp);
                }
            }
            else
            {
                return;
            }

        }
    }
}
