using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataLib
{
    public delegate void SetAndSavePublicData_Delegate(string AxisName, string value);
    public delegate Dictionary<string, string> ReadPublicDataJsonFile_Delegate();
    public partial class PublicDataTool : UserControl
    {
        private static Dictionary<string, string> PublicDataDictionary = new Dictionary<string, string>();
        private string path = AppDomain.CurrentDomain.BaseDirectory + @"Resource\PublicData\" + @"PublicData.json";
        private static SetAndSavePublicData_Delegate setAndSavePublicData_Delegate;
        private static ReadPublicDataJsonFile_Delegate readPublicDataJsonFile_Delegate;
        public PublicDataTool()
        {
            InitializeComponent();
            #region   初始化控件缩放
            x = Width;
            y = Height;
            setTag(this);
            #endregion
            setlGridViewData(ref dataGridView1);
            setAndSavePublicData_Delegate += SetAndSavePublicData;
            readPublicDataJsonFile_Delegate += ReadPublicDataJsonFile;
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

        #region 设置DataGridView的参数
        /// <summary>
        /// 设置DataGridView的参数
        /// </summary>
        /// <param name="mDataGridView"></param>
        public void setlGridViewData(ref DataGridView mDataGridView)
        {
            // 设置dataGridView1允许编辑
            mDataGridView.ReadOnly = false;
            mDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            mDataGridView.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            mDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            mDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            mDataGridView.ColumnHeadersHeight = 80;
            // 设置DataGridView所有行的默认高度
            mDataGridView.RowTemplate.Height = 40;
            mDataGridView.AllowUserToAddRows = false;
            // 设置表头文字居中
            mDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        #endregion

        /// <summary>
        /// 根据参数名称，返回数据
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="variableName"></param>
        public static string GetUserData(string AxisName)
        {
            string value = "";
            try
            {
                value = PublicDataDictionary[AxisName];
                return value;
            }
            catch
            {
                return value;
            }
        }
        /// <summary>
        /// 根据参数名，修改数据，并保存数据
        /// </summary>
        /// <param name="AxisName"></param>
        public void SetAndSavePublicData(string AxisName, string value)
        {
            ReadPublicDataJsonFile();
            try
            {
                List<object> TempArr = new List<object>();
                TempArr.Clear();
                PublicDataDictionary[AxisName] = value;
                // 遍历字典
                foreach (KeyValuePair<string, string> pair in PublicDataDictionary)
                {
                    TempArr.Add(pair.Key.ToString());
                    TempArr.Add(pair.Value.ToString());
                }

                WriteJsonFile(TempArr);
                ReadPublicDataJsonFile();
                addDataGridViewData(ref dataGridView1, ref PublicDataDictionary);
            }
            catch { }
        }
        /// <summary>
        /// 保存Json数据
        /// </summary>
        /// <param name="arr"></param>
        public void WriteJsonFile(List<object> arr)
        {
            FileCopyHelper.FileCreate(path);
            //string path1 = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Data\" + @"UserData.json";
            string jsonString = JsonConvert.SerializeObject(arr, Formatting.Indented);
            File.WriteAllText(path, jsonString);
        }
        public Dictionary<string, string> ReadPublicDataJsonFile()
        {
            Dictionary<string, string> TempUserDataDictionary = new Dictionary<string, string>();
            TempUserDataDictionary.Count();
            PublicDataDictionary.Clear();

            try
            {
                FileCopyHelper.FileCreate(path);
                //string path = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Data\" + @"UserData.json";
                string json = File.ReadAllText(path); // 从文件中读取JSON数据

                string[] items = JsonConvert.DeserializeObject<string[]>(json);

                if (items == null || items.Length == 0)
                {
                    return TempUserDataDictionary;
                }

                string[] DelayName = new string[2];
                int idex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    DelayName[idex] = items[i];
                    if (idex == 1)
                    {
                        PublicDataDictionary.Add(DelayName[0], DelayName[1]);
                        TempUserDataDictionary.Add(DelayName[0], DelayName[1]);
                        idex = 0;
                    }
                    else
                    {
                        idex++;
                    }
                }
                //数据通过委托传出去
                //sendDelayDictionaryDelegate(DelayDictionary);
                addDataGridViewData(ref dataGridView1, ref PublicDataDictionary);
                return TempUserDataDictionary;
            }
            catch
            {
                this.dataGridView1.Rows.Clear();
                this.dataGridView1.DataSource = null;
                return TempUserDataDictionary;
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="count"></param>
        public void addDataGridViewData(ref DataGridView mDataGridView, ref Dictionary<string, string> arr)
        {
            mDataGridView.Rows.Clear();
            mDataGridView.DataSource = null;
            int index;
            int count = 0;
            // 遍历字典
            foreach (KeyValuePair<string, string> pair in arr)
            {
                index = mDataGridView.Rows.Add();
                mDataGridView.Rows[count].Cells[0].Value = pair.Key;
                mDataGridView.Rows[count].Cells[1].Value = pair.Value;
                mDataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                mDataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                count++;
            }
        }

        /// <summary>
        ///  获取保存DataGridView的数据
        /// </summary>
        /// <param name="mDataGridView"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public List<object> setSaveGridViewData(ref DataGridView mDataGridView, int cols)
        {
            List<object> tempArr = new List<object>();
            try
            {
                tempArr.Clear();
                string[] dataGridViewArr = new string[500];//保存数据的缓冲数组
                int RowCount;
                //获取dataGridView3单元格所有行的第一列的数据
                RowCount = dataGridView1.Rows.Count;//获取总行数

                int idex = 0;
                for (int i = 0; i < RowCount; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {

                        if (mDataGridView.Rows[i].Cells[j].Value != null)
                        {
                            string result = (mDataGridView.Rows[i].Cells[j].Value).ToString().Replace(" ", "");
                            dataGridViewArr[idex] = result;
                        }
                        else
                        {
                            dataGridViewArr[idex] = "";
                        }
                        idex++;
                    }
                }

                for (int i = 0; i < RowCount * cols; i++)
                {
                    tempArr.Add(dataGridViewArr[i]);
                }
                return tempArr;
            }
            catch (Exception e)
            {
                tempArr.Add("");
                MessageBox.Show("错误！" + e);
                return tempArr;
            }
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<object> arr = setSaveGridViewData(ref this.dataGridView1, 2);
            if(arr == null)
            {
                arr = new List<object>();
            }

            WriteJsonFile(arr);
            ReadPublicDataJsonFile();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index;
            if (this.dataGridView1.CurrentRow == null)
            {
                index = -1;
            }
            else
            {
                index = this.dataGridView1.CurrentRow.Index;
            }
            if (index < 0)
            {
                MessageBox.Show("请选择要删除的行号!");
                return;
            }
            else
            {
                if (MessageBox.Show("确认删除该行吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.dataGridView1.Rows.RemoveAt(index);
                }
            }
        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index;
            index = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[index].Cells[0].Value = " ";
            this.dataGridView1.Rows[index].Cells[1].Value = " ";
        }

        private void 插入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (indexSelect != -1)
            {
                // 创建新行
                DataGridViewRow row = new DataGridViewRow();
                dataGridView1.Rows.Insert(indexSelect + 1, row);
                this.dataGridView1.Rows[indexSelect + 1].Cells[0].Value = " ";
                this.dataGridView1.Rows[indexSelect + 1].Cells[1].Value = " ";
            }
        }
        int indexSelect = -1;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            indexSelect = dataGridView1.CurrentRow.Index;
        }

        private void PublicDataTool_Resize(object sender, EventArgs e)
        {
            //垂直窗体布局
            ReWinformLayout();
        }

        private void PublicDataTool_Load(object sender, EventArgs e)
        {
            ReadPublicDataJsonFile();
        }
    }
}
