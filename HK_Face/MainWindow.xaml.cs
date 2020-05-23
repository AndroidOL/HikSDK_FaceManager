using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Controls.Ribbon;
using System.Runtime.InteropServices.WindowsRuntime;

namespace HK_Face {
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
        public MainWindow () {
            InitializeComponent ();
            custom_InitComboBoxItems ();
        }

        ~MainWindow () {
            // 确认用户已经登出
            if (UserID > -1) {
                CHCNetSDK.NET_DVR_Logout (UserID);
            }
            // 清理 SDK  资源
            CHCNetSDK.NET_DVR_Cleanup ();
        }

        public static void custom_GetErrorMessage (int iErr) {
            // 获取错误信息
            IntPtr psErr = CHCNetSDK.NET_DVR_GetErrorMsg (ref iErr);
            string sErr = System.Runtime.InteropServices.Marshal.PtrToStringAnsi (psErr);
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
            cListItem curSelected = (cListItem) HikAbilityList.SelectedItem;
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
                    wPort = 8000,
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
                byte[] tDeviceAddress = Encoding.ASCII.GetBytes ("10.0.0.254");
                Array.Copy (tDeviceAddress, 0, pLoginInfo.sDeviceAddress, 0, tDeviceAddress.Length);
                // 设置登录设备的用户名与密码
                byte[] tUserName = Encoding.ASCII.GetBytes ("user");
                Array.Copy (tUserName, 0, pLoginInfo.sUserName, 0, tUserName.Length);
                byte[] tPassword = Encoding.ASCII.GetBytes ("wellin5401");
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

        private void custom_GetDeviceAbility(int LocalUserID, uint dwAbilityType, int channelNo) {
            string channel = channelNo.ToString ();
            IntPtr pInBuf;
            int nSize;
            string dwAbilityName;

            string xmlInput = "<?xml version='1.0' encoding='utf-8'?>";
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
                    xmlInput += "<AlarmAbility version='2.0'><channelID>" + channel + "</channelID></AlarmAbility>";
                    break;
                case CHCNetSDK.DEVICE_DYNCHAN_ABILITY:          // 获取设备数字通道能力
                    dwAbilityName = "获取设备数字通道能力";
                    xmlInput += "<DynChannelAbility version='2.0'><channelNO>" + channel + "</channelNO></DynChannelAbility>";
                    break;
                case CHCNetSDK.DEVICE_USER_ABILITY:             // 获取设备用户管理参数能力
                    dwAbilityName = "获取设备用户管理参数能力";
                    xmlInput += "<UserAbility version='2.0'></UserAbility>";
                    break;
                case CHCNetSDK.DEVICE_NETAPP_ABILITY:           // 获取设备网络应用参数能力
                    dwAbilityName = "获取设备网络应用参数能力";
                    xmlInput += "<NetAppAbility version='2.0'></NetAppAbility>";
                    break;
                case CHCNetSDK.DEVICE_VIDEOPIC_ABILITY:         // 获取设备图像参数能力
                    dwAbilityName = "获取设备图像参数能力";
                    xmlInput += "<VideoPicAbility version='2.0'><channelNO>" + channel + "</channelNO></VideoPicAbility> ";
                    break;
                case CHCNetSDK.DEVICE_JPEG_CAP_ABILITY:         // 获取设备JPEG抓图能力
                    dwAbilityName = "获取设备JPEG抓图能力";
                    xmlInput += "<JpegCaptureAbility version='2.0'><channelNO>" + channel + "</channelNO></JpegCaptureAbility> ";
                    break;
                case CHCNetSDK.DEVICE_SERIAL_ABILITY:           // 获取设备RS232和RS485串口能力
                    dwAbilityName = "获取设备RS232和RS485串口能力";
                    xmlInput += "<SerialAbility version='2.0'><subBoardNo>" + channel + "</subBoardNo></SerialAbility>";
                    break;
                case CHCNetSDK.DEVICE_ABILITY_INFO:             // 设备通用能力类型，具体能力根据发送的能力节点来区分
                    dwAbilityName = "设备通用能力类型";
                    List<string> Ability = new List<string> {
                        "<PTZAbility version='2.0'><channelNO>" + channel + "</channelNO></PTZAbility>",          //获取PTZ能力集
                        "<EventAbility version='2.0'><channelNO>" + channel + "</channelNO></EventAbility>",      //获取报警事件处理能力集
                        "<ROIAbility version='2.0'><channelNO>" + channel + "</channelNO></ROIAbility>",          //获取ROI能力集
                        "<RecordAbility version='2.0'></RecordAbility>",                                          //获取录像相关能力集
                        "<GetAccessDeviceChannelAbility version='2.0'></GetAccessDeviceChannelAbility>",          //NVR前端待接入设备通道能力集
                        "<PreviewSwitchAbility version='2.0'></PreviewSwitchAbility>",                            //获取设备本地预览切换能力集
                        "<NPlusOneAbility version='2.0'></NPlusOneAbility>",                                      //获取设备N+1能力集
                        "<HardDiskAbility version='2.0'></HardDiskAbility>",                                      //获取设备磁盘相关能力集
                        "<IPAccessConfigFileAbility version='2.0'></IPAccessConfigFileAbility>",                  //获取IPC配置文件导入导出能力集
                        "<ChannelInputAbility version='2.0'><channelNO>" + channel + "</channelNO></ChannelInputAbility>",                //获取设备通道输入能力集
                        "<CameraParaDynamicAbility version='2.0'><channelNO>" + channel + "</channelNO><ExposureSetDynamicLinkTo><WDR><WDREnable>2</WDREnable></WDR><IrisMode><IrisType>0</IrisType></IrisMode></ExposureSetDynamicLinkTo><AudioVideoCompressInfoDynamicLinkTo></AudioVideoCompressInfoDynamicLinkTo><VbrAverageCapDynamicLinkTo><streamType></streamType><codeType></codeType><videoQualityControlType></videoQualityControlType><vbrUpperCap></vbrUpperCap></VbrAverageCapDynamicLinkTo></CameraParaDynamicAbility>",    //获取前端参数动态能力集
                        "<AlarmTriggerRecordAbility version='2.0'><channelNO>" + channel + "</channelNO></AlarmTriggerRecordAbility>",    //获取报警触发录像能力集
                        "<GBT28181AccessAbility version='2.0'><channelNO>" + channel + "</channelNO></GBT28181AccessAbility>",            //获取GB/T28181能力集
                        "<IOAbility version='2.0'><channelNO>" + channel + "</channelNO></IOAbility>",            //获取IO口输入输出能力集
                        "<AccessProtocolAbility version='2.0'><channelNO>" + channel + "</channelNO></AccessProtocolAbility>",    //获取协议接入能力集
                        "<SecurityAbility version='2.0'><channelNO>" + channel + "</channelNO></SecurityAbility>",                //获取安全认证配置能力集
                        "<CameraMountAbility  version='2.0'><channelNO>" + channel + "</channelNO></CameraMountAbility>",         //获取摄像机架设参数能力集
                        "<SearchLogAbility version= '2.0'><channelNO>" + channel + "</channelNO></SearchLogAbility>",             //获取日志搜索能力集
                        "<CVRAbility version='2.0'></CVRAbility>",                                                //获取CVR设备能力集
                        "<ImageDisplayParamAbility version='2.0'><channelNO>" + channel + "</channelNO></ImageDisplayParamAbility>",      //获取图像显示参数能力集
                        "<VcaDevAbility version='2.0'></VcaDevAbility>",                                          //获取智能设备能力集
                        "<VcaCtrlAbility version='2.0'><channelNO>" + channel + "</channelNO></VcaCtrlAbility>",  //获取智能通道控制能力集
                        "<VcaChanAbility version='2.0'><channelNO>" + channel + "</channelNO></VcaChanAbility>",  //获取智能通道分析能力集
                        "<BinocularAbility version='2.0'><channelNO>" + channel + "</channelNO></BinocularAbility>"             //获取双目能力集
                    };
                    Random RandomAbility = new Random ();
                    xmlInput += Ability[RandomAbility.Next (Ability.Count)];
                    break;
                case CHCNetSDK.STREAM_ABILITY:                  // 获取设备流能力
                    dwAbilityName = "获取设备流能力";
                    xmlInput += "<StreamAbility version='2.0'></StreamAbility> ";
                    break;
                case CHCNetSDK.MATRIXDECODER_ABILITY:           // 获取多路解码器显示、解码能力
                    dwAbilityName = "获取多路解码器显示、解码能力";
                    break;
                case CHCNetSDK.DECODER_ABILITY:                 // 获取解码器XML能力集
                    dwAbilityName = "获取解码器XML能力集";
                    xmlInput += "<DecoderAbility version='2.0'></DecoderAbility>";
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
                    xmlInput += "<AcsAbility version='2.0'></AcsAbility>";
                    break;
                default:
                    dwAbilityName = "未知类型";
                    break;
            }
            if (string.IsNullOrEmpty(xmlInput.Replace ("<?xml version='1.0' encoding='utf-8'?>", ""))) {
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
                    SimpleLogInfo.Text += "设备名称：" + Encoding.UTF8.GetString (myDeviceCfg.sDVRName) + "\n";
                    SimpleLogInfo.Text += "设备型号：" + Encoding.UTF8.GetString (myDeviceCfg.byDevTypeName) + "\n";
                    SimpleLogInfo.Text += "设备编号：" + Encoding.UTF8.GetString (myDeviceCfg.sSerialNumber) + "\n";
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
