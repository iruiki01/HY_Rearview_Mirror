using HalconDotNet;
using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class GlareParam : ITestParam
    {
        /// <summary>
        /// 判断类型
        /// </summary>
        public string JudgeType { get; set; }
        /// <summary>
        /// 判断值
        /// </summary>
        public string JudgeValue { get; set; }
        /// <summary>
        /// 判断阈值
        /// </summary>
        public string JudgeThreshold { get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        public GlareParam()
        {
            JudgeType = "";
            JudgeValue = "";
            RunResult = false;
        }
    }
    /// <summary>
    /// 数据操作（传入两个数进行判断）
    /// </summary>
    public class GlareFun : IPlugin
    {
        public string PluginId => "数据判断";

        public string moduleName => "数据";

        public string DescribeMessage => "用于数据判断";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;
        [JsonIgnore]
        public GlareForm glareForm = null;
        public GlareParam glareParam = new GlareParam();
        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            glareForm = new GlareForm();

            glareForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            glareForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            glareForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));

            glareForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
                glareParam = testParam as GlareParam;

            return glareForm;
        }

        private void FormClosing()
        {
            glareForm.eventManager.Clear();
        }

        private void SetDataForm_OrderCompleted(List<object> list)
        {
            VersionStr1 = Convert.ToString(list[0]);
            glareParam.JudgeType = list[1].ToString();
            glareParam.JudgeValue = list[2].ToString();
        }

        public void Dispose()
        {
        }

        public ITestParam GetData()
        {
            return glareParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            var p = (GlareParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();


            switch (p.JudgeType)
            {
                case "相减":
                    {
                        double d = Convert.ToDouble(Context.GetInstance().LightValue1) - Convert.ToDouble(Context.GetInstance().LightValue2);

                        if (Math.Abs(d) < Convert.ToDouble(p.JudgeValue))
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试OK");
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试NG");
                            keyValuePairs.Add("结果", false);
                        }
                    }
                    break;
                case "大于":
                    {
                        if (Convert.ToDouble(Context.GetInstance().LightValue1) > Convert.ToDouble(Context.GetInstance().LightValue2))
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试OK");
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试NG");
                            keyValuePairs.Add("结果", false);
                        }
                    }
                    break;
                case "小于":
                    {
                        if (Convert.ToDouble(Context.GetInstance().LightValue1) < Convert.ToDouble(Context.GetInstance().LightValue2))
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试OK");
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试NG");
                            keyValuePairs.Add("结果", false);
                        }
                    }
                    break;
            }

            return keyValuePairs;
        }
        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            switch(glareParam.JudgeType)
            {
                case "相减":
                    {
                        double d = Convert.ToDouble(Context.GetInstance().LightValue1) - Convert.ToDouble(Context.GetInstance().LightValue2);

                        if(Math.Abs(d) < Convert.ToDouble(glareParam.JudgeValue))
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试OK");
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试NG");
                            keyValuePairs.Add("结果", false);
                        }
                    }
                    break;
                case "大于":
                    {
                        if (Convert.ToDouble(Context.GetInstance().LightValue1) > Convert.ToDouble(Context.GetInstance().LightValue2))
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试OK");
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试NG");
                            keyValuePairs.Add("结果", false);
                        }
                    }
                    break;
                case "小于":
                    {
                        if (Convert.ToDouble(Context.GetInstance().LightValue1) < Convert.ToDouble(Context.GetInstance().LightValue2))
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试OK");
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("亮度值1", Context.GetInstance().LightValue1);
                            keyValuePairs.Add("亮度值2", Context.GetInstance().LightValue2);
                            keyValuePairs.Add("描述", "测试NG");
                            keyValuePairs.Add("结果", false);
                        }
                    }
                    break;
            }
    
            return keyValuePairs;
        }
    }
}
