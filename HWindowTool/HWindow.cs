using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace hWindowTool
{
    public partial class HWindow : UserControl
    {
        #region 构造方法
        /// <summary>
        /// halcon封装控件
        /// </summary>
        public HWindow()
        {
            InitializeComponent();
            //使用双缓冲
            this.DoubleBuffered = true;
            //更新外部调用 hWindowControl
            this.hWindowControl = hWindowControl1;
            //订阅在鼠标指针移到halcon控件上时发生
            this.hWindowControl.HMouseMove += hWindowControl1_HMouseMove;
            //实例化HalconWindow行为
            this.hWndAction = new HWndAction(hWindowControl1)
            {
                //初始化HalconWindow行为属性
                DrawModel = drawModel,
                DisplayCrosses = displayCrosses,
                DisplayCoordinate = displayCoordinate,
                CoordinateProportion = coordinateProportion,
                CoordinateTextSize = coordinateTextSize,
                CoordinateMode = coordinateMode,
                DisplayPixelGrid = displayPixelGrid,
                PixelGrid = pixelGrid,
                MinZoom = minZoom,
            };
            //订阅更新状态显示
            this.hWndAction.UpdateStatusDisplay += HWndCtrl_UpdateStatusDisplay;
            //订阅ROI移动完成事件
            this.hWndAction.ROIMoveDone += HWndAction_ROIMoveDone;
            //订阅ROI激活完成事件
            this.hWndAction.ROIActivateDone += HWndAction_ROIActivateDone;
            //订阅ROI正在移动时事件
            this.hWndAction.ROIMoving += HWndAction_ROIMoving;
            //订阅涂抹按下或按下移动时事件
            this.hWndAction.SmearPressOrMove += HWndAction_SmearPressOrMove;
        }
        #endregion

        /// <summary>
        /// 更新显示图
        /// </summary>
        public void UpdataHWndActionImage(HImage _image)
        {
            this.hWndAction.image?. Dispose();
            this.hWndAction.image = _image.Clone();
            _image?.Dispose();
        }

        #region 重写OnLoad方法
        /// <summary>
        /// 重写OnLoad方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //不在设计器模式下
            if (!DesignMode)
            {
                //设置剪切区域(防止像素高时区域显示不全)
                HSystem.SetSystem("clip_region", "false");
            }
        }
        #endregion

        #region 重写处理命令键
        /// <summary>
        /// 全屏显示窗体
        /// </summary>
        private FullScreenDisplay fullScreenDisplay;
        /// <summary>
        /// 重写处理命令键
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //当前不是全屏状态 && F键按下
            if (!全屏显示ToolStripMenuItem.Checked && keyData == Keys.F)
            {
                全屏显示ToolStripMenuItem.Checked = true;
                全屏显示ToolStripMenuItem.Text = "退出全屏(Esc)";
                //进入全屏显示
                fullScreenDisplay = new FullScreenDisplay(this);
                fullScreenDisplay.ShowDialog();
                // 事件已处理  
                return true;
            }
            //当前是全屏状态 && Esc键按下
            else if (全屏显示ToolStripMenuItem.Checked && keyData == Keys.Escape)
            {
                全屏显示ToolStripMenuItem.Checked = false;
                全屏显示ToolStripMenuItem.Text = "进入全屏(f)";
                //退出全屏显示
                fullScreenDisplay.ExitFullScreen();
                // 事件已处理  
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region 取消订阅事件
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void UnsubscribeEvents()
        {
            //取消订阅ROI刷新显示事件
            UnsubscribeROIRefreshDisplayEvent();
            //取消订阅涂抹刷新显示事件
            UnsubscribeSmearRefreshDisplayEvent();
            //取消订阅HObjData对象刷新显示事件
            UnsubscribeHObjDataRefreshDisplayEvent();
            //取消订阅HTextData对象刷新显示事件
            UnsubscribeHTextDataRefreshDisplayEvent();
        }
        #endregion

        #region 属性字段
        /// <summary>
        /// Roi编号
        /// </summary>
        public int RoiNum = 0;
        /// <summary>
        /// 涂抹编号
        /// </summary>
        public int SmearNum = 0;
        /// <summary>
        /// 外部调用 hWindowControl
        /// </summary>
        public readonly HWindowControl hWindowControl;

        /// <summary>
        /// HalconWindow行为
        /// </summary>
        private readonly HWndAction hWndAction;

        /// <summary>
        /// 当前显示图像
        /// </summary>
        private HImage image;
        /// <summary>
        /// 当前显示图像
        /// </summary>
        protected HImage Image
        {
            get { return image; }
            set
            {
                //要更新的图像不为null && 已实例化
                if (value != null && value.IsInitialized())
                {
                    //图像字段存在图像
                    if (image != null)
                    {
                        //对图像字段进行释放
                        image.Dispose();
                    }
                    //更新图像字段
                    image = value;
                    UpdataHWndActionImage(image);
                    //刷新显示
                    RefreshDisplay();
                }
            }
        }

        /// <summary>
        /// 绘制模式,禁止鼠标平移缩放
        /// </summary>
        private bool drawModel;
        /// <summary>
        /// 绘制模式,禁止鼠标平移缩放
        /// </summary>
        public bool DrawModel
        {
            get { return drawModel; }
            set
            {
                drawModel = value;
                hWndAction.DrawModel = drawModel;
                //解除||绑定右键菜单
                hWindowControl.ContextMenuStrip = drawModel ? null : contextMenuStrip1;
            }
        }

        /// <summary>
        /// 显示状态
        /// </summary>
        private bool displayStatus = true;
        /// <summary>
        /// 显示状态
        /// </summary>
        [Category("外观")]
        [Description("是否显示状态栏。")]
        public bool DisplayStatus
        {
            get { return displayStatus; }
            set
            {
                displayStatus = value;
                label_Status.Visible = displayStatus;
            }
        }

        /// <summary>
        /// 显示十字架
        /// </summary>
        private bool displayCrosses = false;
        /// <summary>
        /// 显示十字架
        /// </summary>
        [Category("外观")]
        [Description("是否显示十字架。")]
        public bool DisplayCrosses
        {
            get { return displayCrosses; }
            set
            {
                displayCrosses = value;
                hWndAction.DisplayCrosses = displayCrosses;
            }
        }

        /// <summary>
        /// 显示坐标指示
        /// </summary>
        private bool displayCoordinate;
        /// <summary>
        /// 显示坐标指示
        /// </summary>
        [Category("坐标")]
        [Description("是否显示坐标指示。")]
        public bool DisplayCoordinate
        {
            get { return displayCoordinate; }
            set
            {
                displayCoordinate = value;
                hWndAction.DisplayCoordinate = displayCoordinate;
            }
        }

        /// <summary>
        /// 坐标占图像大小比例
        /// </summary>
        private double coordinateProportion = 0.05;
        /// <summary>
        /// 坐标占图像大小比例
        /// </summary>
        [Category("坐标")]
        [Description("坐标指示在图像中的占比，最大值1。")]
        public double CoordinateProportion
        {
            get { return coordinateProportion; }
            set
            {
                if (value <= 0)
                {
                    coordinateProportion = 0.05;
                }
                else
                {
                    if (value > 1)
                    {
                        coordinateProportion = 1;
                    }
                    else
                    {
                        coordinateProportion = value;
                    }
                }
                hWndAction.CoordinateProportion = coordinateProportion;
            }
        }

        /// <summary>
        /// 坐标文本大小
        /// </summary>
        private int coordinateTextSize = 20;
        /// <summary>
        /// 坐标文本大小
        /// </summary>
        [Category("坐标")]
        [Description("设置坐标文本大小。")]
        public int CoordinateTextSize
        {
            get { return coordinateTextSize; }
            set
            {
                coordinateTextSize = value < 10 ? 10 : value;
                hWndAction.CoordinateTextSize = coordinateTextSize;
            }
        }

        /// <summary>
        /// 坐标模式
        /// </summary>
        private CoordinateMode coordinateMode;
        /// <summary>
        /// 坐标模式
        /// </summary>
        [Category("坐标")]
        [Description("坐标显示模式。")]
        public CoordinateMode CoordinateMode
        {
            get { return coordinateMode; }
            set
            {
                if (coordinateMode != value)
                {
                    coordinateMode = value;
                    hWndAction.CoordinateMode = coordinateMode;
                    //刷新显示
                    RefreshDisplay();
                }
            }
        }

        /// <summary>
        /// 窗口编号
        /// </summary>
        private int windowNumber = 1;
        /// <summary>
        /// 窗口编号
        /// </summary>
        [Category("外观")]
        [Description("显示到状态栏的窗口编号。")]
        public int WindowNumber
        {
            get { return windowNumber; }
            set
            {
                if (windowNumber != value)
                {
                    windowNumber = value;
                    //显示窗口编号
                    if (DisplayWindowNumber)
                    {
                        //更新状态显示文本
                        label_Status.Text = value.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 显示窗口编号
        /// </summary>
        private bool displayWindowNumber;
        /// <summary>
        /// 显示窗口编号
        /// </summary>
        [Category("外观")]
        [Description("是否显示窗口编号。")]
        public bool DisplayWindowNumber
        {
            get { return displayWindowNumber; }
            set
            {
                if (displayWindowNumber != value)
                {
                    displayWindowNumber = value;
                    if (value)
                    {
                        //更新状态显示文本
                        label_Status.Text = WindowNumber.ToString();
                    }
                    else
                    {
                        //更新状态显示文本
                        label_Status.Text = "";
                    }
                }
            }
        }

        /// <summary>
        /// ROI正在移动时事件
        /// 参数:ROI名称,ROI数据,ROI区域对象
        /// </summary>
        [Category("ROI")]
        [Description("ROI正在移动时事件。")]
        public event Action<string, HTuple, HRegion> ROIMoving;

        /// <summary>
        /// ROI移动完成事件
        /// 参数:ROI名称,ROI数据,ROI区域对象
        /// </summary>
        [Category("ROI")]
        [Description("ROI移动完成事件。")]
        public event Action<string, HTuple, HRegion> ROIMoveDone;

        /// <summary>
        /// ROI激活完成事件
        /// 参数:ROI名称,ROI数据,ROI区域对象
        /// </summary>
        [Category("ROI")]
        [Description("ROI激活完成事件。")]
        public event Action<string, HTuple, HRegion> ROIActivateDone;

        /// <summary>
        /// 涂抹按下或按下移动时事件
        /// 参数:涂抹名称,涂抹区域
        /// </summary>
        [Category("涂抹")]
        [Description("涂抹按下或按下移动时事件。")]
        public event Action<string, HRegion> SmearPressOrMove;

        /// <summary>
        /// 显示像素网格
        /// </summary>
        private bool displayPixelGrid = true;
        /// <summary>
        /// 显示像素网格
        /// </summary>
        [Category("外观")]
        [Description("图像缩放时，ROW或Column显示的像素值<= PixelGrid 设置值时，显示像素网格。")]
        public bool DisplayPixelGrid
        {
            get { return displayPixelGrid; }
            set
            {
                displayPixelGrid = value;
                hWndAction.DisplayPixelGrid = value;
            }
        }

        /// <summary>
        /// 像素网格
        /// </summary>
        private int pixelGrid = 30;
        /// <summary>
        /// 像素网格
        /// </summary>
        [Category("外观")]
        [Description("当 DisplayPixelGrid 为 true，ROW或Column显示的像素<=设置值时，显示像素网格。")]
        public int PixelGrid
        {
            get { return pixelGrid; }
            set
            {
                pixelGrid = value;
                hWndAction.PixelGrid = value;
            }
        }

        /// <summary>
        /// 鼠标进入控件自动获取焦点
        /// </summary>
        private bool automaticFocus;
        /// <summary>
        /// 鼠标进入控件自动获取焦点
        /// </summary>
        [Category("焦点")]
        [Description("鼠标进入控件自动获取焦点。")]
        public bool AutomaticFocus
        {
            get { return automaticFocus; }
            set { automaticFocus = value; }
        }

        /// <summary>
        /// 缩放最小时自适应
        /// </summary>
        private bool minZoom;
        /// <summary>
        /// 缩放最小时自适应
        /// </summary>
        [Category("行为")]
        [Description("图像缩放到最小时是否进行图像自适应。")]
        public bool MinZoom
        {
            get { return minZoom; }
            set
            {
                minZoom = value;
                hWndAction.MinZoom = value;
            }
        }

        /// <summary>
        /// 停止刷新
        /// </summary>
        private bool stopRefresh;
        /// <summary>
        /// 停止刷新
        /// </summary>
        public bool StopRefresh
        {
            get { return stopRefresh; }
            set
            {
                //从打开停止刷新 -> 关闭停止刷新
                if (stopRefresh && !value)
                {
                    stopRefresh = value;
                    //刷新显示
                    RefreshDisplay();
                }
                stopRefresh = value;
            }
        }

        #endregion

        #region 清除窗口
        /// <summary>
        /// 清除窗口
        /// </summary>
        public void ClearWindow()
        {
            //图像对象设置为null
            image = null;
            //清除窗口
            hWndAction.ClearWindow();
        }
        #endregion

        #region 清除用户显示
        /// <summary>
        /// 清除用户显示(清除用户显示的对象和文本)
        /// </summary>
        public void ClearUserDisplay()
        {
            //取消订阅HObjData对象刷新显示事件
            UnsubscribeHObjDataRefreshDisplayEvent();
            //取消订阅HTextData对象刷新显示事件
            UnsubscribeHTextDataRefreshDisplayEvent();
            //清除用户显示对象集合
            hWndAction.UsersHObjList.Clear();
            //清除用户显示文本集合
            hWndAction.UserTextList.Clear();
            //刷新显示
            RefreshDisplay();
        }
        #endregion

        #region 更新背景图
        /// <summary>
        /// 显示图像线程锁
        /// </summary>
        private object displayImageLock = new object();
        /// <summary>
        /// 更新背景图
        /// </summary>
        /// <param name="hobject">背景图像</param>
        public void HobjectToHimage(HObject hobject)
        {
            lock (displayImageLock)
            {
                //对象为null || 对象未初始化
                if (hobject == null || !hobject.IsInitialized())
                {
                    //清除窗口
                    hWindowControl.HalconWindow.ClearWindow();
                    //结束方法
                    return;
                }
                //取消订阅HObjData对象刷新显示事件
                UnsubscribeHObjDataRefreshDisplayEvent();
                //取消订阅HTextData对象刷新显示事件
                UnsubscribeHTextDataRefreshDisplayEvent();
                //清除用户显示对象集合
                hWndAction.UsersHObjList.Clear();
                //清除用户显示文本集合
                hWndAction.UserTextList.Clear();
                //关闭涂抹
                hWndAction.CloseSmear();
                //更新当前图像
                Image = new HImage(hobject);
                hobject.Dispose();
            }
        }
        #endregion

        #region 刷新显示
        /// <summary>
        /// 刷新显示
        /// </summary>
        public void RefreshDisplay()
        {
            //未打开停止刷新
            if (!stopRefresh)
            {
                //显示图像
                hWndAction.DisplayImage();
  
            }
        }
        #endregion

        #region 显示对象
        /// <summary>
        /// 显示对象
        /// </summary>
        /// <param name="hObjData">halcon对象数据</param>
        public void DisplayObj(HObjData hObjData)
        {
            //要显示的对象不为null && 对象已初始化
            if (hObjData != null && hObjData.HObject.IsInitialized())
            {
                //取消订阅刷新显示事件(这里防止是获取用户显示对象集合后重新写入)
                hObjData.RefreshDisplay -= RefreshDisplay;
                //订阅刷新显示事件
                hObjData.RefreshDisplay += RefreshDisplay;
                //添加用户显示对象集合
                hWndAction.UsersHObjList.Add(hObjData);
                //显示halcon对象
                hWndAction.DisplayHObj(hObjData);
            }
        }
        #endregion

        #region 取消订阅HObjData对象刷新显示事件
        /// <summary>
        /// 取消订阅HObjData对象刷新显示事件
        /// </summary>
        private void UnsubscribeHObjDataRefreshDisplayEvent()
        {
            //遍历用户对象集合
            foreach (var item in hWndAction.UsersHObjList)
            {
                //取消订阅刷新显示事件
                item.RefreshDisplay -= RefreshDisplay;
            }
        }
        #endregion

        #region 显示文本
        /// <summary>
        /// 显示文本
        /// </summary>
        /// <param name="hTextData">文本数据</param>
        public void DisplayText(HTextData hTextData)
        {
            //文本数据已实例化
            if (hTextData != null)
            {
                //取消订阅刷新显示事件(这里防止是获取用户文本集合后重新写入)
                hTextData.RefreshDisplay -= RefreshDisplay;
                //订阅刷新显示事件
                hTextData.RefreshDisplay += RefreshDisplay;
                //添加文本数据到集合
                hWndAction.UserTextList.Add(hTextData);
                //文本可见
                if (hTextData.VisibleText)
                {
                    //显示halcon文本
                    hWndAction.DisplayHText(hTextData);
                }
            }
        }
        #endregion

        #region 取消订阅HTextData对象刷新显示事件
        /// <summary>
        /// 取消订阅HTextData对象刷新显示事件
        /// </summary>
        private void UnsubscribeHTextDataRefreshDisplayEvent()
        {
            //遍历用户文本集合
            foreach (var item in hWndAction.UserTextList)
            {
                //取消订阅刷新显示事件
                item.RefreshDisplay -= RefreshDisplay;
            }
        }
        #endregion

        #region 绑定ROI数据
        /// <summary>
        /// 绑定ROI数据
        /// </summary>
        /// <param name="rOIDatas">ROI数据</param>
        public void BindingROIDatas(Dictionary<string, ROIData> rOIDatas)
        {
            //ROI数据字典未实例化
            if (rOIDatas == null)
            {
                //结束方法
                return;
            }
            RoiNum = rOIDatas.Count;

            //取消订阅ROI刷新显示事件
            UnsubscribeROIRefreshDisplayEvent();
            //更新ROI数据字典
            hWndAction.ROIDatas = rOIDatas;
            //遍历ROI数据字典
            foreach (var item in rOIDatas)
            {
                //设置ROI名称
                item.Value.SetROIName(item.Key);
            }
            //订阅ROI刷新显示事件
            SubscriptionROIRefreshDisplayEvent();

            //刷新显示
            RefreshDisplay();
        }
        #endregion

        #region 绑定模板轮廓数据
        /// <summary>
        /// 绑定模板轮廓数据
        /// </summary>
        /// <param name="ContourDatas">ROI数据</param>
        public void BindingContourDatas(Dictionary<string, HObject> ContourDatas)
        {
            //ROI数据字典未实例化
            if (ContourDatas == null)
            {
                //结束方法
                return;
            }
            RoiNum = ContourDatas.Count;
            //更新ROI数据字典
            hWndAction.ContourDatas = ContourDatas;

            //刷新显示
            RefreshDisplay();
        }
        #endregion

        #region 添加ROI
        /// <summary>
        /// 添加ROI
        /// </summary>
        /// <param name="rOIName">ROI名称(null时自动分配名称)</param>
        /// <param name="rOIData">ROI数据</param>
        /// <returns>添加成功返回ROI名称，失败返回null</returns>
        public string AddROI(string rOIName, ROIData rOIData)
        {
            //ROI数据未实例化
            if (rOIData == null)
            {
                //添加失败
                return null;
            }
            //获取添加的ROI名称
 
            string name = "ROI" + RoiNum;
            while(true)
            {
                //名称已存在ROI字典键中
                if (hWndAction.ROIDatas.ContainsKey(name))
                {
                    RoiNum++;
                    name = "ROI" + RoiNum;
                }
                else
                {
                    break;
                }
            }
     
            //添加ROI到字典
            hWndAction.ROIDatas[name] = rOIData;
            //设置ROI名称
            hWndAction.ROIDatas[name].SetROIName(name);
            //订阅刷新显示事件
            hWndAction.ROIDatas[name].RefreshDisplay += RefreshDisplay;
            //刷新显示
            RefreshDisplay();
            //添加成功
            return name;
        }
        #endregion

        #region 删除ROI
        /// <summary>
        /// 删除ROI
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        /// <returns>true:删除成功 false:删除失败</returns>
        public bool DeleteROI(string rOIName)
        {
            //ROI名称为null
            if (rOIName == null)
            {
                //删除失败
                return false;
            }
            //ROI名称不存ROI字典中
            if (!hWndAction.ROIDatas.ContainsKey(rOIName))
            {
                //删除失败
                return false;
            }
            //删除ROI
            hWndAction.ROIDatas.Remove(rOIName);
            //刷新显示
            RefreshDisplay();
            //删除成功
            return true;
        }

        /// <summary>
        /// 删除全部ROI
        /// </summary>
        public void DeleteAllROI()
        {
            //删除全部ROI
            hWndAction.ROIDatas.Clear();
            //刷新显示
            RefreshDisplay();
        }
        #endregion

        #region 订阅||取消订阅ROI刷新显示事件
        /// <summary>
        /// 订阅ROI刷新显示事件
        /// </summary>
        private void SubscriptionROIRefreshDisplayEvent()
        {
            //遍历ROI数据字典
            foreach (var item in hWndAction.ROIDatas)
            {
                //订阅刷新显示事件
                item.Value.RefreshDisplay += RefreshDisplay;
            }
        }

        /// <summary>
        /// 取消订阅ROI刷新显示事件
        /// </summary>
        private void UnsubscribeROIRefreshDisplayEvent()
        {
            //遍历ROI数据字典
            foreach (var item in hWndAction.ROIDatas)
            {
                //订阅刷新显示事件
                item.Value.RefreshDisplay -= RefreshDisplay;
            }
        }
        #endregion

        #region 设置ROI激活
        /// <summary>
        /// 设置ROI激活
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        public void SetROIActivate(string rOIName)
        {
            //设置ROI激活
            hWndAction.SetROIActivate(rOIName);
        }
        #endregion

        #region ROI正在移动时发生
        /// <summary>
        /// ROI正在移动时发生
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        /// <param name="hTuple">ROI数据</param>
        /// <param name="hRegion">ROI区域对象</param>
        private void HWndAction_ROIMoving(string rOIName, HTuple hTuple, HRegion hRegion)
        {
            //触发ROI正在移动时事件
            ROIMoving?.Invoke(rOIName, hTuple, hRegion);
        }
        #endregion

        #region ROI移动完成时发生
        /// <summary>
        /// ROI移动完成时发生
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        /// <param name="hTuple">ROI数据</param>
        /// <param name="hRegion">ROI区域对象</param>
        private void HWndAction_ROIMoveDone(string rOIName, HTuple hTuple, HRegion hRegion)
        {
            //触发ROI移动完成事件
            ROIMoveDone?.Invoke(rOIName, hTuple, hRegion);
        }
        #endregion

        #region ROI激活完成时发生
        /// <summary>
        /// ROI激活完成时发生
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        /// <param name="hTuple">ROI数据</param>
        /// <param name="hRegion">ROI区域对象</param>
        private void HWndAction_ROIActivateDone(string rOIName, HTuple hTuple, HRegion hRegion)
        {
            //触发ROI激活完成事件
            ROIActivateDone?.Invoke(rOIName, hTuple, hRegion);
        }
        #endregion

        #region 绑定涂抹数据
        /// <summary>
        /// 绑定涂抹数据
        /// </summary>
        /// <param name="smearDatas">涂抹数据</param>
        public void BindingSmearDatas(Dictionary<string, SmearData> smearDatas)
        {
            //涂抹数据字典未实例化
            if (smearDatas == null)
            {
                //结束方法
                return;
            }
            //取消订阅涂抹刷新显示事件
            UnsubscribeSmearRefreshDisplayEvent();
            //更新涂抹数据字典
            hWndAction.SmearDatas = smearDatas;
            //遍历涂抹数据字典
            foreach (var item in smearDatas)
            {
                //设置涂抹名称
                item.Value.SetSmearName(item.Key);
                //涂抹工作完成(防止没窗体时外面触发了涂抹)
                item.Value.ISmearAction.WorkDone();
            }
            //订阅涂抹刷新显示事件
            SubscriptionSmearRefreshDisplayEvent();
            //刷新显示
            RefreshDisplay();
        }
        #endregion

        #region 添加涂抹
        /// <summary>
        /// 添加涂抹
        /// </summary>
        /// <param name="smearName">涂抹名称(null时自动分配名称)</param>
        /// <param name="smearData">涂抹数据</param>
        /// <returns>添加成功返回涂抹名称，失败返回null</returns>
        public string AddSmear(string smearName, SmearData smearData)
        {
            //涂抹数据未实例化
            if (smearData == null)
            {
                //添加失败
                return null;
            }
            //获取添加的涂抹名称
            string name = "Smear" + SmearNum;
            SmearNum++;
            //名称已存在涂抹字典键中
            if (hWndAction.SmearDatas.ContainsKey(name))
            {
                //添加失败
                return null;
            }
            //添加涂抹到字典
            hWndAction.SmearDatas[name] = smearData;
            //设置涂抹名称
            hWndAction.SmearDatas[name].SetSmearName(name);
            //订阅刷新显示事件
            hWndAction.SmearDatas[name].RefreshDisplay += RefreshDisplay;
            //刷新显示
            RefreshDisplay();
            //添加成功
            return name;
        }
        #endregion

        #region 删除涂抹
        /// <summary>
        /// 删除指定涂抹
        /// </summary>
        /// <param name="smearName">涂抹名称</param>
        /// <returns>true:删除成功 false:删除失败</returns>
        public bool DeleteSmear(string smearName)
        {
            //涂抹名称为null
            if (smearName == null)
            {
                //删除失败
                return false;
            }
            //涂抹名称不存涂抹字典中
            if (!hWndAction.SmearDatas.ContainsKey(smearName))
            {
                //删除失败
                return false;
            }
            //当前要删除的涂抹是当前活动的涂抹
            if (hWndAction.ActionSmear == smearName)
            {
                //关闭涂抹
                hWndAction.CloseSmear();
            }
            //删除涂抹
            hWndAction.SmearDatas.Remove(smearName);
            //刷新显示
            RefreshDisplay();
            //删除成功
            return true;
        }

        /// <summary>
        /// 删除全部涂抹
        /// </summary>
        public void DeleteAllSmear()
        {
            //关闭涂抹
            hWndAction.CloseSmear();
            //删除全部涂抹
            hWndAction.SmearDatas.Clear();
            //刷新显示
            RefreshDisplay();
        }
        #endregion

        #region 订阅||取消订阅涂抹刷新显示事件
        /// <summary>
        /// 订阅涂抹刷新显示事件
        /// </summary>
        private void SubscriptionSmearRefreshDisplayEvent()
        {
            //遍历涂抹数据字典
            foreach (var item in hWndAction.SmearDatas)
            {
                //订阅刷新显示事件
                item.Value.RefreshDisplay += RefreshDisplay;
            }
        }

        /// <summary>
        /// 取消订阅涂抹刷新显示事件
        /// </summary>
        private void UnsubscribeSmearRefreshDisplayEvent()
        {
            //遍历涂抹数据字典
            foreach (var item in hWndAction.SmearDatas)
            {
                //取消订阅刷新显示事件
                item.Value.RefreshDisplay -= RefreshDisplay;
            }
        }
        #endregion

        #region 开始涂抹
        /// <summary>
        /// 开始涂抹
        /// </summary>
        /// <param name="smearName">涂抹名称</param>
        /// <param name="type">类型</param>
        public void StartSmear(string smearName, SmearTypeEnum type)
        {
            //开始涂抹
            hWndAction.StartSmear(smearName, type);
        }
        #endregion

        #region 关闭涂抹
        /// <summary>
        /// 关闭涂抹
        /// </summary>
        public void CloseSmear()
        {
            //关闭涂抹
            hWndAction.CloseSmear();
            //刷新显示(消除涂抹工具)
            RefreshDisplay();
        }
        #endregion

        #region 涂抹按下或按下移动时发生
        /// <summary>
        /// 涂抹按下或按下移动时发生
        /// </summary>
        /// <param name="smearName">涂抹名称</param>
        /// <param name="hRegion">涂抹区域</param>
        private void HWndAction_SmearPressOrMove(string smearName, HRegion hRegion)
        {
            //触发涂抹按下或按下移动时事件
            SmearPressOrMove?.Invoke(smearName, hRegion);
        }
        #endregion

        #region 更新状态显示
        /// <summary>
        /// 更新状态显示
        /// </summary>
        /// <param name="text">状态信息文本</param>
        private void HWndCtrl_UpdateStatusDisplay(string text)
        {
            //更新文本状态
            string textStatus = DisplayWindowNumber ? $"{WindowNumber}  {text}" : text;
            //更新状态显示
            label_Status.Text = textStatus;
        }
        #endregion

        #region 获取原始图像
        /// <summary>
        /// 获取原始图像
        /// </summary>
        /// <returns>原始图像</returns>
        public HImage GetOriginalImage()
        {
            //图像已实例化 && 已初始化
            if (image != null && image.IsInitialized())
            {
                //返回复制的原始图像
                return image.CopyObj(1, -1);
            }
            //返回null
            return null;
        }
        #endregion

        #region 截取Halcon窗口图像
        /// <summary>
        /// 截取Halcon窗口图像
        /// </summary>
        /// <param name="adapt">适应图像大小后截取图像</param>
        /// <returns>截取的图像</returns>
        public HImage CaptureHWindowImage(bool adapt = true)
        {
            //图像已实例化 && 已初始化
            if (image != null && image.IsInitialized())
            {
                //适应窗口图像
                if (adapt)
                {
                    //获取左上角和右下角坐标
                    hWindowControl.HalconWindow.GetPart(out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    //适应图像显示
                    hWndAction.AdaptToImagesDisplay();
                    //获取窗口截取的图像
                    HImage hImage = hWindowControl.HalconWindow.DumpWindowImage();
                    //返回适应前窗口大小
                    hWindowControl.HalconWindow.SetPart(row1, column1, row2, column2);
                    //刷新显示
                    RefreshDisplay();
                    //返回窗口截取的图像
                    return hImage;
                }
                else
                {
                    //获取窗口截取的图像
                    HImage hImage = hWindowControl.HalconWindow.DumpWindowImage();
                    //返回窗口截取的图像
                    return hImage;
                }
            }
            //返回null
            return null;
        }
        #endregion

        #region 获取用户文本集合
        /// <summary>
        /// 获取用户文本集合
        /// </summary>
        /// <returns>用户文本集合</returns>
        public List<HTextData> GetUserTextList()
        {
            //返回复制的用户文本集合
            return new List<HTextData>(hWndAction.UserTextList);
        }
        #endregion

        #region 获取用户显示对象集合
        /// <summary>
        /// 获取用户显示对象集合
        /// </summary>
        /// <returns>用户显示对象集合</returns>
        public List<HObjData> GetUsersHObjList()
        {
            //返回复制的用户显示对象集合
            return new List<HObjData>(hWndAction.UsersHObjList);
        }
        #endregion

        #region 右键菜单功能
        private void 适应图像显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //适应图像显示
            hWndAction.AdaptToImagesDisplay();
        }

        private void 显示隐藏十字架ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //更新显示十字架状态
            DisplayCrosses = !DisplayCrosses;
        }

        private void 显示隐藏状态栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //更新状态显示
            DisplayStatus = !DisplayStatus;
        }

        private void 显示隐藏坐标指示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //更新坐标指示状态
            DisplayCoordinate = !DisplayCoordinate;
        }

        private void 保存原始图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //图像已实例化 && 已初始化
            if (this.hWndAction.image != null && this.hWndAction.image.IsInitialized())
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "BMP图像|*.bmp|所有文件|*.*";
                //用户选择确认
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    //获取到的路径为null或""
                    if (String.IsNullOrEmpty(sfd.FileName))
                    {
                        //结束方法
                        return;
                    }
                    //保存图像
                    this.hWndAction.image.WriteImage("bmp", 0, sfd.FileName);
                }
            }
        }

        private void 保存窗口截取图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //图像已实例化 && 已初始化
            if (image != null && image.IsInitialized())
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG图像|*.png|所有文件|*.*";
                //用户选择确认
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    //获取到的路径为null或""
                    if (String.IsNullOrEmpty(sfd.FileName))
                    {
                        //结束方法
                        return;
                    }
                    //截取当前窗口图像
                    HImage hImage = hWindowControl.HalconWindow.DumpWindowImage();
                    //保存图像
                    hImage.WriteImage("png", 0, sfd.FileName);
                }
            }
        }

        private void 显示隐藏像素格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //更新显示像素网格状态
            DisplayPixelGrid = !DisplayPixelGrid;
        }

        /// <summary>
        /// 全屏显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 全屏显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //检查对象类型实例
            if (sender is ToolStripMenuItem tSMI)
            {
                //当前不是全屏状态
                if (!tSMI.Checked)
                {
                    tSMI.Checked = true;
                    tSMI.Text = "退出全屏(Esc)";
                    //进入全屏显示
                    fullScreenDisplay = new FullScreenDisplay(this);
                    fullScreenDisplay.ShowDialog();
                }
                else
                {
                    tSMI.Checked = false;
                    tSMI.Text = "进入全屏(f)";
                    //退出全屏显示
                    fullScreenDisplay.ExitFullScreen();
                }
            }
        }
        #endregion

        #region 在鼠标指针移到halcon控件上时发生
        /// <summary>
        /// 在鼠标指针移到halcon控件上时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {
            //鼠标进行控件自动获取焦点
            if (automaticFocus)
            {
                //为控件设置输入焦点(这里为了不用点击控件就可使用全屏快捷键f,如果会造成焦点竞争可把鼠标进入自动获取焦点关闭)
                this.Focus();
            }
        }
        #endregion
    }

    #region 坐标模式
    /// <summary>
    /// 坐标模式
    /// </summary>
    public enum CoordinateMode
    {
        /// <summary>
        /// XY模式
        /// </summary>
        XY,
        /// <summary>
        /// Row_Col模式
        /// </summary>
        Row_Col,
    }
    #endregion
}
