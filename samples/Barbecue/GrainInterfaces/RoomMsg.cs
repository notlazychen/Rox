namespace Barbecue.GrainInterfaces;

[GenerateSerializer]
public record RoomMsg
{
    public RoomMsg(string method, string msg)
    {
        this.Method = method;
        this.Msg = msg;
    }

    [Id(0)]
    public string Method { get; init; }

    [Id(1)]
    public string Msg { get; init; }
}
