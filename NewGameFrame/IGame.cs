using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGameFrame
{
    public interface IGame
    {
        /// <summary>
        /// 游戏场景
        /// </summary>
        Scene[] Scenes { get; }
        /// <summary>
        /// 游戏阶段
        /// </summary>
        int Step { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
    }
}
