# Rox
 一切皆是模块

看过了许多风景, 走过了许多泥坑, 心里暗暗下了一些决定:
* 模块是必须的. 
* 模块应该是要用项目(project)隔离代码的.
* 模块需要监听某些生命周期事件.
* 模块间是有依赖的.

关于模块依赖的一些想法:

是不是需要像Abp一样在架构上分层划分模块? 我觉得这不是必要的. 分离Domain\Repository\Service\Api是必要的, 但是用项目来隔离跟用文件夹来分割并没有明显优势(除了强制CRUD程序员不会using过度的namespace). 

模块的划分可以由功能组件的维度来分, 也可以由业务来划分, 也可以在业务模块内部再分架构层级. 这些决定了模块间应当是更复杂的依赖关系, 而不是单一架构维度.

另外在实际的工作中, 愈发地觉得如今热门的微服务应该是有其适用场景的, 在小微型团队中使用微服务并不是一件舒服的事情, 不如先通过模块化组合, 当业务扩大团队扩张之后, 再将模块微服务化.

## 在ConsoleApplication中使用模块
先用Nuget添加包Rox.Extensions.Hosting
```
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureRox<AppModule>()
            .Build();

        await host.RunAsync();
    }
}

public class AppModule: ModuleBase
{
}
```

## 在Asp.Net Core中使用模块
先用Nuget添加包Rox.Extensions.AspNetCore
修改Startup.cs
```
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication<WebModule>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseApplication();
    }
}
```
创建WebModule.cs
```
public class WebModule : ModuleBase
{
    public override Task OnApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
    {
        var app = context.GetApplicationBuilder();
        var env = context.ServiceProvider.GetService<IWebHostEnvironment>();

        app.UseGlobalLoggerMiddleware();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        return Task.CompletedTask;
    }

    public override Task ConfigureServices(ServicesConfigureContext context, CancellationToken cancellationToken)
    {
        context.Services.AddControllers();
        return Task.CompletedTask;
    }
}
```
## 模块依赖
当一个模块想要使用其他模块的功能的时候, 即该模块对其他模块产生了依赖时, 可以使用DependencyAttribute来添加依赖模块.
如:
```
[Dependency(
    typeof(NLogModule),
    typeof(HelloModule)
    )]
public class WebModule : ModuleBase
...
```
