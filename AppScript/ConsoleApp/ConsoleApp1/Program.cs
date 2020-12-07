using AppLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    /// <summary>
    /// 1.输出翻译文件： 
    /// 原表ClinetLocalization->比对已翻译文件->未找到的就输出到->待翻译文件
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ///ClinetLocalization下的文件
            List<ConfigSplit> clinetCfgs = new List<ConfigSplit>();
            List<LanguageConifgSplit> transFileCfgs = new List<LanguageConifgSplit>();

            AppConfig appConfig = AppConfig.Read();

            //获取ClinetLocalization下的文件
            List<FileInfo> clinetFileinfos = new List<FileInfo>();
            AppUtils.GetFileName(clinetFileinfos, appConfig.OrigionFilePath);
            AppUtils.AddItems<ConfigSplit>(clinetFileinfos, clinetCfgs);

            //获取已翻译文件下的文件
            List<FileInfo> transFileinfos = new List<FileInfo>();
            AppUtils.GetFileName(transFileinfos, appConfig.InputTranslatedPath);
            AppUtils.AddItems<LanguageConifgSplit>(transFileinfos, transFileCfgs);

            //导出时清理原配置表
            ConfigOperate.ConfigChecker(clinetCfgs, transFileCfgs);

            //写差异表
            List<LanguageConifgSplit> readyTransFileCfgs = new List<LanguageConifgSplit>();
            for (int i = 0; i < (int)eLanguageEnum.Th; i++)
            {
                string fileName = $"待翻译文件{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}_{((eLanguageEnum)i).ToString()}";
                LanguageConifgSplit newConfig = new LanguageConifgSplit();
                newConfig.SetFileName(fileName);
                newConfig.SetLanguage((eLanguageEnum)i);
                newConfig.AddHead(fileName, new eLanguageEnum[] { (eLanguageEnum)i });
                newConfig.SetPath(AppDefine.AppCurrentDirectory + "/" + appConfig.PendingTranslationPath + $"/{fileName}.txt");
                readyTransFileCfgs.Add(newConfig);
            }

            int totalCount = clinetCfgs.Count * transFileCfgs.Count * readyTransFileCfgs.Count;
            int count = 1;
            foreach (var clinet in clinetCfgs)
            {
                foreach (var trans in transFileCfgs)
                {
                    foreach (var ready in readyTransFileCfgs)
                    {
                        CheckSameLanguage(clinet, trans, ready);
                        Console.WriteLine($"处理{count}/{totalCount}");
                        count++;
                    }
                }
            }

            totalCount = clinetCfgs.Count * readyTransFileCfgs.Count;
            count = 0;

            foreach (var clinet in clinetCfgs)
            {
                foreach (var ready in readyTransFileCfgs)
                {
                    AddNewLanguageKey(clinet, ready);
                    Console.WriteLine($"处理{count}/{totalCount}");
                    count++;
                }
            }

            int length = transFileCfgs.Count;
            Dictionary<eLanguageEnum, bool> hasList = new Dictionary<eLanguageEnum, bool>();
            for (int i = 0; i < length; i++)
            {
                hasList.Add(transFileCfgs[i].EConfigLang, true);
            }

            //var len = (int)eLanguageEnum.Th;
            //for (int i = 0; i < len; i++)
            //{
            //    if (!hasList.ContainsKey((eLanguageEnum)i))
            //    {
            //        var data = readyTransFileCfgs[i];
            //        List<string> keys = GetCountryKeys(clinetCfgs, (eLanguageEnum)i);
            //        foreach (var item in keys)
            //        {
            //            data.AddNewKey(item);
            //        }
            //    }
            //}

            foreach (var newConfig in readyTransFileCfgs)
            {
                newConfig.SaveOrigionFile();
            }

            ConfigOperate.FixConfig(clinetCfgs);
        }

        private static void CheckSameLanguage(ConfigSplit left, LanguageConifgSplit right, LanguageConifgSplit addItem)
        {
            if (right.EConfigLang != addItem.EConfigLang)
            {
                return;
            }

            var lines = right.ConfigLineDatas;
            int length = lines.Count;
            for (int i = 0; i < length; i++)
            {
                var rightData = lines[i];
                if (!left.HasSameValue(rightData))
                {
                    addItem.AddNewKey(rightData.Key);
                }
            }
        }

        private static void AddNewLanguageKey(ConfigSplit left, LanguageConifgSplit addItem)
        {
            var newKey = left.GetNewLanguageKey(addItem.EConfigLang);
            for (int i = 0; i < newKey.Count; i++)
            {
                addItem.AddNewKey(newKey[i]);
            }
        }

        private static List<string> GetCountryKeys(List<ConfigSplit> clinetCfgs,eLanguageEnum eLnag)
        {
            List<string> keys = new List<string>();
            foreach (var clinetData in clinetCfgs)
            {
                var lines = clinetData.ConfigLineDatas;
                int langIndex = clinetData.GetLangIndex(eLnag);
                int cnIndex = clinetData.GetLangIndex(eLanguageEnum.Cn);
                foreach (var line in lines)
                {
                    try
                    {
                        keys.Add(line.Values[cnIndex]);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"语言超过数组界限:{clinetData.FileName},Key = {line.Key}+{ex.ToString()}");
                    } 
                }
            }

            return keys;
        }
    }
}
