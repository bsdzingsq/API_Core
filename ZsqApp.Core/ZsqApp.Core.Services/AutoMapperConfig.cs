using AutoMapper;

namespace ZsqApp.Core.Services
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class AutoMapperConfig
    {
        private static MapperConfiguration _mapperConfiguration;

        /// <summary>
        /// 
        /// </summary>
        public static void Register()
        {
            var moduleInitializers = new ModuleInitializer[]
            {
                new ZsqModuleInitializer()
            };

            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                foreach (var m in moduleInitializers)
                {
                    m.LoadAutoMapper(cfg);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MapperConfiguration GetMapperConfiguration()
        {
            if (_mapperConfiguration == null)
                Register();

            return _mapperConfiguration;
        }
    }
}
