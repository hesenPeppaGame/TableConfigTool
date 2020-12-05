using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLib
{
    /// <summary>
    /// <para>以语言为单位的配置表</para>
    /// <para>改变一行数据<see cref="ChangeLine(string, string)"/></para>
    /// <para>保存精简版（key ，Cn）使用<see cref="SaveOrigionFile"/></para>
    /// </summary>
    public class LanguageConifgSplit : IConfigSplit
    {
        /// <summary>
        /// 语言中文映射表
        /// </summary>
        public static Dictionary<eLanguageEnum, string> chMapping = new Dictionary<eLanguageEnum, string>() {
                {eLanguageEnum.Cn,"语言描述" },
                {eLanguageEnum.Tw,"繁体中文" },
                {eLanguageEnum.En,"英文" },
                {eLanguageEnum.Ko,"韩文" },
                {eLanguageEnum.Ja,"日文" },
                {eLanguageEnum.Vi,"越南文" },
                {eLanguageEnum.Th,"泰文" },
         };

        /// <summary>
        /// 原始字符串
        /// </summary>
        private string origionConfigStr;

        /// <summary>
        /// 文件名
        /// </summary>
        private string fileName;

        /// <summary>
        /// 保存路径
        /// </summary>
        private string savedPath;

        /// <summary>
        /// 真实有效的字段
        /// </summary>
        private int realFeildNum;

        /// <summary>
        /// 配置表语言信息
        /// </summary>
        private eLanguageEnum configLang;

        /// <summary>
        /// 头数据
        /// </summary>
        private List<string> headLineData = new List<string>();

        /// <summary>
        /// 简要信息
        /// 配置表行数据结构
        /// </summary>
        private List<LanguageLineData> configLineDatas = new List<LanguageLineData>();

        /// <summary>
        /// 中文语言描述
        /// </summary>
        public string LanguageCHName => chMapping[configLang];

        /// <summary>
        /// 配置表语言信息
        /// </summary>
        public eLanguageEnum EConfigLang => configLang;

        /// <summary>
        /// 配置表行数据结构
        /// </summary>
        public List<LanguageLineData> ConfigLineDatas => configLineDatas;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath => savedPath;

        /// <summary>
        /// 设置表名字
        /// </summary>
        /// <param name="fileName"></param>
        public void SetFileName(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// 设置语言表
        /// </summary>
        /// <param name="el"></param>
        public void SetLanguage(eLanguageEnum el)
        {
            configLang = el;
        }

        public void AddHead(string desc, eLanguageEnum[] eLangs = null)
        {
            if (eLangs == null)
            {
                eLangs = new eLanguageEnum[] { configLang };
            }

            //表头
            headLineData.Add(desc);

            //类型
            string line = "string\t";
            for (int i = 0; i < eLangs.Length; i++)
            {
                line += "string\t";
            }

            line = line.TrimEnd('\t');
            headLineData.Add(line);

            //描述
            line = "中文\t";
            for (int i = 0; i < eLangs.Length; i++)
            {
                line += chMapping[eLangs[i]] + "\t";
            }

            line = line.TrimEnd('\t');
            headLineData.Add(line);

            //字段
            line = "Cn\t";
            for (int i = 0; i < eLangs.Length; i++)
            {
                line += eLangs[i].ToString() + "\t";
            }

            line = line.TrimEnd('\t');
            headLineData.Add(line);
        }

        /// <summary>
        /// 添加一行数据
        /// </summary>
        /// <param name="key"></param>
        public void AddNewKey(string key)
        {
            var find = false;
            foreach (var item in configLineDatas)
            {
                if (item.Key == key)
                {
                    find = true;
                    break;
                }
            }

            if (!find)
            {
                LanguageLineData configLineData = new LanguageLineData
                {
                    Key = key,
                    eLang = configLang,
                };

                configLineDatas.Add(configLineData);
            }
        }

        /// <summary>
        /// 改变一个配置表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ChangeLine(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                return;
            }

            for (int i = 0; i < configLineDatas.Count; i++)
            {
                var data = configLineDatas[i];
                if (data.Key == key)
                {
                    data.Value = value;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void SaveOrigionFile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < headLineData.Count; i++)
            {
                stringBuilder.AppendLine(headLineData[i]);
            }

            var length = configLineDatas.Count;
            for (int i = 0; i < length; i++)
            {
                var data = configLineDatas[i];
                string line = "";
                line = data.Key + "\t";
                if (data.Value != null)
                {
                    line += data.Value;
                }

                stringBuilder.AppendLine(line);
            }

            using (StreamWriter sw = new StreamWriter(savedPath))
            {
                sw.Write(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="path"></param>
        public void SetPath(string path)
        {
            savedPath = path;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="config"></param>
        public void SplitConfig(string fileName, string config)
        {
            configLineDatas.Clear();
            origionConfigStr = config;
            this.fileName = fileName;
            Sprit();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Sprit()
        {
            var lines = origionConfigStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var len = lines.Length;
            for (int i = 0; i < len; i++)
            {
                string line = lines[i];
                switch (i)
                {
                    case 0:
                    //定义描述
                    case 1:
                    //定义类型
                    case 2:
                        //定义描述信息
                        headLineData.Add(line);
                        break;
                    case 3:
                        //定义字段名字
                        headLineData.Add(line);
                        Enum.TryParse<eLanguageEnum>(line.Split('\t')[1], out configLang);
                        break;
                    default:
                        //正文
                        AddLine(line);
                        break;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool FixConfig()
        {
            return false;
        }

        /// <summary>
        /// 添加行数据
        /// </summary>
        private void AddLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            var items = line.Split('\t');
            LanguageLineData configLineData = new LanguageLineData
            {
                Key = items[0],
                eLang = configLang,
                Value = items[1],
            };

            configLineDatas.Add(configLineData);
        }
    }

    /// <summary>
    /// 行数据,精简表 {key，Cn = value}
    /// </summary>
    public class LanguageLineData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public eLanguageEnum eLang { get; set; }
    }
}