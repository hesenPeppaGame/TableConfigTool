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
            List<ConfigSplit> origionTranslates = new List<ConfigSplit>();
            AppConfig appConfig = AppConfig.Read();

            List<FileInfo> newTransFiles = new List<FileInfo>();
            AppUtils.GetFileName(newTransFiles, appConfig.OrigionFilePath);
            AppUtils.AddItems<ConfigSplit>(newTransFiles, origionTranslates);

            ConfigOperate.FixConfig(origionTranslates);
        }
    }
}
