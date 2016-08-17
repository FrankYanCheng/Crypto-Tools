using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace RSA_Test
{
    /// <summary>
    /// 文件处理助手类
    /// </summary>
   public class File_Handler
    {
       /// <summary>
       /// 文件写入方法
       /// </summary>
       /// <param name="text"></param>
       /// <param name="path"></param>
       public void fileWiter(string text,string path)
       {
           using (FileStream stream = new FileStream(path, FileMode.Create))
           {
               byte[] bytes = (new UnicodeEncoding()).GetBytes(text);
               stream.Write(bytes, 0, bytes.Length);
           }
       }
       /// <summary>
       /// 文件读出方法
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
       public string fileReader(string path)
       {
           using(TextReader tx_reader=new StreamReader(path,Encoding.Unicode))
           {
              return  tx_reader.ReadToEnd();
           }
       }

       /// <summary>
       /// 删除文件数组
       /// </summary>
       /// <param name="file_Info">文件数组</param>
       public void deleteFiles(FileInfo[] file_Info)
       {
           foreach(FileInfo file in file_Info)
           {
               File.Delete(file.FullName);
           }
       }

       /// <summary>
       ///删除文件 
       /// </summary>
       /// <param name="file">文件</param>
       public void deleteFile(FileInfo file)
       {
           File.Delete(file.FullName);
       }
       /// <summary>
       /// 删除文件-路径
       /// </summary>
       /// <param name="path"></param>
       public void deleteFilePath(string path)
       {
           File.Delete(path);
       }

       /// <summary>
       /// 重命名文件
       /// </summary>
       /// <param name="file">文件</param>
       /// <param name="split">文件标识</param>
       public void rename(FileInfo file,char split)
       {
           string root = Path.GetDirectoryName(file.FullName);
           string filename = file.Name;
           int counts=0;
           int index =-1;
           char[] charFile = filename.ToCharArray();
           for (int i= 0; i< charFile.Length; ++i)
           {
               if (charFile[i] == split)
               {
                   if (index == -1)
                       index =i;
                   ++counts;
               }
           }
           if (counts >= 1)
               filename=filename.Substring(0, index);
           string origin_path = file.FullName;
           file.MoveTo(root + "\\" + filename);
           this.deleteFilePath(origin_path);

          
       }
    }
}
