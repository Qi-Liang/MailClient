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

namespace MailClient_beta_v1._1.Pages
{
    /// <summary>
    /// Interaction logic for ClassifiedMail.xaml
    /// </summary>
    public partial class ClassifiedMail : UserControl
    {
        public ClassifiedMail()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewMailGroupName.Visibility = Visibility.Visible;
            AddNewMailGroup.Visibility = Visibility.Visible;
            Add.Visibility = Visibility.Hidden;

        }

        private void AddNewMailGroup_Click(object sender, RoutedEventArgs e)
        {
            string newfolder = NewMailGroupName.Text;
            System.IO.Directory.CreateDirectory(newfolder);

            //--------------界面控件显示/隐藏操作----------------
            NewMailGroupName.Text = null;
            Add.Visibility = Visibility.Visible;
            NewMailGroupName.Visibility = Visibility.Hidden;
            AddNewMailGroup.Visibility = Visibility.Hidden;
            //-------------------------------------------------------
        }
    }
}
