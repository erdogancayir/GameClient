using UnityEngine;
using System.Threading;
using System.Net;

public class GameClient : MonoBehaviour
{
    public GameClientHandler _clientHandler;
    public TcpSocket _TcpConnection;
    public UdpSocket _UdpConnection;
    private static GameClient _instance;


    public static GameClient Instance
    {
        get 
        {
            if (_instance == null)
            {
                Debug.Log("GameClient is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _clientHandler = new GameClientHandler();
        _TcpConnection = new TcpSocket();
        _UdpConnection = new UdpSocket();
        Thread clientThread = new Thread(() => ConnectToServer());
        clientThread.Start();
    }

    public void ConnectToServer()
    {
        _TcpConnection.Connect(Global.GameServerIp, Global.GameServerTcpPort);
    }

    void OnApplicationQuit()
    {
        _TcpConnection.Close();
        _UdpConnection.Close();
    }
}