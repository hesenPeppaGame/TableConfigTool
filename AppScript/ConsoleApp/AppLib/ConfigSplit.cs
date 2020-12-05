using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLib
{
    /// <summary>
    /// <para>配置表分割（客户端使用的全配置表）</para>
    /// </summary>
    public class ConfigSplit: IConfigSplit
    {
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
        /// 配置表行数据结构
        /// </summary>
        private List<ConfigLineData> configLineDatas = new List<ConfigLineData>();

        /// <summary>
        /// 表头
        /// </summary>
        private List<string> headList = new List<string>();

        /// <summary>
        /// 国家列表
        /// </summary>
        private List<string> contryList = new List<string>();

        /// <summary>
        /// 配置表行数据结构
        /// </summary>
        public List<ConfigLineData> ConfigLineDatas => configLineDatas;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get { return fileName; }set { fileName = value; } }

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
        /// <param name="path"></param>
        public void SetPath(string path)
        {
            savedPath = path;
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
                if (i <= 3)
                {
                    headList.Add(lines[i]);
                    GetRealLineFeildNum(lines[i]);
                    if (i == 3)
                    {
                        var rows = lines[i].Split('\t');
                        for (int l = 1; l < rows.Length; l++)
                        {
                            contryList.Add(rows[l]);
                        }
                    }
                }
                else
                {
                    try
                    {
                        AddLine(lines[i]);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"行列不一致，当前行{i + 1}" + ex.Message + ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void SaveOrigionFile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < headList.Count; i++)
            {
                stringBuilder.AppendLine(headList[i]);
            }

            var length = configLineDatas.Count;
            for (int i = 0; i < length; i++)
            {
                var data = configLineDatas[i];
                string line = "";
                line = data.Key;
                if (data.Values != null && data.Values.Length != 0)
                {
                    int count = 0;
                    foreach (var item in data.Values)
                    {
                        if (count > contryList.Count)
                        {
                            break;
                        }

                        line += "\t" + item;
                        count++;
                    }
                }

                stringBuilder.AppendLine(line);
            }

            using (StreamWriter sw = new StreamWriter(savedPath))
            {
                sw.Write(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// 查找相同的Value
        /// </summary>
        /// <param name="lineData"></param>
        /// <returns></returns>
        public bool HasSameValue(LanguageLineData lineData)
        {
            int langIndex = GetLangIndex(lineData.eLang);
            int cnIndex = GetLangIndex(eLanguageEnum.Cn);
            int count = configLineDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var myLineData = configLineDatas[i];
                try
                {
                    var flag1 = myLineData.Values[langIndex];
                    var flag2 = lineData.Value;
                    if (myLineData.Values[cnIndex] == lineData.Key && flag1 == flag2)
                    {
                        return true;
                    }

                    //原表没有值  肯定需要添加
                    if (myLineData.Values[cnIndex] == lineData.Key && string.IsNullOrEmpty(flag1))
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"语言表的行列数{i + 1}找不到比对的语言{lineData.eLang.ToString()}+{ex.Message}+{ex.StackTrace}");
                }
            }

            //原表没有这个Key了  要忽略掉
            var find = true;
            for (int i = 0; i < count; i++)
            {
                var myLineData = configLineDatas[i];
                if (myLineData.Values[cnIndex] == lineData.Key)
                {
                    find = false;
                    break;
                }
            }
             
            return find;
        }

        /// <summary>
        /// 当前语言是否有值，用于判断新增language Key
        /// </summary>
        /// <param name="eLanguage"></param>
        /// <returns></returns>
        public List<string> GetNewLanguageKey(eLanguageEnum eLanguage)
        {
            List<string> newKeyList = new List<string>();
            int langIndex = GetLangIndex(eLanguage);
            int cnIndex = GetLangIndex(eLanguageEnum.Cn);

            int count = configLineDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var myLineData = configLineDatas[i];
                string langValue = myLineData.Values[langIndex];
                if (string.IsNullOrEmpty(langValue))
                {
                    newKeyList.Add(myLineData.Values[cnIndex]);
                }
            }

            return newKeyList;
        }

        /// <summary>
        /// 添加表信息
        /// </summary>
        /// <param name="desc"></param>
        public void AddEmptyLine(string desc)
        {
            ConfigLineData configLineData = new ConfigLineData
            {
                Key = desc,
                Values = new string[0]
            };

            configLineDatas.Add(configLineData);
        }

        /// <summary>
        /// 添加一行数据
        /// </summary>
        /// <param name="key"></param>
        public void AddNewKey(string key)
        {
            ConfigLineData configLineData = new ConfigLineData
            {
                Key = key,
                Values = new string[7]
            };

            configLineData.Values[0] = key;
            configLineDatas.Add(configLineData);
        }

        /// <summary>
        /// 改变一行数据
        /// </summary>
        /// <param name="value"></param>
        public void ChangeLines(LanguageConifgSplit conifgSplitData)
        {
            int langIndex = GetLangIndex(conifgSplitData.EConfigLang);
            for (int i = 0; i < ConfigLineDatas.Count; i++)
            {
                var myData = ConfigLineDatas[i];
                string myKey = myData.Values[GetLangIndex(eLanguageEnum.Cn)];
                int len = conifgSplitData.ConfigLineDatas.Count;
                for (int l = 0; l < len; l++)
                {
                    var oldData = conifgSplitData.ConfigLineDatas[l];
                    var otherKey = oldData.Key;
                    if (myKey == otherKey && !string.IsNullOrEmpty(oldData.Value))
                    {
                        myData.Values[langIndex] = oldData.Value;
                    }
                }
            }
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
            var len = items.Length;
            if (len < (int)eLanguageEnum.Th)
            {
                len = (int)eLanguageEnum.Th;
            }

            ConfigLineData configLineData = new ConfigLineData
            {
                Key = items[0]
            };

            configLineData.Values = new string[len];
            for (int i = 0; i < configLineData.Values.Length; i++)
            {
                configLineData.Values[i] = "";
            }

            for (int i = 1; i < items.Length; i++)
            {
                configLineData.Values[i - 1] = items[i];
            }

            configLineDatas.Add(configLineData);
        }

        public int GetLangIndex(eLanguageEnum eLanguage)
        {
            string str = eLanguage.ToString();
            for (int i = 0; i < contryList.Count; i++)
            {
                if (contryList[i] == str)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool FixConfig()
        {
            bool needFixed = false;
            for (int i = 0; i < configLineDatas.Count; i++)
            {
                var lineData = configLineDatas[i];
                if (IsErrorFeild(lineData.Values))
                {
                    needFixed = true;
                    lineData.Values = FixErrorFeild(lineData.Values);
                }
            }

            return needFixed;
        }

        /// <summary>
        /// 获取行数据字段数量
        /// 忽略无效的空字段
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private void GetRealLineFeildNum(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                throw new Exception($"参考的行字段为空[{fileName}]");
            }

            //最后一个字段
            string[] feilds = line.Split('\t');
            int count = 0;
            for (int i = 0; i < feilds.Length; i++)
            {
                if (!string.IsNullOrEmpty(feilds[i]))
                {
                    count++;
                }
            }

            if (realFeildNum < count)
            {
                realFeildNum = count;
            }
        }

        /// <summary>
        /// 此字段是否是错误的
        /// </summary>
        /// <param name="feilds"></param>
        /// <returns></returns>
        private bool IsErrorFeild(string[] feilds)
        {
            return feilds.Length != realFeildNum -1;
        }

        /// <summary>
        /// 修复字段
        /// </summary>
        /// <param name="feilds">字段</param>
        /// <returns></returns>
        private string[] FixErrorFeild(string[] feilds)
        {
            List<string> feildLsit = new List<string>();
            var count = realFeildNum - 1;
            //填充有效数据，多的截取掉
            var len = feilds.Length > count ? count : feilds.Length;
            for (int i = 0; i < len; i++)
            {
                feildLsit.Add(feilds[i]);
            }

            //补齐少的数据
            if (feilds.Length < count)
            {
                for (int i = feilds.Length; i < count - 1; i++)
                {
                    feildLsit.Add("");
                }
            }

            return feildLsit.ToArray();
        }
    }

    public class ConfigLineData
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string[] Values { get; set; }

    }

}
