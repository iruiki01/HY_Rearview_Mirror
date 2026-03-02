using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace Helper
{
    public class ToCsvHelper
    {
        private static readonly object SaveCSV_Lock = new object();

        /// <summary>
        /// 保存测试记录CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public static void SaveCSV<T>(IEnumerable<T> items)
        {
            lock (SaveCSV_Lock)
            {
                try
                {
                    string sFilePath = "D:\\运行日志\\扫码称重测试记录\\" + DateTime.Now.ToString("yyyyMM") + "扫码称重测试测试记录";
                    string sFileName = DateTime.Now.ToString("yyyyMMdd") + "扫码称重测试记录.csv";
                    sFileName = sFilePath + "\\" + sFileName;

                    if (!Directory.Exists(sFilePath))
                    {
                        Directory.CreateDirectory(sFilePath);
                    }

                    bool fileExists = System.IO.File.Exists(sFileName);
                    using (var writer = new StreamWriter(sFileName, append: true, Encoding.UTF8))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        if (!fileExists)
                        {
                            csv.WriteHeader<T>();
                            csv.NextRecord();
                        }
                        foreach (var item in items)
                        {
                            csv.WriteRecord(item);
                            csv.NextRecord();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("保存CSV文件失败: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 保存错误记录CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public static void SaveErrCSV<T>(IEnumerable<T> items)
        {
            lock (SaveCSV_Lock) // 添加锁机制保持一致性
            {
                try
                {
                    string sFilePath = "D:\\运行日志\\Sn错误记录\\" + DateTime.Now.ToString("yyyyMM") + "Sn错误记录";
                    string sFileName = DateTime.Now.ToString("yyyyMMdd") + "Sn错误记录.csv";
                    sFileName = sFilePath + "\\" + sFileName;

                    if (!Directory.Exists(sFilePath))
                    {
                        Directory.CreateDirectory(sFilePath);
                    }

                    bool fileExists = System.IO.File.Exists(sFileName);
                    using (var writer = new StreamWriter(sFileName, append: true, Encoding.UTF8))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        if (!fileExists)
                        {
                            csv.WriteHeader<T>();
                            csv.NextRecord();
                        }
                        foreach (var item in items)
                        {
                            csv.WriteRecord(item);
                            csv.NextRecord();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("保存错误CSV文件失败: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 通用读取CSV文件方法
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filePath">CSV文件完整路径</param>
        /// <returns>数据列表</returns>
        public static List<T> ReadCSV<T>(string filePath)
        {
            var records = new List<T>();

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("CSV文件不存在: " + filePath);
                    return records;
                }

                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    records = csv.GetRecords<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取CSV文件失败: " + ex.Message);
            }

            return records;
        }
    }
}

/**
 
// 定义数据模型
public class ScanRecord
{
    public string SN { get; set; }
    public string Weight { get; set; }
    public DateTime ScanTime { get; set; }
}

// 保存数据
var records = new List<ScanRecord>
{
    new ScanRecord { SN = "SN12345", Weight = "1.5kg", ScanTime = DateTime.Now }
};
ToCsvHelper.SaveCSV(records);

// 读取数据
var readRecords = ToCsvHelper.ReadTestCSV<ScanRecord>();
foreach (var record in readRecords)
{
    Console.WriteLine($"SN: {record.SN}, Weight: {record.Weight}");
}

 **/