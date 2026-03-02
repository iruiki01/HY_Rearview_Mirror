using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static hWindowTool.HWTools;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace hWindowTool
{
    /// <summary>
    /// HalconWindow行为
    /// </summary>
    public class HWndAction
    {
        #region 构造方法
        /// <summary>
        /// HalconWindow行为构造函数
        /// </summary>
        /// <param name="hWindowControl1">HWindowControl1</param>
        public HWndAction(HWindowControl hWindowControl1)
        {
            //更新hWindow 控件
            this.hWindowControl1 = hWindowControl1;
            //订阅鼠标按下事件
            this.hWindowControl1.HMouseDown += HWindowControl_HMouseDown;
            //订阅鼠标释放事件
            this.hWindowControl1.HMouseUp += HWindowControl_HMouseUp;
            //订阅鼠标移动事件
            this.hWindowControl1.HMouseMove += HWindowControl_HMouseMove;
            //订阅鼠标滚动事件
            this.hWindowControl1.HMouseWheel += HWindowControl_HMouseWheel;
            //订阅控件大小更改时事件
            this.hWindowControl1.SizeChanged += HWindowControl_SizeChanged;
        }
        #endregion

        #region 属性字段
        /// <summary>
        /// HWindowControl1
        /// </summary>
        public HWindowControl hWindowControl1;

        /// <summary>
        /// 当前显示图像
        /// </summary>
        public HImage image = new HImage();

        /// <summary>
        /// 底图
        /// </summary>
        private HImage baseImage;

        /// <summary>
        /// 图像宽高
        /// </summary>
        private int imageWide, imageHigh;

        /// <summary>
        /// 鼠标按下时的坐标
        /// </summary>
        private double mouseDownRow, mouseDownColumn;

        /// <summary>
        /// 当前鼠标位置
        /// </summary>
        private double mouseRow, mouseColumn;

        /// <summary>
        /// 左键按下标志
        /// </summary>
        private bool press;

        /// <summary>
        /// 控件宽高
        /// </summary>
        private int controlWide, controlHigh;

        /// <summary>
        /// 图像比例
        /// </summary>
        private double scale;

        /// <summary>
        /// 显示十字架状态
        /// </summary>
        private bool displayCrosses;
        /// <summary>
        /// 显示十字架状态
        /// </summary>
        public bool DisplayCrosses
        {
            get { return displayCrosses; }
            set
            {
                displayCrosses = value;
                //显示图像
                DisplayImage();
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
            set { drawModel = value; }
        }

        /// <summary>
        /// 显示坐标指示
        /// </summary>
        private bool displayCoordinate;
        /// <summary>
        /// 显示坐标指示
        /// </summary>
        public bool DisplayCoordinate
        {
            get { return displayCoordinate; }
            set
            {
                displayCoordinate = value;
                //显示图像
                DisplayImage();
            }
        }

        /// <summary>
        /// 坐标占图像大小比例
        /// </summary>
        public double CoordinateProportion { get; set; }

        /// <summary>
        /// 坐标文本大小
        /// </summary>
        public int CoordinateTextSize { get; set; }

        /// <summary>
        /// 坐标模式
        /// </summary>
        public CoordinateMode CoordinateMode { get; set; }

        /// <summary>
        /// 显示像素网格
        /// </summary>
        private bool displayPixelGrid;
        /// <summary>
        /// 显示像素网格
        /// </summary>
        public bool DisplayPixelGrid
        {
            get { return displayPixelGrid; }
            set
            {
                if (displayPixelGrid != value)
                {
                    displayPixelGrid = value;
                    //显示图像
                    DisplayImage();
                }
            }
        }

        /// <summary>
        /// 像素网格
        /// </summary>
        public int PixelGrid { get; set; }

        /// <summary>
        /// 缩放最小时自适应
        /// </summary>
        public bool MinZoom { get; set; }

        /// <summary>
        /// 当前正在行动的ROI
        /// </summary>
        private string actionROI;

        /// <summary>
        /// 最新选定ROI
        /// </summary>
        private string newROI;
        /// <summary>
        /// 最新选定ROI
        /// </summary>
        protected string NewROI
        {
            get { return newROI; }
            set
            {
                //新选定的ROI与旧的ROI不是同一个
                if (newROI != value)
                {
                    //上次选定的ROI不为null && 存在ROI处理中
                    if (newROI != null && ROIDatas.ContainsKey(newROI))
                    {
                        //关闭ROI激活
                        ROIDatas[newROI].ROI.IROIAction.Activate = false;
                    }
                    //更新最新选定ROI
                    newROI = value;
                }
            }
        }

        /// <summary>
        /// 当前正在行动的涂抹
        /// </summary>
        private string actionSmear;
        /// <summary>
        /// 当前正在行动的涂抹
        /// </summary>
        public string ActionSmear { get { return actionSmear; } }

        /// <summary>
        /// 工具
        /// </summary>
        private HWTools tools = new HWTools();

        /// <summary>
        /// 用户显示对象集合
        /// </summary>
        public List<HObjData> UsersHObjList = new List<HObjData>();

        /// <summary>
        /// 用户显示文本集合
        /// </summary>
        public List<HTextData> UserTextList = new List<HTextData>();

        /// <summary>
        /// ROI数据字典
        /// </summary>
        public Dictionary<string, ROIData> ROIDatas = new Dictionary<string, ROIData>();

        /// <summary>
        /// 模板轮廓字典
        /// </summary>
        public Dictionary<string, HObject> ContourDatas = new Dictionary<string, HObject>();

        /// <summary>
        /// 涂抹数据字典
        /// </summary>
        public Dictionary<string, SmearData> SmearDatas = new Dictionary<string, SmearData>();

        /// <summary>
        /// 更新状态显示事件
        /// 参数: 当前图像宽高、坐标、灰度
        /// </summary>
        public event Action<string> UpdateStatusDisplay;

        /// <summary>
        /// ROI正在移动时事件
        /// 参数:ROI名称,ROI数据,ROI区域对象
        /// </summary>
        public event Action<string, HTuple, HRegion> ROIMoving;

        /// <summary>
        /// ROI移动完成事件
        /// 参数:ROI名称,ROI数据,ROI区域对象
        /// </summary>
        public event Action<string, HTuple, HRegion> ROIMoveDone;

        /// <summary>
        /// ROI激活完成事件
        /// 参数:ROI名称,ROI数据,ROI区域对象
        /// </summary>
        public event Action<string, HTuple, HRegion> ROIActivateDone;

        /// <summary>
        /// 涂抹按下或按下移动时事件
        /// 参数:涂抹名称,涂抹区域
        /// </summary>
        public event Action<string, HRegion> SmearPressOrMove;
        #endregion

        #region 鼠标按下事件
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseDown(object sender, HMouseEventArgs e)
        {
            //鼠标左键按下
            if (e.Button == MouseButtons.Left)
            {
                //更新鼠标按下时的坐标
                mouseDownRow = e.Y;
                mouseDownColumn = e.X;
                //不是绘制模式
                if (!DrawModel)
                {
                    //更新光标样式
                    hWindowControl1.Cursor = Cursors.Cross;
                }
                //没有在行动中的涂抹
                if (actionSmear == null)
                {
                    //遍历ROI处理
                    foreach (var item in ROIDatas)
                    {
                        //检查ROI是否行动
                        actionROI = item.Value.ROI.IROIAction.IsAction(mouseDownRow, mouseDownColumn);
                        //捕获到行动中ROI
                        if (actionROI != null)
                        {
                            //更新最新选定ROI
                            NewROI = actionROI;
                            //显示图像(IsAction()时如果是在小矩形按下会自己激活,防止未移动前不显示激活效果)
                            DisplayImage();
                            //存在最新绑定的ROI
                            if (ROIDatas.ContainsKey(NewROI))
                            {
                                //获取ROI数据
                                ROIDatas[NewROI].ROI.IROIAction.GetAllROIData(out HTuple hTuple, out HRegion hRegion, out HXLDCont hXLDCont);
                                //触发ROI激活完成事件
                                ROIActivateDone?.Invoke(NewROI, hTuple, hRegion);
                            }
                            //结束遍历
                            break;
                        }
                    }
                }
                //有在行动中的涂抹
                if (actionSmear != null)
                {
                    //涂抹处理存在行动中的涂抹
                    if (SmearDatas.ContainsKey(actionSmear))
                    {
                        //涂抹开始工作
                        SmearDatas[actionSmear].ISmearAction.SmearWork(mouseRow, mouseColumn);
                        //触发涂抹按下或按下移动时事件
                        SmearPressOrMove?.Invoke(actionSmear, SmearDatas[actionSmear].ISmearAction.GetRegion());
                        //显示图像
                        DisplayImage();
                    }
                }
                //打开左键按下标志
                press = true;
            }
        }
        #endregion

        #region 鼠标释放事件
        /// <summary>
        /// 鼠标释放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseUp(object sender, HMouseEventArgs e)
        {
            //更新光标样式
            hWindowControl1.Cursor = Cursors.Default;
            //放行动中的ROI不为null
            if (actionROI != null)
            {
                //行动中ROI存在ROI数据字典中
                if (ROIDatas.ContainsKey(actionROI))
                {
                    //获取ROI数据
                    ROIDatas[actionROI].ROI.IROIAction.GetAllROIData(out HTuple hTuple, out HRegion hRegion, out HXLDCont hXLDCont);
                    //触发ROI移动完成事件
                    ROIMoveDone?.Invoke(actionROI, hTuple, hRegion);
                }
                //释放行动中的ROI
                actionROI = null;
            }
            //关闭左键按下标志
            press = false;
            //显示图像
            DisplayImage();
        }
        #endregion

        #region 鼠标移动事件
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseMove(object sender, HMouseEventArgs e)
        {
            //图像未实例化 || 正在更新图像
            if (image == null || updateImage)
            {
                //结束方法
                return;
            }
            //更新当前鼠标位置
            mouseRow = e.Y;
            mouseColumn = e.X;
            try
            {
                //鼠标在控件范围内
                if (mouseRow != -1 && mouseColumn != -1)
                {
                    //在Row范围内 && 在Column范围内
                    if ((mouseRow > 0 && mouseRow < imageHigh) && (mouseColumn > 0 && mouseColumn < imageWide))
                    {
                        //获取当前点的灰度值
                        HOperatorSet.GetGrayval(image, mouseRow, mouseColumn, out HTuple pointGray);
                        //判断坐标模式
                        switch (CoordinateMode)
                        {
                            case CoordinateMode.XY:
                                //触发更新状态显示事件
                                UpdateStatusDisplay?.Invoke($"W:{imageWide} H:{imageHigh} | X:{mouseColumn:f0} Y:{mouseRow:f0} | Gray:{pointGray}");
                                break;
                            case CoordinateMode.Row_Col:
                                //触发更新状态显示事件
                                UpdateStatusDisplay?.Invoke($"W:{imageWide} H:{imageHigh} | Row:{mouseRow:f0} Col:{mouseColumn:f0} | Gray:{pointGray}");
                                break;
                        }

                    }
                    //左键按下
                    if (press)
                    {
                        //有在行动中的涂抹
                        if (actionSmear != null)
                        {
                            //涂抹处理存在行动中的涂抹
                            if (SmearDatas.ContainsKey(actionSmear))
                            {
                                //涂抹开始工作
                                SmearDatas[actionSmear].ISmearAction.SmearWork(mouseRow, mouseColumn);
                                //触发涂抹按下或按下移动时事件
                                SmearPressOrMove?.Invoke(actionSmear, SmearDatas[actionSmear].ISmearAction.GetRegion());
                                //显示图像
                                DisplayImage();
                            }
                        }
                        //有在行动中的ROI
                        else if (actionROI != null)
                        {
                            //ROI处理存在行动中的ROI
                            if (ROIDatas.ContainsKey(actionROI))
                            {
                                //ROI移动
                                ROIDatas[actionROI].ROI.IROIAction.ROIMove(mouseRow, mouseColumn);
                                //获取ROI数据
                                ROIDatas[actionROI].ROI.IROIAction.GetAllROIData(out HTuple hTuple, out HRegion hRegion, out HXLDCont hXLDCont);
                                //触发ROI正在移动时事件
                                ROIMoving?.Invoke(actionROI, hTuple, hRegion);
                                //显示图像
                                DisplayImage();
                            }
                        }
                        //不是绘制模式
                        else if (!DrawModel)
                        {
                            //当前鼠标行坐标减去按下时的行坐标，得到行坐标的移动值
                            double rowMove = mouseRow - mouseDownRow;
                            //当前鼠标列坐标减去按下时的列坐标，得到列坐标的移动值
                            double colMove = mouseColumn - mouseDownColumn;
                            //获取图像在窗口左上角和右下角坐标
                            hWindowControl1.HalconWindow.GetPart(out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                            //设置图像在窗口左上角和右下角坐标
                            hWindowControl1.HalconWindow.SetPart(row1 - rowMove, column1 - colMove, row2 - rowMove, column2 - colMove);
                            //显示图像
                            DisplayImage();
                        }
                    }
                    else
                    {
                        //有在行动中的涂抹
                        if (actionSmear != null)
                        {
                            //显示图像(让涂抹工具跟随鼠标移动)
                            DisplayImage();
                        }
                        else
                        {
                            //定义光标进入标志
                            bool cursorEntry = false;
                            //复制ROI数据字典
                            Dictionary<string, ROIData> rOIDatas = new Dictionary<string, ROIData>(ROIDatas);
                            //遍历ROI数据字典
                            foreach (var item in rOIDatas)
                            {
                                //ROI不可见
                                if (!item.Value.Visible)
                                {
                                    //遍历下一个
                                    continue;
                                }
                                //光标进入小矩形
                                if (rOIDatas[item.Key].ROI.IROIAction.IsEnterMinRectangle(mouseRow, mouseColumn))
                                {
                                    //打开光标进入标志
                                    cursorEntry = true;
                                    //结束遍历
                                    break;
                                }
                            }
                            //光标进入
                            if (cursorEntry)
                            {
                                //更新光标样式
                                hWindowControl1.Cursor = Cursors.Hand;
                            }
                            else
                            {
                                //更新光标样式
                                hWindowControl1.Cursor = Cursors.Arrow;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region 鼠标滚动事件
        /// <summary>
        /// 当前缩放因子
        /// </summary>
        private double currentZoom = 1;
        /// <summary>
        /// 每次缩放比例
        /// </summary>
        private double eachZoom = 1.2;
        /// <summary>
        /// 鼠标滚动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseWheel(object sender, HMouseEventArgs e)
        {
            //图像未实例化 || 绘制模式禁止平移缩放 || 正在更新图像
            if (image == null || DrawModel || updateImage)
            {
                //结束方法
                return;
            }
            //更新当前鼠标位置
            mouseRow = e.Y;
            mouseColumn = e.X;
            //根据滚动方向更新缩放比例
            HTuple Zoom = e.Delta > 0 ? eachZoom : 1 / eachZoom;
            //获取图像在窗口左上角和右下角坐标
            hWindowControl1.HalconWindow.GetPart(out HTuple lTRow, out HTuple lTColumn, out HTuple rBRow, out HTuple rBColumn);
            HTuple Ht = rBRow - lTRow;
            HTuple Wt = rBColumn - lTColumn;
            //可以继续放大 || 可以继续缩小
            if ((Zoom > 1 && rBRow - lTRow > 0.1 && rBColumn - lTColumn > 0.1) || (Zoom < 1 && currentZoom / eachZoom > 0.01))
            {
                //更新当前缩放因子
                currentZoom = Zoom > 1 ? currentZoom * eachZoom : currentZoom / eachZoom;
                HTuple r1 = (lTRow + ((1 - (1.0 / Zoom)) * (mouseRow - lTRow)));
                HTuple c1 = (lTColumn + ((1 - (1.0 / Zoom)) * (mouseColumn - lTColumn)));
                HTuple r2 = r1 + (Ht / Zoom);
                HTuple c2 = c1 + (Wt / Zoom);
                hWindowControl1.HalconWindow.SetPart(r1, c1, r2, c2);
                //显示图像
                DisplayImage();
            }
            //执行缩小时已到最小 && 缩放最小时自适应
            else if (Zoom < 1 && MinZoom)
            {
                //适应图像
                AdaptToImages();
            }
        }
        #endregion

        #region 控件大小更改时事件
        /// <summary>
        /// 控件大小已更改
        /// </summary>
        private bool sizeChanged;
        /// <summary>
        /// 控件大小更改时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_SizeChanged(object sender, EventArgs e)
        {
            //创建基础位图
            Bitmap bitmap = tools.CreateBaseBitmap(hWindowControl1.ClientRectangle.Width, hWindowControl1.ClientRectangle.Height);
            //基础位图创建成功
            if (bitmap != null)
            {
                //将Bitmap类型转换成HObject类型
                tools.Bitmap2HObjectBpp24(bitmap, out HObject hObject);
                //转换成功
                if (hObject != null)
                {
                    //实例化底图
                    baseImage = new HImage(hObject);
                }
            }
            //控件大小已更改
            sizeChanged = true;
            //显示图像
            DisplayImage();
        }
        #endregion

        #region 清除窗口
        /// <summary>
        /// 清除窗口
        /// </summary>
        internal void ClearWindow()
        {
            //图像对象设置为null
            image = null;
            //清除图像宽高(防止下次打开相同宽高图像不进行自适应)
            imageWide = 0;
            imageHigh = 0;
            //显示图像(图像是null的时默认只显示底图)
            DisplayImage();
        }
        #endregion

        #region 显示图像
        /// <summary>
        /// 更新图像标志
        /// </summary>
        private bool updateImage;
        /// <summary>
        /// 显示图像线程锁
        /// </summary>
        private object displayImageLock = new object();
        /// <summary>
        /// 左上角和右下角位置
        /// </summary>
        private double partRow1, partColumn1, partRow2, partColumn2;
        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="hobject">图像对象</param>
        public void DisplayImage()
        {
            lock (displayImageLock)
            {
                try
                {
                    //对象未实例化
                    if (image == null)
                    {
                        //底图已实例化 && 底图已初始化
                        if (baseImage != null && baseImage.IsInitialized())
                        {
                            //关闭立即显示图形
                            HOperatorSet.SetSystem("flush_graphic", "false");
                            //清除窗口
                            hWindowControl1.HalconWindow.ClearWindow();
                            //自适应底图
                            AdaptiveBaseImage();
                            //显示底图
                            hWindowControl1.HalconWindow.DispObj(baseImage);
                            //打开立即显示图形
                            HOperatorSet.SetSystem("flush_graphic", "true");
                            //在HW上写字符(目的让关闭立即显示后的对象重新显示)
                            hWindowControl1.HalconWindow.WriteString("");
                        }
                        //结束方法
                        return;
                    }
                    //打开更新图像标志
                    updateImage = true;
                    //更新图像字段
                    //image = new HImage(hobject);
                    //获取图像宽高
                    image.GetImageSize(out int imageWide, out int imageHigh);
                    //图像大小更改标志
                    bool sizeChange = false;
                    //获取控件大小
                    int controlWide = hWindowControl1.ClientRectangle.Width;
                    int controlHigh = hWindowControl1.ClientRectangle.Height;
                    //新图像和旧图像大小不一致 
                    if (this.imageWide != imageWide || this.imageHigh != imageHigh || this.controlWide != controlWide || this.controlHigh != controlHigh)
                    {
                        //图像大小已更改
                        sizeChange = true;
                        //更新图像宽高
                        this.imageWide = imageWide;
                        this.imageHigh = imageHigh;
                        //更新控件宽高
                        this.controlWide = controlWide;
                        this.controlHigh = controlHigh;
                    }
                    //关闭立即显示图形
                    HOperatorSet.SetSystem("flush_graphic", "false");
                    //清除窗口
                    hWindowControl1.HalconWindow.ClearWindow();
                    //获取左上角和右下角坐标
                    hWindowControl1.HalconWindow.GetPart(out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    //底图已实例化
                    if (baseImage != null)
                    {
                        //自适应底图
                        AdaptiveBaseImage();
                        //显示底图
                        hWindowControl1.HalconWindow.DispObj(baseImage);
                    }
                    //图像大小更改
                    if (sizeChange)
                    {
                        //适应图像
                        AdaptToImages();
                    }
                    else
                    {
                        //设置适应图像(适应底图前的数据)
                        hWindowControl1.HalconWindow.SetPart(row1, column1, row2, column2);
                    }
                    //显示图像
                    hWindowControl1.HalconWindow.DispObj(image);
                    //显示用户对象
                    DisplayUsersHObj();
                    //显示用户文本
                    DisplayUsersText();
                    //显示涂抹
                    DeleteSmear();
                    //缩放或移动了图像
                    if (partRow1 != row1 || partColumn1 != column1 || partRow2 != row2 || partColumn2 != column2)
                    {
                        //更新左上角或右下角位置
                        partRow1 = row1;
                        partColumn1 = column1;
                        partRow2 = row2;
                        partColumn2 = column2;
                        //(row小于或等于设置的像素网格 || column小于或等于设置的像素网格) && 控件大小未更改 && 显示像素网格
                        if ((row2 - row1 <= PixelGrid || column2 - column1 <= PixelGrid) && !sizeChanged && DisplayPixelGrid)
                        {
                            //绘制像素网格
                            DrawPixelGrid(row1, column1, row2, column2);
                        }
                    }
                    //(row小于或等于设置的像素网格 || column小于或等于设置的像素网格) && 控件大小未更改 && 显示像素网格
                    if ((row2 - row1 <= PixelGrid || column2 - column1 <= PixelGrid) && !sizeChanged && DisplayPixelGrid)
                    {
                        //显示像素网格
                        DisplayHObj(new HObjData(pixelGrid, HColorEnum.dim_gray, true, false, 1));
                    }
                    //关闭控件大小已更改
                    sizeChanged = false;
                    //显示或隐藏十字架
                    ShowOrHideCrosses();
                    //显示或隐藏坐标指示
                    ShowOrHideCoordinate();
                    //显示ROI
                    DisplayROI();
                    //显示模板轮廓
                    DisplayContour();
                    //打开立即显示图形
                    HOperatorSet.SetSystem("flush_graphic", "true");
                    //在HW上写字符(目的让关闭立即显示后的对象重新显示)
                    hWindowControl1.HalconWindow.WriteString("");
                    //关闭更新图像标志
                    updateImage = false;
                }
                catch (Exception)
                {
                    //关闭更新图像标志
                    updateImage = false;
                    //throw;
                }
            }
        }
        #endregion

        #region 适应图像
        /// <summary>
        /// 适应图像
        /// </summary>
        private void AdaptToImages()
        {
            //图像未实例化
            if (image == null)
            {
                //结束方法
                return;
            }
            try
            {
                //更新当前缩放因子
                currentZoom = 1;
                //获取图像大小
                image.GetImageSize(out HTuple imgWidth, out HTuple imgHeight);
                //计算比例
                scale = Math.Max(1.0 * imgWidth.I / controlWide, 1.0 * imgHeight / controlHigh);
                double w = controlWide * scale;
                double h = controlHigh * scale;
                //居中时，Part的区域
                hWindowControl1.HalconWindow.SetPart(-(h - imgHeight) / 2, -(w - imgWidth) / 2, imgHeight + (h - imgHeight.D) / 2, imgWidth + (w - imgWidth) / 2);
            }
            catch(Exception)
            {
               
            }           
        }

        /// <summary>
        /// 适应图像显示
        /// </summary>
        public void AdaptToImagesDisplay()
        {
            //适应图像
            AdaptToImages();
            //显示图像
            DisplayImage();
        }
        #endregion

        #region 自适应底图
        /// <summary>
        /// 自适应底图
        /// </summary>
        private void AdaptiveBaseImage()
        {
            //获取底图宽高
            baseImage.GetImageSize(out int imageWide, out int imageHigh);
            //设置底图自适应
            hWindowControl1.HalconWindow.SetPart(0, 0, imageHigh - 1, imageWide - 1);
        }
        #endregion

        #region 绘制像素网格
        /// <summary>
        /// 像素网格
        /// </summary>
        private HXLDCont pixelGrid = new HXLDCont();
        /// <summary>
        /// 绘制像素网格
        /// </summary>
        /// <param name="row1">左上角row</param>
        /// <param name="column1">左上角column</param>
        /// <param name="row2">右下角row</param>
        /// <param name="column2">右下角column</param>
        private void DrawPixelGrid(double row1, double column1, double row2, double column2)
        {
            //定义左上角和右下角位置
            int partRow1 = (int)(row1 - 1);
            int partColumn1 = (int)(column1 - 1);
            int partRow2 = (int)(row2 + 1);
            int partColumn2 = (int)(column2 + 1);
            //创建像素网格空对象
            pixelGrid.GenEmptyObj();
            //绘制row方向像素网格
            for (int i = partRow1; i < partRow2; i++)
            {
                //定义直线起始和结束
                double beginRow = i + 0.5;
                double beginCol = partColumn1;
                double endRow = i + 0.5;
                double endCol = partColumn2 + 0.5;
                //创建row方向像素网格
                pixelGrid = pixelGrid.ConcatObj(new HXLDCont(new HTuple(beginRow, endRow), new HTuple(beginCol, endCol)));
            }
            //绘制column方向像素网格
            for (int i = partColumn1; i < partColumn2; i++)
            {
                //定义直线起始和结束
                double beginRow = partRow1;
                double beginCol = i + 0.5;
                double endRow = partRow2 + 0.5;
                double endCol = i + 0.5;
                //创建column方向像素网格
                pixelGrid = pixelGrid.ConcatObj(new HXLDCont(new HTuple(beginRow, endRow), new HTuple(beginCol, endCol)));
            }
        }
        #endregion

        #region 显示或隐藏十字架
        /// <summary>
        /// 显示或隐藏十字架
        /// </summary>
        private void ShowOrHideCrosses()
        {
            //显示十字架
            if (DisplayCrosses)
            {
                //定义短边10%长度变量
                int length;
                //找短边10%长度
                if (imageWide < imageHigh)
                {
                    length = Convert.ToInt32(imageWide * 0.1);
                }
                else
                {
                    length = Convert.ToInt32(imageHigh * 0.1);
                }
                //创建中心线
                HXLDCont line1 = new HXLDCont(new HTuple(new double[] { 0, imageHigh / 2 - length / 2 }),
                                              new HTuple(new double[] { imageWide / 2, imageWide / 2 }));
                HXLDCont line2 = new HXLDCont(new HTuple(new double[] { imageHigh / 2 + length / 2, imageHigh }),
                                              new HTuple(new double[] { imageWide / 2, imageWide / 2 }));
                HXLDCont line3 = new HXLDCont(new HTuple(new double[] { imageHigh / 2, imageHigh / 2 }),
                                              new HTuple(new double[] { 0, imageWide / 2 - length / 2 }));
                HXLDCont line4 = new HXLDCont(new HTuple(new double[] { imageHigh / 2, imageHigh / 2 }),
                                              new HTuple(new double[] { imageWide / 2 + length / 2, imageWide }));
                //创建图像中心十字架
                HXLDCont centre = new HXLDCont();
                centre.GenCrossContourXld(imageHigh / 2, imageWide / 2, length * 0.2, 0);
                //显示图像中心线和十字架
                DisplayHObj(new HObjData(line1, HColorEnum.blue, true, false, 2));
                DisplayHObj(new HObjData(line2, HColorEnum.blue, true, false, 2));
                DisplayHObj(new HObjData(line3, HColorEnum.blue, true, false, 2));
                DisplayHObj(new HObjData(line4, HColorEnum.blue, true, false, 2));
                DisplayHObj(new HObjData(centre, HColorEnum.blue, true, false, 2));
            }
        }
        #endregion

        #region 显示或隐藏坐标指示
        /// <summary>
        /// 显示或隐藏坐标指示
        /// </summary>
        private void ShowOrHideCoordinate()
        {
            //显示坐标指示
            if (DisplayCoordinate)
            {
                //定义坐标显示长度
                double length;
                //计算坐标显示长度
                if (imageWide < imageHigh)
                {
                    length = imageWide * CoordinateProportion;
                }
                else
                {
                    length = imageHigh * CoordinateProportion;
                }
                //计算边界距离
                double boundary = length * 0.2;
                //定义结束row，col →
                double endRow1 = boundary;
                double endCol1 = boundary + length;
                //定义结束row，col ↓
                double endRow2 = boundary + length;
                double endCol2 = boundary;
                //定义坐标指示轮廓添加直线1
                HXLDCont coordinate = new HXLDCont(new HTuple(new double[] { boundary, endRow1 }), new HTuple(new double[] { boundary, endCol1 }));
                {
                    //计算直线1弧度
                    double phi1 = HMisc.AngleLx(boundary, boundary, endRow1, endCol1);
                    //循环创建箭头直线
                    for (int i = 0; i < 2; i++)
                    {
                        //创建围绕直线1结束点旋转仿射变换矩阵
                        HHomMat2D hHomMat2D1 = new HHomMat2D();
                        if (i == 0)
                        {
                            hHomMat2D1.VectorAngleToRigid(endRow1, endCol1, 0, endRow1, endCol1, phi1 + 0.5);
                        }
                        else if (i == 1)
                        {
                            hHomMat2D1.VectorAngleToRigid(endRow1, endCol1, 0, endRow1, endCol1, phi1 + -0.5);
                        }
                        //进行仿射变换
                        double arrowRow = hHomMat2D1.AffineTransPoint2d(endRow1, endCol1 - boundary, out double arrowCol);
                        //添加直线1箭头直线
                        coordinate = coordinate.ConcatObj(new HXLDCont(new HTuple(new double[] { arrowRow, endRow1 }), new HTuple(new double[] { arrowCol, endCol1 })));
                    }
                }
                //定义坐标指示轮廓添加直线2
                coordinate = coordinate.ConcatObj(new HXLDCont(new HTuple(new double[] { boundary, endRow2 }), new HTuple(new double[] { boundary, endCol2 })));
                {
                    //计算直线2弧度
                    double phi2 = HMisc.AngleLx(boundary, boundary, endRow2, endCol2);
                    //循环创建箭头直线
                    for (int i = 0; i < 2; i++)
                    {
                        //创建围绕直线2结束点旋转仿射变换矩阵
                        HHomMat2D hHomMat2D2 = new HHomMat2D();
                        if (i == 0)
                        {
                            hHomMat2D2.VectorAngleToRigid(endRow2, endCol2, 0, endRow2, endCol2, phi2 + 0.5);
                        }
                        else if (i == 1)
                        {
                            hHomMat2D2.VectorAngleToRigid(endRow2, endCol2, 0, endRow2, endCol2, phi2 + -0.5);
                        }
                        //进行仿射变换
                        double arrowRow = hHomMat2D2.AffineTransPoint2d(endRow2, endCol2 - boundary, out double arrowCol);
                        //添加直线2箭头直线
                        coordinate = coordinate.ConcatObj(new HXLDCont(new HTuple(new double[] { arrowRow, endRow2 }), new HTuple(new double[] { arrowCol, endCol2 })));
                    }
                }
                //显示坐标轮廓
                DisplayHObj(new HObjData(coordinate, HColorEnum.cyan));
                //判断坐标模式
                switch (CoordinateMode)
                {
                    case CoordinateMode.XY:
                        //显示Column方向文本
                        DisplayHText(new HTextData("X", true, endRow1, endCol1, HColorEnum.cyan, CoordinateTextSize, false, false));
                        //显示ROW方向文本
                        DisplayHText(new HTextData("Y", true, endRow2, endCol2, HColorEnum.cyan, CoordinateTextSize, false, false));
                        break;
                    case CoordinateMode.Row_Col:
                        //显示Column方向文本
                        DisplayHText(new HTextData("Col", true, endRow1, endCol1, HColorEnum.cyan, CoordinateTextSize, false, false));
                        //显示ROW方向文本
                        DisplayHText(new HTextData("Row", true, endRow2, endCol2, HColorEnum.cyan, CoordinateTextSize, false, false));
                        break;
                }
            }
        }
        #endregion

        #region 显示用户对象
        /// <summary>
        /// 显示用户对象
        /// </summary>
        private void DisplayUsersHObj()
        {
            //复制用户对象集合
            List<HObjData> usersHObjList = new List<HObjData>(UsersHObjList);
            //遍历用户对象集合
            foreach (var item in usersHObjList)
            {
                //显示halcon对象-->显示失败
                if (!DisplayHObj(item))
                {
                    //对象设置错误,删除错误的对象
                    UsersHObjList.Remove(item);
                }
            }
        }
        #endregion

        #region 显示用户文本
        /// <summary>
        /// 显示用户文本
        /// </summary>
        private void DisplayUsersText()
        {
            //复制用户文本集合
            List<HTextData> userTextList = new List<HTextData>(UserTextList);
            //遍历用户文本集合
            foreach (var item in userTextList)
            {
                //文本可见
                if (item.VisibleText)
                {
                    //显示halcon文本
                    DisplayHText(item);
                }
            }
        }
        #endregion

        #region 显示ROI
        /// <summary>
        /// 显示ROI
        /// </summary>
        private void DisplayROI()
        {
            //涂抹正在工作中
            if (actionSmear != null)
            {
                //结束方法
                return;
            }
            //复制ROI数据字典
            Dictionary<string, ROIData> rOIDatas = new Dictionary<string, ROIData>(ROIDatas);

            //没右在行动中的ROI(显示全部ROI)
            if (actionROI == null)
            {
                //遍历ROI字典
                foreach (var item in rOIDatas)
                {
                    //ROI不可见
                    if (!item.Value.Visible)
                    {
                        //遍历下一个
                        continue;
                    }
                    //ROI处理不存在
                    if (!ROIDatas.ContainsKey(item.Key))
                    {
                        //遍历下一个
                        continue;
                    }
                    //定义图像短边
                    double imageShortEdge;
                    //获取左上角和右下角坐标
                    hWindowControl1.HalconWindow.GetPart(out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    //Row是短边
                    if (row2 - row1 < column2 - column1)
                    {
                        //图像缩小
                        if (row2 - row1 > imageHigh)
                        {
                            //直接按图像Row计算短边
                            imageShortEdge = imageHigh;
                        }
                        else
                        {
                            //使用当前放大的短边
                            imageShortEdge = row2 - row1;
                        }
                    }
                    //Column是短边
                    else
                    {
                        //图像缩小
                        if (column2 - column1 > imageWide)
                        {
                            //直接按图像Row计算短边
                            imageShortEdge = imageWide;
                        }
                        else
                        {
                            //使用当前放大的短边
                            imageShortEdge = column2 - column1;
                        }
                    }
                    //实例化ROI轮廓对象集合
                    List<HObjData> hObjDataList = new List<HObjData>();
                    //获取ROI轮廓数据
                    ROIDatas[item.Key].ROI.IROIAction.GetROIHXLDContData(ref hObjDataList, imageShortEdge);
                    //获取ROI轮廓对象集合成功
                    if (hObjDataList != null && hObjDataList.Count != 0)
                    {
                        //遍历ROI对象集合
                        foreach (var item1 in hObjDataList)
                        {
                            //显示ROI对象
                            DisplayHObj(item1);
                        }
                        //文本可见
                        if (item.Value.VisibleText)
                        {
                            //获取ROI文本集合
                            List<HTextData> hTextDataList = ROIDatas[item.Key].ROI.IROIAction.GetROIHTextList();
                            //获取ROI文本集合成功
                            if (hTextDataList != null)
                            {
                                //遍历ROI文本集合
                                foreach (var item1 in hTextDataList)
                                {
                                    //显示ROI文本
                                    DisplayHText(item1);
                                }
                            }
                        }
                    }
                }
            }
            //有行动中的ROI(只显示行动中的ROI)
            else
            {
                //当前正在行动的ROI存在字典中 && 存在ROI处理字典中
                if (rOIDatas.ContainsKey(actionROI) && ROIDatas.ContainsKey(actionROI))
                {
                    //实例化ROI轮廓对象集合
                    List<HObjData> hObjDataList = new List<HObjData>();
                    //定义图像短边
                    double imageShortEdge;
                    //获取左上角和右下角坐标
                    hWindowControl1.HalconWindow.GetPart(out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    //Row是短边
                    if (row2 - row1 < column2 - column1)
                    {
                        //图像缩小
                        if (row2 - row1 > imageHigh)
                        {
                            //直接按图像Row计算短边
                            imageShortEdge = imageHigh;
                        }
                        else
                        {
                            //使用当前放大的短边
                            imageShortEdge = row2 - row1;
                        }
                    }
                    //Column是短边
                    else
                    {
                        //图像缩小
                        if (column2 - column1 > imageWide)
                        {
                            //直接按图像Row计算短边
                            imageShortEdge = imageWide;
                        }
                        else
                        {
                            //使用当前放大的短边
                            imageShortEdge = column2 - column1;
                        }
                    }
                    //获取ROI轮廓数据
                    ROIDatas[actionROI].ROI.IROIAction.GetROIHXLDContData(ref hObjDataList, imageShortEdge);
                    //获取ROI轮廓对象集合成功
                    if (hObjDataList != null)
                    {
                        //遍历ROI对象集合
                        foreach (var item1 in hObjDataList)
                        {
                            //显示ROI对象
                            DisplayHObj(item1);
                        }
                        //文本可见
                        if (rOIDatas[actionROI].VisibleText)
                        {
                            //获取ROI文本集合
                            List<HTextData> hTextDataList = ROIDatas[actionROI].ROI.IROIAction.GetROIHTextList();
                            //获取ROI文本集合成功
                            if (hTextDataList != null)
                            {
                                //遍历ROI文本集合
                                foreach (var item1 in hTextDataList)
                                {
                                    //显示ROI文本
                                    DisplayHText(item1);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 显示模板轮廓
        /// <summary>
        /// 显示模板轮廓
        /// </summary>
        private void DisplayContour()
        {
            //涂抹正在工作中
            if (actionSmear != null)
            {
                //结束方法
                return;
            }
            //复制模板轮廓数据字典
            Dictionary<string, HObject> contourDatas = new Dictionary<string, HObject>(ContourDatas);

            //遍历模板轮廓字典
            foreach (var item in contourDatas)
            {
                //ROI处理不存在
                if (!ContourDatas.ContainsKey(item.Key))
                {
                    //遍历下一个
                    continue;
                }

                //设置控件显示颜色
                hWindowControl1.HalconWindow.SetColor(ColorConversion[HColorEnum.indian_red_75]);
                // 3. 设置线宽
                hWindowControl1.HalconWindow.SetLineWidth(10);
                //显示用户对象
                hWindowControl1.HalconWindow.DispObj(item.Value);
            }

        }
        #endregion

        #region 显示halcon对象
        /// <summary>
        /// 显示halcon对象线程锁
        /// </summary>
        private object displayHObjLock = new object();
        /// <summary>
        /// 显示halcon对象
        /// </summary>
        /// <param name="hObjData">halcon对象数据</param>
        /// <returns>true:显示成功 false:显示失败</returns>
        public bool DisplayHObj(HObjData hObjData)
        {
            lock (displayHObjLock)
            {
                try
                {
                    //图像未实例化 || 图像未初始化
                    if (image == null || !image.IsInitialized())
                    {
                        //显示失败
                        return false;
                    }
                    //边缘显示模式
                    if (hObjData.Margin)
                    {
                        hWindowControl1.HalconWindow.SetDraw("margin");
                        //设置显示线宽
                        hWindowControl1.HalconWindow.SetLineWidth(hObjData.LineWidth);
                    }
                    //填充显示模式
                    else
                    {
                        hWindowControl1.HalconWindow.SetDraw("fill");
                    }
                    //显示虚线
                    if (hObjData.DottedLine)
                    {
                        //设置实线和虚线长度
                        hWindowControl1.HalconWindow.SetLineStyle(new HTuple(10, 10));
                    }
                    //跟随缩放(要显示的是Mark十字架)
                    if (hObjData.FollowZoom)
                    {
                        //获取可见部分最大像素
                        double partMaxPixels = partRow2 - partRow1 > partColumn2 - partColumn1 ? partRow2 - partRow1 : partColumn2 - partColumn1;
                        //计算十字架大小
                        HTuple size = new HTuple(partMaxPixels / 100);
                        if (size[0].D < 0.2)
                        {
                            size[0].D = 0.2;
                        }
                        //创建一个十字架轮廓实例
                        HXLDCont cross = new HXLDCont();
                        cross.GenCrossContourXld(hObjData.Row, hObjData.Col, size, tools.ATR(hObjData.Angle));
                        //更新要显示的HObject对象
                        hObjData.HObject = cross;
                    }
                    //要显示的对象已实例化 && 要显示的对象已初始化
                    if (hObjData.HObject != null && hObjData.HObject.IsInitialized())
                    {
                        //获取对象数量
                        int count = hObjData.HObject.CountObj();
                        //获取是否多颜色显示
                        bool multiColorDisplay = hObjData.HColor.ToString().Contains("auto");
                        //只有单个对象 || 不是多颜色显示
                        if (count == 1 || !multiColorDisplay)
                        {
                            //更新显示颜色
                            HColorEnum hColor = hObjData.HColor;
                            //只有一个对象 && 是多颜色显示
                            if (count == 1 && multiColorDisplay)
                            {
                                //使用显示颜色为auto时,没有多个对象时指定的颜色
                                hColor = hObjData.HColor1;
                            }
                            //设置控件显示颜色
                            hWindowControl1.HalconWindow.SetColor(ColorConversion[hColor]);
                            //显示用户对象
                            hWindowControl1.HalconWindow.DispObj(hObjData.HObject);
                        }
                        //有多个对象(多颜色显示)
                        else
                        {
                            //定义颜色索引
                            int index = 0;
                            //定义显示颜色集合
                            List<HColorEnum> colorfulColor = ColorfulColor;
                            //是轮廓类型 || 对象使用边缘模式 || 使用鲜艳颜色不透明
                            if (hObjData.HObject is HXLDCont || hObjData.Margin || hObjData.HColor == HColorEnum.auto)
                            {
                                //使用鲜艳颜色不透明
                                colorfulColor = ColorfulColor;
                            }
                            //使用鲜艳颜色_显示度25
                            else if (hObjData.HColor == HColorEnum.auto_25)
                            {
                                //使用鲜艳颜色_显示度25
                                colorfulColor = ColorfulColor_25;
                            }
                            //使用鲜艳颜色_显示度50
                            else if (hObjData.HColor == HColorEnum.auto_50)
                            {
                                //使用鲜艳颜色_显示度50
                                colorfulColor = ColorfulColor_50;
                            }
                            //使用鲜艳颜色_显示度75
                            else if (hObjData.HColor == HColorEnum.auto_75)
                            {
                                //使用鲜艳颜色_显示度75
                                colorfulColor = ColorfulColor_75;
                            }
                            //循环halcon对象数量
                            for (int i = 1; i <= count; i++)
                            {
                                //颜色已用完
                                if (index >= colorfulColor.Count)
                                {
                                    //重新使用颜色
                                    index = 0;
                                }
                                //设置控件显示颜色
                                hWindowControl1.HalconWindow.SetColor(ColorConversion[colorfulColor[index]]);
                                //显示用户对象
                                hWindowControl1.HalconWindow.DispObj(hObjData.HObject.SelectObj(i));
                                //颜色索引+1
                                index++;
                            }
                        }
                    }
                    //显示虚线
                    if (hObjData.DottedLine)
                    {
                        //改回实线显示
                        hWindowControl1.HalconWindow.SetLineStyle(new HTuple());
                    }
                    //设置显示线宽
                    hWindowControl1.HalconWindow.SetLineWidth(1);
                    //显示成功
                    return true;
                }
                catch (Exception)
                {
                    //显示失败
                    return false;
                }
            }
        }
        #endregion

        #region 显示halcon文本
        /// <summary>
        /// 显示halcon文本线程锁
        /// </summary>
        private object displayHTextLock = new object();
        /// <summary>
        /// 显示halcon文本
        /// </summary>
        /// <param name="hTextData">halcon文本数据</param>
        /// <returns>true:显示成功 false:显示失败</returns>
        public bool DisplayHText(HTextData hTextData)
        {
            lock (displayHTextLock)
            {
                try
                {
                    //图像未实例化 || 图像未初始化
                    if (image == null || !image.IsInitialized())
                    {
                        //显示失败
                        return false;
                    }
                    //设置显示坐标系
                    string coordSystem = hTextData.Image ? "image" : "window";
                    //定义字体样式及大小
                    string fontStyle;
                    //粗体显示
                    if (hTextData.Bold)
                    {
                        //设置字体样式及大小 -Bold 加粗
                        fontStyle = $"微软雅黑-Bold-{hTextData.FontSize}";
                    }
                    else
                    {
                        //设置字体样式及大小 -Bold 加粗
                        fontStyle = $"微软雅黑-{hTextData.FontSize}";
                    }
                    //设置字体样式
                    hWindowControl1.HalconWindow.SetFont(fontStyle);
                    //设置盒子(字体背景)
                    string box = hTextData.Box ? "true" : "false";
                    //显示信息
                    hWindowControl1.HalconWindow.DispText(hTextData.Text, coordSystem, hTextData.Row, hTextData.Col, ColorConversion[hTextData.HColor],
                                                          new HTuple("box", "shadow", "box_color"),
                                                          new HTuple(box, "false", HWTools.ColorConversion[hTextData.BackgroundColor]));
                    //显示成功
                    return true;
                }
                catch (Exception)
                {
                    //显示失败
                    return false;
                }
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
            //ROI名称不为null
            if (rOIName != null)
            {
                //更新最新选定ROI(关闭上一个激活的)
                NewROI = rOIName;
                //ROI存在ROI处理中
                if (ROIDatas.ContainsKey(rOIName))
                {
                    //设置ROI激活
                    ROIDatas[rOIName].ROI.IROIAction.Activate = true;
                    //显示图像
                    DisplayImage();
                }
            }
        }
        #endregion

        #region 关闭行动中的ROI
        /// <summary>
        /// 关闭行动中的ROI
        /// </summary>
        private void CloseActionROI()
        {
            //存在行动中的ROI
            if (actionROI != null)
            {
                //行动中ROI存在ROI数据字典中
                if (ROIDatas.ContainsKey(actionROI))
                {
                    //获取ROI数据
                    ROIDatas[actionROI].ROI.IROIAction.GetAllROIData(out HTuple hTuple, out HRegion hRegion, out HXLDCont hXLDCont);
                    //触发ROI移动完成事件
                    ROIMoveDone?.Invoke(actionROI, hTuple, hRegion);
                }
                //释放行动中的ROI
                actionROI = null;
            }
            //关闭左键按下标志
            press = false;
        }
        #endregion

        #region 显示涂抹
        /// <summary>
        /// 显示涂抹
        /// </summary>
        public void DeleteSmear()
        {
            //ROI正在工作中
            if (actionROI != null)
            {
                //结束方法
                return;
            }
            //复制涂抹数据字典
            Dictionary<string, SmearData> smearDatas = new Dictionary<string, SmearData>(SmearDatas);
            //没右在行动中的涂抹(显示全部涂抹)
            if (actionSmear == null)
            {
                //遍历涂抹字典
                foreach (var item in smearDatas)
                {
                    //涂抹不可见
                    if (!item.Value.Visible)
                    {
                        //遍历下一个
                        continue;
                    }
                    //涂抹处理不存在
                    if (!SmearDatas.ContainsKey(item.Key))
                    {
                        //遍历下一个
                        continue;
                    }
                    //实例化涂抹区域对象集合
                    List<HObjData> hObjDataList = SmearDatas[item.Key].ISmearAction.GetSmearHRegion(mouseRow, mouseColumn);
                    //获取涂抹区域对象集合成功
                    if (hObjDataList != null && hObjDataList.Count != 0)
                    {
                        //遍历Smear对象集合
                        foreach (var item1 in hObjDataList)
                        {
                            //显示Smear对象
                            DisplayHObj(item1);
                        }
                        //文本可见
                        if (item.Value.VisibleText)
                        {
                            //获取涂抹文本集合
                            List<HTextData> hTextDataList = SmearDatas[item.Key].ISmearAction.GetSmearHTextList();
                            //获取涂抹文本集合成功
                            if (hTextDataList != null)
                            {
                                //遍历涂抹文本集合
                                foreach (var item1 in hTextDataList)
                                {
                                    //显示涂抹文本
                                    DisplayHText(item1);
                                }
                            }
                        }
                    }
                }
            }
            //有行动中的涂抹(只显示行动中的涂抹)
            else
            {
                //当前正在行动的涂抹存在字典中 && 存在涂抹处理字典中
                if (smearDatas.ContainsKey(actionSmear) && SmearDatas.ContainsKey(actionSmear))
                {
                    //实例化涂抹区域对象集合
                    List<HObjData> hObjDataList = SmearDatas[actionSmear].ISmearAction.GetSmearHRegion(mouseRow, mouseColumn);
                    //获取涂抹区域对象集合成功
                    if (hObjDataList != null && hObjDataList.Count != 0)
                    {
                        //遍历Smear对象集合
                        foreach (var item1 in hObjDataList)
                        {
                            //显示Smear对象
                            DisplayHObj(item1);
                        }
                        //文本可见
                        if (smearDatas[actionSmear].VisibleText)
                        {
                            //获取涂抹文本集合
                            List<HTextData> hTextDataList = SmearDatas[actionSmear].ISmearAction.GetSmearHTextList();
                            //获取涂抹文本集合成功
                            if (hTextDataList != null)
                            {
                                //遍历涂抹文本集合
                                foreach (var item1 in hTextDataList)
                                {
                                    //显示涂抹文本
                                    DisplayHText(item1);
                                }
                            }
                        }
                    }
                }
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
            //涂抹名称不为null && 涂抹存在涂抹数据字典中 && 涂抹存在涂抹处理字典中
            if (smearName != null && SmearDatas.ContainsKey(smearName) && SmearDatas.ContainsKey(smearName))
            {
                //选择了涂抹类型
                if (type != SmearTypeEnum.None)
                {
                    //上一个涂抹未关闭 && 上一个涂抹存在涂抹处理字典中
                    if (actionSmear != null && SmearDatas.ContainsKey(actionSmear))
                    {
                        //调用涂抹工作完成(让上一个先完成工作)
                        SmearDatas[actionSmear].ISmearAction.WorkDone();
                    }
                    //关闭行动中的ROI,默认涂抹优先
                    CloseActionROI();
                    //更新当前正在行动的涂抹
                    actionSmear = smearName;
                    //设置作业类型
                    SmearDatas[actionSmear].ISmearAction.SetJobType(type);
                }
            }
        }
        #endregion

        #region 关闭涂抹
        /// <summary>
        /// 关闭涂抹
        /// </summary>
        public void CloseSmear()
        {
            //更新当前正在行动的涂抹
            actionSmear = null;
            //遍历涂抹处理
            foreach (var item in SmearDatas)
            {
                //调用涂抹工作完成
                item.Value.ISmearAction.WorkDone();
            }
        }
        #endregion
    }
}
