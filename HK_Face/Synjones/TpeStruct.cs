using System.Runtime.InteropServices;
using System;

namespace WebServicePark {

    #region 入帐更新流水
    ////入帐----更新基本帐户申请
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_FlowUpdateAccountReq {
        public UInt32 OccurIdNo;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;
        public UInt16 TranOper;

        public UInt32 ReqAccountNo;
        public Int32 ReqCardNo;
        public UInt32 JoinNode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;

        public byte reqflagCardNo;
        public byte reqflagCondition;
        public byte reqflagExpireDate;
        public byte reqflagName;
        public byte reqflagPersonID;
        public byte reqflagPassword;
        public byte reqflagAccessControl;
        public byte reqflagBirthday;
        public byte reqflagDepart;
        public byte reqflagIdenti;
        public byte reqflagNation;
        public byte reqflagCertType;
        public byte reqflagCertCode;
        public byte reqflagCreditCardNo;
        public byte reqflagTransferLimit;
        public byte reqflagTransferMoney;
        public byte reqflagTel;
        public byte reqflagEmail;
        public byte reqflagPostalCode;
        public byte reqflagPostalAddr;
        public byte reqflagComment;
        public byte reqflagExtend;

        public UInt32 CardNo;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt32[] Condition;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] ExpireDate;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] PersonID;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Password;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] AccessControl;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] Birthday;

        public Int64 Depart;
        public Int16 Identi;
        public Int16 Nation;
        public byte CertType;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] CertCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]

        public byte[] CreditCardNo;
        public Int32 TransferLimit;
        public Int32 TransferMoney;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Tel;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Email;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] PostalCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] PostalAddr;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] Comment;
        public Int32 ExtendLen;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 255)]
        public byte[] Extend;
    }

    /// <summary>
    /// 入帐应答
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_FlowUpdateAccountRes {
        public Int32 RecordError;
        public UInt32 CenterNo;
        public UInt32 OccurNode;
        public UInt32 OccurIdNo;
        public UInt32 OccurInfo;
    }

    #endregion


    #region 查询流水结构
    /// <summary>
    /// 余额变更
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryFlowRes_Cost {
        public UInt32 CenterNo;     //中心记帐号
        public UInt32 OccurNode;              //发生节点
        public UInt32 OccurIdNo;              //发生节点流水号

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;          //发生时间
        public Int16 TranOper;               //操作员编号

        public UInt32 AccountNo;    //按帐号建立
        public Int32 CardNo;         //按卡号建立AccountNo=0时
        public UInt32 JoinNode;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;

        public Int16 CostType;
        public Int32 TransMoney;
        public Int32 Balance;

        public UInt32 LinkOccurNode;
        public UInt32 LinkOccurIdNo;

        public UInt32 ExtendLen;
    }; //余额变更时如果ExtendLen不为0,则该结构后跟ExtendLen长的数据

    /// <summary>
    /// 开户流水,撤户流水
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryFlowRes_Open {
        public UInt32 CenterNo;     //中心记帐号
        public UInt32 OccurNode;              //发生节点
        public UInt32 OccurIdNo;              //发生节点流水号

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;          //发生时间
        public Int16 TranOper;               //操作员编号

        #region 通过
        //标志
        public Byte resflagCondition; //状态
        public Byte resflagBalance;   //余额
        public Byte resflagCreateTime;//开户时间
        public Byte resflagExpireDate;//有效期
        public Byte resflagName;
        public Byte resflagPersonID;
        public Byte resflagPassword;
        public Byte resflagAccessControl;
        public Byte resflagBirthday;
        public Byte resflagDepart;
        public Byte resflagIdenti;
        public Byte resflagNation;
        public Byte resflagCertType;
        public Byte resflagCertCode;
        public Byte resflagCreditCardNo;
        public Byte resflagTransferLimit;
        public Byte resflagTransferMoney;
        public Byte resflagTel;
        public Byte resflagEmail;
        public Byte resflagPostalCode;
        public Byte resflagPostalAddr;
        public Byte resflagComment;
        public Byte resflagExtend;
        public Byte resflagUpdateTime;
        #endregion


        public UInt32 AccountNo;
        public Int32 CardNo;
        public UInt32 Condition;
        public Int32 Balance;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] CreateTime;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] ExpireDate;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] PersonID;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] Password;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public Byte[] AccessControl;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] Birthday;

        public Int64 Depart;
        public Int16 Identi;
        public Int16 Nation;
        public byte CertType;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] CertCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] CreditCardNo;

        public Int32 TransferLimit;
        public Int32 TransferMoney;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Tel;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Email;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] PostalCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] PostalAddr;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] Comment;

        public int ExtendLen;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 255)]
        public Byte[] Extend;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] UpdateTime;
    };

    ////更新帐户
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryFlowRes_UpdateAccount {
        public UInt32 CenterNo;     //中心记帐号
        public UInt32 OccurNode;              //发生节点
        public UInt32 OccurIdNo;              //发生节点流水号

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;          //发生时间
        public UInt16 TranOper;               //操作员编号

        #region 通过
        //标志
        public Byte resflagCondition; //状态
        public Byte resflagCreateTime;//开户时间
        public Byte resflagExpireDate;//有效期
        public Byte resflagName;
        public Byte resflagPersonID;
        public Byte resflagPassword;
        public Byte resflagAccessControl;
        public Byte resflagBirthday;
        public Byte resflagDepart;
        public Byte resflagIdenti;
        public Byte resflagNation;
        public Byte resflagCertType;
        public Byte resflagCertCode;
        public Byte resflagCreditCardNo;
        public Byte resflagTransferLimit;
        public Byte resflagTransferMoney;
        public Byte resflagTel;
        public Byte resflagEmail;
        public Byte resflagPostalCode;
        public Byte resflagPostalAddr;
        public Byte resflagComment;
        public Byte resflagExtend;
        public Byte resflagUpdateTime;
        #endregion


        public UInt32 AccountNo;
        public Int32 CardNo;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt32[] Condition;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] CreateTime;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] ExpireDate;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public Byte[] PersonID;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] Password;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public Byte[] AccessControl;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public Byte[] Birthday;

        public UInt64 Depart;
        public Int16 Identi;
        public Int16 Nation;
        public byte CertType;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] CertCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        byte[] CreditCardNo;

        public Int32 TransferLimit;
        public Int32 TransferMoney;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Tel;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Email;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] PostalCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] PostalAddr;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] Comment;

        public int ExtendLen;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 255)]
        public Byte[] Extend;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] UpdateTime;
    };


    /// <summary>
    /// 建立对应关系
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryFlowRes_BuildRelation {
        public UInt32 CenterNo;     //中心记帐号
        public UInt32 OccurNode;              //发生节点
        public UInt32 OccurIdNo;              //发生节点流水号

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;          //发生时间
        public Int16 TranOper;               //操作员编号



        public UInt32 AccountNo;    //按帐号建立
        public Int32 CardNo;         //按卡号建立AccountNo=0时
        public UInt32 JoinNode;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;

        public Byte resflagJoinPassword;
        public Byte resflagJoinCondition;
        public Byte resflagJoinComment;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] JoinPassword;

        public UInt32 JoinCondition;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] JoinComment;
    };


    /// <summary>
    /// 更新对应关系,撤消对应关系
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryFlowRes_UpdateRelation {
        public UInt32 CenterNo;     //中心记帐号
        public UInt32 OccurNode;              //发生节点
        public UInt32 OccurIdNo;              //发生节点流水号

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;          //发生时间
        public Int16 TranOper;               //操作员编号


        public UInt32 AccountNo;    //按帐号建立
        public Int32 CardNo;         //按卡号建立AccountNo=0时
        public UInt32 JoinNode;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;

        public Byte resflagJoinPassword;
        public Byte resflagJoinCondition;
        public Byte resflagJoinComment;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] JoinPassword;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt32[] JoinCondition;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public Byte[] JoinComment;
    };



    #endregion

    /// <summary>
    /// TPE_Class 的摘要说明
    /// </summary>
    /// 
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagMhjTest {
        public int RetValue;

        public int AccountNo;
        public int CardNo;
        public int Condition;
        public int Balance;
        /*    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
             public char[] CreateTime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public char[] ExpireDate;*/
    }

    public struct tagTPE_CheckPassword {
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte PlainPassword;//明文
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte CypherPassword;//密文    
    }

    /// <summary>
    /// 按中心记帐号查询流水申请结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryFlowByCenterReq {
        /// <summary>
        /// =1全部(由TPE自动修改条件获得全部结果
        /// =0 部分满足条件的记录,报文包限制
        /// </summary>
        public Byte ReqFlag;
        /// <summary>
        /// 中心起始记帐号
        /// </summary>
        public UInt32 FromCentralNo;
        /// <summary>
        /// 中心截至记帐号,闭区间
        /// </summary>
        public UInt32 ToCentralNo;


        //条件标志
        /// <summary>
        /// 帐号
        /// </summary>
        public Byte reqflagAccountNo;
        /// <summary>
        /// 卡号
        /// </summary>
        public Byte reqflagCardNo;
        /// <summary>
        /// 子系统
        /// </summary>
        public Byte reqflagJoin;
        /// <summary>
        /// 子系统(对应关系)
        /// </summary>
        public Byte reqflagOccurNode;
        /// <summary>
        /// 交易类型
        /// </summary>
        public Byte reqflagTransType;
        /// <summary>
        /// 发生时间
        /// </summary>
        public Byte reqflagRangeOccurTime;
        /// <summary>
        /// 帐号
        /// </summary>
        public UInt32 AccountNo;
        /// <summary>
        /// 卡号
        /// </summary>
        public UInt32 CardNo;
        /// <summary>
        /// 子系统
        /// </summary>
        public UInt32 JoinNode;
        /// <summary>
        /// 子系统内帐号编号
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;
        /// <summary>
        /// 发生节点(每一字节表示一个节点,1:有效)
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] OccurNode;
        /// <summary>
        /// 流水类型(每一字节0/1表示对应流水类型
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 32)]
        public UInt32[] TransType;
        /// <summary>
        /// 发生时间范围.
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] RangeOccurTime;
    };

    /// <summary>
    /// 解析状态结构
    /// </summary>
    public struct tagTPE_ParseCondition {
        /// <summary>
        /// 0位:  0无效     1有效
        /// </summary>
        public int IsExpire;
        /// <summary>
        ///  1位:  0正常    1预撤
        /// </summary>
        public int IsOff;
        /// <summary>
        /// 2位:  0正常     1冻结
        /// </summary>
        public int IsLock;
        /// <summary>
        ///  3位:  0正常     1挂失
        /// </summary>
        public int IsLost;
        /// <summary>
        /// 4位:  0禁用     1启用(自动银行转帐)  
        /// </summary>
        public int IsAutoTran;
        /// <summary>
        /// 5位:  0禁用     1启用(自助银行转帐)
        /// </summary>
        public int IsSelfTran;
        /// <summary>
        /// 6位:  0无效     1有效(性别位)
        /// </summary>
        public int IsSexBit;
        /// <summary>
        /// 7位:  0男性     1女性
        /// </summary>
        public int IsFemale;
    }

    /// <summary>
    /// 白名单通知结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagWhiteListCallBack {
        /// <summary>
        /// 帐号
        /// </summary>
        public int AccountNo;
        /// <summary>
        /// 卡号
        /// </summary>
        public int CardNo;
        /// <summary>
        /// 状态
        /// </summary>
        public int Condition;
        /// <summary>
        /// 余额
        /// </summary>
        public int Balance;
        /// <summary>
        /// 部门
        /// </summary>
        public Int64 Depart;
        /// <summary>
        /// 身份
        /// </summary>
        public Int16 Identi;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_FlowCostReq {
        public int OccurIdNo;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;
        public Int16 TranOper;

        public Int16 CostType;

        public int AccountNo;
        public int CardNo;
        public int JoinNode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;

        public int TransMoney;

        public int LinkOccurNode;   //只有FLOW_TYPE_BALANCEEXTRA时有效
        public int LinkOccurIdNo;

        public int ExtraInfoLen;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 1200)]
        public byte[] ExtraInfo;

    }


    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagWhiteListRec {
        public int AccountNo;
        public int CardNo;
        public int Condition;
        public int Balance;
        public Int64 Depart;
        public Int16 Identi;
        public Byte Sign;

    }

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagNodeNotifyInfo {
        public int CurrentCentralNo;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagNodeNotifyInfoRes {
        public int NotiyfWait;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_GetRelationReq {
        public int JoinNode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;

        public char reqflagJoinPassword;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] JoinPassword;

        //返回标志
        public char resflagAccountNo;
        public char resflagCardNo;
        public char resflagCondition;
        public char resflagBalance;
        public char resflagCreateTime;
        public char resflagExpireDate;
        public char resflagName;
        public char resflagPersonID;
        public char resflagPassword;
        public char resflagAccessControl;
        public char resflagBirthday;
        public char resflagDepart;
        public char resflagIdenti;
        public char resflagNation;
        public char resflagCertType;
        public char resflagCertCode;
        public char resflagCreditCardNo;
        public char resflagTransferLimit;
        public char resflagTransferMoney;
        public char resflagTel;
        public char resflagEmail;
        public char resflagPostalCode;
        public char resflagPostalAddr;
        public char resflagFile;
        public char resflagComment;
        public char resflagExtend;
        public char resflagUpdateTime;

        public char resflagJoinIndex;
        public char resflagJoinNode;
        public char resflagJoinCardHolder;
        public char resflagJoinPassword;
        public char resflagJoinCondition;
        public char resflagJoinComment;
        public char resflagJoinUpdateTime;
    };
    //对应关系调帐应答
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_GetRelationRes {
        public int RetValue;

        public int AccountNo;
        public int CardNo;
        public int Condition;
        public int Balance;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] CreateTime;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] ExpireDate;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] PersonID;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Password;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] AccessControl;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] Birthday;

        public Int64 Depart;
        public Int16 Identi;
        public Int16 Nation;
        public Byte CertType;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] CertCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] CreditCardNo;

        public int TransferLimit;
        public int TransferMoney;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Tel;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Email;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] PostalCode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] PostalAddr;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileNamePicture;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileNameFinger;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileNameAudio;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] Comment;

        public int ExtendLen;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 255)]
        public byte[] Extend;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] UpdateTime;


        public int JoinIndex;
        public int JoinNode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] JoinPassword;


        public int JoinCondition;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] JoinComment;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] JoinUpdateTime;

    };

    /// <summary>
    /// 查询帐户条件结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryStdAccountReq {
        #region 查询条件有效标志

        /// <summary>
        /// 帐号范围
        /// </summary>
        public Byte reqflagAccountNoRange;
        /// <summary>
        /// 状态
        /// </summary>
        public Byte reqflagCondition;
        /// <summary>
        /// 余额范围
        /// </summary>
        public Byte reqflagBalanceRange;
        /// <summary>
        /// 开户日期范围
        /// </summary>
        public Byte reqflagCreateTimeRange;
        /// <summary>
        /// 有下棋范围
        /// </summary>
        public Byte reqflagExpireDateRange;
        /// <summary>
        /// 姓名
        /// </summary>
        public Byte reqflagName;
        /// <summary>
        /// 身份证号 
        /// </summary>
        public Byte reqflagPersonID;
        /// <summary>
        /// 出生日期范围
        /// </summary>
        public Byte reqflagBirthdayRange;
        /// <summary>
        /// 部门编号
        /// </summary>
        public Byte reqflagDepart;
        /// <summary>
        /// 身份编号
        /// </summary>
        public Byte reqflagIdenti;
        /// <summary>
        /// 民族国籍
        /// </summary>
        public Byte reqflagNation;
        /// <summary>
        /// 电话
        /// </summary>
        public Byte reqflagTel;
        /// <summary>
        /// 电邮
        /// </summary>
        public Byte reqflagEmail;

        #endregion

        #region 查询条件

        /// <summary>
        /// 帐号范围,边界有效,闭区间
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] AccountNoRange;
        /// <summary>
        /// 状态,Condition[0]值,Condition[1]为掩码,表示Condition[0]内的有效位是哪些
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] Condition;
        /// <summary>
        /// 余额范围,闭区间
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] BalanceRange;
        /// <summary>
        /// 开户日期范围
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] CreateTimeRange;
        /// <summary>
        /// 有效期范围
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] ExpireDateRange;
        /// <summary>
        /// 姓名
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;
        /// <summary>
        /// 身份证
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] PersonID;
        /// <summary>
        /// 出生日期范围
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] BirthdayRange;
        /// <summary>
        /// 部门
        /// </summary>
        public Int64 Depart;
        /// <summary>
        /// 身份
        /// </summary>
        public Int16 Identi;
        /// <summary>
        /// 民族国籍
        /// </summary>
        public Int16 Nation;
        /// <summary>
        /// 电话
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Tel;
        /// <summary>
        /// 电邮
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Email;

        #endregion

        #region 返回标志

        /// <summary>
        ///卡号
        /// </summary>
        public Byte resflagCardNo;
        /// <summary>
        /// 状态
        /// </summary>
        public Byte resflagCondition;
        /// <summary>
        /// 余额
        /// </summary>
        public Byte resflagBalance;
        /// <summary>
        /// 开户时间
        /// </summary>
        public Byte resflagCreateTime;
        /// <summary>
        /// 有效期
        /// </summary>
        public Byte resflagExpireDate;
        /// <summary>
        /// 姓名
        /// </summary>
        public Byte resflagName;
        /// <summary>
        /// 身份证号
        /// </summary>
        public Byte resflagPersonID;
        /// <summary>
        /// 密码 
        /// </summary>
        public Byte resflagPassword;
        /// <summary>
        /// 访问控制
        /// </summary>
        public Byte resflagAccessControl;
        /// <summary>
        /// 出生日期
        /// </summary>
        public Byte resflagBirthday;
        /// <summary>
        /// 部门
        /// </summary>
        public Byte resflagDepart;
        /// <summary>
        /// 身份
        /// </summary>
        public Byte resflagIdenti;
        /// <summary>
        /// 民族国籍
        /// </summary>
        public Byte resflagNation;
        /// <summary>
        /// 证件类型
        /// </summary>
        public Byte resflagCertType;
        /// <summary>
        /// 证件号码
        /// </summary>
        public Byte resflagCertCode;
        /// <summary>
        /// 银行卡号
        /// </summary>
        public Byte resflagCreditCardNo;
        /// <summary>
        /// 转帐限额 
        /// </summary>
        public Byte resflagTransferLimit;
        /// <summary>
        /// 转帐金额
        /// </summary>
        public Byte resflagTransferMoney;
        /// <summary>
        /// 电话
        /// </summary>
        public Byte resflagTel;
        /// <summary>
        /// 电邮
        /// </summary>
        public Byte resflagEmail;
        /// <summary>
        /// 邮政编码
        /// </summary>
        public Byte resflagPostalCode;
        /// <summary>
        /// 通信地址
        /// </summary>
        public Byte resflagPostalAddr;
        /// <summary>
        /// 文件;
        /// </summary>
        public Byte resflagFile;
        /// <summary>
        /// 注释
        /// </summary>
        public Byte resflagComment;
        /// <summary>
        /// 扩展
        /// </summary>
        public Byte resflagExtend;
        /// <summary>
        /// 最后更新日期
        /// </summary>
        public Byte resflagUpdateTime;

        #endregion

    };

    /// <summary>
    /// 结果记录控制结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe public struct tagTPE_QueryResControl {
        /// <summary>
        /// 返回的记录数.为0表示没有符合条件的记录.
        /// </summary>
        public int ResRecCount;
        /// <summary>
        /// 记录数据的地址,ResRecCount=0,则此字段=NULL
        /// </summary>
        public void* pRes;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryGeneralAccountReq {
        [MarshalAs (UnmanagedType.ByValTStr, SizeConst = 4096)]
        public String SQL;
        //返回标志
        public Byte resflagCardNo;
        public Byte resflagCondition;
        public Byte resflagBalance;
        public Byte resflagCreateTime;
        public Byte resflagExpireDate;
        public Byte resflagName;
        public Byte resflagPersonID;
        public Byte resflagBirthday;
        public Byte resflagDepart;
        public Byte resflagIdenti;
        public Byte resflagNation;
        public Byte resflagCertType;
        public Byte resflagCertCode;
        public Byte resflagCreditCardNo;
        public Byte resflagTransferLimit;
        public Byte resflagTransferMoney;
        public Byte resflagTel;
        public Byte resflagEmail;
        public Byte resflagPostalCode;
        public Byte resflagPostalAddr;
        public Byte resflagFile;
        public Byte resflagComment;
        public Byte resflagExtend;
        public Byte resflagUpdateTime;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_OnLineGetMaxSnRes {
        public int RetValue;
        public int MaxSn;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_FlowCostRes {
        public int RecordError;
        public int CenterNo;
        public int OccurNode;
        public int OccurIdNo;
        public int OccurInfo;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_GetAccountReq {
        //按帐号或卡号
        public uint AccountNo;
        public int CardNo;

        //密码匹配选项
        public char reqflagPassword;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Password;
        //返回字段标志选项
        public Byte resflagAccountNo;
        public Byte resflagCardNo;
        public Byte resflagCondition;
        public Byte resflagBalance;
        public Byte resflagCreateTime;
        public Byte resflagExpireDate;
        public Byte resflagName;
        public Byte resflagPersonID;
        public Byte resflagPassword;
        public Byte resflagAccessControl;
        public Byte resflagBirthday;
        public Byte resflagDepart;
        public Byte resflagIdenti;
        public Byte resflagNation;
        public Byte resflagCertType;
        public Byte resflagCertCode;
        public Byte resflagCreditCardNo;
        public Byte resflagTransferLimit;
        public Byte resflagTransferMoney;
        public Byte resflagTel;
        public Byte resflagEmail;
        public Byte resflagPostalCode;
        public Byte resflagPostalAddr;
        public Byte resflagFile;
        public Byte resflagComment;
        public Byte resflagExtend;
        public Byte resflagUpdateTime;
    };

    /// <summary>
    /// 基本帐户调帐应答结构体
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_GetAccountRes {
        /// <summary>
        /// 一卡通中心返回代码
        /// </summary>
        public int RetValue;

        /// <summary>
        /// 帐号
        /// </summary>
        public int AccountNo;
        /// <summary>
        /// 卡号
        /// </summary>
        public int CardNo;
        /// <summary>
        /// 状态
        /// </summary>
        public uint Condition;
        /// <summary>
        /// 余额
        /// </summary>
        public int Balance;
        /// <summary>
        /// 开户时间
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] CreateTime;
        /// <summary>
        /// 有效期
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] ExpireDate;
        /// <summary>
        /// 姓名
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;
        /// <summary>
        /// 身份证
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] PersonID;
        /// <summary>
        /// 密码,密文
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Password;
        /// <summary>
        /// 访问控制,每一位代表对应的子系统号,对应位=1,则在此系统可以使用 
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] AccessControl;
        /// <summary>
        /// 出生日期
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] Birthday;
        /// <summary>
        /// 部门编号
        /// </summary>
        public Int64 Depart;
        /// <summary>
        /// 身份编号
        /// </summary>
        public Int16 Identi;
        /// <summary>
        /// 民族伙计编号
        /// </summary>
        public Int16 Nation;
        /// <summary>
        /// 证件类型编号
        /// </summary>
        public Byte CertType;
        /// <summary>
        /// 证件号码
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] CertCode;
        /// <summary>
        /// 银行卡号
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] CreditCardNo;
        /// <summary>
        /// 转帐限额
        /// </summary>
        public int TransferLimit;
        /// <summary>
        /// 转帐金额
        /// </summary>
        public int TransferMoney;
        /// <summary>
        /// 电话
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Tel;
        /// <summary>
        /// 电邮
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Email;
        /// <summary>
        /// 邮政编码
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] PostalCode;
        /// <summary>
        /// 通信地址
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] PostalAddr;
        /// <summary>
        /// 照片 
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileNamePicture;
        /// <summary>
        /// 指纹
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileNameFinger;
        /// <summary>
        /// 声音
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileNameAudio;
        /// <summary>
        /// 注释 
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] Comment;
        /// <summary>
        /// 扩展长度
        /// </summary>
        public int ExtendLen;
        /// <summary>
        /// 扩展内容
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 255)]
        public byte[] Extend;
        /// <summary>
        /// 最后更新日期
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] UpdateTime;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_FileInfo {
        public int RetValue;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] LicFileName;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] IniFileName;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_LostReq {
        public int OccurIdNo;
        [MarshalAs (UnmanagedType.ByValTStr, SizeConst = 14)]
        public String OccurTime;
        public Int16 TranOper;

        public int ReqAccountNo;
        public int ReqCardNo;
        public int JoinNode;
        [MarshalAs (UnmanagedType.ByValTStr, SizeConst = 24)]
        public String JoinCardHolder;
        public byte Operation;       //1 挂失, 2 解挂
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_FlowRes {
        public int RecordError;
        public int CenterNo;
        public int OccurNode;
        public int OccurIdNo;
        public int OccurInfo;
    };

    /// <summary>
    /// 枚举民族的申请结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_ConfigEnumNationReq {
        /// <summary>
        /// 民族编号范围
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public Int16[] RangeNationNo;
    };

    /// <summary>
    /// 枚举民族的应答结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_ConfigNationRec {
        /// <summary>
        /// 编号 
        /// </summary>
        public Int16 NationNo;
        /// <summary>
        ///  行政编号
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Code;
        /// <summary>
        /// 姓名
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Name;
    };
    /// <summary>
    /// 枚举身份的申请结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_ConfigEnumIdentiReq {
        /// <summary>
        /// 身份编号范围
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public Int16[] RangeIdenti;
    };

    /// <summary>
    /// 枚举身份的应答结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_ConfigIdentiRec {
        /// <summary>
        /// 身份编号
        /// </summary>
        public Int16 IdentiNo;
        /// <summary>
        /// 行政编号
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Code;
        /// <summary>
        /// 姓名
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Name;
        /// <summary>
        /// 透支额度
        /// </summary>
        public int OverDraft;
    };

    /// <summary>
    /// 枚举部门的申请结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_ConfigEnumDeptReq {
        /// <summary>
        /// 部门编号范围
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt64[] RangeDept;
        /// <summary>
        /// 深度,即到几级 
        /// </summary>
        public byte Depth;
    };

    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_QueryFlowBySQLReq {
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 4096)]
        public byte[] SQL;
    };

    /// <summary>
    /// 部门记录应答结构
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_ConfigDeptRec {
        /// <summary>
        /// 部门编号
        /// </summary>
        public Int64 DeptNo;
        /// <summary>
        /// 行政编号
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Code;
        /// <summary>
        /// 部门名称
        /// </summary>
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Name;
    };


    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_CReturnObj {
        //1-开户tagTPE_QueryFlowRes_Open 2-撤户tagTPE_QueryFlowRes_Open 3-建立对应关系tagTPE_QueryFlowRes_BuildRelation 4-撤消对应tagTPE_QueryFlowRes_BuildRelation 5-更改帐户信息tagTPE_QueryFlowRes_UpdateAccount 6-更改对应关系tagTPE_QueryFlowRes_UpdateRelation 7-19余额变更tagTPE_QueryFlowRes_Cost
        public Int32 TransType;     //类型
        public UInt32 CenterNo;     //中心记帐号
        public UInt32 OccurNode;              //发生节点
        public UInt32 OccurIdNo;              //发生节点流水号
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] OccurTime;          //发生时间

        public UInt32 AccountNo;    //按帐号建立
        public Int32 CardNo;         //按卡号建立AccountNo=0时

        public Int32 TransMoney;
        public Int32 Balance;

        public UInt32 Condition;
        public Int32 TransferLimit;
        public Int32 TransferMoney;

        public UInt32 JoinNode;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] JoinCardHolder;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] OrderID;
    };

    //查询帐户自定义字段应答结构
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct tagTPE_GetAccountExRes {
        public int RetValue;    //-1系统内部错误 -2无此帐户自定义字段记录

        public int DefINT1;
        public int DefINT2;
        public int DefINT3;
        public int DefINT4;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] DefVAR1;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] DefVAR2;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] DefVAR3;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] DefVAR4;
    };

    //流水中海通扩展信息
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public struct HTEXTENDINFO {
        public int DataLen;                                     //长度
        public UInt32 NodeType;                                 //定为0x00030001
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]   //订单编号
        public byte[] OrderID;
        public UInt32 Node;                                     //对应节点号
        public byte BigGroup;                                   //对应大组号
        public byte litGroup;                                   //对应小组号
        public byte Segment;                                    //对应班次号
        public byte POS;                                        //对应POS号
        public int RecordCheck;                                 //校验
    };
}