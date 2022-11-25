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

namespace Rox.Extensions.Hosting
{
    internal class Application
    {
        //no matter what, it should be static.
        private static readonly List<ModuleBase> _modules = new List<ModuleBase>();
        //no matter what, it should be static too.
        private static readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();

        private IServiceCollection _services;
        private IConfiguration _configuration;

        public Application()
        {
        }

        internal IHostBuilder Init<TModule>(IHostBuilder builder) where TModule : ModuleBase, new()
        {
            //反射注册modules
            var type = typeof(TModule);
            GetModuleInfo(type);
            //_modules.Reverse();//后进先出
            //_logger.LogInformation($"模块组: {string.Join(",", _modules.Values.Select(x=>x.GetType().Name))}");
            foreach (var module in _modules)
            {
                //_logger.LogTrace($"模块配置: {module.GetType().Name}");
                builder = module.ConfigureHost(builder);
            }
            return builder;
        }

        internal void Configure(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;

            foreach (var module in _modules)
            {
                module.ConfigureServices(new ServicesConfigureContext(_services, _configuration, _assemblies));
            }
        }

        internal async Task Start(CancellationToken cancellationToken)
        {
            using (var provider = _services.BuildServiceProvider())
            {
                var context = new ApplicationInitializationContext(provider);
                foreach (var module in _modules)
                {
                    await module.PreApplicationInitialization(context, cancellationToken);
                }
                foreach (var module in _modules)
                {
                    await module.OnApplicationInitialization(context, cancellationToken);
                }
            }
        }

        internal async Task Stop(CancellationToken cancellationToken)
        {
            using var sp = _services.BuildServiceProvider();
            foreach (var module in _modules)
            {
                await module.OnStopping(new ApplicationShutdownContext(sp), cancellationToken);
            }
        }

        private void GetModuleInfo(Type type, Stack<Type> parents = null)
        {
            if(parents == null)
            {
                parents = new Stack<Type>();
            }
            else if(parents.Any(x=>x == type))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("denpendences in dead cycle: ");
                foreach (var p in parents)
                {
                    if (p == type)
                    {
                        sb.Append("*");
                        sb.Append(p.Name);
                    }
                    else
                    {
                        sb.Append(p.Name);
                    }
                    sb.Append("-> ");
                }
                sb.Append("*");
                sb.Append(type.Name);
                //throw new InvalidOperationException($"Type {type.FullName} can't depends on type {type.FullName}, becasuse it's in dead cycle!");
                throw new StackOverflowException(sb.ToString());
            }
            parents.Push(type);
            var attrs = type.GetCustomAttributes<DependencyAttribute>();
            foreach (var attr in attrs)
            {
                foreach (var t in attr.DenpendsOnTypes)
                {
                    if (!_modules.Any(x => x.GetType().FullName == t.FullName))
                    {
                        GetModuleInfo(t, parents);
                    }
                }
            }
            parents.Pop();
            _modules.Add(Activator.CreateInstance(type) as ModuleBase);
            if(!_assemblies.Contains(type.Assembly))
            {
                _assemblies.Add(type.Assembly);
            }
        }
    }

}
