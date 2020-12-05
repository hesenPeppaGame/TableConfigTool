using LitJson;
using System.IO;

namespace AppLib
{
    /// <summary>
    /// 目录配置文件
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 原表ClinetLocalization文件夹夹路径
        /// </summary>
        public string OrigionFilePath { get; set; }

        /// <summary>
        /// 待翻译文件
        /// </summary>
        public string PendingTranslationPath { get; set; }

        /// <summary>
        /// 已翻译文件
        /// </summary>
        public string AlreadyTranslatedPath { get; set; }

        /// <summary>
        /// 输入已翻译文件(新回来的已翻译文件)
        /// </summary>
        public string InputTranslatedPath { get; set; }

        /// <summary>
        /// 获取默认配置对象
        /// </summary>
        /// <returns></returns>
        public static AppConfig GetDefaultSaveData()
        {
            return new AppConfig()
            {
                OrigionFilePath = "Conifg/ClientLocalization",
                PendingTranslationPath = "待翻译文件",
                AlreadyTranslatedPath = "已翻译文件",
                InputTranslatedPath = "最终翻译文件",
            };
        }

        public static AppConfig Read()
        {
            AppConfig app;
            if (!File.Exists(AppDefine.AppConfigPath))
            {
                app = GetDefaultSaveData();
                app.Save();
            }
            else
            {
                string jsonStr = File.ReadAllText(AppDefine.AppConfigPath);
                app = JsonMapper.ToObject<AppConfig>(jsonStr);
            }

            return app;
        }

        public void Save()
        {
            string jsonStr = JsonMapper.ToJson(this);
            using (StreamWriter sw = new StreamWriter(AppDefine.AppConfigPath))
            {
                sw.Write(jsonStr);
            }
        }
    }
}
