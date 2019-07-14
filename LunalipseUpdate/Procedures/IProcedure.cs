using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseUpdate.Procedures
{
    public interface IProcedure
    {
        /// <summary>
        /// 过程的入口点
        /// </summary>
        /// <param name="args">参与过程的参数</param>
        /// <returns>返回一个二元组, 第一项指示操作是否成功，第二项为错误信息（仅当第一项为false时）</returns>
        void Main();
    }
}
