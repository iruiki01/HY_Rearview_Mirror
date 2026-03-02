using hWindowTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror
{
    internal class Context
    {
        #region 单例
        /// <summary>
        /// 单例
        /// </summary>
        private static Context instance = null;
        private static readonly object syncRoot = new object();
        public static Context GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new Context();
                }
            }
            return instance;
        }
        #endregion
        public Context() { }

        /*************************************************************感光数据在CAN类中使用*******************************************************************/
        /// <summary>
        /// 前感光值
        /// </summary>
        public string Photosensitive1 {  get; set; }
        /// <summary>
        /// 后感光值
        /// </summary>
        public string Photosensitive2 { get; set; }
        /*************************************************************************************************************************************************************/

        /*******************************************防炫目数据在GlareFun类中使用（前后感光数据也可在此处判断）***********************************************/
        /// <summary>
        /// 亮度值1
        /// </summary>
        public string LightValue1 { get; set; }
        /// <summary>
        /// 亮度值2
        /// </summary>
        public string LightValue2 { get; set; }
        /***********************************************************************************************************************************************************/
    }
}
