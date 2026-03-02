using HY_Rearview_Mirror.InterfaceTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.TypeManager
{
    internal class TypeFactory
    {
        private readonly Dictionary<string, PluginContainerType> PluginContainerTypeS = new Dictionary<string, PluginContainerType>();
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();

        // 注册类型
        public void Register<T>(string key)
        {
            Type _type = typeof(T);

            // 存储元数据，不保留实例
            PluginContainerTypeS[key] = new PluginContainerType
            {
                PluginType = _type,          // 存储类型
            };
        }
        // 创建实例
        public IPlugin CreateInstance(string key)
        {
            if (PluginContainerTypeS.TryGetValue(key, out PluginContainerType type))
            {
                return type.CreateInstance();
            }
            throw new KeyNotFoundException($"未找到类型: {key}");
        }
    }
    public class PluginContainerType
    {
        public Type PluginType { get; set; }
        // 提供创建新实例的方法
        public IPlugin CreateInstance()
        {
            IPlugin instance = null;

            instance = Activator.CreateInstance(PluginType) as IPlugin;
            instance?.Initialize();
            return instance;
        }
    }
}
