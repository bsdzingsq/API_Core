using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.Game;
using ZsqApp.Core.Interfaces.Game;
using ZsqApp.Core.Models.Game;

namespace ZsqApp.Core.Services.Game
{
    public class GameService : IGame
    {

        protected readonly FunHaiNanContext _context;
        private readonly IMapper _mapper;
        public GameService(IMapper mapper, FunHaiNanContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// 记录游戏的流水
        /// author：陶林辉
        /// </summary>
        /// <param name="gameLogDto"></param>
        /// <returns></returns>
        public async Task<bool> RecordGameLog(GameLogDto gameLogDto)
        {
            var entity = _mapper.Map<GameLogDto, GameLogEntity>(gameLogDto);
            _context.GameLog.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
