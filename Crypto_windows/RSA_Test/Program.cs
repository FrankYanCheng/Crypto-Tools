using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
namespace RSA_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = System.Environment.CurrentDirectory;
            Start(path);
        }

        /// <summary>
        ///启动流程 
        /// </summary>
        /// <param name="path">应用所在路径</param>
        static void Start(string path)
        {
            string public_key_name="public_key.txt";
            string private_key_name="private_key.txt";
            DirectoryInfo direct = new DirectoryInfo(path);
            FileInfo[] file_info = direct.GetFiles();
            Console.WriteLine("There are {0} files", file_info.Length);

            List<string> li_exclude = new List<string>();
            //加密文件类
            RSA_File rsaFile = new RSA_File();
            //该文件不加密
            string this_file = System.AppDomain.CurrentDomain.SetupInformation.ApplicationName;
            li_exclude.Add(this_file);
            li_exclude.Add(public_key_name);
            li_exclude.Add(private_key_name);
            //li_exclude.Add("RSA_Test.exe");
            //文件处理类
            File_Handler handler = new File_Handler();
            string private_key=null, public_key=null;
            //查找公钥私钥
            foreach(FileInfo file in file_info)
            {
                if (file.Name == public_key_name)
                    public_key = handler.fileReader(file.FullName);
                if (file.Name == private_key_name)
                    private_key = handler.fileReader(file.FullName);
            }
            //判断是否存在公钥私钥，无则产生公钥私钥
            if(public_key==null&&private_key==null)
            {
                Console.WriteLine("Produce key........");
                rsaFile.produce_key(out public_key, out private_key);
                handler.fileWiter(public_key, public_key_name);
                handler.fileWiter(private_key, private_key_name);
            }
            Console.WriteLine("Encypt or Decypt?");
            string order = Console.ReadLine();
            bool isEncypt = true;
            //加密
            if (order.ToLower() == "encypt" && public_key != null)
            {
                Console.WriteLine("Do You really want to encypt files?  Y/N");
                if (Console.ReadLine().ToLower() == "y")
                    isEncypt = true;
                Cypto(path, public_key, private_key, ref file_info,rsaFile,li_exclude, isEncypt,handler);
            }
            //解密
            else if (order.ToLower() == "decypt" && private_key != null)
            {
                Console.WriteLine("Do You really want to decypt files? Y/N");
                if (Console.ReadLine().ToLower() == "y")
                    isEncypt = false;
                Cypto(path, public_key, private_key, ref file_info,rsaFile,li_exclude, isEncypt,handler);
            }
            else
                Console.WriteLine("You don't have the correct key!");
            Console.WriteLine("The work have been down");
            Console.ReadKey();
        }
        /// <summary>
        /// 加密解密方法
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="public_key">公钥</param>
        /// <param name="private_key">私钥</param>
        /// <param name="file_info">文件</param>
        /// <param name="rsaFile">RSA文件处理类</param>
        /// <param name="li_exclude">排除的不加密解密文件</param>
        /// <param name="isEncypt">true:加密,false:解密</param>
        /// <param name="handler">文件处理类句柄</param>
        static void Cypto(string path, string public_key, string private_key, ref FileInfo[] file_info, RSA_File rsaFile, List<string> li_exclude, bool isEncypt,File_Handler handler)
        {
            int counts = 0;
            //文件全部加密的大小限制
            int size_limit= 50 * 1024;
            //小文件进行加密时读取的字节大小
            int origin_byte_counts = 1;
            string type=isEncypt?"encypting":"decypting";
            //设置是否全部加密
            bool is_total;
            //加密
            foreach (FileInfo file in file_info)
            {
                if (!li_exclude.Contains(file.Name))
                {
                    Console.WriteLine("{0}:{1} is {2}", counts, file.Name, type);
                    rsaFile.SetPath(file.FullName, origin_byte_counts);
                    if (file.Length < size_limit)
                        is_total = true;
                    else
                        is_total = false;
                    string id = Guid.NewGuid().ToString();
                    if (isEncypt)
                    {
                        rsaFile.EncyptNameFile(public_key, is_total, id);
                        handler.deleteFile(file);
                    }
                    else
                    {
                        rsaFile.DecyptNameFile(private_key, file.Name);
                      
                    }
                    ++counts;
                }    
            }
            DirectoryInfo direct = new DirectoryInfo(path);
            file_info = direct.GetFiles();
            if (isEncypt == false)
            {
                foreach (FileInfo file in file_info)
                {
                   if(!li_exclude.Contains(file.Name))
                   {
                       handler.rename(file, '$');
                   }
                }
            }
        }
    }
}
