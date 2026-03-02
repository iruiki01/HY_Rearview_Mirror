using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.Controls
{
    public partial class DataGridComboBox : UserControl
    {
        private TextBox textBox;
        private Button dropButton;
        private DataGridView dataGridView;
        private Form popupForm;
        private ContextMenuStrip contextMenu;

        private bool isEditMode = false;
        private ToolStripMenuItem editModeItem;
        private ToolStripMenuItem selectRowItem;
        private ToolStripMenuItem confirmSelectItem;  // 新增：确认选择菜单项

        public event EventHandler SelectedValueChanged;
        public event DataGridViewCellEventHandler CellValueChanged;
        public event EventHandler<DataGridViewRowEventArgs> RowDataSaved;
        public event EventHandler<DataGridViewRowEventArgs> RowAdded;
        public event EventHandler<DataGridViewRowEventArgs> RowDeleted;

        public bool IsEditMode
        {
            get => isEditMode;
            set
            {
                if (isEditMode != value)
                {
                    ToggleEditMode();
                }
            }
        }

        public DataGridComboBox()
        {
            InitializeComponents();
            InitializePopup();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(300, 25);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                BackColor = Color.White,
                TextAlign = HorizontalAlignment.Left,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(3, 4),
                Size = new Size(this.ClientSize.Width - 26, 17),
                Font = new Font("微软雅黑", 9F)
            };

            dropButton = new Button
            {
                Dock = DockStyle.Right,
                Width = 20,
                Text = "▼",
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            dropButton.FlatAppearance.BorderSize = 0;
            dropButton.Click += (s, e) => TogglePopup();

            this.Controls.Add(textBox);
            this.Controls.Add(dropButton);
            textBox.Click += (s, e) => TogglePopup();
        }

        private void InitializePopup()
        {
            InitializeContextMenu();
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                // 关键修改：先设为 None，等数据绑定后再设置列宽
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ReadOnly = false,
                EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AutoGenerateColumns = true,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowHeadersVisible = false,
                ContextMenuStrip = contextMenu,
            };
            // 窗口大小变化时保持等分（可选）
            dataGridView.SizeChanged += (s, e) => SetupColumnsWidth();
            // 关键修改：单击行为根据模式区分
            dataGridView.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return; // 忽略表头

                if (isEditMode)
                {
                    // 编辑模式：单击进入单元格编辑，不关闭表格
                    dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dataGridView.BeginEdit(true);
                }
                else
                {
                    // 选择模式：仅选中该行（高亮），不关闭表格，不更新文本框
                    dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dataGridView.Rows[e.RowIndex].Selected = true;
                    // 注意：这里不调用 SelectRow，所以不会隐藏 popupForm
                }
            };

            // 新增：双击事件
            dataGridView.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                if (!isEditMode)
                {
                    // 选择模式下双击：确认选择并关闭表格
                    SelectRow(e.RowIndex);
                }
                else
                {
                    // 编辑模式下双击：也进入编辑（与单击一致，作为备用）
                    dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dataGridView.BeginEdit(true);
                }
            };

            // 右键只显示菜单
            dataGridView.CellMouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {
                    dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dataGridView.Rows[e.RowIndex].Selected = true;
                }
            };

            dataGridView.CellValueChanged += (s, e) =>
            {
                CellValueChanged?.Invoke(this, e);

                if (e.RowIndex >= 0 && !string.IsNullOrEmpty(DisplayMember))
                {
                    var col = dataGridView.Columns[e.ColumnIndex];
                    if (col.Name == DisplayMember || col.DataPropertyName == DisplayMember)
                    {
                        var newValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                        if (!string.IsNullOrEmpty(newValue))
                            textBox.Text = newValue;
                    }
                }
            };

            dataGridView.RowValidated += (s, e) =>
            {
                if (e.RowIndex >= 0 && dataGridView.IsCurrentRowDirty)
                {
                    RowDataSaved?.Invoke(this, new DataGridViewRowEventArgs(dataGridView.Rows[e.RowIndex]));

                    if (dataGridView.DataSource is DataTable dt)
                    {
                        var dataRow = ((DataRowView)dataGridView.Rows[e.RowIndex].DataBoundItem).Row;
                        dataRow.AcceptChanges();
                    }
                }
            };

            // 修改键盘逻辑
            dataGridView.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dataGridView.IsCurrentCellInEditMode)
                    {
                        // 正在编辑时，Enter保存编辑
                        dataGridView.EndEdit();
                        e.Handled = true;
                    }
                    else if (!isEditMode && dataGridView.CurrentRow != null)
                    {
                        // 选择模式下，未在编辑状态时，Enter确认选择并关闭
                        SelectRow(dataGridView.CurrentRow.Index);
                    }
                    else if (isEditMode && dataGridView.CurrentRow != null)
                    {
                        // 编辑模式下，Enter进入当前单元格编辑
                        dataGridView.BeginEdit(true);
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    if (dataGridView.IsCurrentCellInEditMode)
                        dataGridView.CancelEdit();
                    else
                        popupForm.Hide();
                }
                else if (e.KeyCode == Keys.Delete && dataGridView.CurrentRow != null)
                {
                    DeleteCurrentRow();
                }
            };

            popupForm = new Form
            {
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                Size = new Size(300, 200),
                BackColor = Color.White,
                ControlBox = false,
                MinimizeBox = false,
                MaximizeBox = false
            };
            popupForm.Controls.Add(dataGridView);

            popupForm.Deactivate += (s, e) =>
            {
                if (dataGridView.IsCurrentCellInEditMode)
                    dataGridView.EndEdit();
                this.BeginInvoke(new Action(() => popupForm.Hide()));
            };
        }
        // 数据绑定后调用此方法设置列宽
        public void SetupColumnsWidth(int firstColumnWidth = 80)
        {
            if (dataGridView.Columns.Count == 0) return;

            // 第一列固定宽度
            dataGridView.Columns[0].Width = firstColumnWidth;
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView.Columns[0].Resizable = DataGridViewTriState.False; // 可选：禁止调整

            // 只有一列直接返回
            if (dataGridView.Columns.Count == 1) return;

            // 其他列等分剩余宽度
            int remainingWidth = dataGridView.ClientSize.Width - firstColumnWidth;
            int otherColumnCount = dataGridView.Columns.Count - 1;
            int equalWidth = remainingWidth / otherColumnCount;

            for (int i = 1; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridView.Columns[i].Width = equalWidth;
            }
        }


        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();

            // 添加
            var addItem = new ToolStripMenuItem("添加", null, (s, e) => AddNewRow())
            {
                ToolTipText = "添加新行"
            };

            // 删除
            var deleteItem = new ToolStripMenuItem("删除", null, (s, e) => DeleteCurrentRow())
            {
                ToolTipText = "删除当前选中的行",
                ForeColor = Color.Red
            };

            var separator1 = new ToolStripSeparator();

            // 编辑模式切换
            editModeItem = new ToolStripMenuItem("进入编辑模式", null, (s, e) => ToggleEditMode())
            {
                CheckOnClick = true,
                Checked = false,
                ToolTipText = "切换编辑/选择模式"
            };

            // 编辑模式专用：选中此行并关闭
            selectRowItem = new ToolStripMenuItem("选中此行", null, (s, e) =>
            {
                if (dataGridView.CurrentRow != null)
                {
                    SelectRow(dataGridView.CurrentRow.Index);
                }
            })
            {
                ToolTipText = "选中当前行并关闭下拉框（编辑模式）",
                Visible = false  // 默认隐藏，仅在编辑模式显示
            };

            // 选择模式专用：确认选择并关闭
            confirmSelectItem = new ToolStripMenuItem("确认选择", null, (s, e) =>
            {
                if (dataGridView.CurrentRow != null)
                {
                    SelectRow(dataGridView.CurrentRow.Index);
                }
            })
            {
                ToolTipText = "确认选择当前行并关闭下拉框（选择模式）",
                Font = new Font("微软雅黑", 9F, FontStyle.Bold)  // 粗体强调
            };

            var separator2 = new ToolStripSeparator();

            // 编辑单元格（备用）
            var editCellItem = new ToolStripMenuItem("编辑单元格", null, (s, e) => BeginEditCurrentCell())
            {
                ToolTipText = "开始编辑当前单元格"
            };

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                addItem,
                deleteItem,
                separator1,
                editModeItem,
                selectRowItem,
                confirmSelectItem,  // 新增
                separator2,
                editCellItem
            });

            contextMenu.Opening += (s, e) =>
            {
                bool hasSelection = dataGridView.CurrentRow != null;
                deleteItem.Enabled = hasSelection;
                editCellItem.Enabled = hasSelection;

                // 更新编辑模式菜单项
                editModeItem.Text = isEditMode ? "退出编辑模式" : "进入编辑模式";
                editModeItem.Checked = isEditMode;

                // 根据模式切换菜单项可见性
                if (isEditMode)
                {
                    selectRowItem.Visible = true;
                    selectRowItem.Enabled = hasSelection;
                    confirmSelectItem.Visible = false;  // 编辑模式隐藏确认选择
                }
                else
                {
                    selectRowItem.Visible = false;
                    confirmSelectItem.Visible = true;   // 选择模式显示确认选择
                    confirmSelectItem.Enabled = hasSelection;
                }
            };
        }

        private void ToggleEditMode()
        {
            isEditMode = !isEditMode;

            if (!isEditMode)
            {
                if (dataGridView.IsCurrentCellInEditMode)
                {
                    dataGridView.EndEdit();
                }
            }

            if (editModeItem != null)
            {
                editModeItem.Text = isEditMode ? "退出编辑模式" : "进入编辑模式";
                editModeItem.Checked = isEditMode;
            }
        }

        private void AddNewRow()
        {
            try
            {
                if (dataGridView.DataSource is DataTable dt)
                {
                    DataRow newRow = dt.NewRow();
                    dt.Rows.Add(newRow);

                    int newIndex = dt.Rows.IndexOf(newRow);
                    dataGridView.CurrentCell = dataGridView.Rows[newIndex].Cells[0];
                    dataGridView.Rows[newIndex].Selected = true;
                    RowAdded?.Invoke(this, new DataGridViewRowEventArgs(dataGridView.Rows[newIndex]));

                    // 如果是编辑模式，自动进入编辑
                    if (isEditMode)
                    {
                        dataGridView.BeginEdit(true);
                    }
                }
                else if (dataGridView.DataSource != null)
                {
                    var list = dataGridView.DataSource as System.Collections.IList;
                    if (list != null)
                    {
                        var itemType = list.GetType().GetGenericArguments().FirstOrDefault();
                        if (itemType != null)
                        {
                            var newItem = Activator.CreateInstance(itemType);
                            list.Add(newItem);
                            dataGridView.Refresh();

                            int newIndex = list.Count - 1;
                            if (newIndex >= 0 && newIndex < dataGridView.Rows.Count)
                            {
                                dataGridView.CurrentCell = dataGridView.Rows[newIndex].Cells[0];
                                dataGridView.Rows[newIndex].Selected = true;
                                RowAdded?.Invoke(this, new DataGridViewRowEventArgs(dataGridView.Rows[newIndex]));

                                if (isEditMode)
                                {
                                    dataGridView.BeginEdit(true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加行失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteCurrentRow()
        {
            if (dataGridView.CurrentRow == null) return;

            int rowIndex = dataGridView.CurrentRow.Index;
            if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count) return;

            if (MessageBox.Show("确定要删除选中的行吗？", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                if (dataGridView.IsCurrentCellInEditMode)
                {
                    dataGridView.EndEdit();
                }

                var row = dataGridView.Rows[rowIndex];
                bool deleted = false;

                if (dataGridView.DataSource is DataTable dt)
                {
                    var dataRow = ((DataRowView)row.DataBoundItem).Row;
                    dt.Rows.Remove(dataRow);
                    deleted = true;
                }
                else if (dataGridView.DataSource is System.Collections.IList list)
                {
                    bool isBindingList = list.GetType().IsGenericType &&
                                        list.GetType().GetGenericTypeDefinition() == typeof(BindingList<>);

                    if (!isBindingList)
                    {
                        dataGridView.DataSource = null;
                        list.RemoveAt(rowIndex);
                        dataGridView.DataSource = list;
                    }
                    else
                    {
                        list.RemoveAt(rowIndex);
                    }
                    deleted = true;
                }

                if (deleted)
                {
                    RowDeleted?.Invoke(this, new DataGridViewRowEventArgs(row));

                    if (dataGridView.Rows.Count == 0)
                    {
                        textBox.Text = "";
                        SelectedValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除行失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BeginEditCurrentCell()
        {
            if (dataGridView.CurrentCell != null)
            {
                dataGridView.BeginEdit(true);
            }
        }

        public object DataSource
        {
            get => dataGridView.DataSource;
            set
            {
                dataGridView.DataSource = value;
                if (value is System.Collections.IList list)
                    Console.WriteLine($"绑定数据：{list.Count} 行");
            }
        }

        public string DisplayMember { get; set; }
        public string ValueMember { get; set; }
        public object SelectedValue { get; private set; }

        #region List 数据获取方法（保持不变）

        public List<Dictionary<string, object>> GetDataAsList()
        {
            var list = new List<Dictionary<string, object>>();

            if (dataGridView.DataSource is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                        dict[col.ColumnName] = row[col];
                    list.Add(dict);
                }
            }
            else if (dataGridView.DataSource != null)
            {
                var source = dataGridView.DataSource as System.Collections.IEnumerable;
                foreach (var item in source)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in item.GetType().GetProperties())
                        dict[prop.Name] = prop.GetValue(item);
                    list.Add(dict);
                }
            }

            return list;
        }

        public Dictionary<string, object> GetSelectedItemAsDict()
        {
            if (dataGridView.CurrentRow == null) return null;

            var dict = new Dictionary<string, object>();

            if (dataGridView.DataSource is DataTable)
            {
                var row = ((DataRowView)dataGridView.CurrentRow.DataBoundItem).Row;
                foreach (DataColumn col in row.Table.Columns)
                    dict[col.ColumnName] = row[col];
            }
            else
            {
                var item = dataGridView.CurrentRow.DataBoundItem;
                if (item != null)
                {
                    foreach (var prop in item.GetType().GetProperties())
                        dict[prop.Name] = prop.GetValue(item);
                }
            }

            return dict;
        }

        public T GetSelectedItem<T>() where T : class
        {
            return dataGridView.CurrentRow?.DataBoundItem as T;
        }

        public List<T> GetList<T>() where T : class
        {
            var list = new List<T>();
            var source = dataGridView.DataSource as System.Collections.IEnumerable;
            if (source != null)
            {
                foreach (var item in source)
                {
                    if (item is T t) list.Add(t);
                }
            }
            return list;
        }

        public List<string> GetColumnValues(string columnName)
        {
            return dataGridView.Rows.Cast<DataGridViewRow>()
                .Select(r => r.Cells[columnName].Value?.ToString())
                .Where(v => v != null)
                .ToList();
        }

        #endregion

        private void TogglePopup()
        {
            if (popupForm.Visible)
            {
                popupForm.Hide();
                return;
            }

            if (dataGridView.DataSource == null)
            {
                MessageBox.Show("请先设置 DataSource", "提示");
                return;
            }

            Point location = this.PointToScreen(new Point(0, this.Height));
            Rectangle workingArea = Screen.GetWorkingArea(this);

            if (location.Y + popupForm.Height > workingArea.Bottom)
                location.Y = this.PointToScreen(Point.Empty).Y - popupForm.Height;

            popupForm.Width = this.Width;
            popupForm.Location = location;
            dataGridView.Refresh();
            dataGridView.ClearSelection();

            popupForm.Show(this);
            popupForm.BringToFront();
        }

        private void SelectRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count) return;

            var row = dataGridView.Rows[rowIndex];

            if (!string.IsNullOrEmpty(DisplayMember) && row.Cells[DisplayMember].Value != null)
                textBox.Text = row.Cells[DisplayMember].Value.ToString();
            else if (row.Cells.Count > 0)
                textBox.Text = row.Cells[0].Value?.ToString();

            if (!string.IsNullOrEmpty(ValueMember) && row.Cells[ValueMember].Value != null)
                SelectedValue = row.Cells[ValueMember].Value;
            else
                SelectedValue = row.Cells[0]?.Value;

            popupForm.Hide();
            SelectedValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public override string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }
    }
}