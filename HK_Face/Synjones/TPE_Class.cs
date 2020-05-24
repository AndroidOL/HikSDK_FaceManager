using System;
using System.Data;
using System.Configuration;
using System.Runtime.InteropServices;

namespace WebServicePark {

    /// <summary>
    /// TPE_Class 的摘要说明
    /// </summary>
    /// 
    public class TPE_Class {
        public TPE_Class () {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_StartTPE")]
        public static extern int TPE_StartTPE ();
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_DownloadAllWL")]
        public static extern int TPE_DownloadAllWL ();
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_StopTPE")]
        public static extern int TPE_StopTPE ();
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_GetWL")]
        public static extern int TPE_GetWL (out tagWhiteListRec Rec);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_GetAccount")]
        public static extern int TPE_GetAccount (int CustomSn, ref tagTPE_GetAccountReq pReq, out tagTPE_GetAccountRes pRes, int bSync);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_FlowCost")]
        public static extern int TPE_FlowCost (int CustomSn, ref tagTPE_FlowCostReq pReq, int FlowCount, out tagTPE_FlowCostRes pRes, int bSync);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_FlowCostOrder")]
        public static extern int TPE_FlowCostOrder (int CustomSn, ref tagTPE_FlowCostReq pReq, ref byte OrderIDS, int FlowCount, out tagTPE_FlowCostRes pRes, int bSync);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_GetNetState")]
        public static extern int TPE_GetNetState ();
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_GetLocalNode")]
        public static extern int TPE_GetLocalNode ();
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_OnLineGetMaxSn")]
        public static extern int TPE_OnLineGetMaxSn (int CustomSn, out tagTPE_OnLineGetMaxSnRes pRes, int bSync);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_GetFileInfo")]
        public static extern int TPE_GetFileInfo (out tagTPE_FileInfo FileInfo);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_Lost")]
        public static extern int TPE_Lost (int CustomSn, ref tagTPE_LostReq pReq, out tagTPE_FlowRes pRes, int bSync);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_QueryStdAccount")]
        public static extern int TPE_QueryStdAccount (int CustomSn, ref tagTPE_QueryStdAccountReq pReq,
                                                            out tagTPE_QueryResControl pResControl,
                                                            int bSync);
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_ConfigEnumDept")]
        public static extern int TPE_ConfigEnumDept (int CustomSn, ref tagTPE_ConfigEnumDeptReq pReq,
                                                    out tagTPE_QueryResControl pResControl,
                                                    int bSync);

        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_QueryGeneralAccount")]
        public static extern int TPE_QueryGeneralAccount (int CustomSn,
                                                       ref tagTPE_QueryGeneralAccountReq pReq,
                                                       out tagTPE_QueryResControl pResControl,
                                                       int bSync);

        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_CheckPassword")]
        public static extern int TPE_CheckPassword (ref tagTPE_CheckPassword pReq);

        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_GetAccountEx")]
        public static extern int TPE_GetAccountEx (int CustomSn, int AccountNo, out tagTPE_GetAccountExRes pRes);

        /// <summary>
        /// 按中心记帐号查询流水
        /// 注意: 由于流水的量很大,因此查询全部流水应该慎重,否则查询的时间会较长,对中心性能将会有很大于影响.
        /// </summary>
        /// <param name="CustomSn">用户指定的交易包唯一标识</param>
        /// <param name="pReq">pReq,查询条件指针</param>
        /// <param name="pResControl">结果记录控制结构</param>
        /// <param name="bSync">函数的工作方式,1:同步,0:异步</param>
        /// <returns></returns>
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_QueryFlowByCenter")]
        public static extern int TPE_QueryFlowByCenter (int CustomSn, ref tagTPE_QueryFlowByCenterReq pReq,
                                                    out tagTPE_QueryResControl pResControl,
                                                    int bSync);

        /// <summary>
        /// 按中心记帐号查询流水
        /// 注意: 由于流水的量很大,因此查询全部流水应该慎重,否则查询的时间会较长,对中心性能将会有很大于影响.
        /// </summary>
        /// <param name="CustomSn">用户指定的交易包唯一标识</param>
        /// <param name="pReq">pReq,查询条件指针</param>
        /// <param name="pResControl">结果记录控制结构</param>
        /// <param name="bSync">函数的工作方式,1:同步,0:异步</param>
        /// <returns></returns>
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_QueryFlowBySQL")]
        public static extern int TPE_QueryFlowBySQL (int CustomSn, ref tagTPE_QueryFlowBySQLReq pReq,
                                                    out tagTPE_QueryResControl pResControl,
                                                    int bSync);
        /// <summary>
        /// 枚举身份
        /// </summary>
        /// <param name="CustomSn">用户指定的交易包唯一标识</param>
        /// <param name="pReq">枚举身份的申请结构指针</param>
        /// <param name="pResControl">结果记录控制结构</param>
        /// <param name="bSync">函数的工作方式,1:同步,0:异步</param>
        /// <returns></returns>
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_ConfigEnumIdenti")]
        public static extern int TPE_ConfigEnumIdenti (int CustomSn, ref tagTPE_ConfigEnumIdentiReq pReq,
                                                    out tagTPE_QueryResControl pResControl,
                                                    int bSync);
        /// <summary>
        /// 更新帐户信息
        /// </summary>
        /// <param name="CustomSn">用户指定的交易包唯一标识</param>
        /// <param name="pReq">更新基本帐户申请</param>
        /// <param name="pRes">更新基本帐户应答</param>
        /// <param name="bSync">函数的工作方式,1:同步,0:异步</param>
        /// <returns></returns>
        [DllImport (@".\Synjones\TPE.dll", EntryPoint = "TPE_FlowUpdateAccount")]
        public static extern int TPE_FlowUpdateAccount (int CustomSn, ref tagTPE_FlowUpdateAccountReq pReq,
                                                        out tagTPE_FlowUpdateAccountRes pRes, int bSync);



        /*位    0意义    1意义
          0     无效     有效
          1     正常     预撤
          2     正常     冻结
          3     正常     挂失
          4     禁用     启用(自动银行转帐)  
          5     禁用     启用(自助银行转帐)
         16     无效     有效(性别位)
         17     男性     女性 */
        public bool judge1 (byte source, int bit)           //bit 从0--7
        {
            byte[] temp = new byte[8] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };

            byte NewValue = (byte)(source & temp[bit]);
            if (NewValue != 0) {
                return true;
            } else {
                return false;
            }

            /*        byte Temp1, Temp2;

                    if(bit>7)
                    {
                        return false;
                    }
                    Temp1 = (byte)(temp>>(7-bit));
                    Temp2 = (byte)1;

                    if(Temp2 & Temp2 ==1)
                        return true;
                    else
                        return false;*/

        }

    }

}
