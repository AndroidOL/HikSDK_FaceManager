using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Controls.Ribbon;
using System.Runtime.InteropServices.WindowsRuntime;
using WebServicePark;
using System.IO;
using System.Windows.Threading;
using System.Collections;
using Newtonsoft.Json;
using System.Drawing;
using System.Threading;

namespace HK_Face {
    public class Person : IComparable<Person> {
        private uint acc;
        public uint AccountNo { get { return acc; } }
        private string name;
        public string Name { get { return name; } }
        private uint card;
        public uint CardNo { get { return card; } }
        private string cert;
        private uint oldCard;
        public uint oldCardNo { get { return oldCard; } set { oldCard = value; } }

        public Person (TPE_GetAccountRes Res) {
            acc = (uint)Res.AccountNo;
            name = Res.Name + Res.PersonID.Substring (14, 4);
            card = (uint)Res.CardNo;
            cert = Res.CertCode;
        }
        public Person(CHCNetSDK.NET_DVR_CARD_CFG_V50 Res) {
            acc = Res.dwEmployeeNo;
            name = Encoding.GetEncoding ("gb2312").GetString (Res.byName).TrimEnd ('\0');
            if (!uint.TryParse (Encoding.ASCII.GetString (Res.byCardNo).TrimEnd ('\0'), out card)) {
                card = 0;
            }
            cert = acc.ToString ();
        }
        public int CompareTo(Person person) {
            if (person == null) {
                return 1;
            } else if (acc == person.acc) {
                return 0;
            } else if (acc > person.acc) {
                return 1;
            } else { return -1; }
            //return acc.CompareTo (person.acc);
        }
        //public int Compare(Person A, Person B) {
        //    return A.acc.CompareTo (B.acc);
        //}
        //protected YKT_Person (string AccountNo, string Name, uint CardNo) {
        //    int accno = 0;
        //    if (string.IsNullOrEmpty(AccountNo) && int.TryParse(AccountNo, out accno)) {
        //        acc = accno > 100000 ? "0000000" : accno.ToString ().PadLeft(7, '0');
        //    } else { acc = "0000000"; }

        //    if (string.IsNullOrEmpty (Name)) {
        //        name = Name;
        //    } else { name = "无名氏"; }

        //    if (CardNo > 0) {
        //        card = CardNo;
        //    } else { card = 0; }
        //}
        public static bool operator < (Person A, Person B) {
            return A.acc < B.acc;
        }
        public static bool operator > (Person A, Person B) {
            return A.acc > B.acc;
        }
        public static bool operator == (Person A, Person B) {
            return A.acc == B.acc;
        }
        public static bool operator != (Person A, Person B) {
            return A.acc != B.acc;
        }
        public override bool Equals (object obj) {
            if (obj == null) { return false; }
            if (obj.GetType().Equals(GetType()) == false) { return false; }
            Person tmp = obj as Person;
            return acc.Equals (tmp.acc) && name.ToUpper ().Equals (tmp.name.ToUpper ()) && card.Equals (tmp.card);
        }
        public override int GetHashCode () {
            return acc.GetHashCode () + name.ToUpper ().GetHashCode () + card.GetHashCode ();
        }
        public override string ToString () {
            return "[" + acc + "]" + name + ": " + card;
        }
    }
    class cListItem {
        private string name;
        public string Name {
            get { return name; }
            set { name = value; }
        }
        private int id;
        public int ID {
            get { return id; }
            set { id = value; }
        }
        public cListItem (int id, string name) {
            this.id = id;
            this.name = name;
        }
        public override string ToString () {
            return this.name;
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool isInitSDK = false;
        private int UserID = -1;
        bool getSlower = false;
        bool getFaster = true;
        int m_GetCardCfgHandle = -1;
        int m_SetCardCfgHandle = -1;
        int m_GetFaceCfgHandle = -1;
        int m_SetFaceCfgHandle = -1;
        CHCNetSDK.RemoteConfigCallback m_GetGatewayCardCallback;
        CHCNetSDK.RemoteConfigCallback m_SetGatewayCardCallback;
        CHCNetSDK.RemoteConfigCallback m_GetFaceGatewayCardCallback;
        CHCNetSDK.RemoteConfigCallback m_SetFaceGatewayCardCallback;
        DispatcherTimer Statup = new DispatcherTimer ();

        // 存储一卡通用户数据
        private static List<Person> YKTPerson = new List<Person> ();
        // 存储人脸机用户数据
        private static List<Person> HikPerson = new List<Person> ();
        // 待处理数据
        private static List<Person> addPerson = new List<Person> ();
        private static List<Person> updPerson = new List<Person> ();
        private static List<Person> delPerson = new List<Person> ();

        private void custom_InitComboBoxItems () {
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_SOFTHARDWARE_ABILITY, "设备软硬件能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_NETWORK_ABILITY, "设备无线网络能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_ENCODE_ALL_ABILITY_V20, "设备所有编码能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.IPC_FRONT_PARAMETER_V20, "设备前端参数"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_RAID_ABILITY, "设备RAID能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_ALARM_ABILITY, "获取设备报警能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_DYNCHAN_ABILITY, "获取设备数字通道能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_USER_ABILITY, "获取设备用户管理参数能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_NETAPP_ABILITY, "获取设备网络应用参数能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_VIDEOPIC_ABILITY, "获取设备图像参数能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_JPEG_CAP_ABILITY, "获取设备JPEG抓图能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_SERIAL_ABILITY, "获取设备RS232和RS485串口能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DEVICE_ABILITY_INFO, "设备通用能力类型，具体能力根据发送的能力节点来区分"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.STREAM_ABILITY, "获取设备流能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.MATRIXDECODER_ABILITY, "获取多路解码器显示、解码能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.DECODER_ABILITY, "获取解码器XML能力集"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.SNAPCAMERA_ABILITY, "获取智能交通摄像机的能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.PIC_CAPTURE_ABILITY, "获取图片能力"));
            HikAbilityList.Items.Add (new cListItem (CHCNetSDK.ACS_ABILITY, "门禁能力集"));
        }

        private void custom_SynjonesStatup (object sender, EventArgs e) {
            string[] TPE_NetStatus = {
                "尚未连接",
                "与中心同步中",
                "连接已经建立"
            };
            int nRet = TPE_Class.TPE_GetNetState ();
            SynjonesStatup.Content = "TPE 状态：" + TPE_NetStatus[nRet - 1] + "\n";
            if (nRet == 3 && !getSlower && Statup.Interval != null) {
                getSlower = true;
                getFaster = false;
                Statup.Interval = new TimeSpan (0, 0, 0, 10, 0);
            } else if (nRet != 3 && !getFaster && Statup.Interval != null) {
                getSlower = false;
                getFaster = true;
                Statup.Interval = new TimeSpan (0, 0, 0, 0, 100);
            }
        }

        private void custom_SynjonesServiceStart () {
            CPublic.AppPath = @".\";
            CPublic.LogPath = CPublic.AppPath + "Log\\";

            // 获取 TPE 网络状态
            int nRet = TPE_Class.TPE_GetNetState ();
            if (nRet != 3) {
                // 启动 TPE 服务
                TPE_Class.TPE_StartTPE ();
                SimpleLogInfo.Text += "TPE 正在启动！\n";
            } else { SimpleLogInfo.Text += "TPE 已成功启动！\n"; }
        }
        public MainWindow () {
            InitializeComponent ();
            custom_InitComboBoxItems ();
            Statup.Tick += new EventHandler (custom_SynjonesStatup);
            Statup.Interval = new TimeSpan (0, 0, 0, 0, 100);
            Statup.Start ();
        }

        ~MainWindow () {
            // 确认用户已经登出
            if (UserID > -1) {
                CHCNetSDK.NET_DVR_Logout (UserID);
            }
            // 清理 SDK  资源
            CHCNetSDK.NET_DVR_Cleanup ();
        }

        private void SynjonesStart_Click (object sender, RoutedEventArgs e) {
            custom_SynjonesServiceStart ();
        }

        private void SynjonesExport_Click (object sender, RoutedEventArgs e) {
            int nRet = TPE_Class.TPE_GetNetState ();
            if (nRet == 3) {
                tagTPE_QueryStdAccountReq Req = new tagTPE_QueryStdAccountReq ();
                tagTPE_QueryResControl ResControl = new tagTPE_QueryResControl ();
                Req.reqflagAccountNoRange = 1;
                Req.AccountNoRange = new int[] { Convert.ToInt32 (100000), Convert.ToInt32 (1599999) };
                Req.resflagName = 1;
                Req.resflagCardNo = 1;
                Req.resflagDepart = 1;
                Req.resflagCertCode = 1;
                Req.resflagPersonID = 1;
                nRet = TPE_Class.TPE_QueryStdAccount (1, ref Req, out ResControl, 1);
                if (nRet == 0 && ResControl.ResRecCount != 0) {
                    // 输出数据
                    using (StreamWriter AccountImport = new StreamWriter (@".\Account.csv", false, Encoding.GetEncoding ("GB2312"))) {
                        // 写入标题
                        if (HikVersion.SelectedIndex != 1) {
                            AccountImport.WriteLine ("规则：, \n, 1.带 * 的为必填项。,\n, 2.性别 1:男 2:女,\n, 3.证件类型 1:身份证 2:学生证 3:军官证 4:港澳通行证 5:驾驶证 6:护照 7:其他证件,\n, 4.学历 1:初中 2:高中 / 专科 3:本科 4:硕士 5:博士,\n, 5.设备操作权限 1:普通用户 2:管理员,\n, 6.日期格式:年 / 月 / 日,\n, 7.请使用‘；’分隔卡号,\n, 8.f1~f10依次表示从左手小拇指到右手小拇指指纹数据。,\n, 9.f1card~f10card依次表示从左手小拇指到右手小拇指指纹数据关联的卡号。");
                            AccountImport.WriteLine ("*人员编号,*组织,*人员姓名,*性别,证件类型,证件号码,出生日期,联系电话,职务,住址,电子邮件,国家,城市,学历,设备操作权限,雇佣开始日期,雇佣结束日期,卡号,f1,f2,f3,f4,f5,f6,f7,f8,f9,f10,f1card,f2card,f3card,f4card,f5card,f6card,f7card,f8card,f9card,f10card");
                        } else { AccountImport.WriteLine ("*人员编号,*组织名称,*人员名称,*性别,联系方式,邮箱,生效时间,失效时间,卡号,房间号,身份证"); }
                        SimpleLogInfo.Text += "标准批量调账成功，总计返回结果" + ResControl.ResRecCount + "条！";
                        tagTPE_GetAccountRes AccRes = new tagTPE_GetAccountRes ();
                        TPE_GetAccountRes tpe_GetAccRes;
                        IntPtr buffer;
                        unsafe {
                            for (int i = 0; i < ResControl.ResRecCount; i++) {
                                buffer = (IntPtr)((Byte*)(ResControl.pRes) + i * Marshal.SizeOf (AccRes));
                                AccRes = (tagTPE_GetAccountRes)Marshal.PtrToStructure (buffer, typeof (tagTPE_GetAccountRes));
                                tpe_GetAccRes = new TPE_GetAccountRes (AccRes);
                                // 0 - 帐号
                                // 1 - 姓名
                                // 2 - 性别：身份证17位 % 2 == 0
                                byte isMale;
                                // 4 - 生日：身份证 5-9 10-11 12-13
                                string birthday = "1970-01-01";
                                if (string.IsNullOrEmpty (tpe_GetAccRes.PersonID) || tpe_GetAccRes.PersonID.Length != 18) {
                                    isMale = 1;
                                    tpe_GetAccRes.PersonID = "000000000000000000";
                                } else {
                                    birthday = tpe_GetAccRes.PersonID.Substring (6, 4) + "/" + tpe_GetAccRes.PersonID.Substring (10, 2) + "/" + tpe_GetAccRes.PersonID.Substring (12, 2);
                                    isMale = (byte)((int.Parse (tpe_GetAccRes.PersonID[16].ToString ()) % 2 == 0) ? 0x02 : 0x01);
                                }
                                // 3 - 身份证或工号
                                // 5 - string.IsNullOrEmpty(Tel) ? 工号 : Tel
                                // 6 - 电子邮件
                                // 7 - 卡号
                                if (HikVersion.SelectedIndex != 1) {
                                    AccountImport.WriteLine (string.Format ("{0},证件卡,{1},{2},1,{3},{4},{5},,,{6},中国,,,1,,,{7},,,,,,,,,,,,,,,,,,,,", tpe_GetAccRes.AccountNo, tpe_GetAccRes.Name + tpe_GetAccRes.PersonID.Substring (14, 4),
                                                                                                                                                          isMale, tpe_GetAccRes.PersonID, birthday,
                                                                                                                                                          string.IsNullOrEmpty (tpe_GetAccRes.Tel) ? tpe_GetAccRes.CertCode : tpe_GetAccRes.Tel,
                                                                                                                                                          tpe_GetAccRes.Email, tpe_GetAccRes.CardNo));
                                } else {
                                    AccountImport.WriteLine (string.Format ("{0},证件卡,{1},{2},{3},{4},2020/01/01,2099/12/31,{5},,{6}", tpe_GetAccRes.AccountNo, tpe_GetAccRes.Name, isMale,
                                                                                                                                                       string.IsNullOrEmpty (tpe_GetAccRes.Tel) ? tpe_GetAccRes.CertCode : tpe_GetAccRes.Tel,
                                                                                                                                                       tpe_GetAccRes.Email, tpe_GetAccRes.CardNo, tpe_GetAccRes.PersonID));
                                }
                            }
                            ResControl.pRes = null;
                        }
                    }
                } else { SimpleLogInfo.Text += "标准批量调账失败或没有返回结果！"; }
            } else { SimpleLogInfo.Text += "TPE 尚未启动！\n"; }
        }

        private void SynjonesLoad_Click (object sender, RoutedEventArgs e) {
            int nRet = TPE_Class.TPE_GetNetState ();
            if (nRet == 3) {
                YKTPerson.Clear ();
                tagTPE_QueryStdAccountReq Req = new tagTPE_QueryStdAccountReq ();
                tagTPE_QueryResControl ResControl = new tagTPE_QueryResControl ();
                Req.reqflagAccountNoRange = 1;
                Req.AccountNoRange = new int[] { Convert.ToInt32 (100000), Convert.ToInt32 (1599999) };
                Req.resflagName = 1;
                Req.resflagCardNo = 1;
                Req.resflagDepart = 1;
                Req.resflagCertCode = 1;
                Req.resflagPersonID = 1;
                nRet = TPE_Class.TPE_QueryStdAccount (1, ref Req, out ResControl, 1);
                if (nRet == 0 && ResControl.ResRecCount != 0) {
                    // 输出数据
                    tagTPE_GetAccountRes AccRes = new tagTPE_GetAccountRes ();
                    IntPtr buffer;
                    unsafe {
                        for (int i = 0; i < ResControl.ResRecCount; i++) {
                            buffer = (IntPtr)((Byte*)(ResControl.pRes) + i * Marshal.SizeOf (AccRes));
                            AccRes = (tagTPE_GetAccountRes)Marshal.PtrToStructure (buffer, typeof (tagTPE_GetAccountRes));
                            string PersonID = Encoding.ASCII.GetString (AccRes.PersonID).TrimEnd ('\0');
                            // 设置过滤条件
                            if (PersonID.Length == 18 && (AccRes.Depart >> 56 == 0x02)) {
                                YKTPerson.Add (new Person (new TPE_GetAccountRes (AccRes)));
                            }
                        }
                        ResControl.pRes = null;
                    }
                    // YKTPerson.Sort ();
                    YKTPerson.Sort ((A, B) => A.AccountNo.CompareTo (B.AccountNo));
                    SimpleLogInfo.Text += "标准批量调账成功，总计返回结果" + YKTPerson.Count + "条！\n";
                } else { SimpleLogInfo.Text += "标准批量调账失败或没有返回结果！\n"; }
            } else { SimpleLogInfo.Text += "TPE 尚未启动！\n"; }
        }

        private void SynjonesUnload_Click (object sender, RoutedEventArgs e) {
            SimpleLogInfo.Text += "当前已经获取" + YKTPerson.Count + "人数据，资源已释放！\n";
            YKTPerson.Clear ();
        }

        public static void custom_GetErrorMessage (int iErr) {
            // 获取错误信息
            IntPtr psErr = CHCNetSDK.NET_DVR_GetErrorMsg (ref iErr);
            string sErr = Marshal.PtrToStringAnsi (psErr);
            throw new Exception (string.Format ("返回错误代码：{0}，错误代码含义：{1}\n", iErr.ToString (), sErr));
        }

        private void HikInit_Click (object sender, RoutedEventArgs e) {
            if (!isInitSDK) {
                // 初始化 SDK
                if (CHCNetSDK.NET_DVR_Init ()) {
                    isInitSDK = true;
                    SimpleLogInfo.Text += "SDK 初始化已完成\n";
                } else {
                    try {
                        // 通过错误代码获取消息内容
                        custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                    } catch (Exception ex) {
                        SimpleLogInfo.Text += "SDK 初始化调用异常：" + ex.Message + "\n";
                    }
                }
            } else { SimpleLogInfo.Text += "SDK 初始化仅允许一次\n"; }
        }

        private void HikSDKLogCfg_Click (object sender, RoutedEventArgs e) {
            // 日志等级
            // 0 - 关闭
            // 1 - ERROR
            // 2 - ERROR + DEBUG
            // 3 - ERROR + DEBUG + INFO
            int LogLevel = 3;
            string LogLocation = ".\\HiKSDK\\LOG\\";
            bool LogAutoClean = false;
            if (CHCNetSDK.NET_DVR_SetLogToFile (LogLevel, LogLocation, LogAutoClean)) {
                SimpleLogInfo.Text += "成功设置日志文件路径为 " + LogLocation + "。\n";
            } else {
                try {
                    // 通过错误代码获取消息内容
                    custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                } catch (Exception ex) {
                    SimpleLogInfo.Text += "日志设置调用异常：" + ex.Message + "\n";
                }
            }
        }

        private void HikSDKCfg_Click (object sender, RoutedEventArgs e) {
            // 声明字段
            byte[] strIP = new byte[16 * 16];   // 存放IP的缓冲区，不能为空 
            uint dwValidNum = 0;                // 所有有效 IP 的数量
            bool bEnableBind = false;
            // 获取配置 IP 信息
            if (CHCNetSDK.NET_DVR_GetLocalIP (strIP, ref dwValidNum, ref bEnableBind)) {
                if (dwValidNum > 0) {
                    SimpleLogInfo.Text += "总计获取地址" + dwValidNum.ToString () + "条。\n";
                    // 总计获取 16 条地址数据
                    for (int i = 0; i < 15 - 1; i++) {
                        string currIP = Encoding.UTF8.GetString (strIP, i * 16, 16).Replace ("\0", "");
                        if (!string.IsNullOrEmpty (currIP)) {
                            SimpleLogInfo.Text += currIP.Length + ": " + currIP + "\n";
                        }
                    }
                }
            }
        }
        private string custom_GetHikSDKVersion () {
            uint HikSDKVersion = CHCNetSDK.NET_DVR_GetSDKVersion ();
            uint HHikSDKVersion = HikSDKVersion >> 16;
            uint LHikSDKVersion = HikSDKVersion << 16 >> 16;
            if (HHikSDKVersion == 0 && LHikSDKVersion == 0) {
                return "获取失败";
            }
            return HHikSDKVersion.ToString () + "." + LHikSDKVersion.ToString ();
        }
        private string custom_GetHikSDKBuildVersion () {
            uint HikSDKBuildVersion = CHCNetSDK.NET_DVR_GetSDKBuildVersion ();
            uint HHikSDKVersion = HikSDKBuildVersion >> 24;
            uint LHikSDKVersion = HikSDKBuildVersion << 8 >> 24;
            uint HikSDKBuild = HikSDKBuildVersion << 16 >> 16;
            string HikVersion;
            if (HHikSDKVersion != 0 && LHikSDKVersion != 0 && HikSDKBuild != 0) {
                HikVersion = HHikSDKVersion.ToString () + "." + LHikSDKVersion.ToString () + " b" + HikSDKBuild.ToString ();
            } else if (HHikSDKVersion == 0 && LHikSDKVersion == 0 && HikSDKBuild != 0) {
                HikVersion = "获取失败 b" + HikSDKBuild.ToString ();
            } else if (HHikSDKVersion != 0 && LHikSDKVersion != 0 && HikSDKBuild == 0) {
                HikVersion = HHikSDKVersion.ToString () + "." + LHikSDKVersion.ToString () + " b获取失败";
            } else { HikVersion = "获取失败"; }
            return HikVersion;
        }

        private void HikSDKInfo_Click (object sender, RoutedEventArgs e) {
            CHCNetSDK.NET_DVR_SDKSTATE SDKState = new CHCNetSDK.NET_DVR_SDKSTATE ();
            bool HikSDKState = CHCNetSDK.NET_DVR_GetSDKState (ref SDKState);

            CHCNetSDK.NET_DVR_SDKABL SDKAbility = new CHCNetSDK.NET_DVR_SDKABL ();
            bool HikSDKAbility = CHCNetSDK.NET_DVR_GetSDKAbility (ref SDKAbility);

            SimpleLogInfo.Text += "SDK 版本：" + custom_GetHikSDKVersion () + "\n";
            SimpleLogInfo.Text += "SDK Build 版本：" + custom_GetHikSDKBuildVersion () + "\n";
            SimpleLogInfo.Text += "SDK 状态：获取" + (HikSDKState ? "成功" : "失败") + "\n";
            SimpleLogInfo.Text += "SDK 能力：获取" + (HikSDKAbility ? "成功" : "失败") + "\n";
        }

        private void HikGetDevice_Click (object sender, RoutedEventArgs e) {
            cListItem curSelected = (cListItem)HikAbilityList.SelectedItem;
            if (curSelected == null) {
                SimpleLogInfo.Text = "未选择\n";
            } else { SimpleLogInfo.Text = curSelected.ID + "\n"; }
            // SimpleLogInfo.Text = "暂未添加该功能\n";
        }

        private void HikLogin_Click (object sender, RoutedEventArgs e) {
            if (UserID == -1) {
                CHCNetSDK.NET_DVR_USER_LOGIN_INFO pLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO {
                    sDeviceAddress = new byte[CHCNetSDK.NET_DVR_DEV_ADDRESS_MAX_LEN],
                    byUseTransport = 0,
                    // 设置登录设备端口
                    wPort = ushort.Parse (HikLogin_Port.Text),
                    sUserName = new byte[CHCNetSDK.NET_DVR_LOGIN_USERNAME_MAX_LEN],
                    sPassword = new byte[CHCNetSDK.NET_DVR_LOGIN_PASSWD_MAX_LEN],
                    // 是否异步登录：0- 否，1- 是 
                    bUseAsynLogin = false,
                    // 0:不使用代理，1：使用标准代理，2：使用EHome代理
                    byProxyType = 0,
                    // 是否使用UTC时间：0- 不进行转换，默认；1- 输入输出UTC时间，SDK进行与设备时区的转换；2- 输入输出平台本地时间，SDK进行与设备时区的转换 
                    byUseUTCTime = 1,
                    // 登录模式：0- SDK私有协议，1- ISAPI协议，2- 自适应（设备支持协议类型未知时使用，一般不建议） 
                    byLoginMode = 0,
                    // 0-不适用tls，1-使用tls 2-自适应
                    byHttps = 0,
                    // 认证方式，0-不认证，1-双向认证，2-单向认证；认证仅在使用TLS的时候生效;
                    byVerifyMode = 0
                };
                // 设置登录设备的 IP 地址
                byte[] tDeviceAddress = Encoding.ASCII.GetBytes (HikLogin_Address.Text);
                Array.Copy (tDeviceAddress, 0, pLoginInfo.sDeviceAddress, 0, tDeviceAddress.Length);
                // 设置登录设备的用户名与密码
                byte[] tUserName = Encoding.ASCII.GetBytes (HikLogin_Username.Text);
                Array.Copy (tUserName, 0, pLoginInfo.sUserName, 0, tUserName.Length);
                string sPassword = HikLogin_Password.Password.Length == 0 ? "wellin5401" : HikLogin_Password.Password;
                byte[] tPassword = Encoding.ASCII.GetBytes (sPassword);
                Array.Copy (tPassword, 0, pLoginInfo.sPassword, 0, tPassword.Length);
                CHCNetSDK.NET_DVR_DEVICEINFO_V40 lpDeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V40 ();
                lpDeviceInfo.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

                if (isInitSDK) {
                    UserID = CHCNetSDK.NET_DVR_Login_V40 (ref pLoginInfo, ref lpDeviceInfo);
                    if (UserID != -1) {
                        SimpleLogInfo.Text += "用户序号" + UserID.ToString () + "登陆成功！\n";
                    } else {
                        try {
                            // 通过错误代码获取消息内容
                            custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                        } catch (Exception ex) {
                            SimpleLogInfo.Text += "登陆调用异常：" + ex.Message + "\n";
                        }
                    }
                } else { SimpleLogInfo.Text += "请先初始化 SDK 后尝试！\n"; }
            } else { SimpleLogInfo.Text += "用户序号" + UserID.ToString () + "登陆成功！（请勿重复登陆）\n"; }
        }

        private void custom_GetDeviceAbility (int LocalUserID, uint dwAbilityType, int channelNo) {
            string channel = channelNo.ToString ();
            IntPtr pInBuf;
            int nSize;
            string dwAbilityName;

            string xmlInput = "";// = "<?xml version='1.0' encoding='utf-8'?>";
            switch (dwAbilityType) {
                case CHCNetSDK.DEVICE_SOFTHARDWARE_ABILITY:     // 设备软硬件能力
                    dwAbilityName = "设备软硬件能力";
                    break;
                case CHCNetSDK.DEVICE_NETWORK_ABILITY:          // 设备无线网络能力
                    dwAbilityName = "设备无线网络能力";
                    break;
                case CHCNetSDK.DEVICE_ENCODE_ALL_ABILITY_V20:   // 设备所有编码能力
                    dwAbilityName = "设备所有编码能力";
                    xmlInput += "<AudioVideoCompressInfo><AudioChannelNumber>" + channel + "</AudioChannelNumber><VoiceTalkChannelNumber>" + channel + "</VoiceTalkChannelNumber><VideoChannelNumber>" + channel + "</VideoChannelNumber></AudioVideoCompressInfo>";
                    break;
                case CHCNetSDK.IPC_FRONT_PARAMETER_V20:         // 设备前端参数
                    dwAbilityName = "设备前端参数";
                    xmlInput += "<CAMERAPARA><ChannelNumber>" + channel + "</ChannelNumber></CAMERAPARA>";
                    break;
                case CHCNetSDK.DEVICE_RAID_ABILITY:             // 设备RAID能力
                    dwAbilityName = "设备RAID能力";
                    break;
                case CHCNetSDK.DEVICE_ALARM_ABILITY:             // 获取设备报警能力
                    dwAbilityName = "获取设备报警能力";
                    xmlInput += "<AlarmAbility version=\"2.0\"><channelID>" + channel + "</channelID></AlarmAbility>";
                    break;
                case CHCNetSDK.DEVICE_DYNCHAN_ABILITY:          // 获取设备数字通道能力
                    dwAbilityName = "获取设备数字通道能力";
                    xmlInput += "<DynChannelAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></DynChannelAbility>";
                    break;
                case CHCNetSDK.DEVICE_USER_ABILITY:             // 获取设备用户管理参数能力
                    dwAbilityName = "获取设备用户管理参数能力";
                    xmlInput += "<UserAbility version=\"2.0\">\r\n</UserAbility>";
                    break;
                case CHCNetSDK.DEVICE_NETAPP_ABILITY:           // 获取设备网络应用参数能力
                    dwAbilityName = "获取设备网络应用参数能力";
                    xmlInput += "<NetAppAbility version=\"2.0\">\r\n</NetAppAbility>";
                    break;
                case CHCNetSDK.DEVICE_VIDEOPIC_ABILITY:         // 获取设备图像参数能力
                    dwAbilityName = "获取设备图像参数能力";
                    xmlInput += "<VideoPicAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></VideoPicAbility> ";
                    break;
                case CHCNetSDK.DEVICE_JPEG_CAP_ABILITY:         // 获取设备JPEG抓图能力
                    dwAbilityName = "获取设备JPEG抓图能力";
                    xmlInput += "<JpegCaptureAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></JpegCaptureAbility> ";
                    break;
                case CHCNetSDK.DEVICE_SERIAL_ABILITY:           // 获取设备RS232和RS485串口能力
                    dwAbilityName = "获取设备RS232和RS485串口能力";
                    xmlInput += "<SerialAbility version=\"2.0\"><subBoardNo>" + channel + "</subBoardNo></SerialAbility>";
                    break;
                case CHCNetSDK.DEVICE_ABILITY_INFO:             // 设备通用能力类型，具体能力根据发送的能力节点来区分
                    dwAbilityName = "设备通用能力类型";
                    List<string> Ability = new List<string> {
                        "<PTZAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></PTZAbility>",          //获取PTZ能力集
                        "<EventAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></EventAbility>",      //获取报警事件处理能力集
                        "<ROIAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></ROIAbility>",          //获取ROI能力集
                        "<RecordAbility version=\"2.0\">\r\n</RecordAbility>",                                          //获取录像相关能力集
                        "<GetAccessDeviceChannelAbility version=\"2.0\">\r\n</GetAccessDeviceChannelAbility>",          //NVR前端待接入设备通道能力集
                        "<PreviewSwitchAbility version=\"2.0\">\r\n</PreviewSwitchAbility>",                            //获取设备本地预览切换能力集
                        "<NPlusOneAbility version=\"2.0\">\r\n</NPlusOneAbility>",                                      //获取设备N+1能力集
                        "<HardDiskAbility version=\"2.0\">\r\n</HardDiskAbility>",                                      //获取设备磁盘相关能力集
                        "<IPAccessConfigFileAbility version=\"2.0\">\r\n</IPAccessConfigFileAbility>",                  //获取IPC配置文件导入导出能力集
                        "<ChannelInputAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></ChannelInputAbility>",                //获取设备通道输入能力集
                        "<CameraParaDynamicAbility version=\"2.0\"><channelNO>" + channel + "</channelNO><ExposureSetDynamicLinkTo><WDR><WDREnable>2</WDREnable></WDR><IrisMode><IrisType>0</IrisType></IrisMode></ExposureSetDynamicLinkTo><AudioVideoCompressInfoDynamicLinkTo></AudioVideoCompressInfoDynamicLinkTo><VbrAverageCapDynamicLinkTo><streamType></streamType><codeType></codeType><videoQualityControlType></videoQualityControlType><vbrUpperCap></vbrUpperCap></VbrAverageCapDynamicLinkTo></CameraParaDynamicAbility>",    //获取前端参数动态能力集
                        "<AlarmTriggerRecordAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></AlarmTriggerRecordAbility>",    //获取报警触发录像能力集
                        "<GBT28181AccessAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></GBT28181AccessAbility>",            //获取GB/T28181能力集
                        "<IOAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></IOAbility>",            //获取IO口输入输出能力集
                        "<AccessProtocolAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></AccessProtocolAbility>",    //获取协议接入能力集
                        "<SecurityAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></SecurityAbility>",                //获取安全认证配置能力集
                        "<CameraMountAbility  version=\"2.0\"><channelNO>" + channel + "</channelNO></CameraMountAbility>",         //获取摄像机架设参数能力集
                        "<SearchLogAbility version= '2.0'><channelNO>" + channel + "</channelNO></SearchLogAbility>",             //获取日志搜索能力集
                        "<CVRAbility version=\"2.0\">\r\n</CVRAbility>",                                                //获取CVR设备能力集
                        "<ImageDisplayParamAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></ImageDisplayParamAbility>",      //获取图像显示参数能力集
                        "<VcaDevAbility version=\"2.0\">\r\n</VcaDevAbility>",                                          //获取智能设备能力集
                        "<VcaCtrlAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></VcaCtrlAbility>",  //获取智能通道控制能力集
                        "<VcaChanAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></VcaChanAbility>",  //获取智能通道分析能力集
                        "<BinocularAbility version=\"2.0\"><channelNO>" + channel + "</channelNO></BinocularAbility>"             //获取双目能力集
                    };
                    Random RandomAbility = new Random ();
                    xmlInput += Ability[RandomAbility.Next (Ability.Count)];
                    break;
                case CHCNetSDK.STREAM_ABILITY:                  // 获取设备流能力
                    dwAbilityName = "获取设备流能力";
                    xmlInput += "<StreamAbility version=\"2.0\">\r\n</StreamAbility> ";
                    break;
                case CHCNetSDK.MATRIXDECODER_ABILITY:           // 获取多路解码器显示、解码能力
                    dwAbilityName = "获取多路解码器显示、解码能力";
                    break;
                case CHCNetSDK.DECODER_ABILITY:                 // 获取解码器XML能力集
                    dwAbilityName = "获取解码器XML能力集";
                    xmlInput += "<DecoderAbility version=\"2.0\">\r\n</DecoderAbility>";
                    break;
                case CHCNetSDK.SNAPCAMERA_ABILITY:              // 获取智能交通摄像机的能力
                    dwAbilityName = "获取智能交通摄像机的能力";
                    break;
                case CHCNetSDK.PIC_CAPTURE_ABILITY:             // 获取图片能力
                    dwAbilityName = "获取图片能力";
                    xmlInput = channel;
                    break;
                case CHCNetSDK.ACS_ABILITY:                     // 门禁能力集
                    dwAbilityName = "门禁能力集";
                    xmlInput += "<AcsAbility version=\"2.0\">\r\n</AcsAbility>";
                    break;
                default:
                    dwAbilityName = "未知类型";
                    break;
            }
            if (string.IsNullOrEmpty (xmlInput)) { // .Replace ("<?xml version='1.0' encoding='utf-8'?>", "")
                pInBuf = IntPtr.Zero;
                nSize = 0;
            } else {
                nSize = xmlInput.Length;
                pInBuf = Marshal.AllocHGlobal (nSize);
                pInBuf = Marshal.StringToHGlobalAnsi (xmlInput);
            }

            int XML_ABILITY_OUT_LEN = 3 * 1024 * 1024;
            IntPtr pOutBuf = Marshal.AllocHGlobal (XML_ABILITY_OUT_LEN);

            string AbilityReturn;
            if (CHCNetSDK.NET_DVR_GetDeviceAbility (LocalUserID, dwAbilityType, pInBuf, (uint)nSize, pOutBuf, (uint)XML_ABILITY_OUT_LEN)) {
                string strOutBuf = Marshal.PtrToStringAnsi (pOutBuf, XML_ABILITY_OUT_LEN).TrimEnd (' ').TrimEnd ('\0');
                // strOutBuf = strOutBuf.Replace (">\n<", ">\r\n<");
                AbilityReturn = "[" + dwAbilityName + "] 能力输出正常：\n" + strOutBuf + "\n";
            } else {
                AbilityReturn = "[" + dwAbilityName + "] 能力获取调用异常（可能不支持该能力）！\n"; // 拼接语句：" + xmlInput.Replace ("<?xml version='1.0' encoding='utf-8'?>", "") + "\n";
                try {
                    // 通过错误代码获取消息内容
                    custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                } catch (Exception ex) {
                    AbilityReturn = AbilityReturn.TrimEnd ('\n') + "错误信息：" + ex.Message + "\n";
                }
            }
            SimpleLogInfo.Text = AbilityReturn;
            Marshal.FreeHGlobal (pInBuf);
            Marshal.FreeHGlobal (pOutBuf);
        }

        /* 暂时不做
        unsafe private bool NET_DVR_GetDeviceSTDAbility (int UserID, int dwAbilityType, int channel) {
            CHCNetSDK.NET_DVR_STD_ABILITY lpAbilityParam = new CHCNetSDK.NET_DVR_STD_ABILITY ();
            const int XML_ABILITY_OUT_LEN = 2 * 1024 * 1024;
            IntPtr pOutBuf = System.Runtime.InteropServices.Marshal.AllocHGlobal (XML_ABILITY_OUT_LEN);
            char* aChar = (char*)Marshal.StringToHGlobalAnsi (1 + "").ToPointer ();

            lpAbilityParam.lpCondBuffer = (IntPtr)aChar;
            lpAbilityParam.dwCondSize = sizeof (int);

            lpAbilityParam.lpOutBuffer = pOutBuf;
            lpAbilityParam.dwOutSize = XML_ABILITY_OUT_LEN;
            lpAbilityParam.lpStatusBuffer = pOutBuf;
            lpAbilityParam.dwStatusSize = XML_ABILITY_OUT_LEN;


            bool bitResult = CHCNetSDK.NET_DVR_GetSTDAbility (UserID, 3500, ref lpAbilityParam);
            if (!bitResult) {
                uint nError = CHCNetSDK.NET_DVR_GetLastError ();
            }

            string strResult = Marshal.PtrToStringAnsi (pOutBuf, XML_ABILITY_OUT_LEN);

            /*
            switch (dwAbilityType) {
                case CHCNetSDK.NET_DVR_GET_SMART_CAPABILITIES:
                    lpAbilityParam.lpCondBuffer = IntPtr.Zero;
                    lpAbilityParam.lpOutBuffer = ;

            }
            return false;
        }*/

        private void HikAbility_Click (object sender, RoutedEventArgs e) {
            if (UserID > -1) {
                cListItem curItem = (cListItem)HikAbilityList.SelectedItem;
                if (curItem == null && HikAbilityList.Items.Count > 0) {
                    curItem = (cListItem)HikAbilityList.Items.GetItemAt (0);
                } else if (curItem == null && HikAbilityList.Items.Count == 0) {
                    curItem = new cListItem (CHCNetSDK.DEVICE_SOFTHARDWARE_ABILITY, "设备软硬件能力");
                }
                custom_GetDeviceAbility (UserID, (uint)curItem.ID, 1);
            } else { SimpleLogInfo.Text += "您当前尚未登陆！\n"; }
        }

        private void HikGetConfig_Click (object sender, RoutedEventArgs e) {
            uint dwReturn = 0;
            CHCNetSDK.NET_DVR_DEVICECFG_V40 myDeviceCfg = new CHCNetSDK.NET_DVR_DEVICECFG_V40 ();
            int nSize = Marshal.SizeOf (myDeviceCfg);
            IntPtr ptrDeviceCfg = Marshal.AllocHGlobal (nSize);
            Marshal.StructureToPtr (myDeviceCfg, ptrDeviceCfg, false);
            if (UserID > -1) {
                if (CHCNetSDK.NET_DVR_GetDVRConfig (UserID, CHCNetSDK.NET_DVR_GET_DEVICECFG_V40, -1, ptrDeviceCfg, (uint)nSize, ref dwReturn)) {
                    myDeviceCfg = (CHCNetSDK.NET_DVR_DEVICECFG_V40)Marshal.PtrToStructure (ptrDeviceCfg, typeof (CHCNetSDK.NET_DVR_DEVICECFG_V40));
                    SimpleLogInfo.Text += "设备名称：" + Encoding.UTF8.GetString (myDeviceCfg.sDVRName).TrimEnd ('\0') + "\n";
                    SimpleLogInfo.Text += "设备型号：" + Encoding.UTF8.GetString (myDeviceCfg.byDevTypeName).TrimEnd ('\0') + "\n";
                    SimpleLogInfo.Text += "设备编号：" + Encoding.UTF8.GetString (myDeviceCfg.sSerialNumber).TrimEnd ('\0') + "\n";
                    uint[] EdwSoftwareVersion = {
                        // 高8位
                        myDeviceCfg.dwSoftwareVersion >> 24,
                        // 高16位低8位
                        myDeviceCfg.dwSoftwareVersion << 8 >> 24,
                        // 低16位
                        myDeviceCfg.dwSoftwareVersion << 16 >> 16
                    };
                    uint[] EdwSoftwareBuildDate = {
                        (myDeviceCfg.dwSoftwareBuildDate >> 16) + 2000,
                        myDeviceCfg.dwSoftwareBuildDate << 16 >> 24,
                        myDeviceCfg.dwSoftwareBuildDate << 24 >> 24
                    };
                    SimpleLogInfo.Text += "软件版本：" + string.Join (".", EdwSoftwareVersion) + " Build " + string.Join ("/", EdwSoftwareBuildDate) + "\n";
                    uint[] EdwHardwareVersion = {
                        myDeviceCfg.dwHardwareVersion >> 16,
                        myDeviceCfg.dwHardwareVersion << 16 >> 16
                    };
                    uint[] EdwPanelVersion = {
                        myDeviceCfg.dwPanelVersion  >> 16,
                        myDeviceCfg.dwPanelVersion  << 16 >> 16
                    };
                    SimpleLogInfo.Text += "硬件版本：" + string.Join (".", EdwHardwareVersion) + " Panel " + string.Join (".", EdwPanelVersion) + "\n";
                    SimpleLogInfo.Text += "报警接口：IN => " + Convert.ToString (myDeviceCfg.byAlarmInPortNum) + " x OUT => " + Convert.ToString (myDeviceCfg.byAlarmOutPortNum) + "\n";
                } else {
                    try {
                        // 通过错误代码获取消息内容
                        custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                    } catch (Exception ex) {
                        SimpleLogInfo.Text += "信息获取调用异常：" + ex.Message + "\n";
                    }
                }
            } else { SimpleLogInfo.Text += "您当前尚未登陆！\n"; }
        }
        //protected override void WndProc (ref Message m) {

        //}
        private void custom_GetCardEventProcess (int status, CHCNetSDK.NET_DVR_CARD_CFG_V50 struCardCfg) {
            if (status == 0x0800 && m_GetCardCfgHandle != -1) {
                m_GetCardCfgHandle = -1;
                CHCNetSDK.NET_DVR_StopRemoteConfig (m_GetCardCfgHandle);
                // HikPerson.Sort ();
                HikPerson.Sort ((A, B) => A.AccountNo.CompareTo (B.AccountNo));
                SimpleLogInfo.Text += "标准批量调账成功，总计返回结果" + HikPerson.Count + "条！\n";
            } else {
                // uint accno = struCardCfg.dwEmployeeNo;
                // string name = Encoding.GetEncoding ("gb2312").GetString (struCardCfg.byName).TrimEnd ('\0');
                // string card = Encoding.ASCII.GetString (struCardCfg.byCardNo).TrimEnd ('\0');
                // SimpleLogInfo.Text += "["+ accno + "]" + name + ": " + card + "\n";
                HikPerson.Add (new Person (struCardCfg));
            }
        }
        private delegate void GetCardCallbackDelegate (int status, CHCNetSDK.NET_DVR_CARD_CFG_V50 struCardCfg);
        private GetCardCallbackDelegate GetCardEventProcess;
        private void custom_ProcessGetGatewayCardCallback (uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData) {
            CHCNetSDK.NET_DVR_CARD_CFG_V50 struCardCfg = new CHCNetSDK.NET_DVR_CARD_CFG_V50 ();
            if (pUserData == null) {
                return;
            } else if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_DATA) {
                struCardCfg = (CHCNetSDK.NET_DVR_CARD_CFG_V50)Marshal.PtrToStructure (lpBuffer, typeof (CHCNetSDK.NET_DVR_CARD_CFG_V50));

                Dispatcher.BeginInvoke (GetCardEventProcess, 0x0801, struCardCfg);
            } else if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS) {
                uint dwStatus = (uint)Marshal.ReadInt32 (lpBuffer);
                if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS) {
                    Dispatcher.BeginInvoke (GetCardEventProcess, 0x0800, struCardCfg);
                } else if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED) {
                    byte[] bRawData = new byte[40];//4字节状态 + 4字节错误码 + 32字节卡号
                    Marshal.Copy (lpBuffer, bRawData, 0, 40);//将非托管内存指针数据复制到数组中

                    byte[] errorb = new byte[4];//4字节错误码
                    Array.Copy (bRawData, 4, errorb, 0, 4);
                    int errorCode = BitConverter.ToInt32 (errorb, 0);

                    byte[] byCardNo = new byte[32];//32字节卡号
                    Array.Copy (bRawData, 8, byCardNo, 0, 32);
                    string strCardNo = Encoding.ASCII.GetString (byCardNo).TrimEnd ('\0');
                    Dispatcher.BeginInvoke (GetCardEventProcess, 0x0800, struCardCfg);
                }
            }
        }

        private void HikGetCard_Click (object sender, RoutedEventArgs e) {
            GetCardEventProcess = new GetCardCallbackDelegate (custom_GetCardEventProcess);
            if (-1 != m_GetCardCfgHandle) {
                if (CHCNetSDK.NET_DVR_StopRemoteConfig (m_GetCardCfgHandle)) {
                    SimpleLogInfo.Text += "未完成卡句柄" + m_GetCardCfgHandle + "已通知终止！\n";
                    m_GetCardCfgHandle = -1;
                }
            }
            HikPerson.Clear ();
            CHCNetSDK.NET_DVR_CARD_CFG_COND struCardCond = new CHCNetSDK.NET_DVR_CARD_CFG_COND ();
            struCardCond.dwSize = (uint)Marshal.SizeOf (struCardCond);
            struCardCond.dwCardNum = 0xFFFFFFFF;
            struCardCond.byCheckCardNo = 0x01;
            struCardCond.wLocalControllerID = 0x00;

            int dwSize = Marshal.SizeOf (struCardCond);
            IntPtr ptrCardStruCond = Marshal.AllocHGlobal (dwSize);
            Marshal.StructureToPtr (struCardCond, ptrCardStruCond, false);
            // 建立回调
            m_GetGatewayCardCallback = new CHCNetSDK.RemoteConfigCallback (custom_ProcessGetGatewayCardCallback);
            m_GetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig (UserID, CHCNetSDK.NET_DVR_GET_CARD_CFG_V50, ptrCardStruCond, dwSize, m_GetGatewayCardCallback, IntPtr.Zero);
            if (m_GetCardCfgHandle == -1) {
                SimpleLogInfo.Text += "建立远程配置已失败，请检查！\n";
            } else { SimpleLogInfo.Text += "建立远程配置已加载，请注意！\n"; }

            Marshal.FreeHGlobal (ptrCardStruCond);
        }
        private void custom_SetCardEventProcess (int status, int mode) {
            if (status == 0x0900 && mode == 0x00 && m_SetCardCfgHandle != -1) {
                SimpleLogInfo.Text += "触发回调成功，应该尝试更新数据";
            } else {
                SimpleLogInfo.Text += "触发回调成功，应该尝试结束配置";
            }
        }
        private delegate void SetCardCallbackDelegate (int status, int mode);
        private SetCardCallbackDelegate SetCardEventProcess;
        private void custom_ProcessSetGatewayCardCallback (uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData) {
            if (pUserData == null) {
                return;
            } else if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_DATA) {
                //struCardCfg = (CHCNetSDK.NET_DVR_CARD_CFG_V50)Marshal.PtrToStructure (lpBuffer, typeof (CHCNetSDK.NET_DVR_CARD_CFG_V50));

                //Dispatcher.BeginInvoke (SetCardEventProcess, 0x0801, struCardCfg);
            } else if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS) {
                uint dwStatus = (uint)Marshal.ReadInt32 (lpBuffer);
                if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS) {
                    // 判断执行类型
                    Dispatcher.BeginInvoke (SetCardEventProcess, 0x0900, 0x00);
                } else if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED) {
                    byte[] bRawData = new byte[40];//4字节状态 + 4字节错误码 + 32字节卡号
                    Marshal.Copy (lpBuffer, bRawData, 0, 40);//将非托管内存指针数据复制到数组中

                    byte[] errorb = new byte[4];//4字节错误码
                    Array.Copy (bRawData, 4, errorb, 0, 4);
                    int errorCode = BitConverter.ToInt32 (errorb, 0);

                    byte[] byCardNo = new byte[32];//32字节卡号
                    Array.Copy (bRawData, 8, byCardNo, 0, 32);
                    string strCardNo = Encoding.ASCII.GetString (byCardNo).TrimEnd ('\0');
                    //Dispatcher.BeginInvoke (SetCardEventProcess, 0x0800, struCardCfg);
                }
            }
        }
        private void custom_CardInfoUpdate (List<Person> people, int mode) {
            CHCNetSDK.NET_DVR_CARD_CFG_COND struCardCond = new CHCNetSDK.NET_DVR_CARD_CFG_COND ();
            struCardCond.dwSize = (uint)Marshal.SizeOf (struCardCond);
            struCardCond.dwCardNum = 0xFFFFFFFF;
            struCardCond.byCheckCardNo = 0x01;
            struCardCond.wLocalControllerID = 0x00;

            int dwSize = Marshal.SizeOf (struCardCond);
            IntPtr ptrCardStruCond = Marshal.AllocHGlobal (dwSize);
            Marshal.StructureToPtr (struCardCond, ptrCardStruCond, false);
            // 建立回调
            m_SetGatewayCardCallback = new CHCNetSDK.RemoteConfigCallback (custom_ProcessSetGatewayCardCallback);
            if(mode == 0x02) {
                for (int i = 0; i < people.Count; i++) {
                    int wait = 0;
                    do {
                        wait++;
                        Thread.Sleep (0);
                        m_SetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig (UserID, CHCNetSDK.NET_DVR_SET_CARD_CFG_V50, ptrCardStruCond, dwSize, m_SetGatewayCardCallback, IntPtr.Zero);
                        SimpleLogInfo.Text += "当前正在配置下发环境：" + m_SetCardCfgHandle + "\n";
                    } while (wait <= 0xFF && m_SetCardCfgHandle == -1);
                    if (m_SetCardCfgHandle != -1) {
                        SimpleLogInfo.Text += "正在准备下发：" + people[i].ToString () + "\n";
                        CHCNetSDK.NET_DVR_CARD_CFG_V50 newCard = new CHCNetSDK.NET_DVR_CARD_CFG_V50 ();
                        newCard.Init ();
                        uint dwuSize = (uint)Marshal.SizeOf (newCard);
                        newCard.dwSize = dwuSize;
                        newCard.dwModifyParamType = 0x00000001;
                        // 设置卡号
                        byte[] sCardNo = Encoding.ASCII.GetBytes (people[i].oldCardNo.ToString ());
                        sCardNo.CopyTo (newCard.byCardNo, 0);
                        newCard.byCardValid = 0;

                        IntPtr ptrStruCard = Marshal.AllocHGlobal ((int)dwuSize);
                        Marshal.StructureToPtr (newCard, ptrStruCard, false);
                        SimpleLogInfo.Text += "发送数据：" + m_SetCardCfgHandle + "+" + (int)CHCNetSDK.LONG_CFG_SEND_DATA_TYPE_ENUM.ENUM_ACS_SEND_DATA + "+" + dwuSize;
                        if (!CHCNetSDK.NET_DVR_SendRemoteConfig (m_SetCardCfgHandle, (int)CHCNetSDK.LONG_CFG_SEND_DATA_TYPE_ENUM.ENUM_ACS_SEND_DATA, ptrStruCard, dwuSize)) {
                            Marshal.FreeHGlobal (ptrStruCard);
                            SimpleLogInfo.Text += string.Format ("新增卡片失败：{0} => {1}\n", Encoding.Default.GetString (newCard.byName).TrimEnd ('\0'), Encoding.ASCII.GetString (newCard.byCardNo).TrimEnd ('\0'));
                        } else { SimpleLogInfo.Text += string.Format ("新增卡片成功：{0} => {1}\n", Encoding.Default.GetString (newCard.byName).TrimEnd ('\0'), Encoding.ASCII.GetString (newCard.byCardNo).TrimEnd ('\0')); }
                        Marshal.FreeHGlobal (ptrStruCard);
                        SimpleLogInfo.Text += "数据下发结束\n";
                        CHCNetSDK.NET_DVR_StopRemoteConfig (m_SetCardCfgHandle);
                    } else {
                        try {
                            // 通过错误代码获取消息内容
                            custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                        } catch (Exception ex) {
                            SimpleLogInfo.Text += "下发环境配置异常：" + ex.Message + "\n";
                        }
                    }
                }
            }
            for (int i = 0; i < people.Count; i++) {
                int wait = 0;
                do {
                    wait++;
                    Thread.Sleep (0);
                    m_SetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig (UserID, CHCNetSDK.NET_DVR_SET_CARD_CFG_V50, ptrCardStruCond, dwSize, m_SetGatewayCardCallback, IntPtr.Zero);
                    SimpleLogInfo.Text += "当前正在配置下发环境：" + m_SetCardCfgHandle + "\n";
                } while (wait <= 0xFF && m_SetCardCfgHandle == -1);
                if (m_SetCardCfgHandle != -1) {
                    SimpleLogInfo.Text += "正在准备下发：" + people[i].ToString () + "\n";
                    CHCNetSDK.NET_DVR_CARD_CFG_V50 newCard = new CHCNetSDK.NET_DVR_CARD_CFG_V50 ();
                    newCard.Init ();
                    uint dwuSize = (uint)Marshal.SizeOf (newCard);
                    newCard.dwSize = dwuSize;
                    newCard.dwModifyParamType = 0x00000FFF;
                    // 设置卡号
                    byte[] sCardNo = Encoding.ASCII.GetBytes (people[i].CardNo.ToString ());
                    sCardNo.CopyTo (newCard.byCardNo, 0);
                    byte[] sCardPassword = Encoding.ASCII.GetBytes ("5401");
                    sCardPassword.CopyTo (newCard.byCardPassword, 0);
                    byte[] sCardHolder = Encoding.Default.GetBytes (people[i].Name);
                    sCardHolder.CopyTo (newCard.byName, 0);
                    newCard.byCardValid = 1;
                    newCard.struValid.byEnable = 1;
                    newCard.struValid.struBeginTime.wYear = (ushort)0x7D0;
                    newCard.struValid.struBeginTime.byMonth = (byte)0x01;
                    newCard.struValid.struBeginTime.byDay = (byte)0x01;
                    newCard.struValid.struBeginTime.byHour = (byte)0x00;
                    newCard.struValid.struBeginTime.byMinute = (byte)0x00;
                    newCard.struValid.struBeginTime.bySecond = (byte)0x00;
                    newCard.struValid.struEndTime.wYear = (ushort)0x7F0;
                    newCard.struValid.struEndTime.byMonth = (byte)0x01;
                    newCard.struValid.struEndTime.byDay = (byte)0x01;
                    newCard.struValid.struEndTime.byHour = (byte)0x00;
                    newCard.struValid.struEndTime.byMinute = (byte)0x00;
                    newCard.struValid.struEndTime.bySecond = (byte)0x00;
                    newCard.byCardType = 1;
                    newCard.byLeaderCard = 1;
                    newCard.byUserType = 1;
                    newCard.dwMaxSwipeTime = 0;
                    newCard.dwEmployeeNo = people[i].AccountNo;

                    IntPtr ptrStruCard = Marshal.AllocHGlobal ((int)dwuSize);
                    Marshal.StructureToPtr (newCard, ptrStruCard, false);
                    SimpleLogInfo.Text += "发送数据：" + m_SetCardCfgHandle + "+" + (int)CHCNetSDK.LONG_CFG_SEND_DATA_TYPE_ENUM.ENUM_ACS_SEND_DATA + "+" + dwuSize;
                    if (!CHCNetSDK.NET_DVR_SendRemoteConfig (m_SetCardCfgHandle, (int)CHCNetSDK.LONG_CFG_SEND_DATA_TYPE_ENUM.ENUM_ACS_SEND_DATA, ptrStruCard, dwuSize)) {
                        Marshal.FreeHGlobal (ptrStruCard);
                        SimpleLogInfo.Text += string.Format ("新增卡片失败：{0} => {1}\n", Encoding.Default.GetString (newCard.byName).TrimEnd ('\0'), Encoding.ASCII.GetString (newCard.byCardNo).TrimEnd ('\0'));
                    } else { SimpleLogInfo.Text += string.Format ("新增卡片成功：{0} => {1}\n", Encoding.Default.GetString (newCard.byName).TrimEnd ('\0'), Encoding.ASCII.GetString (newCard.byCardNo).TrimEnd ('\0')); }
                    Marshal.FreeHGlobal (ptrStruCard);
                    SimpleLogInfo.Text += "数据下发结束\n";
                    CHCNetSDK.NET_DVR_StopRemoteConfig (m_SetCardCfgHandle);
                } else {
                    try {
                        // 通过错误代码获取消息内容
                        custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                    } catch (Exception ex) {
                        SimpleLogInfo.Text += "下发环境配置异常：" + ex.Message + "\n";
                    }
                }
            }
        }
        private void HikSetCardAdd_Click (object sender, RoutedEventArgs e) {
            SetCardEventProcess = new SetCardCallbackDelegate (custom_SetCardEventProcess);
            addPerson.Clear ();
            updPerson.Clear ();
            delPerson.Clear ();

            int YKT_Count = YKTPerson.Count;
            int Hik_Count = HikPerson.Count;
            int Hik_Count_Curr = 0;
            int Max_Count = (YKT_Count > Hik_Count) ? YKT_Count : Hik_Count;
            SimpleLogInfo.Text += "现有一卡通" + YKT_Count + "人、海康" + Hik_Count + "人\n";
            if (YKT_Count == 0) {
                return;
            } else if (Hik_Count == 0) {
                for (int i = 0; i < YKTPerson.Count; i++) {
                    addPerson.Add (YKTPerson[i]);
                }
            } else {
                for (int i = 0; i < YKT_Count; i++) {
                    Person curr_YKT_Person = YKTPerson[i];
                    if (Hik_Count_Curr < Hik_Count) {
                        Person curr_Hik_Person = HikPerson[Hik_Count_Curr];
                        SimpleLogInfo.Text += "一卡通：" + curr_YKT_Person.ToString () + "海康威视：" + curr_Hik_Person.ToString ();
                        if (curr_YKT_Person == curr_Hik_Person) {
                            Hik_Count_Curr++;
                            if (!curr_YKT_Person.Equals (curr_Hik_Person)) {
                                curr_YKT_Person.oldCardNo = curr_Hik_Person.CardNo;
                                updPerson.Add (curr_YKT_Person);
                                SimpleLogInfo.Text += "---数据更新\n";
                            } else { SimpleLogInfo.Text += "---数据一致\n"; }
                        } else {
                            if (curr_YKT_Person < curr_Hik_Person) {
                                SimpleLogInfo.Text += "---增加一卡通人员\n";
                                addPerson.Add (curr_YKT_Person);
                            } else if (curr_YKT_Person > curr_Hik_Person) {
                                SimpleLogInfo.Text += "---删除海康人员\n";
                                Hik_Count_Curr++;
                                delPerson.Add (curr_Hik_Person);
                            }
                        }
                    } else { addPerson.Add (curr_YKT_Person); }
                }
            }
            SimpleLogInfo.Text += "新增：" + addPerson.Count + "、更新：" + updPerson.Count + "、删除：" + delPerson.Count + "\n";
            // for (int i = 0; i < updPerson.Count; i++) { addPerson.Add(updPerson[i]); }

            if (-1 != m_SetCardCfgHandle) {
                if (CHCNetSDK.NET_DVR_StopRemoteConfig (m_SetCardCfgHandle)) {
                    SimpleLogInfo.Text += "未完成卡句柄" + m_SetCardCfgHandle + "已通知终止！\n";
                    m_SetCardCfgHandle = -1;
                }
            }
            custom_CardInfoUpdate (addPerson, 0x01);
            custom_CardInfoUpdate (updPerson, 0x02);
        }
        private static string custom_GetMD5HashFromFile (string fileName) {
            try {
                FileStream file = new FileStream (fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
                byte[] retVal = md5.ComputeHash (file);
                file.Close ();

                StringBuilder sb = new StringBuilder ();
                for (int i = 0; i < retVal.Length; i++) {
                    sb.Append (retVal[i].ToString ("x2"));
                }
                return sb.ToString ();
            } catch (Exception ex) {
                // throw new Exception ("GetMD5HashFromFile() fail,error:" + ex.Message);
            }

            return "";
        }
        private void custom_Png_Bmp_to_Jpg (string filepath) {
            DirectoryInfo FaceFolder = new DirectoryInfo (filepath);
            foreach (FileInfo NextFile in FaceFolder.GetFiles ()) {
                if ((NextFile.Extension.ToLower ().Equals (".png") || NextFile.Extension.ToLower ().Equals (".bmp"))) {
                    Image img = Image.FromFile (NextFile.FullName);
                    // Assumes myImage is the PNG you are converting
                    using (Bitmap b = new Bitmap (img.Width, img.Height)) {
                        b.SetResolution (img.HorizontalResolution, img.VerticalResolution);

                        using (var g = Graphics.FromImage (b)) {
                            g.Clear (Color.White);
                            g.DrawImageUnscaled (img, 0, 0);
                        }
                        b.Save (NextFile.DirectoryName + "\\" + NextFile.Name.Split('.')[0] + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    img.Dispose ();
                    File.Delete (NextFile.FullName);
                }
            }
        }
        public class FaceParam {
            [JsonProperty ("FaceName")]
            public string faceName { get; set; }
            [JsonProperty ("FileSize")]
            public long fileSize { get; set; }
            [JsonProperty ("FileExt")]
            public string fileExt { get; set; }
            [JsonProperty ("FileMD5")]
            public string fileMD5 { get; set; }
            [JsonProperty ("isNUpdated")]
            public bool isUpdated { get; set; }
            [JsonProperty ("CardNo")]
            public uint cardNo { get; set; }
            public FaceParam (FileInfo File) {
                faceName = "";
                fileSize = 0;
                fileExt = "";
                fileMD5 = "";
                isUpdated = true;
                if (File != null) {
                    faceName = File.Name.Split ('.')[0];
                    fileSize = File.Length;
                    if (fileSize / 1024 >= 200) { isUpdated = false; }
                    fileExt = File.Extension.TrimStart ('.').ToLower ();
                    fileMD5 = custom_GetMD5HashFromFile (File.FullName);
                }
            }
            public void doNotUpdate () {
                isUpdated = false;
            }
            public override bool Equals (object obj) {
                if (obj == null) { return false; }
                if (obj.GetType ().Equals (GetType ()) == false) { return false; }
                FaceParam tmp = obj as FaceParam;
                return faceName.Equals (tmp.faceName) && fileSize.Equals (tmp.fileSize) && fileMD5.Equals (tmp.fileMD5);
            }
            public override int GetHashCode () {
                return faceName.GetHashCode () + fileSize.GetHashCode () + fileMD5.GetHashCode ();
            }
            public override string ToString () {
                return faceName + "[" + fileExt + "] = " + fileSize / 1024 + " : " + fileMD5;
            }
        }
        private void custom_GetFaceEventProcess (int status, CHCNetSDK.NET_DVR_FACE_PARAM_CFG lpCardCfg) {
            if (status == 0x0801) {
                SimpleLogInfo.Text += Encoding.ASCII.GetString(lpCardCfg.byCardNo) + lpCardCfg.dwFaceLen;
            } else {
                CHCNetSDK.NET_DVR_StopRemoteConfig (m_GetFaceCfgHandle);
            }
        }
        private delegate void GetFaceCallbackDelegate (int status, CHCNetSDK.NET_DVR_FACE_PARAM_CFG lpCardCfg);
        private GetFaceCallbackDelegate GetFaceEventProcess;
        private void custom_ProcessGetFaceGatewayCardCallback (uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData) {
            if (pUserData == null) {
                return;
            } else if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_DATA) {
                CHCNetSDK.NET_DVR_FACE_PARAM_CFG lpCardCfg = new CHCNetSDK.NET_DVR_FACE_PARAM_CFG ();
                lpCardCfg = (CHCNetSDK.NET_DVR_FACE_PARAM_CFG)Marshal.PtrToStructure (lpBuffer, typeof (CHCNetSDK.NET_DVR_FACE_PARAM_CFG));
                if (1 == lpCardCfg.byEnableCardReader[0]) {
                    Dispatcher.BeginInvoke (GetFaceEventProcess, 0x0801, lpCardCfg);
                }
            } else if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS) {
                uint dwStatus = (uint)Marshal.ReadInt32 (lpBuffer);
                if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS) {
                    // 判断执行类型
                    Dispatcher.BeginInvoke (GetFaceEventProcess, 0x0800, 0x00);
                } else if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED) {
                    byte[] bRawData = new byte[40];//4字节状态 + 4字节错误码 + 32字节卡号
                    Marshal.Copy (lpBuffer, bRawData, 0, 40);//将非托管内存指针数据复制到数组中

                    byte[] errorb = new byte[4];//4字节错误码
                    Array.Copy (bRawData, 4, errorb, 0, 4);
                    int errorCode = BitConverter.ToInt32 (errorb, 0);

                    byte[] byCardNo = new byte[32];//32字节卡号
                    Array.Copy (bRawData, 8, byCardNo, 0, 32);
                    string strCardNo = Encoding.ASCII.GetString (byCardNo).TrimEnd ('\0');
                    //Dispatcher.BeginInvoke (SetCardEventProcess, 0x0800, struCardCfg);
                }
            }
        }
        private void custom_GetSetFaceInfo (List<FaceParam> faceList) {
            CHCNetSDK.NET_DVR_FACE_PARAM_COND struFaceCond = new CHCNetSDK.NET_DVR_FACE_PARAM_COND ();
            struFaceCond.Init ();
            struFaceCond.dwSize = (uint)Marshal.SizeOf (struFaceCond);
            struFaceCond.dwFaceNum = 0xFFFFFFFF;
            struFaceCond.byFaceID = 0x01;
            // 设置卡号
            byte[] sCardNo = Encoding.ASCII.GetBytes (faceList[0].cardNo.ToString ());
            sCardNo.CopyTo (struFaceCond.byCardNo, 0);
            struFaceCond.byEnableCardReader[0] = 0x01;

            int dwSize = Marshal.SizeOf (struFaceCond);
            IntPtr ptrStruCond = Marshal.AllocHGlobal (dwSize);
            Marshal.StructureToPtr (struFaceCond, ptrStruCond, true);
            // 建立回调
            m_GetFaceGatewayCardCallback = new CHCNetSDK.RemoteConfigCallback (custom_ProcessGetFaceGatewayCardCallback);
            m_GetFaceCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig (UserID, CHCNetSDK.NET_DVR_GET_FACE_PARAM_CFG, ptrStruCond, dwSize, m_GetFaceGatewayCardCallback, IntPtr.Zero);


        }
        private void HikCheckFaceImage_click (object sender, RoutedEventArgs e) {
            GetFaceEventProcess = new GetFaceCallbackDelegate (custom_GetFaceEventProcess);
            if (-1 != m_GetFaceCfgHandle) {
                if (CHCNetSDK.NET_DVR_StopRemoteConfig (m_GetFaceCfgHandle)) {
                    SimpleLogInfo.Text += "未完成卡句柄" + m_GetFaceCfgHandle + "已通知终止！\n";
                    m_GetFaceCfgHandle = -1;
                }
            }
            List<FaceParam> FaceList = new List<FaceParam> ();
            List<FaceParam> FaceSave = new List<FaceParam> ();
            DirectoryInfo FaceFolder = new DirectoryInfo (@".\Face\");
            custom_Png_Bmp_to_Jpg (FaceFolder.FullName);
            if (!FaceFolder.Exists) {
                SimpleLogInfo.Text += "人脸配置文件夹" + FaceFolder.FullName + "不存在！\n";
                return;
            }
            foreach (FileInfo NextFile in FaceFolder.GetFiles ()) {
                if ((NextFile.Extension.ToLower ().Equals (".jpg") || NextFile.Extension.ToLower ().Equals (".jpeg")) && NextFile.Length < 1024 * 200) {
                    FaceParam now = new FaceParam (NextFile);
                    int index = YKTPerson.FindIndex (item => item.Name.Equals (now.faceName));
                    if (index > -1) {
                        now.cardNo = YKTPerson[index].CardNo;
                    }
                    FaceList.Add (now);
                }
            }
            if (File.Exists(FaceFolder.FullName + "Storage.json")) {
                try {
                    using (StreamReader jsonStorage = new StreamReader (FaceFolder.FullName + "Storage.json", Encoding.UTF8)) {
                        FaceSave = JsonConvert.DeserializeObject<List<FaceParam>> (jsonStorage.ReadToEnd ());
                    }
                } catch (Exception ex) {
                    FaceSave = new List<FaceParam> ();
                }
            }
            if (FaceSave.Count > 0) {
                foreach (FaceParam now in FaceSave) {
                    int index = FaceList.FindIndex (item => item.fileMD5.Equals (now.fileMD5));
                    if (index > -1) {
                        if (FaceList[index].Equals (now)) { FaceList[index].doNotUpdate (); }
                    }
                }
            }
            custom_GetSetFaceInfo (FaceList);
            using (StreamWriter jsonStorage = new StreamWriter (FaceFolder.FullName + "Storage.json", false, Encoding.UTF8)) {
                jsonStorage.Write (JsonConvert.SerializeObject (FaceList));
            }
        }
        private void HikLogout_Click (object sender, RoutedEventArgs e) {
            if (UserID > -1) {
                if (CHCNetSDK.NET_DVR_Logout(UserID)) {
                    SimpleLogInfo.Text += "用户序号" + UserID.ToString () + "登出成功！\n";
                    UserID = -1;
                } else {
                    try {
                        // 通过错误代码获取消息内容
                        custom_GetErrorMessage ((int)CHCNetSDK.NET_DVR_GetLastError ());
                    } catch (Exception ex) {
                        SimpleLogInfo.Text += "登出调用异常：" + ex.Message + "\n";
                    }
                }
            } else { SimpleLogInfo.Text += "您当前尚未登陆！\n"; }
        }
    }
}
