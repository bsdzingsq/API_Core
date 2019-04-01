using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models.Game;

namespace ZsqApp.Core.Interfaces.Game
{
    public interface IGame
    {
        /// <summary>
        /// 记录游戏的流水
        /// author：陶林辉
        /// </summary>
        /// <param name="gameLogDto"></param>
        /// <returns></returns>
        Task<bool> RecordGameLog(GameLogDto gameLogDto);
    }
}
