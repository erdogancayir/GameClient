using UnityEngine;

public class Player
{
    public GameObject PlayerObject { get; set; } = null;

    public Player(Vector2 position, GameObject playerObject)
    {
        PlayerObject = playerObject;
        PlayerObject.transform.position = position;
    }

    public void SetPosition(Vector2 position)
    {
        PlayerObject.transform.position = position;
    }
}