# Rox
 一切皆是模块

看过了许多风景, 走过了许多泥坑, 心里暗暗下了一些决定:
* 模块是必须的. 
* 模块应该是要用项目(project)隔离代码的.
* 模块需要监听某些生命周期事件.
* 模块间是有依赖的.

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
