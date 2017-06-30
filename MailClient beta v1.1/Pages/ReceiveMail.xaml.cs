using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using MailClient_beta_v1._1.SourceFile;
using System.Collections;
using System.Text.RegularExpressions;

namespace MailClient_beta_v1._1.Pages
{
    /// <summary>
    /// Interaction logic for ReceiveMail.xaml
    /// </summary>

    public partial class ReceiveMail : UserControl
    {
        public ReceiveMail()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewMailGroupName.Visibility = Visibility.Visible;
            AddNewMailGroup.Visibility = Visibility.Visible;
            Add.Visibility = Visibility.Hidden;
            //------------------在此下面添加代码------------------

        }
        //-----------count in AddNewMailGroup_Click--------
        public int count = 0;
        private void AddNewMailGroup_Click(object sender, RoutedEventArgs e)
        {
            string newfolder = NewMailGroupName.Text;
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            string newpath = path + mail_path + newfolder;
            if(newfolder.Length>=20)
            {
                MessageBox.Show("分组名不能长于20个字符!");
                return;
            }
            if (count >= 5)
                MessageBox.Show("不能超过5个自定义分组！");
            else
            {
                if (newfolder != "")
                {
                    if (!System.IO.Directory.Exists(newpath))
                    {
                        System.IO.Directory.CreateDirectory(newpath);
                        count++;
                        if (count == 1)
                        {
                            UserFolder1.Visibility = Visibility.Visible;
                            UserFolder1.Header = newfolder;
                            MailList1.Tag = newfolder;
                        }
                        if (count == 2)
                        {
                            UserFolder2.Visibility = Visibility.Visible;
                            UserFolder2.Header = newfolder;
                            MailList2.Tag = newfolder;
                        }
                        if (count == 3)
                        {
                            UserFolder3.Visibility = Visibility.Visible;
                            UserFolder3.Header = newfolder;
                            MailList3.Tag = newfolder;
                        }
                        if (count == 4)
                        {
                            UserFolder4.Visibility = Visibility.Visible;
                            UserFolder4.Header = newfolder;
                            MailList4.Tag = newfolder;
                        }
                        if (count == 5)
                        {
                            UserFolder5.Visibility = Visibility.Visible;
                            UserFolder5.Header = newfolder;
                            MailList5.Tag = newfolder;
                        }
                    }
                    else
                    {
                        MessageBox.Show("分组已存在！");
                    }
                }
                else
                    MessageBox.Show("分组名不能为空！");
            }
            //--------------界面控件显示/隐藏操作----------------
            NewMailGroupName.Text = null;
            Add.Visibility = Visibility.Visible;
            NewMailGroupName.Visibility = Visibility.Hidden;
            AddNewMailGroup.Visibility = Visibility.Hidden;
            //-------------------------------------------------------

            //--------------在此下面添加代码----------------------

        }

        //----------以下为实现ListBox拖拽给邮件分组部分-------
        private void MailList_MouseMove(object sender, MouseEventArgs e)
        {
            if (MailList.SelectedItem != null)
            {
                htmlLoader.Visibility = Visibility.Visible;
                UnloadMail.Visibility = Visibility.Hidden;
            }
            else
            {
                htmlLoader.Visibility = Visibility.Hidden;
                UnloadMail.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //-------------加载已有分组------------
            MailList.Items.Clear();//清除之前的items内容
            UserFolder1.Visibility = Visibility.Hidden;
            MailList1.Items.Clear();
            UserFolder2.Visibility = Visibility.Hidden;
            MailList2.Items.Clear();
            UserFolder3.Visibility = Visibility.Hidden;
            MailList3.Items.Clear();
            UserFolder4.Visibility = Visibility.Hidden;
            MailList4.Items.Clear();
            UserFolder5.Visibility = Visibility.Hidden;
            MailList5.Items.Clear();
            string DirectoryPath = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\";
            if (Directory.Exists(DirectoryPath))
            {
                System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(DirectoryPath);
                System.IO.DirectoryInfo[] ds = d.GetDirectories("*.*", System.IO.SearchOption.TopDirectoryOnly);
                string path = "MailClient\\" + LoginWindow.NowUsername;
                string mail_path = "\\Receive\\";
                int count = 0;

                foreach (System.IO.DirectoryInfo a in ds)
                {
                    count++;
                    if (count == 1)
                    {
                        UserFolder1.Visibility = Visibility.Visible;
                        UserFolder1.Header = a;
                        MailList1.Tag = a;
                        //-------------加载用户分组下已有邮件---------------
                        string DirectoryPathnow = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\" + a.ToString() + "\\";
                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(DirectoryPathnow);
                        FileInfo[] mails = dir.GetFiles("*.eml");
                        foreach (var tmpname in mails)
                        {
                            string tmp = tmpname.ToString();
                            string name = tmp.Substring(0, tmp.Length - 4);
                            ListBoxItem temp = new ListBoxItem();
                            temp.Content = Convert.ToString(name) + "\r\n" + Receive.regex_title_Sender(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            MailList1.Items.Add(temp);
                        }
                    }
                    else if (count == 2)
                    {
                        UserFolder2.Visibility = Visibility.Visible;
                        UserFolder2.Header = a;
                        MailList2.Tag = a;
                        //-------------加载用户分组下已有邮件---------------
                        string DirectoryPathnow = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\" + a.ToString() + "\\";
                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(DirectoryPathnow);
                        FileInfo[] mails = dir.GetFiles("*.eml");
                        foreach (var tmpname in mails)
                        {
                            string tmp = tmpname.ToString();
                            string name = tmp.Substring(0, tmp.Length - 4);
                            ListBoxItem temp = new ListBoxItem();
                            temp.Content = Convert.ToString(name) + "\r\n" + Receive.regex_title_Sender(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            MailList2.Items.Add(temp);
                        }
                    }
                    else if (count == 3)
                    {
                        UserFolder3.Visibility = Visibility.Visible;
                        UserFolder3.Header = a;
                        MailList3.Tag = a;
                        //-------------加载用户分组下已有邮件---------------
                        string DirectoryPathnow = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\" + a.ToString() + "\\";
                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(DirectoryPathnow);
                        FileInfo[] mails = dir.GetFiles("*.eml");
                        foreach (var tmpname in mails)
                        {
                            string tmp = tmpname.ToString();
                            string name = tmp.Substring(0, tmp.Length - 4);
                            ListBoxItem temp = new ListBoxItem();
                            temp.Content = Convert.ToString(name) + "\r\n" + Receive.regex_title_Sender(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            MailList3.Items.Add(temp);
                        }
                    }
                    else if (count == 4)
                    {
                        UserFolder4.Visibility = Visibility.Visible;
                        UserFolder4.Header = a;
                        MailList4.Tag = a;
                        //-------------加载用户分组下已有邮件---------------
                        string DirectoryPathnow = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\" + a.ToString() + "\\";
                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(DirectoryPathnow);
                        FileInfo[] mails = dir.GetFiles("*.eml");
                        foreach (var tmpname in mails)
                        {
                            string tmp = tmpname.ToString();
                            string name = tmp.Substring(0, tmp.Length - 4);
                            ListBoxItem temp = new ListBoxItem();
                            temp.Content = Convert.ToString(name) + "\r\n" + Receive.regex_title_Sender(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            MailList4.Items.Add(temp);
                        }
                    }
                    else if (count == 5)
                    {
                        UserFolder5.Visibility = Visibility.Visible;
                        UserFolder5.Header = a;
                        MailList5.Tag = a;
                        //-------------加载用户分组下已有邮件---------------
                        string DirectoryPathnow = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\" + a.ToString() + "\\";
                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(DirectoryPathnow);
                        FileInfo[] mails = dir.GetFiles("*.eml");
                        foreach (var tmpname in mails)
                        {
                            string tmp = tmpname.ToString();
                            string name = tmp.Substring(0, tmp.Length - 4);
                            ListBoxItem temp = new ListBoxItem();
                            temp.Content = Convert.ToString(name) + "\r\n" + Receive.regex_title_Sender(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + a.ToString() + "\\" + Convert.ToString(name) + ".eml"));
                            MailList5.Items.Add(temp);
                        }
                    }
                }
                string DirectoryPathnow_ = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\";
                System.IO.DirectoryInfo dir_ = new System.IO.DirectoryInfo(DirectoryPathnow_);
                FileInfo[] mails_ = dir_.GetFiles("*.eml");
                foreach (var tmpname in mails_)
                {
                    string tmp = tmpname.ToString();
                    string name = tmp.Substring(0, tmp.Length - 4);
                    ListBoxItem temp = new ListBoxItem();
                    temp.Content = Convert.ToString(name) + "\r\n" + Receive.regex_title_Sender(File.ReadAllText(path + mail_path + "\\" + Convert.ToString(name) + ".eml"));
                    temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + "\\" + Convert.ToString(name) + ".eml"));
                    MailList.Items.Add(temp);
                }
            }
            else
                return;
            //---------------加载默认分组下已有邮件-------------
            //int number, i;
            //number = Convert.ToInt32(File.ReadAllText(path + "\\number.txt"));
            //if (number != MailList.Items.Count)
            //{
            //    for (i = 1; i <= number; i++)
            //    {
            //        ListBoxItem temp = new ListBoxItem();
            //        temp.Content = Convert.ToString(i) + "\r\n" + Receive.regex_title_Sender(File.ReadAllText(path + mail_path + Convert.ToString(i) + ".eml"));
            //        temp.Content = temp.Content + "\r\n" + Receive.regex_title_Sub(File.ReadAllText(path + mail_path + Convert.ToString(i) + ".eml"));
            //        //temp.PreviewMouseLeftButtonDown += ListBoxItem_PreviewMouseMove;
            //        MailList.Items.Add(temp);
            //    }
            //}
        }

        //private void MailList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    string path = "MailClient\\" + LoginWindow.NowUsername;
        //    string mail_path = "\\Receive\\";
        //    int j = 0;
        //    string NowList;
        //    try
        //    {
        //        NowList = MailList.SelectedItem.ToString();//---------未将该部分设为实例
        //        NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
        //        while (NowList.Substring(j++, 2) != "\r\n") ;
        //        NowList = NowList.Substring(0, j - 1);
        //        //Receive.mail_body(File.ReadAllText(path + mail_path + j + ".eml"));
        //        Uri uri = new Uri(System.IO.Path.GetFullPath(path) + mail_path + NowList + ".html"); //path 
        //        htmlLoader.Source = uri;
        //    }
        //    catch (System.Exception)
        //    {
        //        return;
        //    }
        //}

        private void MailList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            string NowList;
            try
            {
                    NowList = MailList.SelectedItem.ToString();//---------未将该部分设为实例
                    NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                    while (NowList.Substring(j++, 2) != "\r\n") ;
                    NowList = NowList.Substring(0, j - 1);
                    //Receive.mail_body(File.ReadAllText(path + mail_path + j + ".eml"));

                    if (File.Exists(System.IO.Path.GetFullPath(path) + mail_path + NowList + ".html"))
                    {
                        Uri uri = new Uri(System.IO.Path.GetFullPath(path) + mail_path + NowList + ".html"); //path 
                        htmlLoader.Source = uri;
                    }
                    else
                    {
                        MessageBox.Show("不存在邮件体!\n");
                    }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("若要查看邮件，请下载邮件头");
                return;
            }
        }

        private void MailList1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            string NowList;
            try
            {
                NowList = MailList1.SelectedItem.ToString();//---------未将该部分设为实例
                NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                while (NowList.Substring(j++, 2) != "\r\n") ;
                NowList = NowList.Substring(0, j - 1);
                //Receive.mail_body(File.ReadAllText(path + mail_path + j + ".eml"));
                Uri uri = new Uri(System.IO.Path.GetFullPath(path) + mail_path +MailList1.Tag.ToString()+"\\"+ NowList + ".html"); //path 
                htmlLoader.Source = uri;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        private void MailList2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            string NowList;
            try
            {
                NowList = MailList2.SelectedItem.ToString();//---------未将该部分设为实例
                NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                while (NowList.Substring(j++, 2) != "\r\n") ;
                NowList = NowList.Substring(0, j - 1);
                //Receive.mail_body(File.ReadAllText(path + mail_path + j + ".eml"));
                Uri uri = new Uri(System.IO.Path.GetFullPath(path) + mail_path + MailList2.Tag.ToString() + "\\" + NowList + ".html"); //path 
                htmlLoader.Source = uri;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        private void MailList3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            string NowList;
            try
            {
                NowList = MailList3.SelectedItem.ToString();//---------未将该部分设为实例
                NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                while (NowList.Substring(j++, 2) != "\r\n") ;
                NowList = NowList.Substring(0, j - 1);
                //Receive.mail_body(File.ReadAllText(path + mail_path + j + ".eml"));
                Uri uri = new Uri(System.IO.Path.GetFullPath(path) + mail_path + MailList3.Tag.ToString() + "\\" + NowList + ".html"); //path 
                htmlLoader.Source = uri;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        private void MailList4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            string NowList;
            try
            {
                NowList = MailList4.SelectedItem.ToString();//---------未将该部分设为实例
                NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                while (NowList.Substring(j++, 2) != "\r\n") ;
                NowList = NowList.Substring(0, j - 1);
                //Receive.mail_body(File.ReadAllText(path + mail_path + j + ".eml"));
                Uri uri = new Uri(System.IO.Path.GetFullPath(path) + mail_path + MailList4.Tag.ToString() + "\\" + NowList + ".html"); //path 
                htmlLoader.Source = uri;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        private void MailList5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            string NowList;
            try
            {
                NowList = MailList5.SelectedItem.ToString();//---------未将该部分设为实例
                NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                while (NowList.Substring(j++, 2) != "\r\n") ;
                NowList = NowList.Substring(0, j - 1);
                //Receive.mail_body(File.ReadAllText(path + mail_path + j + ".eml"));
                Uri uri = new Uri(System.IO.Path.GetFullPath(path) + mail_path + MailList5.Tag.ToString() + "\\" + NowList + ".html"); //path 
                htmlLoader.Source = uri;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        ListBox dragSource = null;
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            dragSource = parent;
            object data = GetDataFromListBox(dragSource, e.GetPosition(parent));
            if ((data != null)&&(data!=dragSource.Items))
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }
        #region GetDataFromListBox(ListBox,Point)
        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }
                    if (element == source)
                    {
                        return null;
                    }
                }
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }
            return null;
        }

        #endregion
        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            object data = e.Data.GetData(typeof(ListBoxItem));
            ((IList)dragSource.Items).Remove(data);
            parent.Items.Add(data);

            //---------------------------拖拽事件成功后，移动(复制)对应的文件到分组文件夹下------------------------------
            string DirectoryPath = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\";
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(DirectoryPath);
            string temp = ((ListBoxItem)data).Content.ToString();
            int i = temp.IndexOf("\r\n");
            string filename = temp.Substring(0, i);
            FileInfo[] emls = d.GetFiles(filename+".eml");
            foreach(FileInfo ft in emls)
            {
                string a = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\"+((Expander)parent.Parent).Header.ToString() +"\\"+ft.ToString();
                ft.MoveTo(a); //复制到分组文件夹，改为MoveTo变为移动
            }
            FileInfo[] htmls = d.GetFiles(filename + ".html");
            foreach (FileInfo ft in htmls)
            {
                string a = @"MailClient\\" + LoginWindow.NowUsername + "\\Receive\\" + ((Expander)parent.Parent).Header.ToString() + "\\" + ft.ToString();
                ft.MoveTo(a); //复制到分组文件夹，改为MoveTo变为移动
            }

            //-------------修改number.txt----------------
            //string path = @"MailClient\\" + LoginWindow.NowUsername + "\\";
            //int num=System.Int32.Parse(File.ReadAllText(path + "number.txt"));
            //num--;
            //string snum = num.ToString();
            //using (StreamWriter sw1 = File.CreateText(path + "number.txt"))
            //{
            //    sw1.WriteLine(snum);
            //}
        }

        private void answer_Click(object sender, RoutedEventArgs e)
        {
            string Sender, NowList;
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            try
            {
                NowList = MailList.SelectedItem.ToString();
                NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                while (NowList.Substring(j++, 2) != "\r\n") ;
                NowList = NowList.Substring(0, j - 1);
                Sender = Receive.regex_title_Sender(File.ReadAllText(path + mail_path + NowList + ".eml"));
                NavigationCommands.GoToPage.Execute("/pages/Home.xaml", this);
                Task.Factory.StartNew(() =>
                {
                    GlobalUse.Notification.MsgStr1 = Sender;
                });


            }
            catch (System.Exception ex)
            {
                MessageBox.Show("未选择邮件\n" + ex);
                return;
            }


        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            //Home
            string data, NowList;
            string path = "MailClient\\" + LoginWindow.NowUsername;
            string mail_path = "\\Receive\\";
            int j = 0;
            try
            {
                NowList = MailList.SelectedItem.ToString();
                NowList = NowList.Replace("System.Windows.Controls.ListBoxItem: ", "");
                while (NowList.Substring(j++, 2) != "\r\n") ;
                NowList = NowList.Substring(0, j - 1);
                data = File.ReadAllText(path + mail_path + NowList + ".html", System.Text.Encoding.Default);
                NavigationCommands.GoToPage.Execute("/pages/Home.xaml", this);
                Task.Factory.StartNew(() =>
                {
                    GlobalUse.Notification.MsgStr2 = data;
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("未选择邮件\n" + ex);
                return;
            }
        }
    }
}
