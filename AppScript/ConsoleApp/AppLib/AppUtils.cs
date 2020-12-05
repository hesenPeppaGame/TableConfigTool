using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLib
{
    public static class AppUtils
    {
        public static Dictionary<eLanguageEnum, int> enums;

        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary> 
        /// <param name="path">文件写入流</param>
        public static void GetFileName(List<FileInfo> findFileInfo, string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                if (f.FullName.EndsWith(".meta"))
                {
                    continue;
                }

                if (f.Name == ("Language.txt"))
                {
                    continue;
                }

                if (f.Name == ("SdkErrorCodeConfig.txt"))
                {
                    continue;
                }

                findFileInfo.Add(f);
            }
        }

        /// <summary>
        /// 获得指定路径下所有子目录名
        /// </summary> 
        /// <param name="path">文件夹路径</param> 
        public static void GetDirectory(List<FileInfo> findFileInfo, string path)
        {
            GetFileName(findFileInfo, path);
            DirectoryInfo root = new DirectoryInfo(path);
            if (root == null)
            {
                return;
            }

            foreach (DirectoryInfo d in root.GetDirectories())
            {
                if (d.Name.ToLower().Contains(".svn"))
                {
                    continue;
                }

                GetDirectory(findFileInfo, d.FullName);
            }
        }

        /// <summary>
        /// 获取文件名字
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileName(string fileName)
        {
            return fileName.Split('.')[0];
        }

        /// <summary>
        /// 获取文件名字
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string GetFileName(FileInfo fileInfo)
        {
            return GetFileName(fileInfo.Name);
        }

        public static void AddItems<T>(List<FileInfo> fileInfos, List<T> cfgs) where T : IConfigSplit, new()
        {
            if (cfgs == null)
            {
                cfgs = new List<T>();
            }

            for (int i = 0; i < fileInfos.Count; i++)
            {
                T clinetLocalization = new T();
                string fileStr = File.ReadAllText(fileInfos[i].FullName);
                clinetLocalization.SplitConfig(AppUtils.GetFileName(fileInfos[i]), fileStr);
                clinetLocalization.SetPath(fileInfos[i].FullName);
                cfgs.Add(clinetLocalization);
            }
        }
    }
}
