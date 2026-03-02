using HY_Rearview_Mirror.CAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.InterfaceTool
{
    // 参数接口
    public interface ITestParam { }
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// 插件ID唯一标识（必须说明其功能），也是功能类型
        /// </summary>
        string PluginId { get; }
        /// <summary>
        /// 模组名称
        /// </summary>
        string moduleName { get; }
        /// <summary>
        /// 描述
        /// </summary>
        string DescribeMessage { get; }
        /// <summary>
        /// 参数UI
        /// </summary>
        Form CreateUI(ITestParam testParam, string VersionStr = "");
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        ITestParam GetData();
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        string GetVersion();
        string VersionStr { get; }
        // 运行方法
        Dictionary<string, object> Run(ITestParam testParam);
    }
}
