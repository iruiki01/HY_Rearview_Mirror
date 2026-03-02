using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class PhotosensitiveParam : ITestParam
    {
        /// <summary>
        /// 前感光数据发送方式
        /// </summary>
        public string Photosensitive1Type { get; set; }
        /// <summary>
        /// 后感光数据发送方式
        /// </summary>
        public string Photosensitive2Type { get; set; }
        /// <summary>
        /// 前感光校正数据
        /// </summary>
        public string Photosensitive1Value { get; set; }
        /// <summary>
        /// 后感光校正数据
        /// </summary>
        public string Photosensitive2Value { get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        public PhotosensitiveParam()
        {
            Photosensitive1Type = "不校正";
            Photosensitive2Type = "不校正";
            RunResult = false;
        }
    }
    public class PhotosensitiveFun : IPlugin
    {
        public string PluginId => "感光数据";

        public string moduleName => "数据";

        public string DescribeMessage => "用于感光校正";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        [JsonIgnore]
        public PhotosensitiveForm photosensitiveForm = null;

        public PhotosensitiveParam photosensitiveParam = new PhotosensitiveParam();

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            photosensitiveForm = new PhotosensitiveForm();
            photosensitiveForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            photosensitiveForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            photosensitiveForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            photosensitiveForm.SetDataToForm(testParam, VersionStr);

            if (testParam != null)
                photosensitiveParam = testParam as PhotosensitiveParam;

            return photosensitiveForm;
        }

        private void FormClosing()
        {
            photosensitiveForm.eventManager.Clear();
        }
        /// <summary>
        /// 接受界面发送来数据
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SetDataForm_OrderCompleted(List<object> list)
        {
            VersionStr1 = Convert.ToString(list[0]);

            photosensitiveParam.Photosensitive1Value = Convert.ToString(list[1]);

            photosensitiveParam.Photosensitive2Value = Convert.ToString(list[2]);

            photosensitiveParam.Photosensitive1Type = Convert.ToString(list[3]);

            photosensitiveParam.Photosensitive2Type = Convert.ToString(list[4]);

            UIMessageBox.ShowSuccess("参数设置成功！");
        }

        public void Dispose()
        {
        }

        public ITestParam GetData()
        {
            return photosensitiveParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {

        }

        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();
            if (photosensitiveParam.Photosensitive1Type == "不校正")
            {
                if (string.IsNullOrEmpty(Context.GetInstance().Photosensitive1))
                {
                    keyValuePairs.Add("前感光结果", false);
                    keyValuePairs.Add("前感光描述", "前感光值为空");
                }
                else
                {
                    keyValuePairs.Add("前感光结果", true);
                    keyValuePairs.Add("前感光描述", "前感光值设置成功");
                    keyValuePairs.Add("前感光值:", Context.GetInstance().Photosensitive1);
                }
            }
            else
            {
                Context.GetInstance().Photosensitive1 = photosensitiveParam.Photosensitive1Value;
                keyValuePairs.Add("前感光结果", true);
                keyValuePairs.Add("前感光描述", "前感光值设置成功");
                keyValuePairs.Add("前感光值:", Context.GetInstance().Photosensitive1);
            }


            if (photosensitiveParam.Photosensitive2Type == "不校正")
            {
                if (string.IsNullOrEmpty(Context.GetInstance().Photosensitive2))
                {
                    keyValuePairs.Add("后感光结果", false);
                    keyValuePairs.Add("后感光描述", "后感光值为空");
                }
                else
                {
                    keyValuePairs.Add("后感光结果", true);
                    keyValuePairs.Add("后感光描述", "后感光值设置成功");
                    keyValuePairs.Add("后感光值:", Context.GetInstance().Photosensitive2);
                }
            }
            else
            {
                Context.GetInstance().Photosensitive2 = photosensitiveParam.Photosensitive2Value;
                keyValuePairs.Add("后感光结果", true);
                keyValuePairs.Add("后感光描述", "后感光值设置成功");
                keyValuePairs.Add("后感光值:", Context.GetInstance().Photosensitive2);
            }

            photosensitiveParam.RunResult = true;
            return keyValuePairs;
        }
        public Dictionary<string, object> Run(ITestParam testParam)
        {
            var p = (PhotosensitiveParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();
            if (p.Photosensitive1Type == "不校正")
            {
                if(string.IsNullOrEmpty(Context.GetInstance().Photosensitive1))
                {
                    keyValuePairs.Add("前感光结果", false);
                    keyValuePairs.Add("前感光描述", "前感光值为空");
                }
                else
                {
                    keyValuePairs.Add("前感光结果", true);
                    keyValuePairs.Add("前感光描述", "前感光值设置成功");
                    keyValuePairs.Add("前感光值:", Context.GetInstance().Photosensitive1);
                }
            }
            else
            {
                Context.GetInstance().Photosensitive1 = p.Photosensitive1Value;
                keyValuePairs.Add("前感光结果", true);
                keyValuePairs.Add("前感光描述", "前感光值设置成功");
                keyValuePairs.Add("前感光值:", Context.GetInstance().Photosensitive1);
            }


            if (p.Photosensitive2Type == "不校正")
            {
                if (string.IsNullOrEmpty(Context.GetInstance().Photosensitive2))
                {
                    keyValuePairs.Add("后感光结果", false);
                    keyValuePairs.Add("后感光描述", "后感光值为空");
                }
                else
                {
                    keyValuePairs.Add("后感光结果", true);
                    keyValuePairs.Add("后感光描述", "后感光值设置成功");
                    keyValuePairs.Add("后感光值:", Context.GetInstance().Photosensitive2);
                }
            }
            else
            {
                Context.GetInstance().Photosensitive2= p.Photosensitive2Value;
                keyValuePairs.Add("后感光结果", true);
                keyValuePairs.Add("后感光描述", "后感光值设置成功");
                keyValuePairs.Add("后感光值:", Context.GetInstance().Photosensitive2);
            }

            p.RunResult = true;
            return keyValuePairs;
        }
    }
}
