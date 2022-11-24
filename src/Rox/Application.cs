using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace Rox
{
    public class Application
    {
        private readonly List<ModuleBase> _modules = new List<ModuleBase>();

        private IServiceCollection _services;
        private IConfiguration _configuration;

        public Application()
        {
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            using (var provider = _services.BuildServiceProvider())
            {
                var context = new ApplicationInitializationContext(provider);
                foreach (var module in _modules)
                {
                    //_logger.LogTrace($"程序启动前预热模块: {module.GetType().Name}");
                    await module.PreApplicationInitialization(context, cancellationToken);
                }
                foreach (var module in _modules)
                {
                    //_logger.LogTrace($"程序启动完初始化模块: {module.GetType().Name}");
                    await module.OnApplicationInitialization(context, cancellationToken);
                }
            }
        }

        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;

            foreach (var module in _modules)
            {
                module.ConfigureServices(new ServicesConfigureContext(_services, _configuration));
            }
        }

        public IHostBuilder Init<TModule>(IHostBuilder builder) where TModule : ModuleBase, new()
        {
            //反射注册modules
            var type = typeof(TModule);
            FindDependencies(type);
            //_modules.Reverse();//后进先出
            //_logger.LogInformation($"模块组: {string.Join(",", _modules.Values.Select(x=>x.GetType().Name))}");
            foreach (var module in _modules)
            {
                //_logger.LogTrace($"模块配置: {module.GetType().Name}");
                builder = module.ConfigureHost(builder);
            }
            return builder;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            using var sp = _services.BuildServiceProvider();
            foreach (var module in _modules)
            {
                await module.OnStopping(new ApplicationShutdownContext(sp), cancellationToken);
            }
        }

        private void FindDependencies(Type type)
        {
            //throw new InvalidOperationException($"Type {type.FullName} can't depends on type {type.FullName}, becasuse it's in dead cycle!");
            var attrs = type.GetCustomAttributes<DependencyAttribute>();
            foreach (var attr in attrs)
            {
                foreach (var t in attr.DenpendsOnTypes)
                {
                    if (!_modules.Any(x => x.GetType().FullName == t.FullName))
                    {
                        FindDependencies(t);
                    }
                }
            }
            _modules.Add(Activator.CreateInstance(type) as ModuleBase);
        }
    }

}
