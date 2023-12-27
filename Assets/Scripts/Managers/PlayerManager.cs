using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour 
{
    public static PlayerManager Instance { get; private set; }
    [SerializeField] private GameObject OtherPlayer;
    public Dictionary<int, Player> Players { get; set; } = new Dictionary<int, Player>();

    void Awake()
    {
        Instance = this;
    }

    public void UpdateOrCreatePlayer(int playerId, float x, float y)
    {
        Vector2 Position = new Vector2(x, y);
        if (Players.ContainsKey(playerId))
        {
            Players[playerId].SetPosition(new Vector2(x, y));
        }
        else
        {
            Color color = new Color(Random.value, Random.value, Random.value);
            var player = new Player(Position, Instantiate(OtherPlayer, Position, Quaternion.identity));
            player.PlayerObject.GetComponent<SpriteRenderer>().color = color;
            Players.Add(playerId, player);
        }
    }

    public void RemovePlayer(int playerId)
    {
        if (Players.ContainsKey(playerId))
        {
            Destroy(Players[playerId].PlayerObject);
            Players.Remove(playerId);
        }
    }

    public void ResetPlayers()
    {
        foreach (var player in Players)
        {
            Destroy(player.Value.PlayerObject);
        }
        Players.Clear();
    }
}