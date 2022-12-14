using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rox.Extensions.Autolink;


internal static class EdgeChannelMessageQueue
{
    internal static readonly ConcurrentQueue<AutolinkPackage> Output = new ConcurrentQueue<AutolinkPackage>();
    internal static readonly ConcurrentQueue<AutolinkPackage> Input = new ConcurrentQueue<AutolinkPackage>();
}

public class AutolinkClient : IHostedService
{
    public bool IsConnected { get; internal set; }
    public bool IsAuth { get; internal set; }

    private readonly ILogger<AutolinkClient> _logger;
    private readonly IOptions<AutolinkOptions> _options;
    private readonly IChannelFactory _channelFactory;

    //private RpcTokenStation _rpcTokenStation;

    public AutolinkClient(
        ILogger<AutolinkClient> logger,
        IOptions<AutolinkOptions> options,
        IChannelFactory channelFactory)
    {
        this._logger = logger;
        this._options = options;
        this._channelFactory = channelFactory;
        //_rpcTokenStation = rpcTokenStation;
    }

    private Task _task;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _task = Task.Factory.StartNew(OutputLoop, TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _tokenOutput.Cancel();
        return _task;
    }

    public async Task<bool> Auth<TRequest>(TRequest key)
    {
        //var resp = await this.Rpc<AuthRequest, AuthResponse>(new AuthRequest { Key = key });
        //if (!resp.Ok)
        //{
        //    throw new Exception($"序列号{key}错误, 无法注册边缘服务!");
        //}
        //else
        {
            return true;
        }
    }

    //public async Task<TResponse> Rpc<TRequest, TResponse>(TRequest request)
    //    where TRequest : MessageBase
    //    where TResponse : MessageBase
    //{
    //    var resp = await _rpcTokenStation.RpcAsync<TRequest, TResponse>(this, request);
    //    return resp;
    //}

    private int _id_increment = 0;

    public void Send(AutolinkPackage package)
    {
        package.Id = Interlocked.Increment(ref _id_increment);
        package.CreateAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        EdgeChannelMessageQueue.Output.Enqueue(package);
    }

    CancellationTokenSource _tokenOutput = new CancellationTokenSource();

    private async Task OutputLoop()
    {
        var nextCheckConnectionTime = DateTime.Now;
        IChannel channel = null;
        while (!_tokenOutput.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                //检查连接状态
                if (now >= nextCheckConnectionTime)
                {
                    _logger.LogTrace($"【edge】检查连接状态");
                    if (channel == null || !channel.IsConnected)
                    {
                        channel = await Connect();
                    }
                    else
                    {
                        await channel.PingAsync();
                    }
                    nextCheckConnectionTime = now.AddSeconds(1);
                }

                //消费输出队列
                while (!_tokenOutput.IsCancellationRequested
                    && EdgeChannelMessageQueue.Output.TryDequeue(out var pkg))
                {
                    _logger.LogTrace($"【edge】提取包");
                    bool isSent;
                    do
                    {
                        try
                        {
                            await channel.WriteAsync(pkg);
                            isSent = true;
                        }
                        catch (Exception ex)
                        {
                            //重试
                            isSent = false;
                            _logger.LogWarning($"【edge】发送失败，重试，{ex.Message}");
                            channel = await Connect();
                        }
                    } while (!_tokenOutput.IsCancellationRequested && !isSent);
                    _logger.LogTrace($"【edge】发送成功");
                }
            }
            catch (Exception ex)
            {
                //crash 
                _logger.LogError(ex, ex.Message);
                continue;
            }
            await Task.Delay(1);
        }

        if (channel != null && channel.IsConnected)
        {
            await channel.DisconnectAsync();
        }
    }
    private async Task<IChannel> Connect()
    {
        this.IsConnected = false;
        do
        {
            try
            {
                _logger.LogWarning($"【edge】尝试重连");

                var channel = await _channelFactory.BuildChannelAsync(_options.Value, null);
                this.IsConnected = true;

                return channel;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"【edge】连接失败，重试，{ex.Message}");
            }
            await Task.Delay(2000);
        } while (!_tokenOutput.IsCancellationRequested);

        return null;
    }

    //private void MessageReceived(AutolinkPackage package)
    //{
    //    _logger.LogInformation($"[{package.Key}] { JsonConvert.SerializeObject(package.Body)}");
    //}
}

public interface IClient
{

}