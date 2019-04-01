# 服务端-LQHN-APP-App服务端

#### 修改记录


```
版本号        修改日期             修改人          修改内容
v0.1         2018.05.22           陶林辉          创建初稿
v0.2         2018.08.07           白尚德          完善内容
```

                    
                        

#### 项目介绍
App服务端

#### 软件架构
.net core2.0 
efcore
Auto


#### 安装教程

1. xxxx
2. xxxx
3. xxxx

#### 目录结构

```
|-_AlipaySDK
    -AopSdk
            |-Domain
            |-Jayrock
            |-Parser
            |-Request
            |-Response
            |-Test
            |-Util
 |-_Entity
    -ZsqApp.Core.Entity     实体
            |-Currency      实体类库
    -HainanContext          上下文对象
|-_Infrastructure
    -ZsqApp.Core.Infrastructure
            |-Extentions     扩展
    -Sdk              sdk 
|-CCPRestSDK  
    -SysEnum   
           |-RedisEnum      Redis 枚举
    -Utilities              公共的
           |-HashHelper     哈希帮助类
           |-HttpHelper     http帮助类
           |-JsonHelper     json帮助类
           |-RedisHelper    redis帮助类
           |-TimeHelper     时间帮助类
           |-XmlHelper      序列化
|-_Service
    -ZsqApp.Core.Interfaces    定义接口
    -ZsqApp.Core.Models        操作实体
    -ZsqApp.Core.Services      实现接口
          |-AutoMapperConfig       AutoMapper配置文件
          |-ModuleInitializer      模块初始化
          |-ZsqModuleInitializer   加载AutoMapper
-ZsqApp.Core.ViewModel             视图模型
|-_WebApi
    -ZsqApp.Core.WebApi    
          |-Controllers           控制器 
          |-Filters               过滤器
                --ActionApiFilterAttribute           用于验证参数
                --ExceptionFilter                    异常捕获
                --ExternalActionApiFilterAttribute   效检
                --GameFilterAttribute                游戏通知过滤器
         |-Interfacelog           日志记录
         | - Model                视图模型
         |-Utilities              公共的
                --BaseController    
    -appsettings            配置连接字符串   
    -log4net                Log4NET配置设置
    -Program                程序入口
    -Startup                服务注册

```


#### 使用说明

1. 拉取项目文件
2. 安装vs2017
3. 启动项目
#### 参与贡献

1. Fork 本项目
2. 新建 Feat_xxx 分支
3. 提交代码
4. 新建 Pull Request


#### 码云特技

1. 使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2. 码云官方博客 [blog.gitee.com](https://blog.gitee.com)
3. 你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解码云上的优秀开源项目
4. [GVP](https://gitee.com/gvp) 全称是码云最有价值开源项目，是码云综合评定出的优秀开源项目
5. 码云官方提供的使用手册 [http://git.mydoc.io/](http://git.mydoc.io/)
6. 码云封面人物是一档用来展示码云会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)