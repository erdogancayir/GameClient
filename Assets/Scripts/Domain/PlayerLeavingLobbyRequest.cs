using MessagePack;

[MessagePackObject]
public class PlayerLeavingLobbyRequest : BasePack
{
    [Key(1)]
    public string? LobbyID { get; set; }

    [Key(2)]
    public int PlayerID { get; set; }

    [Key(3)]
    public string Token { get; set; } = string.Empty;
}

[MessagePackObject]
public class PlayerLeavingLobbyResponse : BasePack
{
    [Key(1)]
    public bool Success { get; set; }

    [Key(2)]
    public string? Message { get; set; }
}