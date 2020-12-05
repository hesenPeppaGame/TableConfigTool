using System;
using AppLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace FixConfigTool
{
    /// <summary>
    /// 修复配置
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            List<ConfigSplit> newTranslates = new List<ConfigSplit>();
            AppConfig appConfig = AppConfig.Read();

            //新回来的已经翻译的文件
            List<FileInfo> newTransFiles = new List<FileInfo>();
            AppUtils.GetFileName(newTransFiles, appConfig.OrigionFilePath);
            AppUtils.AddItems<ConfigSplit>(newTransFiles, newTranslates);

            for (int i = 0; i < newTranslates.Count; i++)
            {
                var needFix = newTranslates[i].FixConfig();
                if (needFix)
                {
                    newTranslates[i].SaveOrigionFile();
                }
            }
        }
    }
}
