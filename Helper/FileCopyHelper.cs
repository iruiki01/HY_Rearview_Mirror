using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class FileCopyHelper
    {
        //储存所有文件夹名
        private static ArrayList dirs;

        public FileCopyHelper()
        {
            dirs = new ArrayList();
        }

        /// <summary>
        /// 获取所有文件名
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static ArrayList GetFileName(string dirPath)
        {
            ArrayList list = new ArrayList();

            if (Directory.Exists(dirPath))
            {
                list.AddRange(Directory.GetFiles(dirPath));
            }
            return list;
        }

        /// <summary>
        /// 获取所有文件夹名
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static ArrayList GetDirName(string dirPath)
        {
            try
            {
                ArrayList list = new ArrayList();

                if (Directory.GetDirectories(dirPath).Length > 0)
                {
                    foreach (string path in Directory.GetDirectories(dirPath))
                    {
                        list.Add(path);
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        //获取所有文件夹及子文件夹
        public static void GetDirs(string dirPath)
        {
            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    dirs.Add(path);
                    GetDirs(path);
                }
            }
        }
        /// <summary>
        /// 检查文件是否存在，没有就创建
        /// </summary>
        /// <param name="filePath"></param>
        public static bool FileCreate(string filePath)
        {
            // 检查文件是否存在
            if (!File.Exists(filePath))
            {
                // 文件不存在，创建文件
                File.Create(filePath).Close(); // 创建文件并关闭文件流
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="_filePath"></param>
        public static void FileDelete(string _filePath)
        {
            try
            {
                // 检查文件是否存在以避免可能的异常
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath); // 删除文件
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="_filePath"></param>
        public static void DeleteDirectory(string targetDir)
        {
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, true);
        }
        /// <summary>
        /// 检查文件夹是否存在，没有就创建
        /// </summary>
        public static void DirectoryCreate(string folderPath)
        {
            // 检查文件夹是否存在
            if (!Directory.Exists(folderPath))
            {
                // 文件夹不存在，创建文件夹
                Directory.CreateDirectory(folderPath);
            }
        }
        /// <summary>
        /// 获取给出文件夹及其子文件夹下的所有文件名
        /// （文件名为路径加文件名及后缀,
        /// 使用的时候GetAllFileName().ToArray()方法可以转换成object数组
        /// 之后再ToString()分别得到文件名）
        /// </summary>
        /// <param name="rootPath">文件夹根目录</param>
        /// <returns></returns>
        public static ArrayList GetAllFileName(string rootPath)
        {
            dirs.Clear();
            dirs.Add(rootPath);
            GetDirs(rootPath);
            object[] allDir = dirs.ToArray();

            ArrayList list = new ArrayList();

            foreach (object o in allDir)
            {
                list.AddRange(GetFileName(o.ToString()));
            }

            return list;
        }

        // 拷贝文件夹下的所有文件和文件夹
        public void CopyDirectory(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
            }

            if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                destDirName = destDirName + Path.DirectorySeparatorChar;

            String[] files = Directory.GetFiles(sourceDirName);
            foreach (string file in files)
            {
                if (File.Exists(destDirName + Path.GetFileName(file)))
                    continue;
                File.Copy(file, destDirName + Path.GetFileName(file), true);
                File.SetAttributes(destDirName + Path.GetFileName(file), FileAttributes.Normal);
            }

            string[] dirs = Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
            {
                CopyDirectory(dir, destDirName + Path.GetFileName(dir));
            }
        }
        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="originalPath"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public static bool TryRenameFile_Directory(string originalPath, string newPath)
        {
            try
            {
                // 确保原文件存在
                if (Directory.Exists(originalPath))
                {
                    // 修改文件名（本质是移动文件到同一目录下的新名称）
                    Directory.Move(originalPath, newPath);
                    Console.WriteLine("文件夹名修改成功！");
                    return true;
                }
                else
                {
                    Console.WriteLine("原文件不存在！");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
            }
            return false;
        }
        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="originalPath"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public static bool TryRenameFile(string originalPath, string newPath)
        {
            try
            {
                // 确保原文件存在
                if (File.Exists(originalPath))
                {
                    // 修改文件名（本质是移动文件到同一目录下的新名称）
                    Directory.Move(originalPath, newPath);
                    Console.WriteLine("文件名修改成功！");
                    return true;
                }
                else
                {
                    Console.WriteLine("原文件不存在！");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
            }
            return false;
        }
    }
}
