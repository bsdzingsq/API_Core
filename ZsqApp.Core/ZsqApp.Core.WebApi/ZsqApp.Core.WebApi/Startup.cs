using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.F2FPay.AspnetCore;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Text;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.Activity;
using ZsqApp.Core.Interfaces.Channel;
using ZsqApp.Core.Interfaces.Data;
using ZsqApp.Core.Interfaces.Game;
using ZsqApp.Core.Interfaces.Home;
using ZsqApp.Core.Interfaces.Logger;
using ZsqApp.Core.Interfaces.Message;
using ZsqApp.Core.Interfaces.OfPay;
using ZsqApp.Core.Interfaces.Recharge;
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Services;
using ZsqApp.Core.Services.AccoutSystem;
using ZsqApp.Core.Services.Activity;
using ZsqApp.Core.Services.Channel;
using ZsqApp.Core.Services.Data;
using ZsqApp.Core.Services.Game;
using ZsqApp.Core.Services.Home;
using ZsqApp.Core.Services.Logger;
using ZsqApp.Core.Services.Message;
using ZsqApp.Core.Services.OfPay;
using ZsqApp.Core.Services.Recharge;
using ZsqApp.Core.Services.Routine;
using ZsqApp.Core.Services.System;
using ZsqApp.Core.Services.User;
using ZsqApp.Core.Services.ZhangYu;
using ZsqApp.Core.WebApi.Filters;

namespace ZsqApp.Core.WebApi
{
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public static ILoggerRepository repository { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)//(IConfiguration configuration)
        {
            //Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //ConfigureAlipay(services);
            // 注入MVC框架
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            });
            services.AddDbContext<HainanContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Connection_Hainan")));
            services.AddDbContext<NewiBeaconContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Connection_IbcSqlServer")));
            services.AddDbContext<FunHaiNanContext>(options => options.UseMySql(Configuration.GetConnectionString("Connection_MySql"))); ;
            //配置跨域处理
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });
            var alipayOptions = Configuration.GetSection("Alipay_v1").Get<AlipayOptions>();
            //检查RSA私钥
            AlipayConfigChecker.Check(alipayOptions.SignType, alipayOptions.PrivateKey);
            services.AddAlipay(options => options.SetOption(alipayOptions)).AddAlipayF2F();

            //配置Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "乐趣海南接口文档",
                    Description = "RESTful API for TwBusManagement"
                    //TermsOfService = "None",
                    //Contact = new Contact { Name = "Alvin_Su", Email = "asdasdasd@outlook.com", Url = "" }
                });

                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "ZsqApp.Core.WebApi.xml");
                c.IncludeXmlComments(xmlPath);

                //  c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });

            //IServiceContainer servicess = new ServiceContainer();
            //servicess.AddType<IBiz, BizService>();
            //读取配置信息
            services.Configure<ZhangYuSetting>(this.Configuration.GetSection("Zhangyu"));
            services.Configure<RedisSettings>(this.Configuration.GetSection("Redis"));
            services.Configure<MongoSettings>(this.Configuration.GetSection("Mongo"));
            services.Configure<AliPaySetting>(this.Configuration.GetSection("Alipay"));
            services.Configure<ApplepaySetting>(this.Configuration.GetSection("Applepay"));
            services.Configure<OfPaySetting>(this.Configuration.GetSection("OfPay"));
            services.Configure<GameKeySetting>(this.Configuration.GetSection("GameKey"));
            services.Configure<CurrencyKeySetting>(this.Configuration.GetSection("CurrencyKey"));
            services.Configure<ValidateSetting>(this.Configuration.GetSection("ImgUrl"));
            services.Configure<PHPRequestSetting>(this.Configuration.GetSection("PHPRequests"));
            services.Configure<HaiXiaSetting>(this.Configuration.GetSection("HaiXiaSports"));
            services.Configure<HaiXiaPhpSetting>(this.Configuration.GetSection("HaiXiaPhpRequest"));
            services.Configure<StraitAliPaySetting>(this.Configuration.GetSection("StraitAlipay"));
            services.Configure<WeChatPaySetting>(this.Configuration.GetSection("WeChatPay"));
            services.Configure<LqhnWeChatPaySetting>(this.Configuration.GetSection("LqhnWeChatPay"));
            // 1.automapper注入
            services.AddScoped<AutoMapper.IConfigurationProvider>(_ => AutoMapperConfig.GetMapperConfiguration());
            services.AddScoped(_ => AutoMapperConfig.GetMapperConfiguration().CreateMapper());

            //services.AddTransient<ISystems, Systems>();
            //services.AddTransient<IRoutine, RoutineService>();
            // 注册接口和实现类的映射关系
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<ISystems, Systems>();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IPhoneMessage, PhoneMessageService>();
            services.AddScoped<IToken, TokenService>();
            services.AddScoped<IRoutine, RoutineService>();
            services.AddScoped<IBiz, BizService>();
            services.AddScoped<IRecharge, RechargeService>();
            services.AddScoped<IGame, GameService>();
            services.AddScoped<IUserTask, UserTaskService>();
            services.AddScoped<IOfpay, OfPayService>();
            services.AddScoped<IData, DataService>();
            services.AddScoped<IChannel, ChannelService>();
            services.AddScoped<IActivity, ActivityService>();
            services.AddScoped<IAccout, AccoutService>();
            services.AddScoped<IHomeService, HomeService>();
        }



        //private void ConfigureAlipay(IServiceCollection services)
        //{
        //    var alipayOptions = Configuration.GetSection("Alipay").Get<AlipayOptions>();
        //    //检查RSA私钥
        //    AlipayConfigChecker.Check(alipayOptions.SignType, alipayOptions.PrivateKey);
        //    services.AddAlipay(options => options.SetOption(alipayOptions)).AddAlipayF2F();
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())//根据配置的环境为开发环境，则会配置抛出异常错误界面
            {
                app.UseDeveloperExceptionPage();//抛出详细的异常错误

            }
            app.UseStaticFiles(); //静态文件服务
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TwBusManagement API V1");
                //c.ShowExtensions();
            });
            app.UseMvc();

        }
    }
}

