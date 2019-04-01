using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.HomePage;
using ZsqApp.Core.Interfaces.Home;
using ZsqApp.Core.Models.Home;

namespace ZsqApp.Core.Services.Home
{
    public class HomeService : IHomeService
    {
        /// <summary>
        /// app context
        /// </summary>
        protected readonly FunHaiNanContext _funHaiNanContext;

        /// <summary>
        /// aotumapper
        /// </summary>
        protected readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ZsqApp.Core.Services.Activity.ActivityService"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="mapper">Mapper.</param>
        public HomeService(FunHaiNanContext context, IMapper mapper)
        {
            _funHaiNanContext = context;
            _mapper = mapper;

        }

        /// <summary>
        /// 海南竞技首页
        /// </summary>
        /// <returns></returns>
        public HomePageList homePage()
        {

            string banner1 = "https://hxjj-ls0.funhainan.com/Banner_01.png";
            string banner2 = "https://hxjj-ls0.funhainan.com/Banner_02.png";
            string banner3 = "https://hxjj-ls0.funhainan.com/Banner_03.png";
            string banner4 = "https://hxjj-ls0.funhainan.com/Banner_04.png";
            string[] strbanner = { banner1, banner2, banner3, banner4 };

            HomeIntroduce introduce = new HomeIntroduce()
            {
                Img = "https://hxjj-ls0.funhainan.com/Artboard.png",
                Title = "海峡骑行竞技",
                Content = "骑击是集娱乐、健康、竞技、文化为一体的新型游戏游艺项目，目前已获得十余项国际及国内专利。骑击，旨在增强民族素质，提升勇武精神，它利用了现代化的设施设备，结合传统武术技艺，按照现代体育竞技的要求，让经常参与者强身健体得到锻炼。骑击的主要形式为：身穿装备，手持器械（武器），骑于仿真动物（机械马）上，进行多人或多方的对战，由电脑自动计分，立判输赢。它的多人多方对战，演绎多方博弈，引领人们对多方博弈进行研究，可以弥补中华民族多方博弈方法论的不足。把项目推广进校园，结合传统武术教育，符合国家中长期教育改革和发展规划纲要的要求，对增强学生体质，锻炼学生意志品质，传承民族文化以及弘扬民族精神等方面，起到积极独特的作用。",
                Adress = "福州市海峡奥体中心广场2号",
            };
            HomeRiding riding1 = new HomeRiding()
            {
                ReadImg = "https://hxjj-ls0.funhainan.com/%E6%9C%AA%E6%A0%87%E9%A2%98-1.png",
                ReadTitle = "5分钟乐趣票,市场价40元",
                ReadPrice = "38",
                ReadMonery = "3800.00"
            }; HomeRiding riding2 = new HomeRiding()
            {
                ReadImg = "https://hxjj-ls0.funhainan.com/%E8%AE%AD%E7%BB%83%E5%8C%BA.png",
                ReadTitle = "10分钟乐趣票,市场价80元",
                ReadPrice = "48",
                ReadMonery = "4800.00"
            };
            HomeRiding riding3 = new HomeRiding()
            {
                ReadImg = "https://hxjj-ls0.funhainan.com/%E4%BD%93%E9%AA%8C%E5%8C%BA.png",
                ReadTitle = "15分钟乐趣票,市场价120元",
                ReadPrice = "69",
                ReadMonery = "6900.00"
            };
            HomeRiding riding4 = new HomeRiding()
            {
                ReadImg = "https://hxjj-ls0.funhainan.com/%E7%83%AD%E8%A1%80%E7%AB%9E%E6%8A%80_%E9%9D%9E%E6%AF%94%E8%B5%9B%E6%97%B6%E9%97%B4.png",
                ReadTitle = "30分钟训练票,市场价240元",
                ReadPrice = "129",
                ReadMonery = "12900.00"
            };
            HomeRiding riding5 = new HomeRiding()
            {
                ReadImg = "https://hxjj-ls0.funhainan.com/%E7%83%AD%E8%A1%80%E7%AB%9E%E6%8A%80_%E6%AF%94%E8%B5%9B%E6%97%B6%E9%97%B4.png",
                ReadTitle = "15分钟竞技票,市场价598元",
                ReadPrice = "298",
                ReadMonery = "298000.00"
            };
            HomeGame game1 = new HomeGame()
            {
                GameImg = "https://pic.8win.com/game/events/2017/7/17/09c89888-1ab7-4663-aee1-7bf1f3622c41.png",
                GameTitle = "南海渔王",
                GameConet = "一网捞出金元宝",
                Gameurl = "https://hainan.funhainan.com/ZsqImage/newapploading/index.html?gameType=2&gameUrl=\"\"&category=1&zy-action={\"showTitle\":1,\"titleName\":\"南海渔王\"}",
                Gameid = 2
            };
            HomeGame game2 = new HomeGame()
            {
                GameImg = "https://pic.8win.com/game/events/2017/6/7/fa0eb9b7-0cde-4913-a312-734e96e9aa65.png",
                GameTitle = "狗狗冲冲冲",
                GameConet = "猜猜谁跑的快",
                Gameurl = "https://hainan.funhainan.com/ZsqImage/newapploading/index.html?gameType=3&gameUrl=\"\"&category=1&zy-action={\"showTitle\":1,\"titleName\":\"狗狗冲冲冲\"}",
                Gameid = 1
            };
            HomeGame game3 = new HomeGame()
            {
                GameImg = "https://pic.8win.com/game/events/2017/12/27/69c14369-a8ee-48d3-abb9-f2b2ce2ac5bd.png",
                GameTitle = "幸运大转盘",
                GameConet = "赚翻趣多多",
                //Gameurl = "https://hainan.funhainan.com:944/ZsqImage/SpinningLucky/dist/#/main",
                Gameurl = "https://luckyd.funhainan.com/new2/ZsqImage/SpinningLucky/dist/#/main",
                Gameid = 0
            };

            List<HomeIntroduce> homeIntroduces = new List<HomeIntroduce>();
            homeIntroduces.Add(introduce);
            List<HomeRiding> homeRidings = new List<HomeRiding>();
            homeRidings.Add(riding1);
            homeRidings.Add(riding2);
            homeRidings.Add(riding3);
            homeRidings.Add(riding4);
            homeRidings.Add(riding5);
            List<HomeGame> homeGames = new List<HomeGame>();
            homeGames.Add(game1);
            homeGames.Add(game2);
            homeGames.Add(game3);
            HomePageList homePageList = new HomePageList();
            homePageList.BannerImgs = strbanner;
            homePageList.homeIntroduce = homeIntroduces;
            homePageList.homeRiding = homeRidings;
            homePageList.homeGame = homeGames;
            return homePageList;


        }

        /// <summary>
        /// 海南竞技首页2
        /// </summary>
        /// <returns></returns>
        public async Task<HomePageList> homePage2()
        {

            string banner1 = "https://hxjj-ls0.funhainan.com/Banner_01.png";
            string banner2 = "https://hxjj-ls0.funhainan.com/Banner_02.png";
            string banner3 = "https://hxjj-ls0.funhainan.com/Banner_03.png";
            string banner4 = "https://hxjj-ls0.funhainan.com/Banner_04.png";
            string[] strbanner = { banner1, banner2, banner3, banner4 };

            HomeIntroduce introduce = new HomeIntroduce()
            {
                Img = "https://hxjj-ls0.funhainan.com/Artboard.png",
                Title = "海峡骑行竞技",
                Content = "骑击是集娱乐、健康、竞技、文化为一体的新型游戏游艺项目，目前已获得十余项国际及国内专利。骑击，旨在增强民族素质，提升勇武精神，它利用了现代化的设施设备，结合传统武术技艺，按照现代体育竞技的要求，让经常参与者强身健体得到锻炼。骑击的主要形式为：身穿装备，手持器械（武器），骑于仿真动物（机械马）上，进行多人或多方的对战，由电脑自动计分，立判输赢。它的多人多方对战，演绎多方博弈，引领人们对多方博弈进行研究，可以弥补中华民族多方博弈方法论的不足。把项目推广进校园，结合传统武术教育，符合国家中长期教育改革和发展规划纲要的要求，对增强学生体质，锻炼学生意志品质，传承民族文化以及弘扬民族精神等方面，起到积极独特的作用。",
                Adress = "福州市海峡奥体中心广场2号",
            };

            HomeGame game1 = new HomeGame()
            {
                GameImg = "https://pic.8win.com/game/events/2017/7/17/09c89888-1ab7-4663-aee1-7bf1f3622c41.png",
                GameTitle = "南海渔王",
                GameConet = "一网捞出金元宝",
                Gameurl = "https://hi-h5.8win.com/#/lqhn/transferpage?keep=1&url=http://120.92.154.66:8080/proslot/SouthFish/index.html/FoperatorKey/DNHYW/gameId/FishTycoon/language/zh&category=1&zy-action%3d%7b%26quot%3bshowTitle%26quot%3b%3a1%2c%26quot%3btitleName%26quot%3b%3a%26quot%3b%e5%8d%97%e6%b5%b7%e6%b8%94%e7%8e%8b%26quot%3b%7d",
                Gameid = 2
            };
            HomeGame game2 = new HomeGame()
            {
                GameImg = "https://pic.8win.com/game/events/2017/6/7/fa0eb9b7-0cde-4913-a312-734e96e9aa65.png",
                GameTitle = "狗狗冲冲冲",
                GameConet = "猜猜谁跑的快",
                Gameurl = "https://hi-h5.8win.com/#/lqhn/transferpage?keep=1&url=https://8win.joyboat6.cn/zycp_hi/v1/index.html&category=1&zy-action=%7b%22showTitle%22%3a1%2c%22titleName%22%3a%22%e7%8b%97%e7%8b%97%e5%86%b2%e5%86%b2%e5%86%b2%22%7d",
                Gameid = 1
            };
            HomeGame game3 = new HomeGame()
            {
                GameImg = "https://pic.8win.com/game/events/2017/12/27/69c14369-a8ee-48d3-abb9-f2b2ce2ac5bd.png",
                GameTitle = "幸运大转盘",
                GameConet = "赚翻趣多多",
                //Gameurl = "https://hainan.funhainan.com:944/ZsqImage/SpinningLucky/dist/#/main",
                Gameurl = "https://luckyd.funhainan.com/new2/ZsqImage/SpinningLucky/dist/#/main",
                Gameid = 0
            };

            try
            {
                List<HomeIntroduce> homeIntroduces = new List<HomeIntroduce>();
                homeIntroduces.Add(introduce);
                List<HomeRiding> homeRidings = await HomeRidings();
                List<HomeGame> homeGames = new List<HomeGame>();
                List<HomeGames2> homeGames2 = await HomeGames();


                HomeGames2 homeGamelist = new HomeGames2();
                foreach (var item in homeGames2)
                {
                    HomeGame homeGame = new HomeGame();
                    homeGame.GameImg = item.ReadImg;
                    homeGame.GameTitle = item.ReadTitle;
                    homeGame.GameConet = item.Description;
                    homeGame.Gameurl = item.Link_Url;
                    homeGame.Gameid = 1;

                    homeGames.Add(homeGame);
                }


                HomePageList homePageList = new HomePageList();
                homePageList.BannerImgs = strbanner;
                homePageList.homeIntroduce = homeIntroduces;
                homePageList.homeRiding = homeRidings;
                homePageList.homeGame = homeGames;
                return homePageList;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        /// <summary>
        /// 项目介绍
        /// </summary>
        /// <returns></returns>
        public Introductionlist Introduction()
        {
            Introductionlist introduct = new Introductionlist()
            {
                ProjImg = "https://hxjj-ls0.funhainan.com/haixia_map.png",
                Padress = "福州市海峡奥体中心广场2号",
                PRoute = "福州火车站出发，乘车地铁一号线，达道站下车；步行570米，至文化宫(东方百货群升店)站，乘坐302（或316）公交；至福湾斗门下车。（更多路线请自行使用地图软件查询）",
                BusinessHours = "9:00-21:00 \t\t 节假日正常营业（春节期间除外）",
                Phone = "0591-83857183",
                AboutImg = "https://hxjj-ls0.funhainan.com/hxjj/banner1.png",
                AboutContent = "\t\t\t 海峡竞技是以骑击为主题的，集娱乐，健身，竞技，文化于一体的新型游戏游艺项目。主要形式为：身穿装备，手持器械（武器），骑于仿真动物（机械马）上，进行多人或多方的对战，由电脑自动计分，立判输赢。\r\n \t\t\t 骑击，作为娱乐项目，新颖、刺激、舒适！它不仅符合国家关于游戏游艺行业升级转型的新要求，甚至部分超前。骑击率先实现了项目的场所阳光化，内容益智化、健身化、技能化，以及适应人群广泛化的特性，并可组织全国性或区域性的比赛。\r\n \t\t\t 骑击，作为健康项目，可以创造全新的健康体验!它不仅能让成年人锻炼身体，释放压力，改善亚健康，更能让未成年人通过三维空间的运动，刺激前庭觉的发育，有效提高感觉统合能力。\r\n \t\t\t 骑击，作为竞技项目，可以增强民族素质，提升勇武精神！它利用了现代化的设施设备，结合传统武术技艺，并按照现代体育竞技的要求，让人们在对阵搏杀中得到锻炼，达到强身健体的功效。\r\n \t\t\t 骑击，作为文化项目，既是一项高级的智力运动，也是一种文化的创新。它的多人多方对战，演绎多方博弈，引领人们对多方博弈进行研究，可以弥补中华民族多方博弈方法论的不足。把项目推广进校园，结合传统武术教育，符合国家中长期教育改革和发展规划纲要的要求，对增强学生体质，锻炼学生意志品质，传承民族文化以及弘扬民族精神等方面，起到积极独特的作用。 \r\n \t\t\t 骑击，将创造全新的“马术”及“剑术”文化，未来还将在一带一路沿线国家中，超过半数的马背上的国家间，作为文化交流的桥梁，发挥巨大的贡献！",
            };
            return introduct;
        }

        public async Task<List<HomeRiding>> HomeRidings()
        {
            try
            {
                var list = await _funHaiNanContext.StraitHomePage.Where(m => m.Status == 1 && m.HomePage_Type == 0).OrderByDescending(m => m.Short).ToListAsync();
                return _mapper.Map<List<StraitHomePageEntity>, List<HomeRiding>>(list);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<List<HomeGames2>> HomeGames()
        {
            try
            {
                var list2 = await _funHaiNanContext.StraitHomePage.Where(m => m.Status == 1 && m.HomePage_Type == 1).OrderByDescending(m => m.Short).ToListAsync();
                return _mapper.Map<List<StraitHomePageEntity>, List<HomeGames2>>(list2);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //end
    }
}
