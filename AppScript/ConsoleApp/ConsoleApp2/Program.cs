using AppLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp2
{
    /// <summary>
    /// 2.填充翻译后的文件: 
    /// 新回来的已翻译文件->合并老的已翻译文件->得到最终翻译文件->维护原表ClinetLocalization
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            //新回来的已经翻译的文件
            List<LanguageConifgSplit> newTranslates = new List<LanguageConifgSplit>();
            //老的已翻译文件
            List<LanguageConifgSplit> oldTranslates = new List<LanguageConifgSplit>();
            //维护原表ClinetLocalization
            List<ConfigSplit> clinetLocalizations = new List<ConfigSplit>();

            AppConfig appConfig = AppConfig.Read();

            //新回来的已经翻译的文件
            List<FileInfo> newTransFiles = new List<FileInfo>();
            AppUtils.GetFileName(newTransFiles, appConfig.AlreadyTranslatedPath);
            AppUtils.AddItems<LanguageConifgSplit>(newTransFiles, newTranslates);

            //老的已翻译文件
            List<FileInfo> oldTransFiles = new List<FileInfo>();
            AppUtils.GetFileName(oldTransFiles, appConfig.InputTranslatedPath);
            AppUtils.AddItems<LanguageConifgSplit>(oldTransFiles, oldTranslates);

            //原表ClinetLocalization
            List<FileInfo> clientTransFiles = new List<FileInfo>();
            AppUtils.GetFileName(clientTransFiles, appConfig.OrigionFilePath);
            AppUtils.AddItems<ConfigSplit>(clientTransFiles, clinetLocalizations);

            //新的  旧的 往 原表塞
            CombinClinetConfig(newTranslates, clinetLocalizations);
            CombinClinetConfig(oldTranslates, clinetLocalizations);

            //新的  往老的里面塞
            CombinNewConfig(newTranslates, oldTranslates);

            for (int i = 0; i < clinetLocalizations.Count; i++)
            {
                clinetLocalizations[i].SaveOrigionFile();
            }

            for (int i = 0; i < oldTranslates.Count; i++)
            {
                oldTranslates[i].SaveOrigionFile();
            }
        }

        /// <summary>
        /// 合并配置表  newCfg 合并到 oldCfg
        /// </summary>
        /// <param name="newCfg"></param>
        /// <param name="oldCfg"></param>
        public static void CombinClinetConfig(List<LanguageConifgSplit> newCfgs, List<ConfigSplit> oldCfgs)
        {
            foreach (var newCfg in newCfgs)
            {
                foreach (var oldCfg in oldCfgs)
                {
                    oldCfg.ChangeLines(newCfg);
                }
            }
        }

        /// <summary>
        /// 维护两个新表（非客户端的表）
        /// </summary>
        /// <param name="newCfg"></param>
        /// <param name="oldCfg"></param>
        public static void CombinNewConfig(List<LanguageConifgSplit> newCfgs, List<LanguageConifgSplit> oldCfgs)
        {
            AppConfig appConfig = AppConfig.Read();

            foreach (var newCfg in newCfgs)
            {
                foreach (var oldCfg in oldCfgs)
                {
                    if (newCfg.EConfigLang != oldCfg.EConfigLang)
                    {
                        continue;
                    }

                    foreach (var item in newCfg.ConfigLineDatas)
                    {
                        oldCfg.ChangeLine(item.Key, item.Value);
                    }
                }
            }

            //此处可能有新增配置表  需要添加
            foreach (var newCfg in newCfgs)
            {
                bool isNew = true;
                foreach (var oldCfg in oldCfgs)
                {
                    if (newCfg.EConfigLang == oldCfg.EConfigLang)
                    {
                        isNew = false;
                        break;
                    }
                }

                if (isNew)
                {
                    string[] fileNames = newCfg.FilePath.Split(new string[] {@"\" }, StringSplitOptions.RemoveEmptyEntries);
                    string fileName = fileNames[fileNames.Length - 1];
                    string allText = File.ReadAllText(newCfg.FilePath);
                    using (StreamWriter sw= new StreamWriter(AppDefine.AppCurrentDirectory + "/" + appConfig.InputTranslatedPath + "/"+ fileName))
                    {
                        sw.Write(allText);
                    }

                }
            }
        }
    }
}
