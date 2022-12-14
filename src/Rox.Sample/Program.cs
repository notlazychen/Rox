using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Rox.Extensions.Hosting;
using Rox.Extensions.Mediate;
using Microsoft.Extensions.DependencyInjection;
using Rox.Extensions.Autolink;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Reflection;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Rox.Sample;

class Program
{
    static async Task Main(string[] args)
    {
        //await TestMediator();

        await TestAutolink();
    }

    static async Task TestAutolink()
    {
        string address = "http://192.168.200.34:30019/im";
        string deviceId = "372b19f4fed50f0849a674ba559e8be6";
        string userId = "3g7b72k2vaT";
        string token = "b1EANhXY_0AXTRXEQHCsbDdbsPjZrsBYQG5_pcxETzGZV99BFTE-R3JEz25Hs21b";


        using var host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging => logging.ClearProviders().AddLog4Net().SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(ss =>
            {
                ss.AddAutolink<SignalRChannelFactory>(new AutolinkOptions() { Address = address });
            })
            .UseRox<AppModule>()
            .Build();

        await host.StartAsync();

        var client = host.Services.GetService<AutolinkClient>();
        while (true)
        {
            string input = Console.ReadLine();
            if (input == "q")
            {
                break;
            }
            client.Send(new AutolinkPackage()
            {
                Key = "Auth",
                Body = new [] 
                { 
                    new AuthRequest
                    {
                        UserId = userId,
                        ClientType = 1,//1mobile, 2web, 3pc
                        Token = token,
                        DeviceId = deviceId,
                        Act = 1,
                        Version = 1,
                    }
                }
            });
        }
        Console.WriteLine("byebye");
        await host.StopAsync();
    }

    static async Task TestMediator()
    {
        using var host = Host.CreateDefaultBuilder()
            .UseRox<AppModule>()
            .Build();

        await host.StartAsync();

        while (true)
        {
            string input = Console.ReadLine();
            if (input == "q")
            {
                break;
            }
            var mediator = host.Services.GetService<IMediator>();
            if (int.TryParse(input, out var n))
            {
                await mediator.Send(new MyContext<int> { Request = n, ServerId = "aaa" }, CancellationToken.None);
            }
            else
            {
                await mediator.Send(new MyContext<string> { Request = input, ServerId = "aaa" }, CancellationToken.None);
            }
        }
        Console.WriteLine("byebye");
        await host.StopAsync();
    }
}

#region Autolink
class SignalRChannel : IChannel
{
    public string Id { get; } = Guid.NewGuid().ToString();

    private IMediator _mediator;
    private HubConnection _connection;

    public SignalRChannel(IMediator mediator)
    {
        _mediator = mediator;
    }
    //private MessageReceived _messageReceived { set; get; }
    //public SignalRChannel(MessageReceived messageReceived)
    //{
    //    _messageReceived = messageReceived;
    //}

    public bool IsConnected { get; private set; }

    private bool _isAuthed = false;
    private AuthRequest _lastAuthRequest;

    public async Task ConnectAsync(string address)
    {
        if (File.Exists("auth.json"))
        {
            var x = JsonConvert.DeserializeObject<AuthRequest>(File.ReadAllText("auth.json", Encoding.UTF8));
            if (x != null)
            {
                _isAuthed = true;
                _lastAuthRequest = x;
            }
        }
        _connection = new HubConnectionBuilder()
          .WithUrl(address, options =>
          {
          }).Build();

        _connection.Closed += (error) =>
        {
            IsConnected = false;
            return Task.CompletedTask;
        };

        _connection.On<string>("Echo", msg => _onMessageReceived("Echo", msg));
        _connection.On<long, string>("Receive", (seq, payload) => _onMessageReceived("Receive", seq, payload));
        _connection.On<string>("Kickout", (reason) => _onMessageReceived("Kickout", reason));
        _connection.On<string, int, string>("Post", (rpcId, statusCode, content) => _onMessageReceived("Post", rpcId, statusCode, content));
        _connection.On<AuthResponse>("Auth", (message) => _onMessageReceived("Auth", message));

        await _connection.StartAsync();
        if (_isAuthed)
        {
            _connection.SendAsync("Auth", _lastAuthRequest);
        }
        IsConnected = true;
    }

    private void _onMessageReceived(string method, params object[] args)
    {
        var package = new AutolinkPackage { Key = method, Body = args };
        object msg = null;
        switch (method)
        {
            case "Auth":
                var authResponse = args[0] as AuthResponse;
                msg = authResponse;
                if(msg != null && authResponse.Ok)
                {
                    _lastAuthRequest.Act = 0;
                    _isAuthed = true;
                    //存档
                    File.WriteAllText("auth.json", JsonConvert.SerializeObject(_lastAuthRequest), Encoding.UTF8);
                }
                break;
            case "Echo":
                msg = args[0] as string;
                break;
            case "Receive":
                var payload = (string)args[1];
                var body = JsonConvert.DeserializeObject<SendPackageBody>(payload);
                package.RpcTiger = body.Source;
                msg = body;
                break;
            default:
                break;
        }
        if (msg != null)
        {
            _mediator.Send(msg, CancellationToken.None);
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            IsConnected = false;
            await _connection.DisposeAsync();
        }
        catch (Exception)
        {

        }
    }

    public async Task WriteAsync<TPackage>(TPackage package) where TPackage : AutolinkPackage
    {
        var objs = package.Body as object[];
        string methodName = package.Key;
        if(methodName == "Auth")
        {
            _lastAuthRequest = objs[0] as AuthRequest;
        }
        switch (objs.Length)
        {
            case 1:
                await _connection.SendAsync(methodName, objs[0]);
                break;
            case 2:
                await _connection.SendAsync(methodName, objs[0], objs[1]);
                break;
            case 3:
                await _connection.SendAsync(methodName, objs[0], objs[1], objs[2]);
                break;
            case 4:
                await _connection.SendAsync(methodName, objs[0], objs[1], objs[2], objs[3]);
                break;
        }
    }

    public async Task PingAsync()
    {        
        var unixTimeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        byte[] data = BitConverter.GetBytes(unixTimeNow);
        await _connection.SendAsync("Echo", (object)data);
    }
}

class SignalRChannelFactory : IChannelFactory
{
    private IMediator _mediator;
    public SignalRChannelFactory(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IChannel> BuildChannelAsync(AutolinkOptions options, MessageReceived messageReceived)
    {
        var c = new SignalRChannel(_mediator);
        await c.ConnectAsync(options.Address);
        return c;
    }
}
#endregion Autolink

#region Rox\Mediator
[Dependency(
    typeof(FooModule),
    typeof(Hello2Module),
    typeof(MediateModule)
    //typeof(AutolinkModule)
    )]
public class AppModule: ModuleBase
{
}

public class Hello1Module : ModuleBase
{
    public override void ConfigureServices(ServicesConfigureContext context)
    {
        Console.WriteLine("Hello1");
        base.ConfigureServices(context);
    }
}


[Dependency(
    typeof(Hello1Module)
    )]
public class Hello2Module : ModuleBase
{
    public override void ConfigureServices(ServicesConfigureContext context)
    {
        Console.WriteLine("Hello2");
        base.ConfigureServices(context);
    }
}

[Dependency(
    typeof(Hello1Module)
    )]
public class FooModule : ModuleBase
{
    public override void ConfigureServices(ServicesConfigureContext context)
    {
        Console.WriteLine("Foo");
        base.ConfigureServices(context);
    }
}

public class StringCommand : ICommand<MyContext<string>>
{
    private Guid _id = Guid.NewGuid();

    public Task ExecuteAsync(MyContext<string> request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{_id}] {request.Request}");
        return Task.CompletedTask;
    }
}

public class StringCommand2 : ICommand<MyContext<string>>
{
    private Guid _id = Guid.NewGuid();

    public Task ExecuteAsync(MyContext<string> request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{_id}] {request.Request}");
        return Task.CompletedTask;
    }
}

public class IntCommand2 : ICommand<MyContext<int>>
{
    private Guid _id = Guid.NewGuid();

    public Task ExecuteAsync(MyContext<int> request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{_id}] [{DateTime.Now:U}] {request.Request}");
        return Task.CompletedTask;
    }
}

public class MyContext<T>
{
    public T Request { get; set; }
    public string ServerId { get; set; }
}
#endregion Rox\Mediator




public class AuthRequest
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public int ClientType { get; set; }
    public int Version { get; set; }
    public string DeviceId { get; set; }
    /// <summary>
    /// 0断线重连，1初始化
    /// </summary>
    public int Act { get; set; }
}

public class AuthResponse
{
    public bool Ok { get; set; }
    public string Info { get; set; }
    public string Token { get; set; }
    /// <summary>
    /// PC端在线时是否开启手机通知
    /// </summary>
    public bool IsNotice { get; set; }
    /// <summary>
    /// 当前在线的客户端类型
    /// </summary>
    public int[] OnlineClients { get; set; }
    /// <summary>
    /// gzip压缩后的
    /// </summary>
    public string OfflineMsgs { get; set; }
    public string Data { get; set; }
}


public class SendPackageBody
{
    /// <summary>
    /// 包类型
    /// </summary>
    public int Type { get; set; }
    /// <summary>
    /// 包体内容
    /// </summary>
    public object Data { get; set; }
    /// <summary>
    /// 来源模块
    /// </summary>
    public string Source { get; set; }
}