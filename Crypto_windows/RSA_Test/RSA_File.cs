/*
 *  文件处理类，负责读取文件获得二进制文件流，并进行加密并写入
 *  时间：2016年8月12日10:24:43
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RSA_Test
{
    /// <summary>
    /// RSA文件处理类
    /// </summary>
   public class RSA_File
    {
       RSA_Producer rsa_producer;
       RSACryptoServiceProvider mysra;
       //加密数据处理大小
       int origin_byte_counts;
       //文件路径
       string path;
       //文件名标记符号，用于文件名称加密识别
       string name_symbol = "$";
       //文件名称判别字节大小
       int byte_name_counts = 100;
       //设置普通数组字节
       int byte_size = 2;
       //设置一次解压缩字节读取量
       int decypt_file_counts=128;
       //最大字节流量
       int max_bytes = 1024 * 1024;
       string file_name;
       string parent_path;
       public RSA_File()
       {
           mysra = new RSACryptoServiceProvider();
           rsa_producer = new RSA_Producer(mysra);
       }

       /// <summary>
       /// 设置加密文件原路径名称
       /// </summary>
       /// <param name="path"></param>
       public void SetPath(string path,int origin_byte_counts)
       {
           this.origin_byte_counts = origin_byte_counts;
           this.path = path;
           this.file_name = Path.GetFileName(path);
           this.parent_path = Path.GetDirectoryName(path);
       }

       /// <summary>
       /// 产生私钥公钥
       /// </summary>
       /// <param name="public_key"></param>
       /// <param name="private_key"></param>
      public void produce_key(out string public_key,out string private_key)
       {
           rsa_producer.produceKey(out public_key, out private_key);
       }

       /// <summary>
       /// 加密文件
       /// </summary>
       /// <param name="encypt_name">加密后文件名称</param>
       /// <param name="public_key">公钥</param>
       /// <param name="is_total">是否全部加密,true:全部加密 false,部分加密</param>
      public void EncyptFile(string encypt_name,string public_key,bool is_total)
       {
           FileStream fs = File.OpenRead(path);
           FileStream writer = new FileStream(encypt_name, FileMode.Append, FileAccess.Write);
           try
           {
               byte[] bytes = new byte[origin_byte_counts];
               if (is_total == false)
               {
                   fs.Read(bytes, 0, bytes.Length);
                   byte[] getBytes = rsa_producer.Encypt(public_key, bytes);                 
                   writer.Write(getBytes, 0, getBytes.Length);
                   while (fs.Read(bytes, 0, bytes.Length) > 0)
                   {
                       writer.Write(bytes, 0, bytes.Length);
                       writer.Flush();
                   }
               }
               else
               {
                   while (fs.Read(bytes, 0, bytes.Length) > 0)
                   {
                       byte[] getBytes = rsa_producer.Encypt(public_key, bytes);
                       writer.Write(getBytes, 0, getBytes.Length);
                       writer.Flush();
                   }
               }
           }
           finally
           {
               writer.Close();
               fs.Dispose();
           }            
       }
       
       /// <summary>
       /// 加密文件,包含文件名
       /// </summary>
       /// <param name="public_key">公钥</param>
       /// <param name="is_total">是否全部加密,true:全部加密 false:部分加密</param>
      /// <param name="encypt_name">加密后的文件名</param>
      public void EncyptNameFile(string public_key, bool is_total,string encypt_name)
      {
          string str_total=is_total?"T":"F";
          FileStream fs = File.OpenRead(path);
          FileStream writer = new FileStream(encypt_name, FileMode.Append, FileAccess.Write);
          try
          {
              //包含规范化长度的文件名字节数组
              byte[] byte_name = new byte[byte_name_counts];
              //获得文件名称
              byte[] byte_temp = (new UnicodeEncoding()).GetBytes(file_name);
              //文件名称加密操作
              for (int counts = 0; counts < byte_temp.Length; ++counts)
              {
                  byte_name[counts] = byte_temp[counts];
              }
              //填充字节，规范化名称字节流
              byte[] byte_symbol=(new UnicodeEncoding()).GetBytes(name_symbol);
              for(int counts=byte_temp.Length;counts<byte_name.Length-1;counts+=2)
              {
                  byte_name[counts] = byte_symbol[0];
                  byte_name[counts+1] = byte_symbol[1];
              }
              writer.Write(byte_name, 0, byte_name_counts);
              //填充是否全部加密
              byte[] byte_isTotal = (new UnicodeEncoding()).GetBytes(str_total);
              writer.Write(byte_isTotal, 0, byte_isTotal.Length);
              byte[] bytes = new byte[origin_byte_counts];   
              //是否全部加密
              if (is_total == false)
              {
                  fs.Read(bytes, 0, bytes.Length);
                  byte[] getBytes = rsa_producer.Encypt(public_key, bytes);
                  writer.Write(getBytes, 0, getBytes.Length);
                  bytes = new byte[max_bytes];
                  while (fs.Read(bytes, 0, bytes.Length) > 0)
                  {
                      writer.Write(bytes, 0, bytes.Length);
                      writer.Flush();
                  }
              }
              else
              {
                  while (fs.Read(bytes, 0, bytes.Length) > 0)
                  {
                      byte[] getBytes = rsa_producer.Encypt(public_key, bytes);
                      writer.Write(getBytes, 0, getBytes.Length);
                      writer.Flush();
                  }
              }
          }
          finally
          {
              writer.Close();
              fs.Dispose();
          }
      }

       /// <summary>
       /// 解密文件
       /// </summary>
       /// <param name="encypt_name">加密的文件名称</param>
       /// <param name="decypt_name">解密后文件名称</param>
       /// <param name="private_key">私钥</param>
       /// <param name="is_total">是否完全加密 True:完全加密 False:部分加密</param>
     public void DecyptFile(string encypt_name,string decypt_name,string private_key,bool is_total)
      {
          FileStream fs = File.OpenRead(encypt_name);
          FileStream writer = new FileStream(decypt_name, FileMode.Append, FileAccess.Write);
          try
          {
              byte[] bytes = new byte[origin_byte_counts];
              if (is_total == false)
              {
                  fs.Read(bytes, 0, bytes.Length);
                  byte[] getBytes = rsa_producer.Decypt(private_key,bytes);
                  writer.Write(getBytes, 0, getBytes.Length);
                  while (fs.Read(bytes, 0, bytes.Length) > 0)
                  {
                      writer.Write(bytes, 0, getBytes.Length);
                      writer.Flush();
                  }
              }
              while (fs.Read(bytes, 0, bytes.Length) > 0)
              {
                  byte[] getBytes = rsa_producer.Decypt(private_key, bytes);
                  writer.Write(getBytes, 0, getBytes.Length);
                  writer.Flush();
              }
          }
          finally
          {
              writer.Close();
              fs.Dispose();
          }            
      }

     /// <summary>
     /// 解密文件
     /// </summary>
     /// <param name="private_key">私钥</param>
     /// <param name="is_total">是否完全加密 True:完全加密 False:部分加密</param>
     /// <param name="file_name">需要解密的文件名</param>
     public void DecyptNameFile(string private_key,string encypt_name)
     {
         FileStream fs = File.OpenRead(parent_path+"\\"+encypt_name);
         byte[] byte_name=new byte[byte_name_counts];
         fs.Read(byte_name,0,byte_name.Length);
         string decypt_name=(new UnicodeEncoding()).GetString(byte_name);
         //解密文件名
         FileStream writer = new FileStream(decypt_name, FileMode.Append, FileAccess.Write);
         byte[] byte_isTatal=new byte[byte_size];
         fs.Read(byte_isTatal, 0, byte_isTatal.Length);
         //判断是否完全加密
         string str_isTotal = (new UnicodeEncoding()).GetString(byte_isTatal);
         try
         {
             byte[] bytes = new byte[decypt_file_counts];
             //部分加密情况
             if (str_isTotal=="F")
             {
                 fs.Read(bytes, 0, bytes.Length);
                 byte[] getBytes = rsa_producer.Decypt(private_key, bytes);
                 writer.Write(getBytes, 0, getBytes.Length);
                 bytes = new byte[max_bytes];
                 while (fs.Read(bytes, 0, bytes.Length) > 0)
                 {
                     writer.Write(bytes, 0, bytes.Length);
                     writer.Flush();
                 }
             }
             else
             {
                 while (fs.Read(bytes, 0, bytes.Length) > 0)
                 {
                     byte[] getBytes = rsa_producer.Decypt(private_key, bytes);
                     writer.Write(getBytes, 0, getBytes.Length);
                     writer.Flush();
                 }
             }
         }
         finally
         {
             writer.Close();
             fs.Dispose();
         }
     }
   }
}
