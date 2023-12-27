using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

public class MasterClient : MonoBehaviour
{
    public MasterClientHandler masterClientHandler;
    private static MasterClient _instance;
    public string Token { get; set; }
    private TcpClient _tcpClient;
    private NetworkStream _stream;

    public static MasterClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("MasterClient").AddComponent<MasterClient>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        masterClientHandler = new MasterClientHandler();
    }

    void Start()
    {
        _tcpClient = new TcpClient();
        Debug.Log("Connecting to master server...");
        _tcpClient.BeginConnect(Global.MasterIpAddress, Global.MasterServerPort, OnConnect, null);
    }

    private void OnConnect(IAsyncResult ar)
    {
        byte[] buffer = new byte[1024];
        _tcpClient.EndConnect(ar);
        _stream = _tcpClient.GetStream();
        _stream.BeginRead(buffer, 0, buffer.Length, OnReceiveData, buffer);
    }

    private void OnReceiveData(IAsyncResult ar)
    {
        byte[] buffer = (byte[])ar.AsyncState;
        int read = _stream.EndRead(ar);
        if (read < 1)
        {
            Debug.Log("Disconnected from master server");
            return;
        }
        byte[] data = new byte[read];
        Array.Copy(buffer, data, read);
        HandleReceivedData(buffer, read);
        _stream.BeginRead(buffer, 0, buffer.Length, OnReceiveData, buffer);
    }

    private void HandleReceivedData(byte[] data, int length)
    {
        if (length < 1)
        {
            Debug.Log("Disconnected from master server");
            return;
        }

        var basePacket = MessagePackSerializer.Deserialize<BasePack>(data);
        OperationType operationType = (OperationType)basePacket.OperationTypeId;
        if (!Enum.IsDefined(typeof(OperationType), operationType))
        {
            Debug.Log("Invalid operation type");
            return;
        }

        Action<NetworkStream, byte[]> tempHandler = null;
        var handlerExists = masterClientHandler._handlers?.TryGetValue(operationType, out tempHandler) == true;
        if (handlerExists && tempHandler != null)
        {
            try
            {
                tempHandler(_stream, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing handler for operation {operationType}: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"No handler found for operation type: {operationType}");
        }
    }

    public void SendTcpData(byte[] data)
    {
        _stream.Write(data, 0, data.Length);
    }
}
