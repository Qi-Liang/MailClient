using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;


namespace MailClient_beta_v1._1.SourceFile
{
    public class Receive
    {
        private Encoding EncodingImap = Encoding.ASCII;
        private Encoding EcondingImapPro
        {
            get
            {
                return EncodingImap;
            }
        }

        private Socket socket_;
        public Socket socket_pro
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

        private SslStream SslStream_;

        public SslStream Pop3Ssl
        {
            get
            {
                return SslStream_;
            }
            private set
            {
                SslStream_ = value;

            }
        }

        private sealed class Pop3_CMD
        {
            public const string USER = "user ";
            public const string PASS = "pass ";
            public const string LIST = "list ";
            public const string DOWN = "retr ";
            public const string DEL = "dele ";
            public const string ENTER = "\r\n";
        }

        private sealed class Pop3_BACK
        {
            public const string OK = "+OK";
            public const string ERROR = "-ERR";
        }

        private sealed class Pop3_para
        {
            public const int LEN = 1024;
        }

        public class var
        {
            public static string hostIP;
            public static string ID;
            public static string passwd;
            public static string Setting = "[Setting]";
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        #region 建立TCP连接
        public bool connect(string hostIP, int port, bool Ssl)
        {
            byte[] temp = new byte[Pop3_para.LEN];
            try
            {
                socket_pro = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket_pro.Connect(hostIP, port);
                if (Ssl)
                {
                    Pop3Ssl = new SslStream(new NetworkStream(socket_pro), true, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    //socketreceive(1024);//receive ready
                    Pop3Ssl.AuthenticateAsClient(hostIP);
                    //Pop3Ssl.Read(temp, 0, Pop3_para.LEN);
                    //Console.WriteLine(Encoding.ASCII.GetString(temp, 0, Pop3_para.LEN));
                }
                Console.WriteLine(socketreceive(Pop3_para.LEN, Ssl));
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex + "\n连接建立失败!");
                return false;
            }

        }
        #endregion

        #region send
        public void socketsend(string sendStr, bool Ssl)
        {
            byte[] sendbyte = EcondingImapPro.GetBytes(sendStr);
            try
            {
                if (Ssl)
                {
                    Pop3Ssl.Write(sendbyte);
                }
                else
                {
                    socket_pro.Send(sendbyte);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("发送失败" + ex);
            }

        }
        #endregion

        #region receive
        public string socketreceive(int len, bool Ssl)
        {
            byte[] Receive = new byte[len + 2];
            int ReceiveLen;
            //List<> list = new List<>(1024);
            try
            {
                if (Ssl)
                {
                    ReceiveLen = Pop3Ssl.Read(Receive, 0, len);
                    return Encoding.ASCII.GetString(Receive, 0, ReceiveLen);
                }
                else
                {
                    ReceiveLen = socket_pro.Receive(Receive);
                    string ReceiveInf = EcondingImapPro.GetString(Receive, 0, ReceiveLen);
                    return ReceiveInf;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("接收失败！" + ex);
                return "false";
            }
        }
        #endregion

        #region login
        public bool login(string ID, string passwd, ref int num, bool Ssl)
        {
            string rec;
            Regex regex = new Regex(@"(?<=\+OK\s).+?(?=\smessage)");
            socketsend(Pop3_CMD.USER + ID + Pop3_CMD.ENTER, Ssl);
            if (socketreceive(Pop3_para.LEN, Ssl).Substring(0, 3) == Pop3_BACK.ERROR)
            {
                //Console.WriteLine("The ID do not exit!");
                MessageBox.Show("The ID do not exit!");
                return false;
            }
            socketsend(Pop3_CMD.PASS + passwd + Pop3_CMD.ENTER, Ssl);
            rec = socketreceive(Pop3_para.LEN, Ssl);
            if (rec.Substring(0, 3) == Pop3_BACK.ERROR)
            {
                //Console.WriteLine("passwd ERROR!");
                MessageBox.Show("passwd ERROR!");
                return false;
            }
            Match result = regex.Match(rec);
            if (result.Success)
                num = Convert.ToInt32(result.Value);
            else
                return false;
            //Console.WriteLine(result.Value);
            return true;
        }
        #endregion


        //path——路径、name——文件名、data——写数据
        public void Write(string path, string name, string data)
        {

            //string path = "G:\\mail\\" + ID + "\\";
            //path += mail_ID + "\\";
            if (!Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Create();
            }
            FileStream fs = new FileStream(path + name, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(data);
            sw.Close();
            fs.Close();
        }

        #region download
        public void download(string ID, int num, bool Ssl, bool competle, bool del)
        {
            int i;
            string I, mail;
            string path = "MailClient\\" + ID + "\\receive\\";
            delete("MailClient\\" + ID + "\\receive");
            try
            {
                for (i = 1; i <= num; i++)
                {
                    I = Convert.ToString(i);
                    mail = "";
                    socketsend(Pop3_CMD.DOWN + I + Pop3_CMD.ENTER, Ssl);
                    //Thread.Sleep(50);
                    while (true)
                    {
                        mail = mail + socketreceive(Pop3_para.LEN, Ssl);
                        if (mail.Substring(mail.Length - 3, 3) == ".\r\n")
                        {
                            break;
                        }
                    }
                    //string path = "MailClient\\" + ID + "\\receive\\";
                    Write(path, I + ".eml", mail);
                    if (competle)
                    {
                        //if (I == "30")
                        //    MessageBox.Show("666");
                        Write(path, I + ".html", ReadEML(path + I + ".eml", false).Replace("utf-8", "gb2312").Replace("UTF-8", "gb2312"));
                        //Write(path, I + ".html", ASCIIEncoding.UTF32.GetString(Encoding.UTF8.GetBytes(ReadEML(path + I + ".eml"))));
                        //Write(path, I + ".html", System.Text.Encoding.GetEncoding("UTF-8").GetString(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(ReadEML(path + I + ".eml"))));
                        //Write(path, I + ".html", LanChange(ReadEML(path + I + ".eml").Replace("utf-8", "gb2312")));

                        FileInfo MyFileInfo = new FileInfo(path + I + ".html");
                        float MyFileSize = (float)MyFileInfo.Length;
                        if (MyFileSize == 0)
                        {
                            if (regex_title_CTE(mail) == "base64")
                            {
                                Write(path, I + ".html", ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(mail_body(File.ReadAllText(path + I + ".eml", System.Text.Encoding.Default)))));
                            }
                            else
                                Write(path, I + ".html", ReadEML(path + I + ".eml", true).Replace("utf-8", "gb2312").Replace("UTF-8", "gb2312"));
                        }

                    }
                    else
                    {
                        Write(path, I + ".eml", mail_title(mail));
                    }
                    if (del)
                        socketsend(Pop3_CMD.DEL + I + Pop3_CMD.ENTER, Ssl);
                }
                MessageBox.Show("邮件更新完成！\n");
            }
            catch(System.Exception ex)
            {
                MessageBox.Show("下载出错\n" + ex);
            }
            
        }
        #endregion

        //#region
        //private const string QpSinglePattern = "(\\=([0-9A-F][0-9A-F]))";

        //private const string QpMutiplePattern = @"((\=[0-9A-F][0-9A-F])+=?\s*)+";

        //public static string Decode(string contents, Encoding encoding)
        //{
        //    if (contents == null)
        //    {
        //        throw new ArgumentNullException("contents");
        //    }

        //    // 替换被编码的内容
        //    string result = Regex.Replace(contents, QpMutiplePattern, new MatchEvaluator(delegate (Match m)
        //    {
        //        List<byte> buffer = new List<byte>();
        //        // 把匹配得到的多行内容逐个匹配得到后转换成byte数组
        //        MatchCollection matches = Regex.Matches(m.Value, QpSinglePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //        foreach (Match match in matches)
        //        {
        //            buffer.Add((byte)HexToByte(match.Groups[2].Value.Trim()));
        //        }
        //        return encoding.GetString(buffer.ToArray());
        //    }), RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //    // 替换多余的链接=号
        //    result = Regex.Replace(result, @"=\s+", "");

        //    return result;
        //}

        //private static int HexToByte(string hex)
        //{
        //    int num1 = 0;
        //    string text1 = "0123456789ABCDEF";
        //    for (int num2 = 0; num2 < hex.Length; num2++)
        //    {
        //        if (text1.IndexOf(hex[num2]) == -1)
        //        {
        //            return -1;
        //        }
        //        num1 = (num1 * 0x10) + text1.IndexOf(hex[num2]);
        //    }
        //    return num1;
        //}
        //#endregion

        private bool delete(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return true;
            }
            return false;
        }

        public static string regex_title_Sender(string data)
        {
            //Regex Sender = new Regex(@"(?<=Sender:\s).+?(?=\r\n)");
            //Match Sender_match = Sender.Match(data);// match Sender

            //if (Sender_match.Success)
            //    return Convert.ToString(Sender_match.Value);
            //else
            //{
            Regex from = new Regex(@"(?<=\r\nFrom:(\s|)).+?(?=\r\n)");
            Match from_match = from.Match(data);// match Sender
            if (from_match.Success)
            {
                Regex address = new Regex(@"(?<=<).+?(?=>)");
                Match address_match = address.Match(Convert.ToString(from_match.Value));// match Sender
                if (address_match.Success)
                    return Convert.ToString(address_match.Value);
                else
                {
                    Regex from_2 = new Regex(@"(?<=(B|b)\?).+?(?=\?)");
                    Match from_2_match = from_2.Match(Convert.ToString(from_match.Value));// match Subject

                    if (from_2_match.Success)
                    {
                        Regex Sub_utf8 = new Regex(@"(?<=\?).+?(?=\?)");
                        Match Sub_utf8_match = Sub_utf8.Match(Convert.ToString(from_match.Value));// match Subject
                        if (Sub_utf8_match.Success)
                        {
                            if ((Sub_utf8_match.Value == "UTF-8") || (Sub_utf8_match.Value == "utf-8"))
                                return ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Convert.ToString(from_2_match.Value)));
                        }
                        return ASCIIEncoding.Default.GetString(Convert.FromBase64String(Convert.ToString(from_2_match.Value)));
                    }
                    return Convert.ToString(from_match.Value);
                }
                
            }
            else
            {
                Regex Sender = new Regex(@"(?<=Sender:\s).+?(?=\r\n)");
                Match Sender_match = Sender.Match(data);// match Sender

                if (Sender_match.Success)
                    return Convert.ToString(Sender_match.Value);
            }
            //}
            return "no Sender";
        }
        private string regex_title_TO(string data)
        {
            Regex To = new Regex(@"(?<=To:\s).+?(?=\r\n)");
            Match To_match = To.Match(data);// match To
            if (To_match.Success)
                return Convert.ToString(To_match.Value);
            else
                return "no title";
        }
        private string regex_title_charset(string data)
        {
            Regex charset = new Regex(@"(?<=charset=).+?(?=\r\n)");
            Match charset_match = charset.Match(data);// match charset
            if (charset_match.Success)
                return Convert.ToString(charset_match.Value);
            else
                return "ERROR";
        }

        public static string regex_title_Sub(string data)//表达式还有待确定----------------------
        {
            Regex Sub = new Regex(@"(?<=Subject:).+?(?==\r\n)");
            Match Sub_match = Sub.Match(data);// match Subject
            if (Sub_match.Success)
            {
                Regex Sub_2 = new Regex(@"(?<=B\?).+?(?=\?)");
                Match Sub_2_match = Sub_2.Match(Convert.ToString(Sub_match.Value));// match Subject
                if (Sub_2_match.Success)
                {
                    Regex Sub_utf8 = new Regex(@"(?<=\?).+?(?=\?)");
                    Match Sub_utf8_match = Sub_utf8.Match(Convert.ToString(Sub_match.Value));// match Subject
                    if (Sub_utf8_match.Success)
                    {
                        if ((Sub_utf8_match.Value == "UTF-8") || (Sub_utf8_match.Value == "utf-8"))
                            return ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Convert.ToString(Sub_2_match.Value)));
                    }
                    return ASCIIEncoding.Default.GetString(Convert.FromBase64String(Convert.ToString(Sub_2_match.Value)));
                }
                else
                {
                    Regex Sub_2_p = new Regex(@"(?<=b\?).+?(?=\?)");
                    Match Sub_2_p_match = Sub_2_p.Match(Convert.ToString(Sub_match.Value));// match Subject
                    if (Sub_2_p_match.Success)
                    {
                        Regex Sub_utf8 = new Regex(@"(?<=\?).+?(?=\?)");
                        Match Sub_utf8_match = Sub_utf8.Match(Convert.ToString(Sub_match.Value));// match Subject
                        if (Sub_utf8_match.Success)
                        {
                            if ((Sub_utf8_match.Value == "UTF-8") || (Sub_utf8_match.Value == "utf-8"))
                                return ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Convert.ToString(Sub_2_p_match.Value)));
                        }
                        return ASCIIEncoding.Default.GetString(Convert.FromBase64String(Convert.ToString(Sub_2_p_match.Value)));
                    }
                        
                }
            }
            else
            {
                Regex Sub_p = new Regex(@"(?<=SUBJECT:).+?(\r\n)");
                Match Sub_p_match = Sub_p.Match(data);// match Subject
                if (Sub_p_match.Success)
                {
                    return Convert.ToString(Sub_p_match.Value);//明文
                }
            }
          //  Regex Sub_3 = new Regex(@"(?<=Subject:\s?).+?(?=\r\n)");
            Regex Sub_3 = new Regex(@"(?<=\r\nSubject:(\s|)?).+?(?=\r\n)");
            Match Sub_3_match = Sub_3.Match(data);// match Subject
            if (Sub_3_match.Success)
                return Convert.ToString(Sub_3_match.Value);//明文
            return "can not find Subject";
        }

        private static string regex_title_CTE(string data)
        {
            Regex CTE = new Regex(@"(?<=Content-Transfer-Encoding:(\s|)).+?(?=\r\n)");
            //Regex CTE = new Regex(@"base64");
            Match CTE_match = CTE.Match(data);//Content - Transfer - Encoding
            if (CTE_match.Success)
                return Convert.ToString(CTE_match.Value);
            else
                return "ERROR";
        }

        //public static string mail_body(string data)///--------------------------
        //{
        //    int i = 0;
        //    int end;
        //    string body;

        //    while (true)
        //    {
        //        if ((data.Substring(i++, 3) == "\r\n."))
        //        {
        //            i = i - 10;
        //            break;
        //        }
        //        if ((data.Substring(i++, 5) == "\r\n\r\n."))
        //        {
        //            i = i - 10;
        //            break;
        //        }

        //    }

        //    end = i + 9;
        //    //i = i - 2;
        //    while (true)
        //    {
        //        if (data.Substring(i, 4) == "\r\n\r\n")
        //        {
        //            body = data.Substring(i + 4, end - i - 3);
        //            break;
        //        }
        //        i--;
        //    }
        //    return body;
        //}

        public static string mail_body(string data)///--------------------------
        {
            int i = 0;
            string body;
            while (true)
            {
                if (data.Substring(i, 4) == "\r\n\r\n")
                {
                    body = data.Substring(i + 4, data.Length - i - 7);
                    break;
                }
                i++;
            }
            return body;
        }

        private string mail_title(string data)
        {
            int i = 0;
            string title;
            while (true)
            {
                if (data.Substring(i, 4) == "\r\n\r\n")
                {
                    title = data.Substring(0, i + 4);
                    break;
                }
                i++;
            }
            title = title + "邮件未下载.\r\n";
            return title;
        }
        public static string ReadEML(string file, bool x)
        {
            CDO.Message oMsg = new CDO.Message();
            ADODB.Stream stm = null;
            //读取EML文件到CDO.MESSAGE，做分析的话，实际是用了下面的部分
            try
            {
                stm = new ADODB.Stream();
                stm.Open(System.Reflection.Missing.Value,
                         ADODB.ConnectModeEnum.adModeUnknown,
                         ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified,
                         "", "");
                stm.Type = ADODB.StreamTypeEnum.adTypeBinary;//二进制方式读入

                stm.LoadFromFile(file); //将EML读入数据流

                oMsg.DataSource.OpenObject(stm, "_stream"); //将EML数据流载入到CDO.Message，要做解析的话，后面就可以了。 

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return ex.ToString();
            }
            finally
            {
                stm.Close();
            }
            if (x)
            {
                return oMsg.TextBody;
            }
            else
                return oMsg.HTMLBody;//oMsg里包含了邮件相关的所有信息
        }
        public void CloseSocket()
        {
            socket_pro.Close();
        }
    }
}
