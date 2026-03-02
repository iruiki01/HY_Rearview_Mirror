using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;


namespace Helper
{
    public class WriteIniFile
    {
        static FileIniDataParser parser = new FileIniDataParser();
        /// <summary>
        /// 写Ini文件
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="KeyName"></param>
        /// <param name="Value"></param>
        static public void Write(string SectionName, string KeyName, string Value, string path)
        {
            //string path1 = AppDomain.CurrentDomain.BaseDirectory + @"\Data\" + @"file.ini";
            // 创建一个新的 IniData 对象，并设置配置信息
            IniData newData = new IniData();
            newData.Sections.AddSection(SectionName);
            newData[SectionName].AddKey(KeyName, Value);
            // 将 IniData 对象写入 INI 文件
            parser.WriteFile(path, newData);
        }
        /// <summary>
        /// 读ini文件
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        static public string Read(string SectionName, string KeyName, string path)
        {
            string value = "";
            try
            {
                IniData data = parser.ReadFile(path);
                // 访问 INI 文件中的配置信息
                value = data[SectionName][KeyName];
                return value;
            }
            catch
            {
                return value;
            }
        }

        #region 引用系统自带的 Kenl32.dll
        [DllImport("kernel32")] //win32 dll 对应的命令
        //写配置文件ini 返回0 表示失败的 非0 表示成功（C# 掉用非托管类型的DLL）
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")] //win32 dll 对应的命令
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 读取方法
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="NoText"></param>
        /// <param name="iniFilePath"></param>
        /// <returns></returns>
        public static string ReadIni(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 写ini数据
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="iniFilePath"></param>
        /// <returns></returns>
        public static bool WriteIni(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                return WritePrivateProfileString(Section, Key, Value, iniFilePath) != 0;
            }
            return false;
        }
        #endregion
    }
}
