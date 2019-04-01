using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Models.Routine;

namespace ZsqApp.Core.ViewModel.Routine
{
    //**************操作记录******************
    //创建时间：2018.02.05
    //作者：陶林辉
    //内容描述：Banner返回值
    //***************************************
    public class BannerListView
    {
        public List<BannerDto> Staple { get; set; }

        public List<BannerDto> Subsidiary { get; set; }
    }
}
