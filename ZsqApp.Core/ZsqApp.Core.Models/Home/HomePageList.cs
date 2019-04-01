using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Home
{
    public class HomePageList
    {
        public String[] BannerImgs { get; set; }
        public List<HomeIntroduce> homeIntroduce { get; set; }

        public List<HomeRiding> homeRiding { get; set; }

        public List<HomeGame> homeGame { get; set; }

    }
    public class HomeIntroduce
    {
        public string Img { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Adress { get; set; }
    }
    public class HomeRiding
    {
        public string ReadImg { get; set; }
        public string ReadTitle { get; set; }
        public string ReadPrice { get; set; }
        public string ReadMonery { get; set; }

    }
    public class HomeGame
    {
        public string GameImg { get; set; }
        public string GameTitle { get; set; }
        public string GameConet { get; set; }
        public string Gameurl { get; set; }
        public int Gameid { get; set; }

    }

    public class HomeGames2
    {
        public string ReadImg { get; set; }
        public string ReadTitle { get; set; }
        public string Description { get; set; }
        public string Link_Url { get; set; }
    }
}
