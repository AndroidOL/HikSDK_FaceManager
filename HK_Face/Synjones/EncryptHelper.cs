using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace WebServicePark
{
    public class EncryptHelper
    {
        public string DesEncrypt(string strPlain, string strKey)
        {
            try
            {
                CDES des = new CDES();
                byte[] key = Encoding.UTF8.GetBytes(strKey);
                byte[] plainArray = Encoding.UTF8.GetBytes(strPlain);
                byte[] cipherArray = des.Encrypt(plainArray, 0, plainArray.Length, key);
                //return Encoding.UTF8.GetString(cipherArray);
                return Convert.ToBase64String(cipherArray);
            }
            catch(Exception e)
            {
                CPublic.WriteLog("DES 加密过程异常:" + e.Message);
                return null;
            }
        }
        public string DesDecrypt(string strCipher, string strKey)
        {
            try
            {
                CDES des = new CDES();
                byte[] key = Encoding.UTF8.GetBytes(strKey);
                byte[] cipherArray = Convert.FromBase64String(strCipher);
                //byte[] cipherArray = Encoding.UTF8.GetBytes(strCipher);
                byte[] plainArray = des.Decrypt(cipherArray, 0, cipherArray.Length, key);
                return Encoding.UTF8.GetString(plainArray);
            }
            catch(Exception e)
            {
                CPublic.WriteLog("DES 解密过程异常:" + e.Message);
                return null;
            }
        }

        public string MD5(string strPlain)
        {
            try
            {
                CMD5 md5 = new CMD5();
                byte[] plainArray = Encoding.UTF8.GetBytes(strPlain);
                byte[] mac = md5.MAC(plainArray);
                return Convert.ToBase64String(mac);
            }
            catch (Exception e)
            {
                CPublic.WriteLog("MD5 签名过程异常:" + e.Message);
                return null;
            }
        }
    }

    public class CDES
    {
        public CDES()
        {
        }

        public byte[] Encrypt(byte[] plain, int offset, int length, byte[] key)
        {
            byte[] cipher = null;
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Mode = CipherMode.ECB;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(key, provider.IV), CryptoStreamMode.Write))
                {
                    cs.Write(plain, offset, length);
                    cs.FlushFinalBlock();
                    cipher = ms.ToArray();
                }
            }

            return cipher;
        }

        public byte[] Decrypt(byte[] cipher, int offset, int length, byte[] key)
        {
            byte[] plain = null;
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Mode = CipherMode.ECB;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(key, provider.IV), CryptoStreamMode.Write))
                {
                    cs.Write(cipher, offset, length);
                    cs.FlushFinalBlock();
                    plain = ms.ToArray();
                }
            }

            return plain;
        }
    }
    public class CMD5
    {
        public CMD5()
        {

        }
        public byte[] MAC(byte[] plain)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            provider.Initialize();
            provider.TransformFinalBlock(plain, 0, plain.Length);
            return provider.Hash;
        }
    }
}
