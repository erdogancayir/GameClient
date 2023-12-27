using System;
using System.Net;
using System.Net.Sockets;
using MessagePack;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class UdpSocket
{
    public UdpClient Socket;
    public IPEndPoint _IPEndPoint;
    byte[] receivedBytes = new byte[1024];

    public UdpSocket()
    {
        _IPEndPoint = new IPEndPoint(IPAddress.Parse(Global.GameServerIp), 8082);
    }

    public void Connect(int port)
    {
        Socket = new UdpClient(port);

        Socket.Connect(_IPEndPoint);

        Socket.BeginReceive(ReceiveCallBack, null);
    }

    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            receivedBytes = Socket.EndReceive(ar, ref _IPEndPoint);
            Socket.BeginReceive(ReceiveCallBack, null);
            if (receivedBytes.Length < 1)
                return;
            var basePack = MessagePackSerializer.Deserialize<BasePack>(receivedBytes);
            var receivedData = MessagePackSerializer.Deserialize<PlayerPositionUpdate>(receivedBytes);

            var handler = GameClient.Instance._clientHandler.GetUdpOperationHandler((OperationType)basePack.OperationTypeId);
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                handler?.Invoke(receivedData);
            });
            Socket.BeginReceive(ReceiveCallBack, null);
        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Console.WriteLine(e.ToString()));
        }
    }

    public void SendPlayerPosition(Vector3 Transform)
    {
        var playerMoveData = new PlayerPositionUpdate((int)OperationType.PlayerPositionUpdate, Global.PlayerId, Transform.x, Transform.y, Transform.z);

        byte[] bytes = MessagePackSerializer.Serialize(playerMoveData);
        Socket.BeginSend(bytes, bytes.Length, null, null);
    }

    public void SendData(byte[] data)
    {
        Socket.BeginSend(data, data.Length, null, null);
    }

    public void Close()
    {
        Socket.Close();
    }
}