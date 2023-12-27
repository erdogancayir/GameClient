using MessagePack;

[MessagePackObject]
public class PlayerPositionUpdate : BasePack
{
    [Key(1)]
    public int PlayerId { get; set; }

    [Key(2)]
    public float X;

    [Key(3)]
    public float Y;


    public PlayerPositionUpdate()
    {
        PlayerId = 0;
        X = 0;
        Y = 0;
    }

    public PlayerPositionUpdate(int requestType,int id, float x, float y, float z)
    {
        OperationTypeId = requestType;
        PlayerId = id;
        this.X = x;
        this.Y = y;
    }
}