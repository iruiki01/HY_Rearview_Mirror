using HalconDotNet;
using Helper;
using HslCommunication.Algorithms.PID;
using HY_Rearview_Mirror.CAN;
using HY_Rearview_Mirror.Controls;
using HY_Rearview_Mirror.Functions;
using HY_Rearview_Mirror.InterfaceTool;
using HY_Rearview_Mirror.TypeManager;
using HY_Rearview_Mirror.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HY_Rearview_Mirror.Functions.CameraController;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace HY_Rearview_Mirror
{
    public partial class MainForm : UIForm
    {
        public static event EventHandler<string> UpdataTreeViewInitHandler;
        public static event EventHandler<HImage> SendHImagetHandler;

        SerialPortData serialportdata = new SerialPortData();
        /// <summary>
        /// 当前操作图像
        /// </summary>
        private HImage g_hImage;
        private bool isModbusSerialOpen;

        public MainForm()
        {
            InitializeComponent();       
            #region   初始化控件缩放
            x = Width;
            y = Height;
            setTag(this);
            #endregion
            AppConfig.GetInstance().LoadConfig();

            global.writeLogProduce.AttachRichTextBox(richTextBox1);
            global.writeLogSystem.AttachRichTextBox(richTextBox2);

            // 启动插件系统
            //global.pluginManager.Start();
            this.WindowState = FormWindowState.Maximized;
            // 设置单元格样式
            DataGridViewCellStyle style = new DataGridViewCellStyle
            {
                WrapMode = DataGridViewTriState.True
            };
            uiDataGridView1.DefaultCellStyle = style;

            // 设置行自适应
            uiDataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            // 显示行标题
            uiDataGridView1.RowHeadersVisible = true;

            // 设置行标题宽度（可选）
            uiDataGridView1.RowHeadersWidth = 40;

            // 绑定事件
            uiDataGridView1.RowPostPaint += DataGridView1_RowPostPaint;

            LoginForm.send_Login_Delegate += Login;

            global.G_eventManager.AddListener("发送运行时间", new Action<string>(Run_Time)); 
            global.G_eventManager.AddListener("发送良率", new Action<int,int,string>(Run_Yield)); 

            uiTextBox8.Text = "请登录";


            if(AppConfig.GetInstance()._config.Station_No != 0)
            {
                uiTitlePanel4.Visible = false;
                uiTitlePanel5.Visible = false;

                uiTitlePanel3.Dock = DockStyle.Fill;
                hWindow1.Dock = DockStyle.Fill;
            }    
        }

        private void Run_Yield(int count,int countOk,string obj)
        {
            // 检查是否需要Invoke
            if (uiTextBox5.InvokeRequired)
            {
                uiTextBox5.Invoke(new Action(() => Run_Yield(count, countOk,obj)));
                return;
            }
            uiTextBox3.Text = count.ToString();
            uiTextBox4.Text = countOk.ToString();
            uiTextBox5.Text = obj;
            uiTextBox6.Text = (count - countOk).ToString();
        }

        private void Run_Time(string obj)
        {
            // 检查是否需要Invoke
            if (uiLabel11.InvokeRequired)
            {
                uiLabel11.Invoke(new Action(() => Run_Time(obj)));
                return;
            }
            uiLabel11.Text = obj;
        }

        private void Login(string type)
        {
            if(type == "操作员")
            {
                uiTextBox8.Text = "操作员";

                uiTextBox3.ReadOnly = true;
                uiTextBox4.ReadOnly = true;
                uiTextBox5.ReadOnly = true;
                uiTextBox6.ReadOnly = true;
            }
            else if (type == "工程师")
            {
                uiTextBox8.Text = "工程师";

                uiTextBox3.ReadOnly = true;
                uiTextBox4.ReadOnly = true;
                uiTextBox5.ReadOnly = true;
                uiTextBox6.ReadOnly = true;
            }
            else if (type == "管理员")
            {
                uiTextBox8.Text = "管理员";

                uiTextBox3.ReadOnly = false;
                uiTextBox4.ReadOnly = false;
                uiTextBox5.ReadOnly = false;
                uiTextBox6.ReadOnly = false;
            }
            else if (type == "空")
            {
                uiTextBox8.Text = "请登录";

                uiTextBox3.ReadOnly = true;
                uiTextBox4.ReadOnly = true;
                uiTextBox5.ReadOnly = true;
                uiTextBox6.ReadOnly = true;
            }
        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;

            // 生成行号（从1开始）
            string rowNumber = (e.RowIndex + 1).ToString();

            // 创建居中格式
            using (StringFormat centerFormat = new StringFormat())
            {
                centerFormat.Alignment = StringAlignment.Center;      // 水平居中
                centerFormat.LineAlignment = StringAlignment.Center;  // 垂直居中

                // 绘制行号（完全居中）
                e.Graphics.DrawString(
                    rowNumber,
                    grid.Font,
                    SystemBrushes.ControlText,
                    e.RowBounds.Left + grid.RowHeadersWidth / 2,  // X中心点
                    e.RowBounds.Top + e.RowBounds.Height / 2,     // Y中心点
                    centerFormat
                );
            }
        }
        #region 控件大小随窗体大小等比例缩放

        private readonly float x; //定义当前窗体的宽度
        private readonly float y; //定义当前窗体的高度

        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ";" + con.Height + ";" + con.Left + ";" + con.Top + ";" + con.Font.Size;
                if (con.Controls.Count > 0) setTag(con);
            }
        }

        private void setControls(float newx, float newy, Control cons)
        {
            //遍历窗体中的控件，重新设置控件的值
            foreach (Control con in cons.Controls)
            {
                //获取控件的Tag属性值，并分割后存储字符串数组
                if (con.Tag != null)
                {
                    var mytag = con.Tag.ToString().Split(';');
                    //根据窗体缩放的比例确定控件的值
                    con.Width = Convert.ToInt32(Convert.ToSingle(mytag[0]) * newx); //宽度
                    con.Height = Convert.ToInt32(Convert.ToSingle(mytag[1]) * newy); //高度
                    con.Left = Convert.ToInt32(Convert.ToSingle(mytag[2]) * newx); //左边距
                    con.Top = Convert.ToInt32(Convert.ToSingle(mytag[3]) * newy); //顶边距
                    var currentSize = Convert.ToSingle(mytag[4]) * newy; //字体大小                   
                    if (currentSize > 0) con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    con.Focus();
                    if (con.Controls.Count > 0) setControls(newx, newy, con);
                }
            }
        }
        /// <summary>
        /// 重置窗体布局
        /// </summary>
        private void ReWinformLayout()
        {
            var newx = Width / x;
            var newy = Height / y;
            setControls(newx, newy, this);
        }
        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            global.G_eventManager.Clear();
            try
            {
                if (global.cameraController.MyCameraArr.TryGetValue("相机1", out MyCamera myCamera1))
                {
                    myCamera1.FrameReceived -= GetImage0;
                }

                if(AppConfig.GetInstance()._config.Station_No == 0)
                {
                    if (global.cameraController.MyCameraArr.TryGetValue("相机2", out MyCamera myCamera2))
                    {
                        myCamera2.FrameReceived -= GetImage1;
                    }
                }
           

                foreach (var root3 in global.RootNodeDataList)
                {
                    foreach (var root in new[] { root3.RootData1, root3.RootData2, root3.RootData3 })
                    {
                        foreach (var child in root.Children)
                        {
                            // 通过 ID 重新加载插件
                            //child.Plugin = global.pluginManager.CreatePlugin(child.Name);
                            // 把保存的参数在各个控件上显示
                            //child.Plugin.SetData(child.TestParam, child.Plugin.VersionStr);
                            //child.Plugin.CreateUI().ShowDialog();
                            child.Plugin.Dispose();
                        }
                    }
                }

                if(AppConfig.GetInstance()._config.Station_No == 0)
                {
                    global.gYCamTool.CloseCam();
                }
             
                global.cameraController?.Dispose();
                WorkThread.GetInstance().SetCellValueThreadSafeHandler -= SetCellValueThreadSafe;
                WorkThread.GetInstance().SetCellColorHandler -= SetCellColor;
                WorkThread.GetInstance().RunProcessBarHandler -= RunProcessBar;
                WorkThread.GetInstance().RestorePluginsHandler -= RestorePlugins;
                WorkThread.GetInstance().GYCamBitmapHandler -= GYCamBitmap;
                global.serialHelper_GW?.Dispose();

                AppConfig.GetInstance().SaveConfig();
            }
            catch { }

            e.Cancel = true;
            this.Dispose();

            //System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.SuspendLayout();
            // 垂直窗体布局
            ReWinformLayout();
            this.ResumeLayout(true); // true 表示自动重新布局
        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (global.indexSelect >= 0)
            {
                // 创建新行
                DataGridViewRow row = new DataGridViewRow();
                this.uiDataGridView1.Rows.Insert(global.indexSelect + 1, row);
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[0].Value = " ";
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[1].Value = " ";
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[2].Value = " ";
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[3].Value = " ";
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[4].Value = " ";
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[5].Value = " ";
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[6].Value = " ";
                this.uiDataGridView1.Rows[global.indexSelect + 1].Cells[7].Value = " ";

                global.RootNodeDataList.Insert(global.indexSelect + 1,new RootNode3Data());
            }
            else
            {
                int index = this.uiDataGridView1.Rows.Add();
                this.uiDataGridView1.Rows[index].Cells[0].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[1].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[2].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[3].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[4].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[5].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[6].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[7].Value = " ";

                global.RootNodeDataList.Add(new RootNode3Data());
            }
        }

        private void uiDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (uiDataGridView1.CurrentRow == null) return;
            try
            {
                global.indexSelect = uiDataGridView1.CurrentRow.Index;
            }
            catch (Exception)
            {
                global.indexSelect = -1;
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index;
            if (this.uiDataGridView1.CurrentRow == null)
            {
                index = -1;
            }
            else
            {
                index = this.uiDataGridView1.CurrentRow.Index;
            }
            if (index < 0)
            {
                UIMessageBox.ShowError("请选择要删除的行号!");
                return;
            }
            else
            {
                if (MessageBox.Show("确认删除该行吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.uiDataGridView1.Rows.RemoveAt(index);
                    global.RootNodeDataList.RemoveAt(index);
                }
            }
        }

        private void uiDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (uiDataGridView1.CurrentRow == null) return;
            int id = -1;
            try
            {
                id = uiDataGridView1.CurrentRow.Index;
            }
            catch (Exception)
            {
                id = -1;
            }
            if(id >= 0)
            {            
                try
                {
                    EditForm editForm = new EditForm();
                    UpdataTreeViewInitHandler?.Invoke(this, global.RootNodeDataList[global.indexSelect].Name);
                    EditForm.LoadDataHandler += LoadData;
                    editForm.ShowDialog();
                    EditForm.LoadDataHandler -= LoadData;
                }
                catch (Exception ex)
                { 
                    Console.WriteLine(ex.ToString());   
                }          
            }
        }

        public  void LoadData(string path)
        {
            if (File.Exists(path))
            {
                // 2. 配置Json.NET（转换器现在已包含所有注册的类型）
                //var settings = new JsonSerializerSettings
                //{
                //    Converters = { new SafeTestParamConverter() }
                //};

                // 序列化时（保存数据时）
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto, // 只在需要时添加类型信息
                    // 或者使用引用保留（推荐）
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Formatting = Formatting.Indented
                };

                try
                {
                    string json = File.ReadAllText(path);
                    var data = JsonConvert.DeserializeObject<SerializableGlobalData>(json, settings);

                    global.RootNodeDataList = data?.RootNodeDataList ?? new List<RootNode3Data>();

                    RestorePlugins();
                 }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载数据失败: {ex.Message}");
                    global.RootNodeDataList = new List<RootNode3Data>();
                    global.indexSelect = -1;
                }
            }
            else
            {
                UIMessageBox.ShowError("未找到该文件！");
                return;
            }    
        }
        // 加载后恢复插件
        public void RestorePlugins()
        {
            // 检查是否需要Invoke
            if (uiDataGridView1.InvokeRequired)
            {
                uiDataGridView1.Invoke(new Action(() => RestorePlugins()));
                return;
            }

            uiDataGridView1.Rows.Clear();
            List<string[]> list = new List<string[]>();
            //uiDataGridView1.Columns.Clear();

            int id;
            foreach (var root3 in global.RootNodeDataList)
            {
                string[] data = new string[3];
                id = 0;
                foreach (var root in new[] { root3.RootData1, root3.RootData2, root3.RootData3})
                {
                    id++;
                    int id1 = 0;
                    foreach (var child in root.Children)
                    {
                        id1++;
                        // 通过 ID 重新加载插件
                        //child.Plugin = global.pluginManager.CreatePlugin(child.Name);
                        // 把保存的参数在各个控件上显示
                        //child.Plugin.SetData(child.TestParam, child.Plugin.VersionStr);
                        //child.Plugin.CreateUI().ShowDialog();

                        data[id - 1] += id1 + "：" + child.Plugin.PluginId + "->"+ child.Plugin.VersionStr + "\r\n";
                    }
                }
                list.Add(data);
            }

            id = 0;
            foreach (var root3 in global.RootNodeDataList)
            {
                id ++;
                int index = this.uiDataGridView1.Rows.Add();
                this.uiDataGridView1.Rows[index].Cells[0].Value = root3.Name;
                this.uiDataGridView1.Rows[index].Cells[1].Value = root3.Name;

                if(list[id - 1][0] != null )
                {
                    this.uiDataGridView1.Rows[index].Cells[2].Value = list[id - 1][0].Trim();
                }
                else
                {
                    this.uiDataGridView1.Rows[index].Cells[2].Value = "";
                }

                if (list[id - 1][1] != null)
                {
                    this.uiDataGridView1.Rows[index].Cells[3].Value = list[id - 1][1].Trim();
                }
                else
                {
                    this.uiDataGridView1.Rows[index].Cells[3].Value = "";
                }

                if (list[id - 1][2] != null)
                {
                    this.uiDataGridView1.Rows[index].Cells[4].Value = list[id - 1][2].Trim();
                }
                else
                {
                    this.uiDataGridView1.Rows[index].Cells[4].Value = "";
                }
                
                this.uiDataGridView1.Rows[index].Cells[5].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[6].Value = " ";
                this.uiDataGridView1.Rows[index].Cells[7].Value = " ";
            }
        }
        /// <summary>
        /// 启动按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (!CheckPermission(UserState.操作员)) return;

            if (global.RootNodeDataList.Count <= 0)
            {
                UIMessageBox.ShowError("测试内容不能为空，请打开要测试的文件！");
                return;
            }
            WorkThread.GetInstance().Run(true);
            uiButton1.Enabled = false;
            global.writeLogSystem.Info("启动按钮按下");
        }
        /// <summary>
        /// 停止按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            WorkThread.GetInstance().Run(false);
            uiButton1.Enabled = true;           
        }       
        /// <summary>
        /// 创建所有的插件类型
        /// </summary>
        public void CreateType()
        {
            /*******************************************************仪器初始化******************************************************************/
            switch (AppConfig.GetInstance()._config.Station_No)
            {
                case 0:// AOI测试工位                   
                    {
                        GYCamFun gYCamFun = new GYCamFun();
                        gYCamFun.Initialize();
                        global._loadedPlugins.Add(gYCamFun.PluginId, gYCamFun);
                        global.factory.Register<GYCamFun>(gYCamFun.PluginId);
                        global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = gYCamFun.moduleName, 功能类型 = gYCamFun.PluginId, 描述 = gYCamFun.DescribeMessage });
                    }
                    break;
                case 1:// 感光标定工位 
                    PhotosensitiveFun photosensitiveFun = new PhotosensitiveFun();
                    photosensitiveFun.Initialize();
                    global._loadedPlugins.Add(photosensitiveFun.PluginId, photosensitiveFun);
                    global.factory.Register<PhotosensitiveFun>(photosensitiveFun.PluginId);
                    global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = photosensitiveFun.moduleName, 功能类型 = photosensitiveFun.PluginId, 描述 = photosensitiveFun.DescribeMessage });
                    break;
                case 2:// 性能测试工位
                    {
                        IT6722AFun iT6722AFun = new IT6722AFun();
                        iT6722AFun.Initialize();
                        global._loadedPlugins.Add(iT6722AFun.PluginId, iT6722AFun);
                        global.factory.Register<IT6722AFun>(iT6722AFun.PluginId);
                        global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = iT6722AFun.moduleName, 功能类型 = iT6722AFun.PluginId, 描述 = iT6722AFun.DescribeMessage });


                        GW_MultimeterFun gW_MultimeterFun = new GW_MultimeterFun();
                        gW_MultimeterFun.Initialize();
                        global._loadedPlugins.Add(gW_MultimeterFun.PluginId, gW_MultimeterFun);
                        global.factory.Register<GW_MultimeterFun>(gW_MultimeterFun.PluginId);
                        global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = gW_MultimeterFun.moduleName, 功能类型 = gW_MultimeterFun.PluginId, 描述 = gW_MultimeterFun.DescribeMessage });

                        GlareFun glareFun = new GlareFun();
                        glareFun.Initialize();
                        global._loadedPlugins.Add(glareFun.PluginId, glareFun);
                        global.factory.Register<GlareFun>(glareFun.PluginId);
                        global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = glareFun.moduleName, 功能类型 = glareFun.PluginId, 描述 = glareFun.DescribeMessage });
                    }                
                    break;
                case 3:// 反射率测试工位                
                    EK2000ProFun eK2000ProFun = new EK2000ProFun();
                    eK2000ProFun.Initialize();
                    global._loadedPlugins.Add(eK2000ProFun.PluginId, eK2000ProFun);
                    global.factory.Register<EK2000ProFun>(eK2000ProFun.PluginId);
                    global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = eK2000ProFun.moduleName, 功能类型 = eK2000ProFun.PluginId, 描述 = eK2000ProFun.DescribeMessage });                 
                    break;
                default:
                    break;
            }
            /*********************************************************************************************************************************/
            Can1Fun can1Fun = new Can1Fun();
            can1Fun.Initialize();
            global._loadedPlugins.Add(can1Fun.PluginId, can1Fun);
            global.factory.Register<Can1Fun>(can1Fun.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = can1Fun.moduleName, 功能类型 = can1Fun.PluginId, 描述 = can1Fun.DescribeMessage });

            DelayFun delayFun = new DelayFun();
            delayFun.Initialize();
            global._loadedPlugins.Add(delayFun.PluginId, delayFun);
            global.factory.Register<DelayFun>(delayFun.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = delayFun.moduleName, 功能类型 = delayFun.PluginId, 描述 = delayFun.DescribeMessage });

            CheckLight checkLight = new CheckLight();
            checkLight.Initialize();
            global._loadedPlugins.Add(checkLight.PluginId, checkLight);
            global.factory.Register<CheckLight>(checkLight.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = checkLight.moduleName, 功能类型 = checkLight.PluginId, 描述 = checkLight.DescribeMessage });

            TemplateMatchingFun templateMatchingFun = new TemplateMatchingFun();
            templateMatchingFun.Initialize();
            global._loadedPlugins.Add(templateMatchingFun.PluginId, templateMatchingFun);
            global.factory.Register<TemplateMatchingFun>(templateMatchingFun.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = templateMatchingFun.moduleName, 功能类型 = templateMatchingFun.PluginId, 描述 = templateMatchingFun.DescribeMessage }); 

            UartFun uartFun = new UartFun();
            uartFun.Initialize();
            global._loadedPlugins.Add(uartFun.PluginId, uartFun);
            global.factory.Register<UartFun>(uartFun.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = uartFun.moduleName, 功能类型 = uartFun.PluginId, 描述 = uartFun.DescribeMessage });

            MuraFun muraFun = new MuraFun();
            muraFun.Initialize();
            global._loadedPlugins.Add(muraFun.PluginId, muraFun);
            global.factory.Register<MuraFun>(muraFun.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = muraFun.moduleName, 功能类型 = muraFun.PluginId, 描述 = muraFun.DescribeMessage });

            CheckDefectFun checkDefectFun = new CheckDefectFun();
            checkDefectFun.Initialize();
            global._loadedPlugins.Add(checkDefectFun.PluginId, checkDefectFun);
            global.factory.Register<CheckDefectFun>(checkDefectFun.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = checkDefectFun.moduleName, 功能类型 = checkDefectFun.PluginId, 描述 = checkDefectFun.DescribeMessage });

            ZhongshengFun zhongshengFun = new ZhongshengFun();
            zhongshengFun.Initialize();
            global._loadedPlugins.Add(zhongshengFun.PluginId, zhongshengFun);
            global.factory.Register<ZhongshengFun>(zhongshengFun.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = zhongshengFun.moduleName, 功能类型 = zhongshengFun.PluginId, 描述 = zhongshengFun.DescribeMessage });

            CheckRGB checkRGB = new CheckRGB();
            checkRGB.Initialize();
            global._loadedPlugins.Add(checkRGB.PluginId, checkRGB);
            global.factory.Register<CheckRGB>(checkRGB.PluginId);
            global._PluginDescribeList.Add(new PluginDescribe() { 模组名称 = checkRGB.moduleName, 功能类型 = checkRGB.PluginId, 描述 = checkRGB.DescribeMessage });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {     
                //Task.Run(async () => { global.gYCamTool = new GYCamTool(); });

                global.cameraController.RefreshDeviceList();

                try
                {
                    if(AppConfig.GetInstance()._config.Station_No == 0)
                    {
                        global.cameraController.OpenCamDevice("相机2");
                        global.cameraController.CamDeviceSoftTrigger("相机2");
                        global.cameraController.CamDeviceStartGrab("相机2");
                    }
                    global.cameraController.OpenCamDevice("相机1");
                    global.cameraController.CamDeviceSoftTrigger("相机1");
                    global.cameraController.CamDeviceStartGrab("相机1");

               

                    //global.cameraController.CamDeviceContinuesMode("123");
                }
                catch (Exception ex)
                {
                    global.writeLogSystem.Error(ex.ToString());
                }

                if (global.cameraController.MyCameraArr.TryGetValue("相机1", out MyCamera myCamera1))
                {
                    myCamera1.FrameReceived += GetImage0;
                }

                if (AppConfig.GetInstance()._config.Station_No == 0)
                {
                    if (global.cameraController.MyCameraArr.TryGetValue("相机2", out MyCamera myCamera2))
                    {
                        myCamera2.FrameReceived += GetImage1;
                    }
                }
             
            }
            catch (Exception ex)
            {
                global.writeLogSystem.Error("查找相机出错了！");
            }
            /*******************************************************仪器初始化******************************************************************/
            switch(AppConfig.GetInstance()._config.Station_No)
            {
                case 0:// AOI测试工位                   
                    global.gYCamTool = new GYCamTool();
                    break;
                case 1:// 感光标定工位
                    global.serialHelper_Light = new RobustSerialPort("COM5", 19200, 8, Parity.None, StopBits.One);
                    break;
                case 2:// 性能测试工位
                    global.serialHelper_Light = new RobustSerialPort("COM5", 19200, 8, Parity.None, StopBits.One);
                    global.serialHelper_GW = new RobustSerialPort("COM11", 115200, 8, Parity.None, StopBits.One);
                    global.iT6722ATool = new IT6722ATool();
                    break;
                case 3:// 反射率测试工位
                    global.use_EK2000Pro = new Use_EK2000Pro();
                    break;
                default:
                    break;
            }
            /*********************************************************************************************************************************/
           
            try
            {
                CreateType();
                //LoadData(this, EventArgs.Empty);

                WorkThread.GetInstance().factory.Start();
                WorkThread.GetInstance().SetCellValueThreadSafeHandler += SetCellValueThreadSafe;
                WorkThread.GetInstance().SetCellColorHandler += SetCellColor;
                WorkThread.GetInstance().RunProcessBarHandler += RunProcessBar;
                WorkThread.GetInstance().RestorePluginsHandler += RestorePlugins;
                WorkThread.GetInstance().GYCamBitmapHandler += GYCamBitmap;

                //string path = AppDomain.CurrentDomain.BaseDirectory + "Resource\\Image\\O.bmp";
                //// 读取图像
                //hImage = new HImage(path);
                ////显示到控件
                //hWindow1.HobjectToHimage(hImage.Clone());

                AppConfig.GetInstance()._config.SetSynchronizationContext(SynchronizationContext.Current);
                uiTextBox9.DataBindings.Add("Text", AppConfig.GetInstance()._config, "SN", true, DataSourceUpdateMode.OnPropertyChanged);


                global.ZS_ModbusRtuHelpe = new ModbusRtuHelper();
                global.ZS_ModbusRtuHelpe.Configure("COM3", 9600, station: 1);
                var result3 = global.ZS_ModbusRtuHelpe.Open();
                if (result3.IsSuccess)
                {
                    global.writeLogSystem.Info("RFID连接成功!");
                    global.writeLogSystem.Info("中盛IO卡连接成功!");
                }
                else
                {
                    global.writeLogSystem.Error("RFID连接失败!");
                    global.writeLogSystem.Error("中盛IO卡连接失败!");
                }

                timer1.Enabled = true;
                timer1.Interval = 100;
                timer1.Start();
            }
            catch (Exception ex)
            {
                UIMessageBox.ShowError("程序启动失败了！" + ex.Message);
                return;
            }
        }

        private void GYCamBitmap(Bitmap bitmap)
        {
            // 检查是否需要Invoke
            if (hWindow3.InvokeRequired)
            {
                hWindow3?.Invoke(new Action(() => GYCamBitmap(bitmap)));
                return;
            }

            HObject ho_img = new HObject();
            ImageTool.GetInstance().Bitmap2HObjectBpp24(bitmap, out ho_img);

            hWindow3.HobjectToHimage(ho_img);  // 显示

            bitmap?.Dispose();
        }

        private void GetImage0(object sender, Bitmap image)
        {
            HObject ho_img = new HObject();
            ImageTool.GetInstance().Bitmap2HObjectBpp24(image, out ho_img);

            // 深拷贝：用 CopyObj 创建独立图像
            HObject ho_copy;
            HOperatorSet.CopyObj(ho_img, out ho_copy, 1, 1);
            HImage hImage = new HImage(ho_copy);

            if(g_hImage != null)
            {
                g_hImage.Dispose();
            }
            g_hImage =  new HImage(ho_copy);  //  独立副本

            // 发送图片
            SendHImagetHandler?.Invoke(this, g_hImage);
            //if (global.WaitNewCam0MatFlag)
            //{
            //    global.imageCam0?.Dispose();
            //    global.imageCam0 = new Bitmap(image);  // Bitmap 深拷贝

            //    WorkThread.GetInstance().Cam0_HImage?.Dispose();
            //    WorkThread.GetInstance().Cam0_HImage = new HImage(ho_copy);  //  独立副本
            //    global.WaitNewCam0MatFlag = false;
            //}

            hWindow1.HobjectToHimage(hImage);  // 显示

            //hImage.Dispose();
            ho_img?.Dispose();
            ho_copy?.Dispose();  //  释放中间对象
            image?.Dispose();
        }

        private void GetImage1(object sender, Bitmap image)
        {
            HObject ho_img = new HObject();
            ImageTool.GetInstance().Bitmap2HObjectBpp24(image, out ho_img);

            // 深拷贝：用 CopyObj 创建独立图像
            HObject ho_copy;
            HOperatorSet.CopyObj(ho_img, out ho_copy, 1, 1);
            HImage hImage = new HImage(ho_copy);

            if (g_hImage != null)
            {
                g_hImage.Dispose();
            }
            g_hImage = new HImage(ho_copy);  //  独立副本

            // 发送图片
            SendHImagetHandler?.Invoke(this, g_hImage);
            //if (global.WaitNewCam0MatFlag)
            //{
            //    global.imageCam0?.Dispose();
            //    global.imageCam0 = new Bitmap(image);  // Bitmap 深拷贝

            //    WorkThread.GetInstance().Cam0_HImage?.Dispose();
            //    WorkThread.GetInstance().Cam0_HImage = new HImage(ho_copy);  //  独立副本
            //    global.WaitNewCam0MatFlag = false;
            //}

            hWindow2.HobjectToHimage(hImage);  // 显示

            //hImage.Dispose();
            ho_img?.Dispose();
            ho_copy?.Dispose();  //  释放中间对象
            image?.Dispose();
        }
        private void RunProcessBar(int Value)
        {
            // 检查是否需要Invoke
            if (uiDataGridView1.InvokeRequired)
            {
                uiDataGridView1.Invoke(new Action(() => RunProcessBar(Value)));
                return;
            }
            uiProcessBar1.Value = Value;
        }
        /// <summary>
        /// 线程安全地设置单元格值
        /// </summary>
        public void SetCellValueThreadSafe(int rowIndex, int columnIndex, object value)
        {
            // 检查是否需要Invoke
            if (uiDataGridView1.InvokeRequired)
            {
                uiDataGridView1.Invoke(new Action(() => SetCellValueThreadSafe(rowIndex, columnIndex, value)));
                return;
            }

            // 主线程中执行
            if (rowIndex < 0 || rowIndex >= uiDataGridView1.Rows.Count ||
                columnIndex < 0 || columnIndex >= uiDataGridView1.Columns.Count)
                return;

            if (uiDataGridView1.Rows[rowIndex].IsNewRow)
                return;

            uiDataGridView1.Rows[rowIndex].Cells[columnIndex].Value = value;

            uiDataGridView1.FirstDisplayedScrollingRowIndex = uiDataGridView1.Rows.Count - 1;
        }
        /// <summary>
        /// 设置DataGridView指定单元格的颜色
        /// </summary>
        /// <param name="dgv">DataGridView控件</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="isGreen">true设为绿色，false设为红色</param>
        public void SetCellColor(int rowIndex, int columnIndex, bool isGreen)
        {
            // 检查是否需要Invoke
            if (uiDataGridView1.InvokeRequired)
            {
                uiDataGridView1.Invoke(new Action(() => SetCellColor(rowIndex, columnIndex, isGreen)));
                return;
            }
            // 检查索引是否有效
            if (rowIndex < 0 || rowIndex >= uiDataGridView1.Rows.Count ||
                columnIndex < 0 || columnIndex >= uiDataGridView1.Columns.Count)
                return;

            // 设置单元格背景色
            uiDataGridView1.Rows[rowIndex].Cells[columnIndex].Style.BackColor =
                isGreen ? Color.Green : Color.Red;

            // 设置文字颜色为白色，确保可读性
            uiDataGridView1.Rows[rowIndex].Cells[columnIndex].Style.ForeColor = Color.White;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // 获取空闲时间
                //TimeSpan idle = IdleMonitor.GetIdleTime();
                //Console.WriteLine($"用户空闲时长: {idle:hh\\:mm\\:ss}");

                // 判断空闲是否超过5分钟
                if (IdleMonitor.IsIdleFor(TimeSpan.FromMinutes(1)))
                {
                    if(LoginPassword.LoginState != (int)UserState.空)
                    {
                        uiTextBox8.Text = "操作员";

                        uiTextBox3.ReadOnly = true;
                        uiTextBox4.ReadOnly = true;
                        uiTextBox5.ReadOnly = true;
                        uiTextBox6.ReadOnly = true;
                    }                    
                }
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine($"调用系统API失败: {ex.Message}");
            }

            //// 日志显示
            //if (this.uiListBox2.Items.Count > 1000)
            //{
            //    this.uiListBox2.Items.Clear();
            //}
            //try
            //{
            //    if (WriteLogGlobal.writeLogSystem.ringBuffer.Count > 0)
            //    {
            //        WriteLogGlobal.writeLogSystem.ringBuffer.TryDequeue(out string logInfo);
            //        uiListBox2.Items.Add(logInfo);
            //    }
            //    // 滚动到最后一行
            //    uiListBox2.TopIndex = uiListBox2.Items.Count - 1;
            //}
            //catch { }

            //// 日志显示
            //if (this.uiListBox1.Items.Count > 1000)
            //{
            //    this.uiListBox1.Items.Clear();
            //}
            //try
            //{
            //    if (WriteLogGlobal.writeLogProduce.ringBuffer.Count > 0)
            //    {
            //        WriteLogGlobal.writeLogProduce.ringBuffer.TryDequeue(out string logInfo);
            //        uiListBox1.Items.Add(logInfo);
            //    }
            //    // 滚动到最后一行
            //    uiListBox1.TopIndex = uiListBox1.Items.Count - 1;
            //}
            //catch { }
        }
        /// <summary>
        /// 检查权限
        /// </summary>
        public bool CheckPermission(UserState userState)
        {
            if((int)userState != LoginPassword.LoginState)
            {
                UIMessageBox.ShowError("请登录权限！");
                return false;
            }
            return true;
        }

        private void uiNavBar1_NodeMouseClick(TreeNode node, int menuIndex, int pageIndex)
        {
            if(node.Text == "新建")
            {
                string name = uiTextBox10.Text;
                global.ProjectName = name;
                if (string.IsNullOrEmpty(name))
                {
                    UIMessageBox.ShowError("请填写工程名称！");
                    return;
                }

                string FileName = global.FilePath + @"\" + name + ".json";
                // 检查文件是否存在
                if (File.Exists(FileName))
                {
                    UIMessageBox.ShowError("文件已存在，新建失败！");
                    return;
                }
                else
                {
                    // 文件不存在，创建文件
                    File.Create(FileName).Close(); // 创建文件并关闭文件流

                    global.RootNodeDataList.Clear();
                    UIMessageBox.ShowSuccess("文件新建成功！");
                }          
            }
            else if (node.Text == "打开")
            {
                // 创建打开文件对话框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "选择文件",
                    Filter = "文本文件|*.json|所有文件|*.*",
                    InitialDirectory = global.FilePath, // 初始目录
                    Multiselect = false, // 是否允许多选
                    CheckFileExists = true // 检查文件是否存在
                };

                // 显示对话框并判断是否点击了"确定"
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // 获取文件名不带扩展名
                    global.ProjectName = Path.GetFileNameWithoutExtension(selectedFilePath);
                    uiTextBox10.Text = global.ProjectName;

                    LoadData(global.FilePath + @"\" + global.ProjectName + ".json");
                }
            }
            else if (node.Text == "保存")
            {
                try
                {
                    if (!EditForm.SaveData()) return;
                    LoadData(global.FilePath + @"\" + global.ProjectName + ".json");                   
                }
                catch (Exception ex)
                {
                    UIMessageBox.ShowError("保存失败！" + ex.Message);
                }
            }
            else if (node.Text == "登录")
            {
                LoginForm loginForm = new LoginForm();
                loginForm.ShowDialog();
            }
            else if (node.Text == "日志")
            {
                LogForm logForm = new LogForm();
                logForm.ShowDialog();
            }
            else if (node.Text == "MES")
            {

            }
            else if (node.Text == "参数设置")
            {
                ParameterForm parameterForm = new ParameterForm();
                parameterForm.ShowDialog();
            }
            else if(node.Text == "读取RFID")
            {
                rfidTestForm rfidTestForm = new rfidTestForm();
                rfidTestForm.ShowDialog();
            }
        }

        private void 执行一次ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index;
            if (this.uiDataGridView1.CurrentRow == null)
            {
                index = -1;
            }
            else
            {
                index = this.uiDataGridView1.CurrentRow.Index;
            }
            if (index < 0)
            {
                UIMessageBox.ShowError("请选择要执行的行号!");
                return;
            }
            else
            {
                List<bool> _boolList = new List<bool>();
                _boolList.Clear();
                string ChildResult = "";

                int id1 = 0;

                foreach (var root in new[] { global.RootNodeDataList[index].RootData1, global.RootNodeDataList[index].RootData2, global.RootNodeDataList[index].RootData3 })// 前提条件   操作步骤  期望结果
                {
                    id1++;
                    foreach (var child in root.Children)
                    {
                        var param = child.TestParam;
                        var result = child.Plugin?.Run(param);// 得到结果

                        if (id1 == 3)// 期望结果
                        {
                            int id2 = 0;
                            foreach (var kv in result)
                            {
                                id2++;
                                ChildResult += id2 + "：" + kv.Key + "：" + kv.Value + "\r\n";
                                global.writeLogProduce.Info(id2 + "：" + kv.Key + "：" + kv.Value + "\r\n");
                                SetCellValueThreadSafe(index, 5, ChildResult.Trim());
                                if (kv.Key.Contains("结果"))
                                {
                                    if (kv.Value is bool)
                                    {
                                        _boolList.Add(Convert.ToBoolean(kv.Value));
                                    }
                                }
                                else if (kv.Key == "图片")
                                {
                                    GYCamBitmap((Bitmap)kv.Value);
                                }
                            }
                        }
                    }
                    if (id1 == 3)
                    {
                        if (_boolList.All(x => x))
                        {
                            SetCellValueThreadSafe(index, 6, "PASS");
                            SetCellColor(index, 6, true);
                        }
                        else
                        {
                            SetCellValueThreadSafe(index, 6, "FAIL");
                            SetCellColor(index, 6, false);
                        }
                        SetCellValueThreadSafe(index, 7, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
            }
        }
        /// <summary>
        /// 放行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            //global.rFIDHelper.Write("155555测试qwefg");
            //var valeu = global.rFIDHelper.Read();    

            // 1：打开光源控制器
            //string cmd = "$WAN#";
            //global.serialHelper_Light.SendString(cmd);

            //global.inovanceH5UTcpTool.Write("D110", 1);

            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = false;
            Thread.Sleep(2000);
            WorkThread.GetInstance().workStation.Run_Step[(int)WorkThreadStep.测试] = 16;
            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = true;
        }
        /// <summary>
        /// 重测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = false;
            Thread.Sleep(2000);
            global.inovanceH5UTcpTool.Write("D112", 1);
            global.writeLogProduce.Info("给PLC发送重测, D112写入1！");
            WorkThread.GetInstance().workStation.Run_Step[(int)WorkThreadStep.测试] =10;
            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = true;
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.GetInstance().SaveConfig();
        }
    }
    // 辅助类：用于序列化
    public class SerializableGlobalData
    {
        public List<RootNode3Data> RootNodeDataList { get; set; }
    }
}
