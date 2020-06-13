using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rox
{
    public abstract class ModuleBase
    {
        public virtual Task ConfigureServices(ServicesConfigureContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task PreApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnStopping(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class Application
    {
        private readonly Dictionary<string, ModuleBase> _modules = new Dictionary<string, ModuleBase>();
        private readonly IServiceCollection _services; 
        private readonly IConfiguration _configuration;

        public Application(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;
        }

        public void Start(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>(_modules.Count);
            using (var provider = _services.BuildServiceProvider())
            {
                var context = new ApplicationInitializationContext(provider);
                foreach (var module in _modules.Values)
                {
                    tasks.Add(module.PreApplicationInitialization(context, cancellationToken));
                }
                Task.WhenAll(tasks).Wait();

                tasks.Clear();
                foreach (var module in _modules.Values)
                {
                    tasks.Add(module.OnApplicationInitialization(context, cancellationToken));
                }

                Task.WhenAll(tasks).Wait();
            }
        }

        public void Init<TModule>(CancellationToken cancellationToken) where TModule : ModuleBase, new()
        {
            //反射注册modules
            var type = typeof(TModule);
            _modules.Add(type.FullName, new TModule());
            FindDependencies(type);

            var tasks = new List<Task>(_modules.Count);
            foreach (var module in _modules.Values)
            {
                tasks.Add(module.ConfigureServices(new ServicesConfigureContext(_services, _configuration), cancellationToken));
            }
            Task.WhenAll(tasks).Wait();
        }

        public void Stop(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>(_modules.Count);
            foreach (var module in _modules.Values)
            {
                tasks.Add(module.OnStopping(cancellationToken));
            }
            Task.WhenAll(tasks).Wait();
        }

        private void FindDependencies(Type type)
        {
            //if (_modules.ContainsKey(type.FullName))
            {
                //todo: 其实这里判断是不对的
                //throw new InvalidOperationException($"Type {type.FullName} can't depends on type {type.FullName}, becasuse it's in dead cycle!");
            }
            var attrs = type.GetCustomAttributes<DependencyAttribute>();
            foreach (var attr in attrs)
            {
                foreach (var t in attr.DenpendsOnTypes)
                {
                    if (_modules.ContainsKey(t.FullName))
                    {
                        _modules.Add(t.FullName, Activator.CreateInstance(t) as ModuleBase);
                        FindDependencies(t);
                    }
                }
            }
        }
    }

    public class ApplicationInitializationContext
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public ApplicationInitializationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }

    public class ServicesConfigureContext
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public ServicesConfigureContext(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyAttribute : Attribute
    {
        private Type[] _types;
        public DependencyAttribute(params Type[] types)
        {
            _types = types;
            foreach (var t in types)
            {
                if (!t.IsSubclassOf(typeof(ModuleBase)))
                {
                    throw new InvalidCastException("All denpendencies must be subclass of ModuleBase");
                }
            }
        }

        public IEnumerable<Type> DenpendsOnTypes
        {
            get { return _types; }
        }
    }


    public static class RoxExtensions
    {
        public static IServiceCollection AddApplication<TModule>(this IServiceCollection services) where TModule: ModuleBase, new()
        {
            using (var provider = services.BuildServiceProvider())
            {
                var configuration = provider.GetService<IConfiguration>();
                var application = new Application(services, configuration);
                application.Init<TModule>(CancellationToken.None);
                services.AddSingleton(application);
            }
            return services;
        } 
    }
    
}
