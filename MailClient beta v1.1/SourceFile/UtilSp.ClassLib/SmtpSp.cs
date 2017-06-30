using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Windows;
using System.Text.RegularExpressions;

namespace UtilSp.ClassLib
{
    public class SmtpSp
    {
        #region Member

        #region boundary normal Property
        private string boundary_ = Guid.NewGuid().ToString();
        public string boundary_pro  //MIME协议边界值
        {
            get
            {
                return boundary_;
            }
            set
            {
                boundary_ = value;
            }
        }
        #endregion

        #region exception  Property
        private Exception exception_ = null;
        public Exception exception_pro  //提示异常字符串
        {
            get
            {
                return exception_;
            }
            set
            {
                exception_ = value;
            }
        }
        #endregion

        #region encoding_smtp normal Property
        private Encoding encoding_smtp_ = Encoding.ASCII;//SMTP编码类型必须为ASCII smtp encoding must be ascii.
        private Encoding encoding_smtp_pro
        {
            get
            {
                return encoding_smtp_;
            }
        }
        #endregion

        #region encoding_subject normal Property
        private Encoding encoding_subject_ = Encoding.UTF8; //邮件主题用UTF8编码
        public Encoding encoding_subject_pro
        {
            get
            {
                return encoding_subject_;
            }
            set
            {
                encoding_subject_ = value;
            }
        }
        #endregion

        #region encoding_content normal Property
        private Encoding encoding_content_ = Encoding.GetEncoding("gb2312");//邮件内容采用简体中文字符集编码
        public Encoding encoding_content_pro
        {
            get
            {
                return encoding_content_;
            }
            set
            {
                encoding_content_ = value;
            }
        }
        #endregion

        #region receiveInfo normal Property
        private string receiveInfo_ = "";
        public string receiveInfo_pro   //从服务器返回的信息
        {
            get
            {
                return receiveInfo_;
            }
            set
            {
                receiveInfo_ = value;
            }
        }
        #endregion

        #region socket normal Property
        private Socket socket_;
        public Socket socket_pro  //实现套接字接口
        {
            get
            {
                return socket_;
            }
            private set
            {
                socket_ = value;
            }
        }
        #endregion

        #endregion

        private sealed class SMTP_BACKS   //SMTP协议中返回的响应码
        {
            public const string SERVER_READY_OK = "220";
            public const string OPERATE_COMPLETE = "250";
            public const string AUTH_LOGIN_OK = "334";
            public const string USER_NAME_LOGIN_OK = "334";
            public const string PASSWORD_LOGIN_OK = "235";
            public const string DATA_OK = "354";
        }

        private sealed class SMTP_CMD  //SMTP中的命令名
        {
            public const string AUTH_LOGIN = "auth login\r\n";
            public const string MAIL_FROM = "mail from:";
            public const string RCPT_TO = "rcpt to:";
            public const string SEND_BODY = "data\r\n";
            public const string VERIFY = "ehlo hello\r\n";
        }

        public sealed class MailInfo   //邮件信息
        {
            #region attachments normal Property
            private List<string> attachments_ = new List<string>();  //附件路径列表
            public List<string> attachments_pro
            {
                get
                {
                    return attachments_;
                }
                set
                {
                    attachments_ = value;
                }
            }
            #endregion

            #region content normal Property
            private string content_ = "";
            public string content_pro   //邮件内容
            {
                get
                {
                    return content_;
                }
                set
                {
                    content_ = value;
                }
            }
            #endregion

            #region password normal Property
            private string password_ = "";
            public string password_pro   //密码属性
            {
                get
                {
                    return password_;
                }
                set
                {
                    password_ = value;
                }
            }
            #endregion

            #region receiverAddresses normal Property
            private List<string> receiverAddresses_ = new List<string>();
            public List<string> receiverAddresses_pro  //接受者地址的字符串列表
            {
                get
                {
                    return receiverAddresses_;
                }
                set
                {
                    receiverAddresses_ = value;
                }
            }
            #endregion

            #region  抄送地址属性
            private List<string> ccReceiverAddress_ = new List<string>();
            public List<string> ccReceiverAddress_pro
            {
                get
                {
                    return ccReceiverAddress_;
                }
                set
                {
                    ccReceiverAddress_ = value;
                }
            }
            #endregion
            #region  密送地址属性
            private List<string> BccReceiverAddress_ = new List<string>();
            public List<string> BccReceiverAddress_pro
            {
                get
                {
                    return BccReceiverAddress_;
                }
                set
                {
                    BccReceiverAddress_ = value;
                }
            }
            #endregion

            #region senderAddress normal Property
            private string senderAddress_ = "";
            public string senderAddress_pro   //发送者的地址
            {
                get
                {
                    return senderAddress_;
                }
                set
                {
                    senderAddress_ = value;
                }
            }
            #endregion

            #region subject normal Property
            private string subject_ = "";
            public string subject_pro  //邮件主题
            {
                get
                {
                    return subject_;
                }
                set
                {
                    subject_ = value;
                }
            }
            #endregion

            #region userName normal Property
            private string userName_ = "";
            public string userName_pro   //发送者的用户名
            {
                get
                {
                    return userName_;
                }
                set
                {
                    userName_ = value;
                }
            }
            #endregion
        }

        #region send Function
        public bool send(string mailHostIP, int port, MailInfo mailInfo)  //mailHostIP为服务器主机名，port为端口号，mailInfo为邮件信息
        {
            try
            {
                exception_pro = null;  //初始化异常提示信息
                if (!isLegal(mailHostIP, port, mailInfo)) //检查端口号和发送者地址是否合法
                {
                    return false;
                }
                socket_pro = GetSocket(mailHostIP, port);
                //      socket_pro = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //      socket_pro.Connect(mailHostIP, port);
                if (!socket_pro.Connected)   //判断Socket是否连成功
                {
                    socket_pro.Close();
                    exception_pro = new Exception(Tip.ConnectFail);
                    return false;
                }

                if (!isServerReady())  //是否连接上服务器
                {
                    exception_pro = new Exception(Tip.ServerReadyFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!verify())   //HELO
                {
                    exception_pro = new Exception(Tip.VerifyFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!authLogin())  //请求认证
                {
                    exception_pro = new Exception(Tip.AuthLoginFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!userNameLogin(mailInfo.userName_pro))  //用户名认证
                {
                    exception_pro = new Exception(Tip.UserNameLoginFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!passowrdLogin(mailInfo.password_pro))   //密码认证
                {
                    exception_pro = new Exception(Tip.PasswordLoginFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!mailFrom(mailInfo.senderAddress_pro))  //发送者邮箱
                {
                    exception_pro = new Exception(Tip.MailFromFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!rcptTo(mailInfo.receiverAddresses_pro))  //接受者邮箱
                {
                    exception_pro = new Exception(Tip.RcptToFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!sendMail(mailInfo)) //发送邮件
                {
                    exception_pro = new Exception(Tip.SendMailFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                socket_pro.Close();   //关闭socket，邮件传输结束
                return true;
            }
            catch (System.Exception ex)
            {
                exception_pro = ex;
                return false;
            }
        }

        private string analyReceiveError()
        {
            if (string.IsNullOrEmpty(receiveInfo_pro))
            {
                return Tip.ReceiveInfoEmpty;
            }
            return receiveInfo_pro;
        }

        #endregion

        private int socketSend(string sendStr)
        {
            byte[] sendBuffer = encoding_smtp_pro.GetBytes(sendStr);  //将字符串sendStr编码为ASCII码字节序列
            return socket_pro.Send(sendBuffer);  //将字节序列sendBuffer发送至套接字
        }
        private string receive()
        {
            byte[] receiveData = new byte[10240];
            int receiveLen = socket_pro.Receive(receiveData);  //从绑定的套接字接收数据，并将数据存入接收缓冲区receiveData
            receiveInfo_pro = encoding_smtp_pro.GetString(receiveData, 0, receiveLen); //将缓冲区中的数据解码为Unicode字符
            Console.WriteLine(receiveInfo_pro);
            return receiveInfo_pro;
        }

        #region send ready functions

        #region isLegal Function
        public bool isLegal(string mailHostIP, int port, MailInfo mailInfo)
        {
            if (!IpPortSp.isPort(port.ToString())) //检查端口号是否合法
            {
                exception_pro = new Exception(Tip.PortIllegal);
                return false;
            }
            if (!MailSp.isMail(mailInfo.senderAddress_pro))  //检查发送者地址输入是否合法
            {
                exception_pro = new Exception(Tip.SenderIllegal);
                return false;
            }
            return true;
        }
        #endregion

        #region isServerReady Function
        public bool isServerReady()
        {
            string back = receive();
            return back.Substring(0, 3).Equals(SMTP_BACKS.SERVER_READY_OK); //检测服务器返回码是否为220
        }
        #endregion

        #region verify Function
        public bool verify()
        {
            socketSend(SMTP_CMD.VERIFY);  //发送"ehlo hello\r\n"至套接字 
            string[] verifyBacks = receive().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //将服务器的响应存到字符串数组verifyBacks,以"\r\n"分隔，不返回空子字符串
            for (int index = 0; index < verifyBacks.Length; index++)
            {
                string verifyBack = verifyBacks[index];
                if (verifyBack.Length <= 3)
                {
                    return false;
                }

                if (!verifyBack.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE)) //检测响应码是否为250
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region authLogin function
        private bool authLogin()
        {
            socketSend(SMTP_CMD.AUTH_LOGIN);  //客户端发送 "auth login\r\n"至套接字
            string authLoginBack = receive();   //authLoginBack存储服务器响应
            if (string.IsNullOrEmpty(authLoginBack) || authLoginBack.Length <= 3) //服务器响应不能为空且字符数大于3
            {
                return false;
            }
            return authLoginBack.Substring(0, 3).Equals(SMTP_BACKS.AUTH_LOGIN_OK); //检测服务器响应码是否为334
        }
        #endregion

        #region userNameLogin function
        private bool userNameLogin(string userName)
        {
            if (string.IsNullOrEmpty(userName))  //检测用户名是否为空
            {
                return false;
            }
            string base64UserName = Base64Sp.tobase64Str(userName); //将userName转化为用base64数字编码的等效字符串形式
            socketSend(base64UserName + "\r\n"); //发送经base64编码的用户名
            //  socketSend(userName + "\r\n");
            string userNameLoginBack = receive();  //服务器响应存储到字符串userNameLoginBack
            if (string.IsNullOrEmpty(userNameLoginBack) || userNameLoginBack.Length <= 3)
            {
                return false;
            }
            return userNameLoginBack.Substring(0, 3).Equals(SMTP_BACKS.USER_NAME_LOGIN_OK);  //服务器正常响应码为334
        }


        #endregion

        #region passowrdLogin function
        //***********************************************************
        private bool passowrdLogin(string password)
        {
            if (string.IsNullOrEmpty(password))  //检测密码是否为空
            {
                return false;
            }
            socketSend(Base64Sp.tobase64Str(password) + "\r\n");  //发送经base64编码的password
            // socketSend(password+"\r\n");
            string passowrdLoginBack = receive();  //passwordLoginBack为服务器响应
            if (string.IsNullOrEmpty(passowrdLoginBack) || passowrdLoginBack.Length <= 3)
            {
                return false;
            }
            return passowrdLoginBack.Substring(0, 3).Equals(SMTP_BACKS.PASSWORD_LOGIN_OK); //服务器正常响应码为235
        }
        //****************************************************************

        #endregion

        #region mailFrom function
        private bool mailFrom(string senderAddress)
        {
            socketSend(SMTP_CMD.MAIL_FROM + "<" + senderAddress + ">\r\n"); //发送"mail from:<senderAddress>\r\n"
            string mailFromBack = receive();
            if (string.IsNullOrEmpty(mailFromBack) || mailFromBack.Length <= 3)
            {
                return false;
            }
            return mailFromBack.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE);  //服务器正常响应码为250
        }
        #endregion

        #region rcptTo function
        private bool rcptTo(List<string> receiverAddresses)
        {
            foreach (string receiverAddress in receiverAddresses)
            {
                socketSend(SMTP_CMD.RCPT_TO + "<" + receiverAddress + ">\r\n");
                //发送"rcpt to:<receiverAddress>\r\n",可以有多行
                string rcptBack = receive();
                if (string.IsNullOrEmpty(rcptBack) || rcptBack.Length <= 3)
                {
                    return false;
                }

                if (!rcptBack.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE)) //服务器正常返回码为250
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #endregion

        #region send mail body functions
        private bool sendMail(MailInfo mailInfo)
        {
            socketSend(SMTP_CMD.SEND_BODY); //发送"data\r\n"
            string back = receive();
            if (string.IsNullOrEmpty(back) || back.Length <= 3)
            {
                return false;
            }

            if (!back.Substring(0, 3).Equals(SMTP_BACKS.DATA_OK))  //服务器正常响应码354
            {
                return false;
            }

            sendMailHeader(mailInfo);  //发送邮件头部
            sendMailContent(mailInfo.content_pro);  //发送邮件体
            sendMailAttachment(mailInfo.attachments_pro);  //发送附件
            sendMailTail();  //发送邮件尾"\r\n.\r\n"

            back = receive();
            if (string.IsNullOrEmpty(back) || back.Length <= 3)
            {
                return false;
            }

            if (!back.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE))  //服务器正常响应码250
            {
                return false;
            }

            return true;
        }

        private void sendMailAttachment(List<string> attachments)
        {
            foreach (string attachment in attachments)
            {
                if (!File.Exists(attachment))  //检测指定的路径是否存在
                {
                    continue;
                }
                sendAttachmentHeader(attachment);  //发送附件头部
                sendAttachmentData(attachment);  //发送附件内容
            }
        }

        private void sendAttachmentData(string attachment)
        {
            byte[] attachmentData = FileSp.readFileBytes(attachment);  //将文件中数据以字节形式读入缓冲区attachmentData
            socketSend(Convert.ToBase64String(attachmentData));
            //将attachment转化为base64字符串形式

            //            socketSend("\r\n\r\n");
            socketSend("\r\n");
            socketSend("--" + boundary_pro + "--" + "\r\n\r\n\r\n");
        }

        private void sendAttachmentHeader(string attachment)
        {
            //string contentType = "application/octet-stream";//text/plain;
            string contentType = "text/plain";
            string header = "--" + boundary_pro + "\r\n"
                    + "Content-Type:" + contentType + ";name=" + attachment + "\r\n"
                    + "Content-Transfer-Encoding:base64\r\n"
                    + "Content-Disposition:attachment;filename=\"" + attachment + "\"\r\n";
            socketSend(header + "\r\n");    //发送附件头部
        }

        private void sendMailTail()
        {
            socketSend("\r\n.\r\n");//. is content end flag.        
        }

        private void sendMailHeader(MailInfo mailInfo)  //发送邮件头
        {
            string to = "";  //收信人地址
            string cc = "";  //抄送人地址
            string bcc = "";  //密送人地址
            foreach (string receiverAddress in mailInfo.receiverAddresses_pro)
            {
                to += "To:" + receiverAddress + "\r\n";  //to = "To: receiverAddress\r\n",可以有多行
            }

            foreach (string CcreceiverAddress in mailInfo.ccReceiverAddress_pro)
            {
                cc += "Cc:" + CcreceiverAddress + "\r\n";
            }

            foreach (string BccreceiverAddress in mailInfo.BccReceiverAddress_pro)
            {
                bcc += "Bcc:" + BccreceiverAddress + "\r\n";
            }

            //if you want to hide receiver,may assign to="To:abc@a.com\r\n";
            //string to = "To:abc@a.com\r\n";
            //           socketSend(to);  //发送"To"头部域
            //            socketSend(cc);  //发送"Cc"头部域
            //            socketSend(bcc); //发送"Bcc"头部域

            string from = "From:" + mailInfo.senderAddress_pro + "\r\n";
            socketSend(from);  //发送"From"头部 “From: senderAddress\r\n"
            socketSend(to);  //发送"To"头部域
            socketSend(cc);  //发送"Cc"头部域
            socketSend(bcc); //发送"Bcc"头部域

            string subject = "Subject:=?" + encoding_subject_pro.BodyName + "?B?" + Base64Sp.tobase64Str(mailInfo.subject_pro, Encoding.UTF8) + "?=\r\n";
            socketSend(subject);
            //发送"Subject"头部，“Subject:=?<编码名称>？B?<对subject_pro进行UTF-8编码，再对其进行base64编码>?=\r\n"
            string header = "Mime-Version:1.0\r\n"
                      + "Content-type:multipart/mixed;" + "boundary=\"" + boundary_pro + "\";\r\n"
                //        + "Content-Transfer-Encoding:7bit\r\n"
                      + "Content-Transfer-Encoding:8bit\r\n"
                      + "This is a multi-part message in MIME format\r\n";  //附件文本行，解码时忽略
            socketSend(header + "\r\n");
        }

        private void sendMailContent(string content)
        {
            sendContentHeader();
            sendContentBody(content); //发送邮件内容
        }

        private void sendContentBody(string content)
        {
            //If content is too long,need split some blocks and send.
            //Length of one line is always 80 byte.
            string newContent = Base64Sp.tobase64Str(content, encoding_content_pro);
            //将gb2312编码的字符串表示为base64编码
            socketSend(newContent + "\r\n");   //内容结尾处要有一行空行
        }

        private void sendContentHeader()  //发送邮件体头部
        {
            string header = "--" + boundary_pro + "\r\n"
                 + "Content-type:text/plain;charset=" + encoding_content_pro.BodyName + "\r\n"
                 + "Content-Transfer-Encoding:base64\r\n";
            socketSend(header + "\r\n");   //头部后要有一行空行
        }

        #endregion


        private class Tip
        {
            public static string AuthLoginFail = "Auth login fail!";
            public static string ConnectFail = "Connect mail host fail!";
            public static string IpIllegal = "Ip is illegal!";
            public static string MailFromFail = "Input sender address fail!";
            public static string PasswordLoginFail = "Password login fail!";
            public static string PortIllegal = "Port is illegal!";
            public static string ReceiveInfoEmpty = "Receive info is empty!";
            public static string RcptToFail = "Input receiver address fail!";
            public static string SenderIllegal = "Sender mail address is illegal!";
            public static string SendMailFail = "Send mail fail!";
            public static string ServerReadyFail = "Server ready fail!";
            public static string UserNameLoginFail = "User name login fail!";
            public static string VerifyFail = "Verify identity fail!";
        }

        public Socket GetSocket(string ServerName, int port)
        {
            IPHostEntry hostEntry = null;
            hostEntry = GetHostEntry(ServerName);
            if (hostEntry != null)
            {
                foreach (IPAddress address in hostEntry.AddressList)
                {
                    socket_pro = TryGetSocket(address, port); //与主机address的port端口建立连接
                    if (socket_pro != null) { break; }
                }
            }
            return socket_pro;
        }

        private IPHostEntry GetHostEntry(string ServerName)  //获得目标主机ServerName的IP地址
        {
            try
            {
                return Dns.GetHostEntry(ServerName);  //解析地址，返回IPHostEntry实例
            }
            catch
            {
                MessageBox.Show("解析地址错误");
            }
            return null;
        }

        private Socket TryGetSocket(IPAddress address, int port)  //Socket连接
        {
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket tc = null;
            try
            {
                tc = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                tc.Connect(ipe);
                if (tc.Connected == false)
                {
                    MessageBox.Show(Tip.ConnectFail);
                }
            }
            catch
            {
                tc = null;
                MessageBox.Show("连接失败!");
            }
            return tc;
        }


        public string ServernameReg(string s)
        {
            string pattern = @"@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            Match MatchText;
            string value;
            string Servername;
            string hostname;
            MatchText = reg.Match(s);
            value = MatchText.Value;
            //去除@
            hostname = value.Remove(0, 1);
            Servername = string.Concat("smtp.", hostname);
            return Servername;
        }

        public string Username(string s)
        {
            string pattern = @"@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            Match MatchText;
            string value;
            string Username;
            MatchText = reg.Match(s);
            value = MatchText.Value;
            //hostname = value.Remove(0, 1);
            Username = s.Replace(value, "");
            return Username;
        }

    }
}
