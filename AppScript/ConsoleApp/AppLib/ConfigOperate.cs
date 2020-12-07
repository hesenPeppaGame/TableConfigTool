using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLib
{
    public class ConfigOperate
    {

        /// <summary>
        /// 检测配置表与翻译后的配置表文字内容是否发生变动，如果是，就替换原表的值为空
        /// </summary>
        /// <param name="clinetCfgs">客户端原表</param>
        /// <param name="transFileCfgs">翻译后的表</param>
        public static void ConfigChecker(List<ConfigSplit> clinetCfgs, List<LanguageConifgSplit> transFileCfgs)
        {
            if (clinetCfgs.Count == 0 || transFileCfgs.Count == 0)
            {
                return;
            }

            //发生更改的
            foreach (var clientCfg in clinetCfgs)
            {
                foreach (var transCfg in transFileCfgs)
                {
                    clientCfg.CheckAndReplaceLines(transCfg);
                }
            }

            foreach (var item in clinetCfgs)
            {
                item.SaveOrigionFile();
            }
        }

        /// <summary>
        /// 修复配置表
        /// </summary>
        /// <param name="newTranslates"></param>
        public static void FixConfig(List<ConfigSplit> newTranslates)
        {
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
