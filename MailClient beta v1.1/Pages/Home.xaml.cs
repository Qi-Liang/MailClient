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

namespace MailClient_beta_v1._1.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>

    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            this.DataContext = MailClient_beta_v1._1.SourceFile.GlobalUse.Notification;
        }

        private void 目标邮箱_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }



        private void 发送_Click(object sender, RoutedEventArgs e)
        {
            //     Process();
            UtilSp.ClassLib.SmtpSp smtpSp = new UtilSp.ClassLib.SmtpSp();
            UtilSp.ClassLib.SmtpSp.MailInfo mailInfo = new UtilSp.ClassLib.SmtpSp.MailInfo();
            string servername;
            string MainDirec = @"MailClient\\" + MailClient_beta_v1._1.LoginWindow.NowUsername + "\\users";
            string UnPath = @MainDirec + "\\Username.txt";
            string PsPath = @MainDirec + "\\Password.txt";
            using (StreamReader sr = File.OpenText(UnPath))
            {
                mailInfo.senderAddress_pro = sr.ReadLine();
            }
            using (StreamReader sr = File.OpenText(PsPath))
            {
                mailInfo.password_pro = sr.ReadLine();
            }
            mailInfo.receiverAddresses_pro = new List<string>(目标邮箱.Text.Split(';'));
            mailInfo.ccReceiverAddress_pro = new List<string>(抄送邮箱.Text.Split(';'));
            mailInfo.BccReceiverAddress_pro = new List<string>(密送邮箱.Text.Split(';'));
            mailInfo.subject_pro = 主题内容.Text;
            mailInfo.content_pro = 写信内容.Text;
            mailInfo.userName_pro = smtpSp.Username(MailClient_beta_v1._1.LoginWindow.NowUsername);
            servername = smtpSp.ServernameReg(MailClient_beta_v1._1.LoginWindow.NowUsername);
            if (listBoxAttachment.Items.Count > 0)
            {
                string[] attachments = new string[listBoxAttachment.Items.Count];
                listBoxAttachment.Items.CopyTo(attachments, 0);
                mailInfo.attachments_pro = new List<string>(attachments);
            }
            Process();
            sendstat_pro = smtpSp.send(servername, 25, mailInfo);
            Process2();
            /*       if(!sendstat_pro)
                   {
                       MessageBox.Show("send fail!");
                   }
                   else
                   {
                       MessageBox.Show("send ok");
                  }          */
        }

        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        private void Process()
        {
            ProgressBar1.Visibility = Visibility.Visible;
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 200;
            ProgressBar1.Value = 0;

            double value = 0;
            double temp = 150;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar1.SetValue);

            do
            {
                value += 1;
                Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });

            }
            //while (ProgressBar1.Value != ProgressBar1.Maximum);
            while (ProgressBar1.Value != temp);
            /*          ProgressBar1.Visibility = Visibility.Hidden;
                      if (!sendstat_pro)
                      {
                          MessageBox.Show("send fail!");
                      }
                      else
                      {
                          MessageBox.Show("send ok");
                      }          */
        }

        private void Process2()
        {
            double value = 150;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar1.SetValue);
            do
            {
                value += 1;
                Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });

            }
            while (ProgressBar1.Value != ProgressBar1.Maximum);
            ProgressBar1.Visibility = Visibility.Hidden;
            if (!sendstat_pro)
            {
                MessageBox.Show("send fail!");
            }
            else
            {
                MessageBox.Show("send ok");
            }
        }



        private void listBoxAttachment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private bool sendstat_;
        public bool sendstat_pro
        {
            get
            {
                return sendstat_;
            }
            set
            {
                sendstat_ = value;
            }
        }

        private void listBoxAttachment_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void listBoxAttachment_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void listBoxAttachment_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String s in files)
                {
                    (sender as ListBox).Items.Add(s);
                }
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            目标邮箱.Clear();
            抄送邮箱.Clear();
            密送邮箱.Clear();
            主题内容.Clear();
            写信内容.Clear();
        }

        private void listBoxAttachment_DragLeave_1(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void listBoxAttachment_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            (sender as ListBox).Items.Remove((sender as ListBox).SelectedItem.ToString());
        }


    }
}
