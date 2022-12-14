using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.Autolink;

public interface IChannel
{
    bool IsConnected { get; }

    Task WriteAsync<TPackage>(TPackage package) where TPackage: AutolinkPackage;

    Task DisconnectAsync();

    Task PingAsync();
}

public interface IChannelFactory
{
    Task<IChannel> BuildChannelAsync(AutolinkOptions options, MessageReceived messageReceived);
}

public delegate IChannel BuildChannel();
public delegate void MessageReceived(AutolinkPackage package);
