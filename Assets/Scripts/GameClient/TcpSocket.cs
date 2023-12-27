using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;
using PimDeWitte.UnityMainThreadDispatcher;

public class TcpSocket
{
    public TcpClient Socket;
    public NetworkStream _NetworkStream;
    private byte[] _receivedBytes = new byte[1024];

    public TcpSocket()
    {
        Socket = new TcpClient();
    }

    public void Connect(string ip, int port)
    {
        Socket.BeginConnect(ip, port, ConnectCallBack, null);
    }

    private void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket.EndConnect(ar);
            GameClient.Instance._UdpConnection.Connect(((IPEndPoint)Socket.Client.LocalEndPoint).Port);
            _NetworkStream = Socket.GetStream();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        _NetworkStream.BeginRead(_receivedBytes, 0, _receivedBytes.Length, ReceiveCallBack, null);

    }

    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            int byteLength = _NetworkStream.EndRead(ar);
            if (byteLength <= 0)
            {
                return;
            }
            byte[] data = new byte[byteLength];
            Array.Copy(_receivedBytes, data, byteLength);

            HandleReceivedData(data, byteLength);
            _NetworkStream.BeginRead(_receivedBytes, 0, _receivedBytes.Length, ReceiveCallBack, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void HandleReceivedData(byte[] data, int readBytes)
    {
        if (readBytes <= 0)
        {
            Console.WriteLine("Disconnected from game server");
            return;
        }
        var basePack = MessagePackSerializer.Deserialize<BasePack>(data);
        OperationType operationType = (OperationType)basePack.OperationTypeId;

        if (!Enum.IsDefined(typeof(OperationType), operationType))
        {
            Console.WriteLine("Invalid operation type");
            return;
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                var handler = GameClient.Instance._clientHandler.GetTcpOperationHandler(operationType);

                if (handler != null)
                {
                    handler(_NetworkStream, data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        });
    }

    public void SendData(byte[] data)
    {
        _NetworkStream.Write(data, 0, data.Length);
    }

    public void Close()
    {
        Socket.Close();
    }

    public void SendLobbyDisconnectRequest()/////////////////////////
    {
        var playerLeavingLobbyRequest = new PlayerLeavingLobbyRequest()
        {
            OperationTypeId = (int)OperationType.LeaveLobbyRequest,
            LobbyID = Global.LobbyId,
            PlayerID = Global.PlayerId,
            Token = Global.Token,
        };
        var data = MessagePackSerializer.Serialize(playerLeavingLobbyRequest);
        Debug.WriteLine($"Sending leave {playerLeavingLobbyRequest.LobbyID} {playerLeavingLobbyRequest.PlayerID}");
        MasterClient.Instance.SendTcpData(data);
    }

    public void SendGameOverRequest()///////////////////////
    {
        var finishRequest = new GameOverPack()
        {
            OperationTypeId = (int)OperationType.GameOverRequest,
            LobbyId = Global.LobbyId,
            WinnerPlayerId = Global.PlayerId,
        };
        var data = MessagePackSerializer.Serialize(finishRequest);
        SendData(data);
    }

}