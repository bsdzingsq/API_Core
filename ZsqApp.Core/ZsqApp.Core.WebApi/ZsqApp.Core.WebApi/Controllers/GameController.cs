using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Game;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Models.Game;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.Game;
using ZsqApp.Core.WebApi.Filters;
using ZsqApp.Core.WebApi.Model;
using ZsqApp.Core.WebApi.Utilities;

namespace ZsqApp.Core.WebApi.Controllers
{
    /// <summary>
    /// 游戏异步通知
    /// </summary>
    [Produces("application/json")]
    [Route("api/v1")]
    [EnableCors("any")]
    [GameFilterAttribute]
    public class GameController : BaseController
    {
        #region dependency injection
        /// <summary>
        /// IGame 游戏相关服务
        /// </summary>
        private readonly IGame _game;
        /// <summary>
        /// ISystems 系统相关服务
        /// </summary>
        protected readonly ISystems _sys;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="game"></param>
        /// <param name="sys"></param>
        public GameController(IGame game, ISystems sys)
        {
            _game = game;
            _sys = sys;
        }
        #endregion

        /// <summary>
        /// 
        /// 游戏投注异步通知接口
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gameBet")]
        public async Task<GameResponesViewModel<GameViewModel>> GameBet([FromBody]GameRequestViweModel obj)
        {
            GameResponesViewModel<GameViewModel> response = null;
            GameViewModel result = new GameViewModel();
            obj = JsonHelper.DeserializeJsonToObject<GameRequestViweModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            GameLogDto gameLog = new GameLogDto()
            {
                Amount = decimal.Parse((string)obj.data.amount),
                Createtime = DateTime.Now,
                Description = (string)obj.data.description,
                Game_key = (string)obj.data.gameKey,
                Game_setId = (string)obj.data.gameSetId,
                Operate_time = (string)obj.data.operateTime,
                Order_id = (string)obj.data.orderId,
                Types = 1,
                User_id = long.Parse((string)obj.data.userId)

            };
            result.amount = double.Parse(gameLog.Amount.ToString());
            if (!await _game.RecordGameLog(gameLog))
            {
                sysCode = SysCode.Err;
            }
            response = new GameResponesViewModel<GameViewModel>(sysCode, sysCode == SysCode.Ok ? result : null, _sys, obj.appKey, obj.requestId, obj.nonce);
            return response;
        }


        /// <summary>
        /// 游戏异步通知接口
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gamePrize")]
        public async Task<GameResponesViewModel<GameViewModel>> GamePrize([FromBody]GameRequestViweModel obj)
        {
            GameResponesViewModel<GameViewModel> response = null;
            GameViewModel result = new GameViewModel();
            obj = JsonHelper.DeserializeJsonToObject<GameRequestViweModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            GameLogDto gameLog = new GameLogDto()
            {
                Amount = decimal.Parse((string)obj.data.amount),
                Createtime = DateTime.Now,
                Description = (string)obj.data.description,
                Game_key = (string)obj.data.gameKey,
                Game_setId = (string)obj.data.gameSetId,
                Operate_time = (string)obj.data.operateTime,
                Order_id = (string)obj.data.orderId,
                Types = 2,
                User_id = long.Parse((string)obj.data.userId)

            };
            result.amount = double.Parse(gameLog.Amount.ToString());
            if (!await _game.RecordGameLog(gameLog))
            {
                sysCode = SysCode.Err;
            }
            response = new GameResponesViewModel<GameViewModel>(sysCode, sysCode == SysCode.Ok ? result : null, _sys, obj.appKey, obj.requestId, obj.nonce);
            return response;
        }

        /// <summary>
        /// 游戏退款异步通知接口
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gameRefund")]
        public async Task<GameResponesViewModel<GameViewModel>> GameRefund([FromBody]GameRequestViweModel obj)
        {
            GameResponesViewModel<GameViewModel> response = null;
            GameViewModel result = new GameViewModel();
            obj = JsonHelper.DeserializeJsonToObject<GameRequestViweModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            GameLogDto gameLog = new GameLogDto()
            {
                Amount = decimal.Parse((string)obj.data.amount),
                Createtime = DateTime.Now,
                Description = (string)obj.data.description,
                Game_key = (string)obj.data.gameKey,
                Game_setId = (string)obj.data.gameSetId,
                Operate_time = (string)obj.data.operateTime,
                Order_id = (string)obj.data.orderId,
                Types = 3,

            };
            result.amount = double.Parse(gameLog.Amount.ToString());
            if (!await _game.RecordGameLog(gameLog))
            {
                sysCode = SysCode.Err;
            }
            response = new GameResponesViewModel<GameViewModel>(sysCode, sysCode == SysCode.Ok ? result : null, _sys, obj.appKey, obj.requestId, obj.nonce);
            return response;
        }
    }

}