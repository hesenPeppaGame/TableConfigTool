using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLib
{
    /// <summary>
    /// 配置表数据接口类
    /// </summary>
   public interface IConfigSplit
    {
        /// <summary>
        /// 分割一个配置表
        /// </summary>
        /// <param name="fileName">文件名字</param>
        /// <param name="config">配置表文本</param>
        void SplitConfig(string fileName, string config);

        /// <summary>
        /// 修复配置表
        /// </summary>
        /// <returns></returns>
        bool FixConfig();

        /// <summary>
        /// 设置保存路径
        /// </summary>
        /// <param name="path"></param>
        void SetPath(string path);

        /// <summary>
        /// 分割表
        /// </summary>
        void Sprit();

        /// <summary>
        /// 保存
        /// </summary>
        void SaveOrigionFile();
    }
}
