using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
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

namespace HY_Rearview_Mirror
{
    partial class EditForm : UIForm
    {
        //PluginManager pluginManager = null;
        string pluginName = "";
        TreeNode rootNode = null;
        TreeNode rootNode1 = null;
        TreeNode rootNode2 = null;
        string rootName = string.Empty;
        public static event Action<string> LoadDataHandler;
        public EditForm()
        {
            InitializeComponent();

            //pluginManager = new PluginManager("Plugins");
            // 启动插件系统
            //global.pluginManager.Start();
            // 执行所有插件
            //pluginManager.ExecuteAllPlugins();
            
            uiDataGridView1.DataSource = global._PluginDescribeList;

            InitializeTreeView();

            // 在窗体构造函数或Load事件中
            uiTreeView1.NodeMouseClick += UiTreeView1_NodeMouseClick;
            MainForm.UpdataTreeViewInitHandler += TreeViewInit;
        }
        ~ EditForm()
        {
            uiTreeView1.NodeMouseClick -= UiTreeView1_NodeMouseClick;
            MainForm.UpdataTreeViewInitHandler -= TreeViewInit;
        }
        // 重写OnFormClosing方法
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            MainForm.UpdataTreeViewInitHandler -= TreeViewInit;
            uiTreeView1.NodeMouseClick -= UiTreeView1_NodeMouseClick;
        }
        private void UiTreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 获取点击的节点
            TreeNode clickedNode = e.Node as TreeNode;
            if (clickedNode == null) return;

            // 获取根节点
            TreeNode rootNode = GetRootNode(clickedNode);

            // 显示根节点名称
            rootName = rootNode?.Text ?? "未找到根节点";
        }

        /// <summary>
        /// 获取指定节点的根节点
        /// </summary>
        private TreeNode GetRootNode(TreeNode node)
        {
            if (node == null) return null;

            // 如果当前节点就是根节点（Parent为null）
            if (node.Parent == null)
            {
                return node;
            }

            // 否则向上递归查找
            TreeNode parent = node.Parent;
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }
            return parent;
        }

        /// <summary>
        /// 测试运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            //if(string.IsNullOrEmpty(pluginName))
            //{
            //    UIMessageBox.ShowError("请选择要运行的项目！");
            //    return;
            //}
            //uiListBox1.DataSource = null;

            //var result = ExecutePlugin(pluginName);

            //List<string> list = new List<string>();
            //foreach (var kv in result)
            //{
            //    list.Add(kv.Key);
            //    list.Add(Convert.ToString(kv.Value));
            //}
            //uiListBox1.DataSource = list;
        }

        public Dictionary<string, object> ExecutePlugin(string pluginId)
        {
            if (global._loadedPlugins.TryGetValue(pluginId, out var container))
            {
                var param = container.GetData();

                var result = container?.Run(param);
                return result;
            }
            else
            {
                return null;
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
            if (id >= 0)
            {
                // 方法1：使用Cells集合
                string content = uiDataGridView1.CurrentRow.Cells["功能类型"].Value?.ToString();
                OpenSetDataForm(content);
            }
        }
        public void OpenSetDataForm(string pluginId)
        {
            if (global._loadedPlugins.TryGetValue(pluginId, out var container))
            {
                container.CreateUI(container.GetData(), container.VersionStr).ShowDialog();
            }
        }
        private void uiDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
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
            if (id >= 0)
            {
                pluginName = uiDataGridView1.CurrentRow.Cells["功能类型"].Value?.ToString();
            }
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        public void InitializeTreeView()
        {
            // 清空现有节点
            uiTreeView1.Nodes.Clear();

            // 创建根节点
            rootNode = new TreeNode("前提条件");
            rootNode.ImageIndex = 0;  // 设置图标
            uiTreeView1.Nodes.Add(rootNode);

            // 创建根节点
            rootNode1 = new TreeNode("操作步骤");
            rootNode1.ImageIndex = 0;  // 设置图标
            uiTreeView1.Nodes.Add(rootNode1);

            // 创建根节点
            rootNode2 = new TreeNode("期望结果");
            rootNode2.ImageIndex = 0;  // 设置图标
            uiTreeView1.Nodes.Add(rootNode2);

            // 或展开所有节点
            // uiTreeView1.ExpandAll();
        }
        /// <summary>
        /// 添加子节点（传入根节点）
        /// </summary>
        public void AddChildNode(ref TreeNode rootNode,string TreeNodeName)
        {
            try
            {
                TreeNode childNode = new TreeNode(TreeNodeName);
                childNode.ImageIndex = 1;
                rootNode.Nodes.Add(childNode);

                // 展开根节点
                rootNode.Expand();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
        }
        /// <summary>
        /// 移除子节点（传入根节点）
        /// </summary>
        public void RemoveChildNode(ref TreeNode rootNode)
        {
            try
            {
                if (MouseClickNodeIndex == -1) return;
                rootNode.Nodes.RemoveAt(MouseClickNodeIndex);
                MouseClickNodeIndex = -1;
                // 展开根节点
                rootNode.Expand();
            }
            catch { }
        }

        /// <summary>
        /// 添加到运行列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                UIMessageBox.ShowError("请选择要添加的项目！");
                return;
            }

            switch(rootName)
            {
                case "前提条件":
                    {
                        // 1. 创建新的数据对象
                        var newChildData = new ChildNodeData();

                        newChildData.Name = pluginName;
                        newChildData.Plugin = InitType(pluginName);

                        global.RootNodeDataList[global.indexSelect].RootData1.Children.Add(newChildData);

                        AddChildNode(ref rootNode, pluginName);
                    }
                    break;
                case "操作步骤":
                    {
                        // 1. 创建新的数据对象
                        var newChildData = new ChildNodeData();

                        newChildData.Name = pluginName;
                        newChildData.Plugin = InitType(pluginName);

                        global.RootNodeDataList[global.indexSelect].RootData2.Children.Add(newChildData);

                        AddChildNode(ref rootNode1, pluginName);
                    }
                    break;
                case "期望结果":
                    {
                        // 1. 创建新的数据对象
                        var newChildData = new ChildNodeData();

                        newChildData.Name = pluginName;
                        newChildData.Plugin = InitType(pluginName);

                        global.RootNodeDataList[global.indexSelect].RootData3.Children.Add(newChildData);

                        AddChildNode(ref rootNode2, pluginName);
                    }
                    break;
                    default:
                    UIMessageBox.ShowError("请选择要操作的根节点");
                    break;
            }      
        }

        /// <summary>
        /// 根据名称返回对应的插件
        /// </summary>
        /// <param name="Name"></param>
        public IPlugin InitType(string Name)
        {
            return global.factory.CreateInstance(Name);
        }

        int idex = 0;
        // 根据配置文件加载树形控件
        public void TreeViewInit(object sender, string Name)
        {
            rootNode.Nodes.Clear();
            rootNode1.Nodes.Clear();
            rootNode2.Nodes.Clear();

            uiTextBox1.Text = Name;
            if(global.RootNodeDataList[global.indexSelect].IsUse)
            {
                uiCheckBox1.Checked = true;
            }
            else
            {
                uiCheckBox1.Checked = false;
            }

            var root3 = global.RootNodeDataList[global.indexSelect];
            idex = 0;
            //foreach (var root3 in global.RootNodeDataList)
            {
                foreach (var root in new[] { root3.RootData1, root3.RootData2, root3.RootData3 })
                {
                    idex++;
                    foreach (var child in root.Children)
                    {
                        if(idex == 1)
                        {
                            AddChildNode(ref rootNode, child.Name +" =>" + "（"+child.Plugin.GetVersion() + "）");
                        }
                        else if (idex == 2)
                        {
                            AddChildNode(ref rootNode1, child.Name + " =>" + "（" + child.Plugin.GetVersion() + "）");
                        }
                        else if (idex == 3)
                        {
                            AddChildNode(ref rootNode2, child.Name + " =>" + "（" + child.Plugin.GetVersion() + "）");
                        }
                    }
                }
            }
        }
        private void uiButton3_Click(object sender, EventArgs e)
        {           
            switch (rootName)
            {
                case "前提条件":
                    {
                        global.RootNodeDataList[global.indexSelect].RootData1.Children.RemoveAt(MouseClickNodeIndex);                       
                        RemoveChildNode(ref rootNode);                
                    }
                    break;
                case "操作步骤":
                    {
                        global.RootNodeDataList[global.indexSelect].RootData2.Children.RemoveAt(MouseClickNodeIndex);
                        RemoveChildNode(ref rootNode1);
                    }
                    break;
                case "期望结果":
                    {
                        global.RootNodeDataList[global.indexSelect].RootData3.Children.RemoveAt(MouseClickNodeIndex);
                        RemoveChildNode(ref rootNode2);
                    }
                    break;
                default:
                    UIMessageBox.ShowError("请选择要操作的根节点");
                    break;
            }
        }

        int MouseClickNodeIndex = -1;
        private void uiTreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            MouseClickNodeIndex = e.Node.Index;
        }

        /// <summary>
        /// 将指定索引的元素向上移动一位
        /// </summary>
        public bool MoveUp<T>(List<T> list, int index)
        {
            if (index > 0 && index < list.Count)
            {
                T item = list[index];
                list.RemoveAt(index);
                list.Insert(index - 1, item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将指定索引的元素向下移动一位
        /// </summary>
        public bool MoveDown<T>(List<T> list, int index)
        {
            if (index >= 0 && index < list.Count - 1)
            {
                T item = list[index];
                list.RemoveAt(index);
                list.Insert(index + 1, item);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 上移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton6_Click(object sender, EventArgs e)
        {
            if (uiTreeView1.SelectedNode is TreeNode node)
            {
                switch (rootName)
                {
                    case "前提条件":
                        {
                            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
                            int index = nodes.IndexOf(node);
                            MoveUp(global.RootNodeDataList[global.indexSelect].RootData1.Children, index);
                        }
                        break;
                    case "操作步骤":
                        {
                            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
                            int index = nodes.IndexOf(node);
                            MoveUp(global.RootNodeDataList[global.indexSelect].RootData2.Children, index);
                        }
                        break;
                    case "期望结果":
                        {
                            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
                            int index = nodes.IndexOf(node);
                            MoveUp(global.RootNodeDataList[global.indexSelect].RootData3.Children, index);
                        }
                        break;
                    default:
                        UIMessageBox.ShowError("请选择要操作的根节点");
                        break;
                }
                MoveNodeUp(node);
            }                
        }
        private void MoveNodeUp(TreeNode node)
        {
            if (node == null) return;

            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
            int index = nodes.IndexOf(node);

            if (index > 0)  // 不是第一个
            {
                nodes.RemoveAt(index);
                nodes.Insert(index - 1, node);
                uiTreeView1.SelectedNode = node;  // 保持选中
            }
        }
        /// <summary>
        /// 下移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            if (uiTreeView1.SelectedNode is TreeNode node)
            {              
                switch (rootName)
                {
                    case "前提条件":
                        {
                            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
                            int index = nodes.IndexOf(node);
                            MoveDown(global.RootNodeDataList[global.indexSelect].RootData1.Children, index);
                        }
                        break;
                    case "操作步骤":
                        {
                            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
                            int index = nodes.IndexOf(node);
                            MoveDown(global.RootNodeDataList[global.indexSelect].RootData2.Children, index);
                        }
                        break;
                    case "期望结果":
                        {
                            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
                            int index = nodes.IndexOf(node);
                            MoveDown(global.RootNodeDataList[global.indexSelect].RootData3.Children, index);
                        }
                        break;
                    default:
                        UIMessageBox.ShowError("请选择要操作的根节点");
                        break;
                }
                MoveNodeDown(node);
            }  
        }
        private void MoveNodeDown(TreeNode node)
        {
            if (node == null) return;

            TreeNodeCollection nodes = node.Parent?.Nodes ?? uiTreeView1.Nodes;
            int index = nodes.IndexOf(node);

            if (index < nodes.Count - 1)  // 不是最后一个
            {
                nodes.RemoveAt(index);
                nodes.Insert(index + 1, node);
                uiTreeView1.SelectedNode = node;
            }
        }

        private void uiTreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (rootName)
            {
                case "前提条件":
                    {
                        try
                        {
                            //child.Plugin.SetData(child.TestParam, child.Plugin.VersionStr);
                            global.RootNodeDataList[global.indexSelect].RootData1.Children[e.Node.Index].Plugin
                                .CreateUI(global.RootNodeDataList[global.indexSelect].RootData1.Children[e.Node.Index].TestParam, global.RootNodeDataList[global.indexSelect].RootData1.Children[e.Node.Index].Plugin.VersionStr).ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            //UIMessageBox.ShowError(ex.ToString());
                            break;
                        }
                    }
                    break;
                case "操作步骤":
                    {
                        try
                        {
                            global.RootNodeDataList[global.indexSelect].RootData2.Children[e.Node.Index].Plugin
                                .CreateUI(global.RootNodeDataList[global.indexSelect].RootData2.Children[e.Node.Index].TestParam, global.RootNodeDataList[global.indexSelect].RootData2.Children[e.Node.Index].Plugin.VersionStr).ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            //UIMessageBox.ShowError(ex.ToString());
                            break;
                        }
                    }
                    break;
                case "期望结果":
                    {
                        try
                        {
                            global.RootNodeDataList[global.indexSelect].RootData3.Children[e.Node.Index].Plugin
                                .CreateUI(global.RootNodeDataList[global.indexSelect].RootData3.Children[e.Node.Index].TestParam, global.RootNodeDataList[global.indexSelect].RootData3.Children[e.Node.Index].Plugin.VersionStr).ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            //UIMessageBox.ShowError(ex.ToString());
                            break;
                        }
                    }
                    break;
            }
        }
     
        private void uiButton4_Click(object sender, EventArgs e)
        {
            if (global.ProjectName == "") return;
            string name = uiTextBox1.Text;
            bool isUse = uiCheckBox1.Checked;

            if (string.IsNullOrEmpty(name))
            {
                UIMessageBox.ShowError("序列名称不能为空！");
                return;
            } 
            global.RootNodeDataList[global.indexSelect].Name = name;
            global.RootNodeDataList[global.indexSelect].IsUse= isUse;
            if (!SaveData()) return;

            LoadDataHandler?.Invoke(global.FilePath + @"\" + global.ProjectName + ".json");
            TreeViewInit(this, global.RootNodeDataList[global.indexSelect].Name);
        }      
        public static bool SaveData()
        {
            if (global.ProjectName == "")
            {
                UIMessageBox.ShowError("工程名不能为空，保存失败！");
                return false;
            }
            // 保存前先获取所有插件的运行参数
            foreach (var root3 in global.RootNodeDataList)
            {
                foreach (var root in new[] { root3.RootData1, root3.RootData2, root3.RootData3 })
                {
                    foreach (var child in root.Children)
                    {
                        // 通过 ID获取对应的参数
                        child.TestParam = child.Plugin.GetData();
                    }
                }
            }

            var data = new SerializableGlobalData
            {
                RootNodeDataList = global.RootNodeDataList,
            };

            // 正确：必须传递包含转换器的 settings
            //var settings = new JsonSerializerSettings
            //{
            //    Converters = { new SafeTestParamConverter() }
            //};

            // 序列化时（保存数据时）
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,  // 只在需要时添加类型信息
                // 或者使用引用保留（推荐）
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };

            // 使用 Newtonsoft.Json
            string json = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(global.FilePath + @"\" + global.ProjectName + ".json", json);

            UIMessageBox.ShowSuccess("保存成功！");
            return true;
        }

        private void EditForm_Load(object sender, EventArgs e)
        {

        }
    }
}
