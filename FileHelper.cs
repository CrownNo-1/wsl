using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileHelp
{
    public class FileHelper
    {
        public FileHelper(string path) { }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
       
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);



        //从文件获取数据
        public static string Get_init_data(string title, string key_, string ini_path)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(title, key_, "", sb, 255, ini_path);
            return sb.ToString();
        }
        public static void Set_init_data(string title, string key, string value, string ini_path)
        {
            WritePrivateProfileString(title, key, value,ini_path);
        }


        public static List<string> GetKeys(string path, string title)
        {
            //缓冲区
            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(title, buffer, 2048, path);

            String[] tmp = Encoding.Default.GetString(buffer).Trim('\0').Split('\0');
            List<string> list = new List<string>();
            foreach (String entry in tmp)
            {
                string[] v = entry.Split('=');
                list.Add(v[0]);
            }
            return list ;
        }
        /// <summary>
        /// 自动删除指定时间的文件夹
        /// </summary>
        public static void AutoDeleteDirectory(string path,int days) {
            //获取最近三天的文件夹的名字
            var list = new List<string>();
            for (int i = 0; i < days; i++)
            {
                var saveFile = DateTime.Now.AddDays(-i).ToString("yyyy年MM月dd日");
                list.Add(saveFile);
            }
            //获取path下所有的文件夹
            DirectoryInfo di = new DirectoryInfo(path);

            DirectoryInfo[] arr = di.GetDirectories();
            foreach (DirectoryInfo item in arr)
            {
                if (!list.Contains(item.Name))
                {
                    //Console.WriteLine(item.FullName);
                    item.Delete(true);
                }
            }
        }

        /// <summary> 读取CSV表里面全部的内容
        /// </summary>
        public static List<string> Read_CSV(string FliePath)
        {
            string str2 = FliePath;
            //if (!Directory.Exists(str2))// 如果CSV文件不存在了，就需要提示
            if (!File.Exists(str2))
            {
                return null;
            }
            StreamReader reader = new StreamReader(str2, System.Text.Encoding.Default);
            string line = "";
            List<string> listStrArr = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                listStrArr.Add(line);//将文件内容分割成数组
            }
            reader.Close();
            return listStrArr;
        }

        public static void Write_CSV(object[] content, string[] TitleArray, FileMode filemode = FileMode.Append, string fileName = "")// CSV表写入操作
        {
            StreamWriter writer = null;
            //string RootPath = System.Windows.Forms.Application.StartupPath;
            string RootPath = Environment.CurrentDirectory;
            try
            {
                string str2 = RootPath + "\\" + (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";
                if (!File.Exists(str2))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(str2, TitleArray);
                }
                writer = new StreamWriter(new FileStream(str2, filemode, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                Console.WriteLine("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！" + ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示");
            }
        }
        /// <summary>
        /// 获取所有键值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Dictionary<string, string> GetKeysAll(string path, string title)
        {
            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(title, buffer, 2048, path);
            String[] tmp = Encoding.Default.GetString(buffer).Trim('\0').Split('\0');
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (String entry in tmp)
            {
                string[] v = entry.Split('=');
                result.Add(v[0], v[1]);
            }
            return result;
        }
        public static void Write_CSV_Title(string fileName, object[] content) // 写入表头数据
        {
            StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);
            // 向文件里面写入数据
            string str3 = "";
            int index = 0;
            try
            {
                while (true)
                {
                    if (index >= content.Length)
                    {
                        writer.WriteLine(str3);
                        writer.Close();
                        break;
                    }
                    if (index > 0)
                    {
                        str3 = str3 + ",";
                    }
                    str3 = str3 + content[index];
                    index++;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("导出错误" + exception.Message);
            }
        }
    }
}
