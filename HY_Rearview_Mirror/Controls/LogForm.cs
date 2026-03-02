using Sunny.UI;
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

namespace HY_Rearview_Mirror.Controls
{
    public partial class LogForm : UIForm
    {
        private string _Log1FliePath = "";

        // 存储原始数据（从ListBox加载时保存）
        private List<string> originalItems = new List<string>();
        private bool isSearchMode = false;
        public LogForm()
        {
            InitializeComponent();

            // 设置搜索框事件
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyDown += TxtSearch_KeyDown;
        }
        #region 数据保存与恢复

        // 保存ListBox中的原始数据
        private void SaveOriginalData()
        {
            originalItems.Clear();
            foreach (var item in uiListBox1.Items)
            {
                originalItems.Add(item.ToString());
            }
        }

        // 恢复原始数据
        private void RestoreOriginalData()
        {
            uiListBox1.Items.Clear();
            foreach (string item in originalItems)
            {
                uiListBox1.Items.Add(item);
            }
            isSearchMode = false;
        }

        #endregion

        #region 搜索功能

        // 搜索框文本变化事件（实时搜索）
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                RestoreOriginalData();
                return;
            }

            // 延迟搜索（防止频繁刷新）
            // 实际使用中可以考虑使用Timer延迟
            PerformSearch(txtSearch.Text);
        }

        // 搜索框按键事件
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch(txtSearch.Text);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
                RestoreOriginalData();
            }
        }

        // 搜索按钮点击事件
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch(txtSearch.Text);
        }

        // 清除搜索按钮
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            RestoreOriginalData();
        }

        // 执行搜索
        private void PerformSearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                RestoreOriginalData();
                return;
            }

            List<string> results = SearchContains(originalItems, searchText, true);

            // 更新显示
            UpdateSearchResults(results, searchText);
        }

        // 更新搜索结果到ListBox
        private void UpdateSearchResults(List<string> results, string searchText)
        {
            uiListBox1.BeginUpdate(); // 批量更新，提高性能
            uiListBox1.Items.Clear();

            if (results.Count == 0)
            {
                uiListBox1.Items.Add($"未找到包含 \"{searchText}\" 的记录");
            }
            else
            {
                // 添加高亮显示的搜索结果
                foreach (string result in results)
                {
                    uiListBox1.Items.Add(result);
                }
            }

            uiListBox1.EndUpdate();
            isSearchMode = true;

            // 如果有结果，自动选中第一条
            if (results.Count > 0)
            {
                uiListBox1.SelectedIndex = 0;
            }
        }
        // 模糊搜索（包含关键词）
        private List<string> SearchContains(List<string> items, string keyword, bool caseSensitive)
        {
            List<string> results = new List<string>();

            StringComparison comparison = caseSensitive ?
                StringComparison.Ordinal :
                StringComparison.OrdinalIgnoreCase;

            // 支持多个关键词（空格分隔）
            string[] keywords = keyword.Split(new[] { ' ', '，', ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in items)
            {
                bool allMatched = true;

                foreach (string kw in keywords)
                {
                    if (!item.Contains(kw))
                    {
                        allMatched = false;
                        break;
                    }
                }

                if (allMatched)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        #endregion
        private void LogForm_Load(object sender, EventArgs e)
        {
            this.FormClosed += LogForm_FormClosed;
        }

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            string path = @"D:\DataSystem" + @"\" + _Log1FliePath;

            // 清空ListBox
            uiListBox1.Items.Clear();

            try
            {
                // 检查文件是否存在
                if (!File.Exists(path))
                {
                    uiListBox1.Items.Add("日志文件不存在");
                    return;
                }

                // 读取文件所有行
                string[] lines = File.ReadAllLines(path, Encoding.Default);

                // 添加到ListBox
                foreach (string line in lines)
                {
                    uiListBox1.Items.Add(line);
                }

                // 保存原始数据
                SaveOriginalData();
            }
            catch (Exception ex)
            {
                uiListBox1.Items.Add($"读取日志失败: {ex.Message}");
            }
        }
        private void uiDatetimePicker1_ValueChanged(object sender, DateTime value)
        {
            _Log1FliePath = $"Log{value:yyyy-MM-dd}.log";
        }
    }
}
