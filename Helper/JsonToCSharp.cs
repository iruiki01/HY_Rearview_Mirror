using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helper
{
    public static class JsonToCSharp
    {
        private static int _classCounter = 0;
        private static readonly HashSet<string> _usedClassNames = new HashSet<string>();

        public static string GenerateClassesFromJson(string jsonString, string rootClassName = "RootObject")
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                throw new ArgumentException("JSON字符串不能为空");

            try
            {
                 JsonDocument doc = JsonDocument.Parse(jsonString);
                _classCounter = 0;
                _usedClassNames.Clear();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine();
                sb.AppendLine("namespace GeneratedModels");
                sb.AppendLine("{");

                GenerateClassFromElement(doc.RootElement, rootClassName, sb, 1);

                sb.AppendLine("}");
                doc?.Dispose();
                return sb.ToString();
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"无效的JSON格式: {ex.Message}");
            }
        }

        private static void GenerateClassFromElement(JsonElement element, string className, StringBuilder sb, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 4);

            // 确保类名唯一
            className = GetUniqueClassName(className);

            sb.AppendLine($"{indent}public class {className}");
            sb.AppendLine($"{indent}{{");

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    string propertyName = SanitizePropertyName(property.Name);
                    string propertyType = GetPropertyType(property.Value, propertyName);

                    sb.AppendLine($"{indent}    public {propertyType} {propertyName} {{ get; set; }}");
                }
            }
            else if (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() > 0)
            {
                // 处理数组，取第一个元素作为类型参考
                var firstElement = element.EnumerateArray().First();
                string itemType = GetPropertyType(firstElement, $"{className}Item");

                sb.AppendLine($"{indent}    public List<{itemType}> Items {{ get; set; }}");
            }

            sb.AppendLine($"{indent}}}");
            sb.AppendLine();

            // 递归生成嵌套类
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Object)
                    {
                        GenerateClassFromElement(property.Value, property.Name, sb, indentLevel);
                    }
                    else if (property.Value.ValueKind == JsonValueKind.Array && property.Value.GetArrayLength() > 0)
                    {
                        var firstItem = property.Value.EnumerateArray().First();
                        if (firstItem.ValueKind == JsonValueKind.Object)
                        {
                            GenerateClassFromElement(firstItem, $"{property.Name}Item", sb, indentLevel);
                        }
                    }
                }
            }
        }

        private static string GetPropertyType(JsonElement element, string propertyName)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return "string";
                case JsonValueKind.Number:
                    return GetNumberType(element);
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return "bool";
                case JsonValueKind.Object:
                    return GetUniqueClassName(propertyName);
                case JsonValueKind.Array:
                    return GetArrayType(element, propertyName);
                case JsonValueKind.Null:
                    return "object";
                default:
                    return "object";
            }
        }

        private static string GetNumberType(JsonElement element)
        {
            if (element.TryGetInt32(out _)) return "int";
            if (element.TryGetInt64(out _)) return "long";
            if (element.TryGetDouble(out _)) return "double";
            if (element.TryGetDecimal(out _)) return "decimal";
            return "double"; // 默认
        }

        private static string GetArrayType(JsonElement element, string propertyName)
        {
            if (element.GetArrayLength() == 0)
                return "List<object>";

            var firstItem = element.EnumerateArray().First();
            string itemType = GetPropertyType(firstItem, $"{propertyName}Item");
            return $"List<{itemType}>";
        }

        private static string GetUniqueClassName(string className)
        {
            // 清理类名：移除非法字符，首字母大写
            className = SanitizeClassName(className);

            if (!_usedClassNames.Contains(className))
            {
                _usedClassNames.Add(className);
                return className;
            }

            // 如果类名已存在，添加数字后缀
            int counter = 1;
            string newClassName;
            do
            {
                newClassName = $"{className}{counter++}";
            } while (_usedClassNames.Contains(newClassName));

            _usedClassNames.Add(newClassName);
            return newClassName;
        }

        private static string SanitizeClassName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return $"Class{++_classCounter}";

            // 移除非法字符，只保留字母、数字，首字符必须是字母
            string sanitized = Regex.Replace(name, @"[^a-zA-Z0-9_]", "");
            if (string.IsNullOrEmpty(sanitized) || char.IsDigit(sanitized[0]))
                sanitized = "C" + sanitized;

            // 首字母大写
            return char.ToUpper(sanitized[0]) + (sanitized.Length > 1 ? sanitized.Substring(1) : "");
        }

        private static string SanitizePropertyName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "UnknownProperty";

            // 移除非法字符，只保留字母、数字
            string sanitized = Regex.Replace(name, @"[^a-zA-Z0-9_]", "");
            if (string.IsNullOrEmpty(sanitized))
                return "UnknownProperty";

            // 首字母大写
            return char.ToUpper(sanitized[0]) + (sanitized.Length > 1 ? sanitized.Substring(1) : "");
        }
    }
}

/***
    // 简单使用
    var generator = new JsonToCSharp();
    string json = "{\r\n    \"code\": 200,\r\n    \"data\": {\r\n        \"Success\": true,\r\n        \"Method\": null,\r\n        \"Validation\": null,\r\n        \"Message\": null,\r\n        \"Result\": {\r\n            \"SN\": \"2509ALY250431C00301\",\r\n            \"ProductCode\": \"Z2U10101020940\",\r\n            \"BatchNo\": \"GLY250431C\",\r\n            \"BeginSN\": \"2509ALY250431C00001\",\r\n            \"EndSN\": \"2509ALY250431C00500\",\r\n            \"Model\": \"50V6B\",\r\n            \"CoreCode\": \"TC80T1B2-LA\",\r\n            \"Totality\": 500,\r\n            \"SizeNotInBase\": \"1111*70*644\"\r\n        },\r\n        \"Context\": null\r\n    },\r\n    \"message\": \"调用GetProductSnInfo接口成功\"\r\n}\r\n";
    string csharpCode = generator.GenerateClassesFromJson(json, "MyModel");
    Console.WriteLine(csharpCode);
    // 可选：保存到文件
    File.WriteAllText("GeneratedModels.cs", csharpCode);
 ***/