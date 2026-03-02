using HalconDotNet;
using Helper;
using hWindowTool;
using HY_Rearview_Mirror.Controls;
using HY_Rearview_Mirror.Functions;
using HY_Rearview_Mirror.InterfaceTool;
using MvCameraControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace HY_Rearview_Mirror.User
{
    public partial class GYCamSetForm : UIForm
    {
        List<Point> _Pos9List = new List<Point>();

        public EventManager eventManager = new EventManager();
        //public event EventHandler<List<object>> OrderCompleted;
        //public event Func<Dictionary<string, object>> RunHandler;
        //public event Action FormClosingHandler;
        public int _uiDataGridView2_iD = -1;
        DataGridComboBox dataGridComboBox_Brightness = new DataGridComboBox(); // 亮度
        DataGridComboBox dataGridComboBox_Chroma = new DataGridComboBox(); // 色度
        DataGridComboBox dataGridComboBox_Contrast = new DataGridComboBox(); // 对比度
        DataGridComboBox dataGridComboBox_Brightness_Chroma = new DataGridComboBox(); // 亮度，色度
        DataGridComboBox dataGridComboBox_Brightness_Chroma_Contrast = new DataGridComboBox(); // 亮度，色度，对比度

        public GYCamSetForm()
        {
            InitializeComponent();


            /*************************************************生成参数配置表格*****************************************************/
            dataGridComboBox_Brightness = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            var products = new List<Product>
            {
                new Product { 亮度 = "3",阈值 = "6"},
            };
            dataGridComboBox_Brightness.DataSource = products;  // 绑定List;
            dataGridComboBox_Brightness.Dock = DockStyle.Fill;
            panel5.Controls.Add(dataGridComboBox_Brightness);



            dataGridComboBox_Chroma = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            var products1 = new List<Product1>
            {
                new Product1 { 色坐标X = "3",色坐标Y = "6", 色坐标X阈值 = "10", 色坐标Y阈值 = "10"},
            };
            dataGridComboBox_Chroma.DataSource = products1;  // 绑定List;
            dataGridComboBox_Chroma.Dock = DockStyle.Fill;
            panel5.Controls.Add(dataGridComboBox_Chroma);



            dataGridComboBox_Contrast = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            var products2 = new List<Product2>
            {
                new Product2 { 对比度= "3",阈值 = "10",},
            };
            dataGridComboBox_Contrast.DataSource = products2;  // 绑定List;
            dataGridComboBox_Contrast.Dock = DockStyle.Fill;
            panel5.Controls.Add(dataGridComboBox_Contrast);



            dataGridComboBox_Brightness_Chroma = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            var products3 = new List<Product3>
            {
                new Product3 {  名称= "亮度",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "色坐标X",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "色坐标Y",标准值 = "20",阈值 = "10",},
            };
            dataGridComboBox_Brightness_Chroma.DataSource = products3;  // 绑定List;
            dataGridComboBox_Brightness_Chroma.Dock = DockStyle.Fill;
            panel5.Controls.Add(dataGridComboBox_Brightness_Chroma);



            dataGridComboBox_Brightness_Chroma_Contrast = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            var products4 = new List<Product3>
            {
                new Product3 {  名称= "黑场亮度",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "黑场色坐标X",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "黑场色坐标Y",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "白场亮度",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "白场色坐标X",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "白场色坐标Y",标准值 = "20",阈值 = "10",},
                new Product3 {  名称= "对比度",标准值 = "20",阈值 = "10",}
            };
            dataGridComboBox_Brightness_Chroma_Contrast.DataSource = products4;  // 绑定List;
            dataGridComboBox_Brightness_Chroma_Contrast.Dock = DockStyle.Fill;
            panel5.Controls.Add(dataGridComboBox_Brightness_Chroma_Contrast);

            /**************************************************************************************************************************/

            global.gYCamTool.SendImagHandler += GetImag;
            //订阅halcon控件鼠标按下事件
            hWindow1.hWindowControl.HMouseDown += HWindowControl_HMouseDown;

            // 关键设置：禁用自动添加行
            this.uiDataGridView2.AllowUserToAddRows = false;
            GenerateNineEmployees();
        }
        /// <summary>
        /// halcon控件鼠标按下时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseDown(object sender, HalconDotNet.HMouseEventArgs e)
        {
            if (uiCheckBox1.Checked)
            {
                if(_uiDataGridView2_iD == -1)
                {
                    return;
                }
                else
                {
                    double X = e.X;
                    double Y = e.Y;

                    this.uiDataGridView2.Rows[_uiDataGridView2_iD].Cells[1].Value = Math.Round(X, 0).ToString();
                    this.uiDataGridView2.Rows[_uiDataGridView2_iD].Cells[2].Value = Math.Round(Y, 0).ToString();
                }
            }           
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            global.gYCamTool.SendImagHandler -= GetImag;
            eventManager.Trigger("关闭窗体");
            this.Dispose();
        }
        private void GenerateNineEmployees()
        {
            for (int i = 1; i <= 9; i++)
            {
                int index = this.uiDataGridView2.Rows.Add();
                this.uiDataGridView2.Rows[index].Cells[0].Value = i.ToString();
                this.uiDataGridView2.Rows[index].Cells[1].Value = " ";
                this.uiDataGridView2.Rows[index].Cells[2].Value = " ";
            }
        }

        public void GetImag(Bitmap bitmap)
        {
            // 检查是否需要Invoke
            if (hWindow1.InvokeRequired)
            {
                hWindow1?.Invoke(new Action(() => GetImag(bitmap)));
                return;
            }

            HObject ho_img = new HObject();
            ImageTool.GetInstance().Bitmap2HObjectBpp24(bitmap, out ho_img);

            hWindow1.HobjectToHimage(ho_img);  // 显示

            bitmap?.Dispose();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            string str = uiTextBox1.Text;
            string str1 = uiTextBox2.Text;
            string str2 = uiTextBox4.Text;
            string str3 = uiTextBox5.Text;
            string str4 = uiTextBox6.Text;
            global.gYCamTool.SetExpoGain(str, str1, str2, str3, str4);
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            var result = eventManager.TriggerWithResult("运行");

            if (result is Dictionary<string, object> dict)  // 类型转换
            {
                List<string> list = new List<string>();
                foreach (var kv in dict)
                {
                    list.Add(kv.Key);
                    list.Add(Convert.ToString(kv.Value));
                }
                uiListBox1.DataSource = list;
            }
            else if (result != null)
            {
                MessageBox.Show($"返回值不是字典类型，实际类型: {result.GetType()}");
            }
        }
        /// <summary>
        /// 遍历所有单元格
        /// </summary>
        private bool TraverseAllCells()
        {
            if (uiDataGridView2.Rows.Count == 0) return false;

            _Pos9List.Clear();
            int id = 0;
            Point point;
            // 遍历所有行（包括新行）
            for (int row = 0; row < uiDataGridView2.Rows.Count; row++)
            {
                // 跳过新行（如果是允许添加新行）
                if (uiDataGridView2.Rows[row].IsNewRow) continue;

                point = new Point();
                // 遍历所有列
                for (int col = 0; col < uiDataGridView2.Columns.Count; col++)
                {
                    id++;

                    DataGridViewCell cell = uiDataGridView2.Rows[row].Cells[col];
                    // 获取单元格值
                    object cellValue = cell.Value;

                    if(string.IsNullOrEmpty(cellValue.ToString()))
                    {                      
                        return false;
                    }

                    if (id ==2)
                    {
                        try
                        {
                            point.X = Convert.ToInt32(cellValue);
                        }
                        catch
                        {
                            return false;
                        }
                        
                    }
                    else if (id == 3)
                    {
                        try
                        {
                            point.Y = Convert.ToInt32(cellValue);
                        }
                        catch
                        {
                            return false;
                        }
                    }                    
                }
                _Pos9List.Add(point);
                id = 0;
            }
            return true;
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            if(!TraverseAllCells())
            {
                UIMessageBox.ShowError("请填写完整数据！");
                return;
            }
            string str1 = uiTextBox3.Text;
            string str2 = uiTextBox1.Text;
            string str3 = uiTextBox2.Text;
            string str4 = uiTextBox4.Text;
            string str5 = uiTextBox5.Text;
            string str6 = uiTextBox6.Text;
            string str7 = uiComboBox1.Text;
            string str8 = uiComboBox2.Text;

            if (str7 == "亮度")
            {
                List<Product> list = dataGridComboBox_Brightness.GetList<Product>();
                List<string> list2 = new List<string>();
                list2.Clear();
                foreach (var itm in list)
                {
                    list2.Add(itm.亮度);
                    list2.Add(itm.阈值);
                }     
                eventManager.Trigger("发送界面参数", new List<object>() { _Pos9List, str1, str2, str3, str4, str5, str6, str7, str8, list2 });
            }
            else if (str7 == "色度")
            {
                List<Product1> list = dataGridComboBox_Chroma.GetList<Product1>();
                List<string> list2 = new List<string>();
                list2.Clear();
                foreach (var itm in list)
                {
                    list2.Add(itm.色坐标X);
                    list2.Add(itm.色坐标Y);
                    list2.Add(itm.色坐标X阈值);
                    list2.Add(itm.色坐标Y阈值);
                }
                eventManager.Trigger("发送界面参数", new List<object>() { _Pos9List, str1, str2, str3, str4, str5, str6, str7, str8, list2 });
            }
            else if(str7 == "对比度")
            {
                List<Product2> list = dataGridComboBox_Contrast.GetList<Product2>();
                List<string> list2 = new List<string>();
                list2.Clear();
                foreach (var itm in list)
                {
                    list2.Add(itm.对比度);
                    list2.Add(itm.阈值);
                }
                eventManager.Trigger("发送界面参数", new List<object>() { _Pos9List, str1, str2, str3, str4, str5, str6, str7, str8, list2 });
            }        
            else if (str7 == "亮度、色度")
            {
                List<Product3> list = dataGridComboBox_Brightness_Chroma.GetList<Product3>();
                List<string> list2 = new List<string>();
                list2.Clear();
                foreach (var itm in list)
                {
                    list2.Add(itm.标准值);
                    list2.Add(itm.阈值);
                }
                eventManager.Trigger("发送界面参数", new List<object>() { _Pos9List, str1, str2, str3, str4, str5, str6, str7, str8, list2 });
            }
            else if (str7 == "亮度、色度、对比度")
            {
                List<Product3> list = dataGridComboBox_Brightness_Chroma_Contrast.GetList<Product3>();
                List<string> list2 = new List<string>();
                list2.Clear();
                foreach (var itm in list)
                {
                    list2.Add(itm.标准值);
                    list2.Add(itm.阈值);
                }
                eventManager.Trigger("发送界面参数", new List<object>() { _Pos9List, str1, str2, str3, str4, str5, str6, str7, str8, list2 });
            }

            UIMessageBox.ShowSuccess("参数设置成功！");
        }
        public void SetDataToForm(ITestParam testParam, string VersionStr = "")
        {
            if (testParam == null) return;
            GYCamTestParam pressureTestParam = testParam as GYCamTestParam;

            var Pos9List = (List<Point>)pressureTestParam.Pos9List;

            if(Pos9List.Count > 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    this.uiDataGridView2.Rows[i].Cells[1].Value = Pos9List[i].X;
                    this.uiDataGridView2.Rows[i].Cells[2].Value = Pos9List[i].Y;
                }
            }
        
            uiTextBox3.Text = VersionStr;
            uiTextBox1.Text = pressureTestParam.exposure;
            uiTextBox2.Text = pressureTestParam.Gain;
            uiTextBox4.Text = pressureTestParam.X_exposure;
            uiTextBox5.Text = pressureTestParam.Y_exposure;
            uiTextBox6.Text = pressureTestParam.Z_exposure;
            uiComboBox1.Text = pressureTestParam.ChangeDataType;
            uiComboBox2.Text = pressureTestParam.JudgeType;

            if (pressureTestParam.ChangeDataType == "亮度")
            {
                var products = new List<Product>
                {
                    new Product { 亮度 = pressureTestParam.JudgeValueList[0],阈值 = pressureTestParam.JudgeValueList[1]}
                };
                dataGridComboBox_Brightness.DataSource = products;
            }
            else if (pressureTestParam.ChangeDataType == "色度")
            {
                var products = new List<Product1>
                {
                    new Product1 {  色坐标X = pressureTestParam.JudgeValueList[0], 色坐标Y = pressureTestParam.JudgeValueList[1],
                    色坐标X阈值 = pressureTestParam.JudgeValueList[2],  色坐标Y阈值 = pressureTestParam.JudgeValueList[3]}
                };
                dataGridComboBox_Chroma.DataSource = products;
            }
            else if (pressureTestParam.ChangeDataType == "对比度")
            {
                var products = new List<Product2>
                {
                    new Product2 {  对比度= pressureTestParam.JudgeValueList[0],  阈值 = pressureTestParam.JudgeValueList[1] }
                };
                dataGridComboBox_Contrast.DataSource = products;
            }
            else if (pressureTestParam.ChangeDataType == "亮度、色度")
            {
                var products = new List<Product3>
                {
                    new Product3 {  名称= "亮度",标准值 = pressureTestParam.JudgeValueList[0],阈值 = pressureTestParam.JudgeValueList[1]},
                    new Product3 {  名称= "色坐标X",标准值 = pressureTestParam.JudgeValueList[2],阈值 = pressureTestParam.JudgeValueList[3]},
                    new Product3 {  名称= "色坐标Y",标准值 = pressureTestParam.JudgeValueList[4],阈值 = pressureTestParam.JudgeValueList[5]}
                };
                dataGridComboBox_Brightness_Chroma.DataSource = products;
            }
            else if (pressureTestParam.ChangeDataType == "亮度、色度、对比度")
            {
                var products = new List<Product3>
                {
                    new Product3 {  名称= "黑场亮度",标准值 = pressureTestParam.JudgeValueList[0],阈值 = pressureTestParam.JudgeValueList[1]},
                    new Product3 {  名称= "黑场色坐标X",标准值 = pressureTestParam.JudgeValueList[2],阈值 = pressureTestParam.JudgeValueList[3]},
                    new Product3 {  名称= "黑场色坐标Y",标准值 = pressureTestParam.JudgeValueList[4],阈值 = pressureTestParam.JudgeValueList[5]},
                    new Product3 {  名称= "白场亮度",标准值 = pressureTestParam.JudgeValueList[6],阈值 = pressureTestParam.JudgeValueList[7]},
                    new Product3 {  名称= "白场色坐标X",标准值 = pressureTestParam.JudgeValueList[8],阈值 = pressureTestParam.JudgeValueList[9]},
                    new Product3 {  名称= "白场色坐标Y",标准值 = pressureTestParam.JudgeValueList[10],阈值 = pressureTestParam.JudgeValueList[11]},
                    new Product3 {  名称= "对比度",标准值 = pressureTestParam.JudgeValueList[12],阈值 = pressureTestParam.JudgeValueList[13]}
                };
                dataGridComboBox_Brightness_Chroma_Contrast.DataSource = products;
            }
        }

        private void uiDataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (uiDataGridView2.CurrentRow == null) return;
            try
            {
                _uiDataGridView2_iD = uiDataGridView2.CurrentRow.Index;
            }
            catch (Exception)
            {
                _uiDataGridView2_iD = -1;
            }
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            global.gYCamTool.VideoStart();
        }
        /// <summary>
        /// 根据配置项显示不同的表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = uiComboBox1.Text;
            if(str == "亮度")
            {
                dataGridComboBox_Brightness.Visible = true;
                dataGridComboBox_Chroma.Visible = false;
                dataGridComboBox_Contrast.Visible = false;
                dataGridComboBox_Brightness_Chroma .Visible = false;
                dataGridComboBox_Brightness_Chroma_Contrast.Visible = false;

            }
            else if (str == "色度")
            {
                dataGridComboBox_Brightness.Visible = false;
                dataGridComboBox_Chroma.Visible = true;
                dataGridComboBox_Contrast.Visible = false;
                dataGridComboBox_Brightness_Chroma.Visible = false;
                dataGridComboBox_Brightness_Chroma_Contrast.Visible = false;
            }
            else if (str == "对比度")
            {
                dataGridComboBox_Brightness.Visible = false;
                dataGridComboBox_Chroma.Visible = false;
                dataGridComboBox_Contrast.Visible = true;
                dataGridComboBox_Brightness_Chroma.Visible = false;
                dataGridComboBox_Brightness_Chroma_Contrast.Visible = false;
            }
            else if (str == "亮度、色度")
            {
                dataGridComboBox_Brightness.Visible = false;
                dataGridComboBox_Chroma.Visible = false;
                dataGridComboBox_Contrast.Visible = false;
                dataGridComboBox_Brightness_Chroma.Visible = true;
                dataGridComboBox_Brightness_Chroma_Contrast.Visible = false;
            }
            else if (str == "亮度、色度、对比度")
            {
                dataGridComboBox_Brightness.Visible = false;
                dataGridComboBox_Chroma.Visible = false;
                dataGridComboBox_Contrast.Visible = false;
                dataGridComboBox_Brightness_Chroma.Visible = false;
                dataGridComboBox_Brightness_Chroma_Contrast.Visible = true;
            }
        }
        /// <summary>
        /// 导出九点坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            TraverseAllCells();
            if (_Pos9List.Count < 9)return;

            // 序列化时（保存数据时）
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,  // 只在需要时添加类型信息
                // 或者使用引用保留（推荐）
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };

            // 使用 Newtonsoft.Json
            string json = JsonConvert.SerializeObject(_Pos9List, settings);
            File.WriteAllText(global.UserFilePath + @"\成像色度仪九点坐标.json", json);

            UIMessageBox.ShowSuccess("坐标导出成功！");
        }
        /// <summary>
        /// 导入九点坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton6_Click(object sender, EventArgs e)
        {
            string path = global.UserFilePath + @"\成像色度仪九点坐标.json";
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
                    _Pos9List = JsonConvert.DeserializeObject<List<Point>>(json, settings);

                    if (_Pos9List.Count == 9)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            this.uiDataGridView2.Rows[i].Cells[1].Value = _Pos9List[i].X;
                            this.uiDataGridView2.Rows[i].Cells[2].Value = _Pos9List[i].Y;
                        }
                    }
                    UIMessageBox.ShowSuccess("坐标导入成功！");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载数据失败: {ex.Message}");
                }
            }
        }
    }
    public class Product
    {
        public string 亮度 { get; set; }
        public string 阈值 { get; set; }
    }
    public class Product1
    {
        public string 色坐标X { get; set; }
        public string 色坐标Y { get; set; }
        public string 色坐标X阈值 { get; set; }
        public string 色坐标Y阈值 { get; set; }
    }
    public class Product2
    {
        public string 对比度 { get; set; }
        public string 阈值 { get; set; }
    }
    public class Product3
    {
        public string 名称 { get; set; }
        public string 标准值 { get; set; }
        public string 阈值 { get; set; }
    }
}
