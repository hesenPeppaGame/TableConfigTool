using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLib
{
    public enum eLanguageEnum
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        //[Header("Cn简体中文")]
        Cn = 0,
        /// <summary>
        /// 繁体中文
        /// </summary>
        //[Header("Tw繁体中文")]
        Tw = 1,
        /// <summary>
        /// 英文
        /// </summary>
        //[Header("En英文")]
        En = 2,
        /// <summary>
        /// 韩文
        /// </summary>
        //[Header("ko韩文")]
        Ko = 3,
        /// <summary>
        /// 日文
        /// </summary>
        //[Header("Ja日文")]
        Ja = 4,
        /// <summary>
        /// 越南文
        /// </summary>
        //[Header("Vi越南文")]
        Vi = 5,
        /// <summary>
        /// 泰文
        /// </summary>
        //[Header("Th泰文")]
        Th = 6,
    }

    public static class AppDefine
    {
        /// <summary>
        /// APP存在的目录
        /// </summary>
        public static string AppCurrentDirectory
        {
            get { return Directory.GetCurrentDirectory(); }
        }

        public static string AppConfigPath
        {
            get { return Directory.GetCurrentDirectory() + "/AppConfig.txt"; }
        }

    }
}
