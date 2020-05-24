using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace WebServicePark
{
    /// <summary>
    /// 返回包Base
    /// </summary>
    public class CReturnObj
    {
        public string Result = ""; //OK ERROR
        public string Msg = "";
    }

    /// <summary>
    /// 基本帐户调帐应答返回包
    /// </summary>
    public class CReturnGetAccountRes : CReturnObj
    {
        public TPE_GetAccountRes Data;
        public List<TPE_GetAccountRes> ListDate;
    }

    /// <summary>
    /// 流水调账常用返回包
    /// </summary>
    public class CReturnCReturnObj : CReturnObj
    {
        public TPE_CReturnObj Data;
        public List<TPE_CReturnObj> ListDate;
    }

    /// <summary>
    /// 部门记录返回包
    /// </summary>
    public class CReturnConfigDeptRec : CReturnObj
    {
        public TPE_ConfigDeptRec Data;
        public List<TPE_ConfigDeptRec> ListDate;
    }

    /// <summary>
    /// 枚举身份返回包
    /// </summary>
    public class CReturnConfigIdentiRec : CReturnObj
    {
        public TPE_ConfigIdentiRec Data;
        public List<TPE_ConfigIdentiRec> ListDate;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CReturnFlowRes : CReturnObj
    {
        public TPE_FlowRes Data;
        public List<TPE_FlowRes> ListDate;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CReturnFlowCostRes : CReturnObj
    {
        public TPE_FlowCostRes Data;
        public List<TPE_FlowCostRes> ListDate;
    }

    /// <summary>
    /// 入帐应答返回包
    /// </summary>
    public class CReturnFlowUpdateAccountRes : CReturnObj
    {
        public TPE_FlowUpdateAccountRes Data;
        public List<TPE_FlowUpdateAccountRes> ListDate;
    }

    /// <summary>
    /// 用户自定义字段
    /// </summary>
    public class CReturnGetAccountExRes : CReturnObj
    {
        public TPE_GetAccountExRes Data;
        public List<TPE_GetAccountExRes> ListDate;
    }

    /// <summary>
    /// 基本帐户调帐应答
    /// </summary>
    public class TPE_GetAccountRes
    {
        public TPE_GetAccountRes() { }

        public TPE_GetAccountRes(tagTPE_GetAccountRes Res)
        {
            AccessControl = CPublic.ByteArrayToStr(Res.AccessControl);
            AccountNo = Res.AccountNo;
            Balance = Res.Balance;
            Birthday = CPublic.ConvertDateTime(Res.Birthday);
            CardNo = Res.CardNo;
            CertCode = CPublic.ByteArrayToStr(Res.CertCode);
            CertType = Res.CertType;
            Comment = CPublic.ByteArrayToStr(Res.Comment);
            Condition = Res.Condition;
            CreateTime = CPublic.ConvertDateTime(Res.CreateTime);
            CreditCardNo = CPublic.ByteArrayToStr(Res.CreditCardNo);
            Depart = Res.Depart;
            Email = CPublic.ByteArrayToStr(Res.Email);
            ExpireDate = CPublic.ConvertDateTime(Res.ExpireDate);
            Extend = CPublic.ByteArrayToStr(Res.Extend);
            ExtendLen = Res.ExtendLen;
            FileNameAudio = CPublic.ByteArrayToStr(Res.FileNameAudio);
            FileNameFinger = CPublic.ByteArrayToStr(Res.FileNameFinger);
            FileNamePicture = CPublic.ByteArrayToStr(Res.FileNamePicture);
            Identi = Res.Identi;
            Name = CPublic.ByteArrayToStr(Res.Name);
            Nation = Res.Nation;
            Password = CPublic.ByteArrayToStr(Res.Password);
            PersonID = CPublic.ByteArrayToStr(Res.PersonID);
            PostalAddr = CPublic.ByteArrayToStr(Res.PostalAddr);
            PostalCode = CPublic.ByteArrayToStr(Res.PostalCode);
            RetValue = Res.RetValue;
            Tel = CPublic.ByteArrayToStr(Res.Tel);
            TransferLimit = Res.TransferLimit;
            TransferMoney = Res.TransferMoney;
            UpdateTime = CPublic.ConvertDateTime(Res.UpdateTime);
        }
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
        public string CreateTime;
        /// <summary>
        /// 有效期
        /// </summary>
        public string ExpireDate;
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name;
        /// <summary>
        /// 身份证
        /// </summary>
        public string PersonID;
        /// <summary>
        /// 密码,密文
        /// </summary>
        public string Password;
        /// <summary>
        /// 访问控制,每一位代表对应的子系统号,对应位=1,则在此系统可以使用 
        /// </summary>
        public string AccessControl;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday;
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
        public string CertCode;
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CreditCardNo;
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
        public string Tel;
        /// <summary>
        /// 电邮
        /// </summary>
        public string Email;
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode;
        /// <summary>
        /// 通信地址
        /// </summary>
        public string PostalAddr;
        /// <summary>
        /// 照片 
        /// </summary>
        public string FileNamePicture;
        /// <summary>
        /// 指纹
        /// </summary>
        public string FileNameFinger;
        /// <summary>
        /// 声音
        /// </summary>
        public string FileNameAudio;
        /// <summary>
        /// 注释 
        /// </summary>
        public string Comment;
        /// <summary>
        /// 扩展长度
        /// </summary>
        public int ExtendLen;
        /// <summary>
        /// 扩展内容
        /// </summary>
        public string Extend;
        /// <summary>
        /// 最后更新日期
        /// </summary>
        public string UpdateTime;
    }

    /// <summary>
    /// 流水调账常用字段
    /// </summary>
    public class TPE_CReturnObj
    {
        public TPE_CReturnObj() { }

        public TPE_CReturnObj(tagTPE_CReturnObj Cro)
        {
            TransType = Cro.TransType;
            CenterNo = Cro.CenterNo;
            OccurNode = Cro.OccurNode;
            OccurIdNo = Cro.OccurIdNo;
            OccurTime = CPublic.ConvertDateTime(Cro.OccurTime);
            AccountNo = Cro.AccountNo;
            CardNo = Cro.CardNo;
            TransMoney = Cro.TransMoney;
            Balance = Cro.Balance;
            Condition = Cro.Condition;
            TransferLimit = Cro.TransferLimit;
            TransferMoney = Cro.TransferMoney;
            JoinNode = Cro.JoinNode;
            JoinCardHolder = CPublic.ByteArrayToStr(Cro.JoinCardHolder);
        }
        //1-开户tagTPE_QueryFlowRes_Open 
        //2-撤户tagTPE_QueryFlowRes_Open 
        //3-建立对应关系tagTPE_QueryFlowRes_BuildRelation 
        //4-撤消对应tagTPE_QueryFlowRes_BuildRelation
        //5-更改帐户信息tagTPE_QueryFlowRes_UpdateAccount
        //6-更改对应关系tagTPE_QueryFlowRes_UpdateRelation 
        //7-19余额变更tagTPE_QueryFlowRes_Cost
        public Int32 TransType;     //类型
        public UInt32 CenterNo;		//中心记帐号
        public UInt32 OccurNode;              //发生节点
        public UInt32 OccurIdNo;              //发生节点流水号
        public string OccurTime;          //发生时间
        public UInt32 AccountNo;	//按帐号建立
        public Int32 CardNo;         //按卡号建立AccountNo=0时
        public Int32 TransMoney;
        public Int32 Balance;
        public UInt32 Condition;
        public Int32 TransferLimit;
        public Int32 TransferMoney;
        public UInt32 JoinNode;
        public string JoinCardHolder;
        public string OrderID;
        public string OrderStatus;
        public short CostType;
    }

    /// <summary>
    /// 部门记录
    /// </summary>
    public class TPE_ConfigDeptRec
    {
        public TPE_ConfigDeptRec() { }

        public TPE_ConfigDeptRec(tagTPE_ConfigDeptRec Cdr)
        {
            DeptNo = Cdr.DeptNo;
            Code = CPublic.ByteArrayToStr(Cdr.Code);
            Name = CPublic.ByteArrayToStr(Cdr.Name);
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public Int64 DeptNo;
        /// <summary>
        /// 行政编号
        /// </summary>
        public string Code;
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name;
    }

    /// <summary>
    /// 枚举身份
    /// </summary>
    public class TPE_ConfigIdentiRec
    {
        public TPE_ConfigIdentiRec() { }

        public TPE_ConfigIdentiRec(tagTPE_ConfigIdentiRec Cir)
        {
            IdentiNo = Cir.IdentiNo;
            Code = CPublic.ByteArrayToStr(Cir.Code);
            Name = CPublic.ByteArrayToStr(Cir.Name);
            OverDraft = Cir.OverDraft;
        }
        /// <summary>
        /// 身份编号
        /// </summary>
        public Int16 IdentiNo;
        /// <summary>
        /// 行政编号
        /// </summary>
        public string Code;
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name;
        /// <summary>
        /// 透支额度
        /// </summary>
        public int OverDraft;
    }

    public class TPE_FlowRes
    {
        public TPE_FlowRes() { }

        public TPE_FlowRes(tagTPE_FlowRes Fr)
        {
            RecordError = Fr.RecordError;
            CenterNo = Fr.RecordError;
            OccurNode = Fr.OccurNode;
            OccurIdNo = Fr.OccurIdNo;
            OccurInfo = Fr.OccurInfo;
        }

        public int RecordError;
        public int CenterNo;
        public int OccurNode;
        public int OccurIdNo;
        public int OccurInfo;
    }

    public class TPE_FlowCostRes
    {
        public TPE_FlowCostRes() { }

        public TPE_FlowCostRes(tagTPE_FlowCostRes Fr)
        {
            RecordError = Fr.RecordError;
            CenterNo = Fr.RecordError;
            OccurNode = Fr.OccurNode;
            OccurIdNo = Fr.OccurIdNo;
            OccurInfo = Fr.OccurInfo;
        }

        public int RecordError;
        public int CenterNo;
        public int OccurNode;
        public int OccurIdNo;
        public int OccurInfo;
    }

    /// <summary>
    /// 入帐应答
    /// </summary>
    public class TPE_FlowUpdateAccountRes
    {
        public TPE_FlowUpdateAccountRes() { }
        public TPE_FlowUpdateAccountRes(tagTPE_FlowUpdateAccountRes Fuar)
        {
            RecordError = Fuar.RecordError;
            CenterNo = Fuar.CenterNo;
            OccurNode = Fuar.OccurNode;
            OccurIdNo = Fuar.OccurIdNo;
            OccurInfo = Fuar.OccurInfo;
        }
        public Int32 RecordError;
        public UInt32 CenterNo;
        public UInt32 OccurNode;
        public UInt32 OccurIdNo;
        public UInt32 OccurInfo;
    }

    /// <summary>
    /// 账户自定义字段
    /// </summary>
    public class TPE_GetAccountExRes
    {
        public TPE_GetAccountExRes() { }
        public TPE_GetAccountExRes(tagTPE_GetAccountExRes GAE)
        {
            RetValue = GAE.RetValue;
            DefINT1 = GAE.DefINT1;
            DefINT2 = GAE.DefINT2;
            DefINT3 = GAE.DefINT3;
            DefINT4 = GAE.DefINT4;
            DefVAR1 = CPublic.ByteArrayToStr(GAE.DefVAR1);
            DefVAR2 = CPublic.ByteArrayToStr(GAE.DefVAR2);
            DefVAR3 = CPublic.ByteArrayToStr(GAE.DefVAR3);
            DefVAR4 = CPublic.ByteArrayToStr(GAE.DefVAR4);
        }
        public int RetValue;  	//-1系统内部错误 -2无此帐户自定义字段记录

        public int DefINT1;
        public int DefINT2;
        public int DefINT3;
        public int DefINT4;
        public string DefVAR1;
        public string DefVAR2;
        public string DefVAR3;
        public string DefVAR4;
    }
}