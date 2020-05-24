using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace WebServicePark {
    /*
    public class QRToken {
        // 0x00 禁用
        // 0x01 付款
        // 0x02 签到
        private byte TokenType;
        // 0x00 无可用
        // 0x01 已生成
        // 0x02 已显示
        // 0x03 已使用
        public byte TokenStatus;
        private byte[] TokenValue;
        public DateTime TokenLifetime;
        public DateTime Lifetime { get { return TokenLifetime; } }

        public QRToken (byte type) {
            TokenType = type;
            TokenStatus = 0x01;
            TokenValue = Encoding.ASCII.GetBytes (Token.RandomGenerator (8, false));
            TokenLifetime = DateTime.Now.AddMinutes (1);
        }

        public byte[] GetToken () {
            if (TokenStatus == 0x01) {
                TokenStatus = 0x02;
                return TokenValue;
            }
            return new byte[0];
        }

        public byte CheckToken (byte[] myTokenValue) {
            if (TokenStatus == 0x02 && myTokenValue != null) {
                if (TokenValue.Length == myTokenValue.Length && TokenValue.SequenceEqual (myTokenValue)) {
                    TokenStatus = 0x03;
                    return TokenType;
                }
            }
            return 0;
        }

        public void DeleteToken () {
            TokenStatus = 0x00;
            TokenValue = new byte[0];
        }

        ~QRToken () {
            TokenValue = null;
        }
    }
    public class Token {
        public int TokenIndex;
        public string TokenUser;
        public DateTime TokenCreateTime;
        public DateTime TokenUpdateTime;
        private int TokenUsage;
        private int TokenLifetime;
        private string TokenAuth;
        private bool isPubliced;
        public string oldTokenAuth;
        public DateTime oldTokenLifetime;
        public int Index { get { return TokenIndex; } }
        public string User { get { return TokenUser; } }
        private DateTime CreateTime { get { return TokenCreateTime; } }
        private int Lifetime { get { return TokenLifetime; } }
        private string TokenKey { get { return TokenAuth; } }
        public string Init (int Index, string User, int Minutes, Token oldToken) {
            TokenIndex = Index;
            TokenUser = User;
            TokenCreateTime = DateTime.Now;
            TokenUpdateTime = DateTime.Now;
            TokenUsage = 30;
            if (Minutes > 30) { Minutes = 30; }
            if (Minutes < 0) { Minutes = 0; }
            TokenLifetime = Minutes * 60;
            TokenAuth = RandomGenerator (32, true);
            isPubliced = false;
            if (oldToken != null) {
                if (oldToken.Usable ()) {
                    oldTokenAuth = oldToken.TokenKey;
                    oldTokenLifetime = DateTime.Now.AddSeconds (5);
                } else if (oldToken.TokenUpdateTime.AddSeconds (3) > DateTime.Now && oldToken.TokenCreateTime.AddSeconds (oldToken.TokenLifetime) > DateTime.Now) {
                    oldTokenAuth = oldToken.TokenKey;
                    oldTokenLifetime = DateTime.Now.AddSeconds (5);
                }
            }
            return TokenAuth;
        }
        public void Init (Token oldToken) {
            TokenIndex = oldToken.TokenIndex;
            TokenUser = oldToken.TokenUser;
            TokenCreateTime = oldToken.TokenCreateTime;
            TokenUpdateTime = oldToken.TokenUpdateTime;
            TokenUsage = oldToken.TokenUsage;
            TokenLifetime = oldToken.TokenLifetime;
            TokenAuth = oldToken.TokenAuth;
            isPubliced = oldToken.isPubliced;
        }
        public bool Usable () {
            bool inTimes = TokenUsage > 0 ? true : false;
            bool inLifeTime = TokenCreateTime.AddSeconds (TokenLifetime) > DateTime.Now ? true : false;
            if (!inTimes && inLifeTime && string.IsNullOrEmpty (oldTokenAuth)) {
                oldTokenAuth = TokenAuth;
                if (TokenUpdateTime.AddSeconds (3) > DateTime.Now) {
                    oldTokenLifetime = DateTime.Now.AddSeconds (5);
                } else { oldTokenLifetime = DateTime.Now; }
            }
            return inTimes && inLifeTime;
        }
        public bool Replaceable () {
            bool inLifeTime = oldTokenLifetime > DateTime.Now ? true : false;
            return inLifeTime;
        }
        public void Update () {
            TokenUsage = TokenUsage - 1;
            TokenUpdateTime = DateTime.Now;
        }
        public int TokenVaildCheck (string User, string param, string sha, string DoingLog) {
            // -1: 需更新令牌
            // 0: 验证正确
            // 1: 验证失败
            // 2: 令牌无效
            // 3: 信息错误
            //            CPublic.WriteLog ("提供的 SHA：" + sha.ToUpper () + "，计算的 SHA：" + SHA1 (param + "$" + TokenAuth).ToUpper () + "，内容：" + param + "$" + TokenAuth);
            //            if (!string.IsNullOrEmpty (oldTokenAuth)) {
            //                CPublic.WriteLog ("提供的 SHA：" + sha.ToUpper () + "，计算的 SHA：" + SHA1 (param + "$" + oldTokenAuth).ToUpper () + "，内容：" + param + "$" + oldTokenAuth);
            //                CPublic.WriteLog ("检测到存在旧密钥");
            //            } else { CPublic.WriteLog ("未检测到存在旧密钥"); }
            if (TokenUser == User) {
                if (Usable ()) {
                    if (sha.ToUpper ().Equals (SHA1 (param + "$" + TokenAuth).ToUpper ())) {
                        Update ();
                        CPublic.WriteLog (TokenUser + ": " + DoingLog);
                        return 0;
                    } else if (!string.IsNullOrEmpty (oldTokenAuth) && oldTokenLifetime > DateTime.Now) {
                        if (sha.ToUpper ().Equals (SHA1 (param + "$" + oldTokenAuth).ToUpper ())) {
                            CPublic.WriteLog (TokenUser + ": " + DoingLog);
                            return -1;
                        }
                        return 2;
                    }
                    return 1;
                } else if (Replaceable ()) {
                    if (sha.ToUpper ().Equals (SHA1 (param + "$" + TokenAuth).ToUpper ())) {
                        CPublic.WriteLog (TokenUser + ": " + DoingLog);
                        return -1;
                    }
                    return 1;
                }
                return 2;
            }
            return 3;
        }

        public static string RandomGenerator (int intLength, bool Sign) {
            bool booNumber = true;
            bool booSign = Sign;
            bool booSmallword = true;
            bool booBigword = true;

            Random ranA = new Random ();
            int intResultRound = 0;
            int intA = 0;
            string strB = "";

            while (intResultRound < intLength) {
                //生成随机数A，表示生成类型  
                //1=数字，2=符号，3=小写字母，4=大写字母  
                intA = ranA.Next (1, 5);

                //如果随机数A=1，则运行生成数字  
                //生成随机数A，范围在0-10  
                //把随机数A，转成字符  
                //生成完，位数+1，字符串累加，结束本次循环  
                if (intA == 1 && booNumber) {
                    intA = ranA.Next (0, 10);
                    strB = intA.ToString () + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //如果随机数A=2，则运行生成符号  
                //生成随机数A，表示生成值域  
                //1：33-47值域，2：58-64值域，3：91-96值域，4：123-126值域  
                if (intA == 2 && booSign == true) {
                    intA = ranA.Next (1, 5);

                    //如果A=1  
                    //生成随机数A，33-47的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  
                    if (intA == 1) {
                        intA = ranA.Next (33, 48);
                        strB = ((char)intA).ToString () + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //如果A=2  
                    //生成随机数A，58-64的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  
                    if (intA == 2) {
                        intA = ranA.Next (58, 65);
                        strB = ((char)intA).ToString () + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //如果A=3  
                    //生成随机数A，91-96的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  
                    if (intA == 3) {
                        intA = ranA.Next (91, 97);
                        strB = ((char)intA).ToString () + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //如果A=4  
                    //生成随机数A，123-126的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  
                    if (intA == 4) {
                        intA = ranA.Next (123, 127);
                        strB = ((char)intA).ToString () + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                }

                //如果随机数A=3，则运行生成小写字母  
                //生成随机数A，范围在97-122  
                //把随机数A，转成字符  
                //生成完，位数+1，字符串累加，结束本次循环  
                if (intA == 3 && booSmallword == true) {
                    intA = ranA.Next (97, 123);
                    strB = ((char)intA).ToString () + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //如果随机数A=4，则运行生成大写字母  
                //生成随机数A，范围在65-90  
                //把随机数A，转成字符  
                //生成完，位数+1，字符串累加，结束本次循环  
                if (intA == 4 && booBigword == true) {
                    intA = ranA.Next (65, 89);
                    strB = ((char)intA).ToString () + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }
            }
            return strB;
        }
        public static String SHA1 (String content) {
            Encoding encode = Encoding.UTF8;
            try {
                SHA1 sha1 = new SHA1CryptoServiceProvider ();//创建SHA1对象
                byte[] bytes_in = encode.GetBytes (content);//将待加密字符串转为byte类型
                byte[] bytes_out = sha1.ComputeHash (bytes_in);//Hash运算
                sha1.Dispose ();//释放当前实例使用的所有资源
                String result = BitConverter.ToString (bytes_out);//将运算结果转为string类型
                result = result.Replace ("-", "").ToUpper ();
                return result;
            } catch (Exception ex) {

            }
            return "";
        }
    }
    */
    public static class CPublic {
        public static string LocalNode = "";
        public static string AppPath = "";
        public static string LogPath = @".\";
        public static string ConnString = "";//@"Data Source=127.0.0.1;Initial Catalog=ConCard;User ID=sa;Password=2008sa";
        //public static List<string> myFunc = new List<string> ();
        //public static int TokenCount = 0;
        //public static bool isLoadUser = false;
        //public static List<Token> myTokenList = new List<Token> ();
        //public static Dictionary<string, string> userList = new Dictionary<string, string> ();
        //public static Dictionary<int, QRToken> QRTokenList = new Dictionary<int, QRToken> ();
        //public static SqlConnection conn = new SqlConnection();

        //@"Data Source=" + IP + ";Initial Catalog=SMS;User ID=" + UserName + ";Password=" + Password;
        public static int Init () {
            try {
                if (!Directory.Exists (CPublic.LogPath))
                    Directory.CreateDirectory (CPublic.LogPath);

                if (File.Exists (AppPath + "DBConfig.cfg")) {
                    using (FileStream fs = new FileStream (AppPath + "DBConfig.cfg", FileMode.Open, FileAccess.Read, FileShare.None)) {
                        byte[] buf = new byte[2048];
                        int nReadNum = fs.Read (buf, 0, 2048);
                        if (nReadNum != 2048) {
                            CPublic.WriteLog ("读取数据库连年配置文件失败,长度不足.");
                        } else {
                            EncryptHelper eh = new EncryptHelper ();

                            int len = BitConverter.ToInt32 (buf, 0);
                            ConnString = eh.DesDecrypt (Encoding.GetEncoding ("gb2312").GetString (buf, 4, len), "synjones");

                            len = BitConverter.ToInt32 (buf, 1024);
                            string ConOperKey = eh.DesDecrypt (Encoding.GetEncoding ("gb2312").GetString (buf, 1028, len), "synjones");
                        }
                    }
                } else {
                    CPublic.WriteLog ("数据库连接配置文件不存在,请使用工具生,并放在目录:" + AppPath);
                }

                return 0;
            } catch (Exception e) {
                CPublic.WriteLog ("初始化异常:" + e.Message);
                return -1;
            }
        }
        /*
        public static byte[] MakeQRToken (byte type, int AccountNo) {
            if (QRTokenList.ContainsKey (AccountNo)) {
                QRTokenList.Remove (AccountNo);
            }
            QRToken myQRToken = new QRToken (type);
            QRTokenList.Add (AccountNo, myQRToken);
            return myQRToken.GetToken ();
        }
        public static byte UseQRToken (int AccountNo, byte[] myTokenValue) {
            QRToken myQRToken = null;
            if (QRTokenList.TryGetValue (AccountNo, out myQRToken)) {
                byte myQRTokenType = myQRToken.CheckToken (myTokenValue);
                if (myQRToken.TokenLifetime > DateTime.Now && myQRTokenType > 0) {
                    return myQRTokenType;
                }
                QRTokenList.Remove (AccountNo);
            }
            return 0;
        }
        public static bool AuthUpdate () {
            string TU0Username = "0";
            string TU0Password = "00000000";
            string TU1Username = "1";
            string TU1Password = "11111111";
            if (userList.Count == 0) {
                userList.Add (TU0Username, TU0Password);
                userList.Add (TU1Username, TU1Password);
            }
            ExeConfigurationFileMap map = new ExeConfigurationFileMap ();
            map.ExeConfigFilename = CPublic.AppPath + "setting.xml";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration (map, ConfigurationUserLevel.None);
            myFunc.Clear ();
            if (config.Sections["appSettings"] != null) {
                KeyValueConfigurationCollection kv = config.AppSettings.Settings;
                foreach (KeyValueConfigurationElement el in kv) {
                    if (el.Key.Contains ("TPE_") && "FALSE".Equals (el.Value.ToUpper ())) {
                        myFunc.Add (el.Key);
                    } else if (!isLoadUser) {
                        switch (el.Key) {
                            case "TokenUsername0":
                                TU0Username = string.IsNullOrEmpty (el.Value) ? TU0Username : el.Value;
                                break;
                            case "TokenPassword0":
                                TU0Password = string.IsNullOrEmpty (el.Value) ? TU0Password : el.Value;
                                break;
                            case "TokenUsername1":
                                TU1Username = string.IsNullOrEmpty (el.Value) ? TU1Username : el.Value;
                                break;
                            case "TokenPassword1":
                                TU1Password = string.IsNullOrEmpty (el.Value) ? TU1Password : el.Value;
                                break;
                        }
                    }
                }
                if (!isLoadUser) {
                    isLoadUser = true;
                    userList.Clear ();
                    userList.Add (TU0Username, TU0Password);
                    userList.Add (TU1Username, TU1Password);
                }
                return true;
            }
            return false;
        }
        public static string MakeToken (string User) {
            Token oldToken = null;
            int Index = TokenCount + 1;
            int isExist = myTokenList.FindIndex (item => item.User.Equals (User));
            if (isExist >= 0) {
                oldToken = new Token ();
                oldToken.Init (myTokenList[isExist]);
                myTokenList.RemoveAt (isExist);
            }
            Token myToken = new Token ();
            string Token = myToken.Init (Index, User, 30, oldToken);
            myTokenList.Add (myToken);
            return Token;
        }
        public static bool ValidUser (string username, string password) {
            string getPassword = "";
            userList.TryGetValue (username, out getPassword);
            if (!string.IsNullOrEmpty (getPassword)) {
                if (getPassword.Equals (password)) {
                    return true;
                }
            }
            return false;
        }
        public static int ValidToken (string User, string param, string sha, string DoingLog) {
            // 0: 验证正确
            // 1: 验证失败
            // 2: 令牌失效
            // 3: 信息错误
            // 4: 令牌无效
            int isExist = myTokenList.FindIndex (item => item.User.Equals (User));
            if (isExist >= 0) {
                Token myToken = myTokenList[isExist];
                return myToken.TokenVaildCheck (User, param, sha, DoingLog);
            }
            return 4;
        }
        */
        public static int WriteLog (string Msg) {
            try {
                string CurLog = LogPath + DateTime.Today.ToShortDateString () + ".txt";
                FileStream fs = new FileStream (CurLog, FileMode.Append, FileAccess.Write, FileShare.None);
                StreamWriter sw = new StreamWriter (fs);
                sw.WriteLine (DateTime.Now.ToLongTimeString () + " ： " + Msg);
                sw.Close ();
            } catch {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// 把从第三方接口中提取的日期,转成标准的日期格式
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string ConvertDateTime (byte[] birthday) {
            string YY = string.Empty;
            string MM = string.Empty;
            string DD = string.Empty;
            string hh = string.Empty;
            string mm = string.Empty;
            string ss = string.Empty;
            string dt = string.Empty;

            if (birthday[0] == '\0') {
                dt = string.Empty;
                YY = "1900";
                MM = "00";
                DD = "00";
                hh = "00";
                mm = "00";
                ss = "00";
            } else {
                dt = System.Text.Encoding.Default.GetString (birthday);
                YY = dt.Substring (0, 4);
                MM = dt.Substring (4, 2);
                DD = dt.Substring (6, 2);
                hh = dt.Substring (8, 2);
                mm = dt.Substring (10, 2);
                ss = dt.Substring (12, 2);

            }
            dt = YY + "-" + MM + "-" + DD + " " + hh + ":" + mm + ":" + ss;
            return dt;
        }
        public static string ByteArrayToStr (byte[] ByteArray) {
            if (ByteArray == null)
                return "";

            int len = 0;
            foreach (byte b in ByteArray) {
                if (b == 0)
                    break;

                len++;
            }

            return Encoding.GetEncoding ("gb2312").GetString (ByteArray, 0, len);
        }
        public static string ByteArrayToStr (byte[] ByteArray, int offset, int length) {
            if (ByteArray == null)
                return "";

            if (length > ByteArray.Length - offset)
                return "";

            byte[] buf = new byte[length];
            Array.Copy (ByteArray, offset, buf, 0, length);

            int len = 0;
            foreach (byte b in buf) {
                if (b == 0)
                    break;

                len++;
            }

            return Encoding.GetEncoding ("gb2312").GetString (buf, 0, len);
        }
    }
}

