
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class ChatHub : Hub
{
    public const string Url = "/chathub";

    public static Dictionary<string, string> Connections = new();

    public override Task OnConnectedAsync()
    {
        string username = GetRandomUsernameForConnectionId(Context.ConnectionId);

        Console.WriteLine($"{Context.ConnectionId} as {username} connected");

        var message = $"{username} joined the chat.";
        Clients.Caller.SendAsync("OnConnectedSuccessful", Context.ConnectionId, username);
        Clients.Others.SendAsync("Broadcast", message);
        Clients.All.SendAsync("OnUpdatedConnectionsList", Connections);

        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? e)
    {
        var username = Connections.FirstOrDefault(x => x.Key == Context.ConnectionId).Value;
        Connections.Remove(Context.ConnectionId);

        var message = $"{username} disconnected from the chat.";
        Clients.Others.SendAsync("Broadcast", message);
        Clients.All.SendAsync("OnUpdatedConnectionsList", Connections);
        await base.OnDisconnectedAsync(e);
    }

    public async Task Broadcast(string message)
    {
        await Clients.All.SendAsync("Broadcast", message);
    }

    public async Task MessageAll(string connectionId, string message)
    {
        var username = Connections.Single(x => x.Key == connectionId).Value;
        await Clients.All.SendAsync("MessageAll", connectionId, username, message);
    }

    private static string GetRandomUsernameForConnectionId(string connectionId)
    {
        Random random = new();

        string[] names = new string[]
        {
            "Rude Manatee",
            "Melted Pigeon",
            "Dazzling Crow",
            "Spooky Moose",
            "Hapless Giraffe",
            "Plausible Leopard",
            "Dreary Camel",
            "Heavy Stinkbug",
            "Glistening Ram",
            "Fumbling Emu",
            "Picayune Woodpecker",
            "Juvenile Jackal",
            "Rural Hornet",
            "Clean Hedgehog",
            "Shaggy Hyena",
            "Untidy Nightingale",
            "Well-groomed Baboon",
            "Psychotic Cheetah",
            "Righteous Wallaby",
            "Great Boar",
            "Kind Pheasant",
            "Acid Nightingale",
            "Thankful Swan",
            "Rustic Partridge",
            "Capricious Elk",
            "Flowery Antelope",
            "Nosy Lemur",
            "Bored Wallaby",
            "Unruly Lobster",
            "Funny Pigeon"
        };

        names = names.Except(Connections.Values.ToList()).ToArray();
        var name = names[random.Next(0, names.Length)];
        Connections.Add(connectionId, name);
        return name;
    }
}
