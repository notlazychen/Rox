using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rox
{
    public abstract class ModuleBase
    {
        public virtual Task ConfigureServices(AppContext appContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnApplicationInitialization(AppContext appContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task PreApplicationInitialization(AppContext appContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnStopping(AppContext appContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class ModuleCenter
    {
        private readonly Dictionary<string, ModuleBase> _modules = new Dictionary<string, ModuleBase>();
        private readonly AppContext _appContext;

        public ModuleCenter(IServiceCollection services)
        {
            _appContext = new AppContext(services);
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>(_modules.Count);

            foreach (var module in _modules.Values)
            {
                tasks.Add(module.PreApplicationInitialization(_appContext, cancellationToken));
            }
            await Task.WhenAll(tasks);

            tasks.Clear();
            foreach (var module in _modules.Values)
            {
                tasks.Add(module.OnApplicationInitialization(_appContext, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        public Task Configure(CancellationToken cancellationToken)
        {
            //todo: 反射注册modules

            var tasks = new List<Task>(_modules.Count);
            foreach (var module in _modules.Values)
            {
                tasks.Add(module.ConfigureServices(_appContext, cancellationToken));
            }
            return Task.WhenAll(tasks);
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>(_modules.Count);
            foreach (var module in _modules.Values)
            {
                tasks.Add(module.OnStopping(_appContext, cancellationToken));
            }
            return Task.WhenAll(tasks);
        }
    }

    public class AppContext
    {
        private readonly IServiceCollection _services;
        public AppContext(IServiceCollection services)
        {
            _services = services;
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

    
}
