/*
 *  SRA加密处理类，产生私钥公钥、加密解密方法
 *  时间：2016年8月17日19:25:54
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSA_Test
{
    /// <summary>
    /// SRA加密处理类
    /// </summary>
   public class RSA_Producer
    {
       RSACryptoServiceProvider myrsa;
       public RSA_Producer(RSACryptoServiceProvider rsa)
       {
           this.myrsa = rsa;
       }
        /// <summary>
        /// 产生私钥公钥
        /// </summary>
        /// <param name="public_key"></param>
        /// <param name="private_key"></param>
        public void produceKey(out string public_key, out string private_key)
        {
            public_key = myrsa.ToXmlString(false);
            private_key = myrsa.ToXmlString(true);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="public_key">公钥</param>
        /// <param name="text">字符串</param>
        /// <returns>加密字节数组</returns>
        public byte[] EncyptText(string public_key,string text)
        {
            byte[] origin_bytes = (new UnicodeEncoding()).GetBytes(text);
            myrsa.FromXmlString(public_key);
            byte[] encypt_bytes = myrsa.Encrypt(origin_bytes, false);
            return encypt_bytes;
        }

        /// <summary>
        /// 加密字节数组
        /// </summary>
        /// <param name="public_key">公钥</param>
        /// <param name="bytes">原始字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public byte[] Encypt(string public_key,byte[] bytes)
        {
            myrsa.FromXmlString(public_key);
            byte[] encypt_bytes = myrsa.Encrypt(bytes, false);
            return encypt_bytes;
        }


        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="private_key">私钥</param>
        /// <param name="encypt_bytes">字符串</param>
        /// <returns>解密字符串</returns>
        public string DecyptText(string private_key,byte[] encypt_bytes)
        {
            myrsa.FromXmlString(private_key);
            byte[] origin_bytes = myrsa.Decrypt(encypt_bytes, false);
            string text = (new UnicodeEncoding()).GetString(origin_bytes);
            return text;
        }

        /// <summary>
        /// 解密字节数组
        /// </summary>
        /// <param name="private_key">私钥</param>
        /// <param name="encypt_bytes">字符串</param>
        /// <returns>解密字节数组</returns>
        public byte[] Decypt(string private_key, byte[] encypt_bytes)
        {
            myrsa.FromXmlString(private_key);
            byte[] origin_bytes = myrsa.Decrypt(encypt_bytes, false);
            return origin_bytes;
        }
    }
}
