using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Numerics;
using MessagePack;
using PimDeWitte.UnityMainThreadDispatcher;

public class GameClientHandler
{
    private Dictionary<OperationType, Action<PlayerPositionUpdate>> _udpOperationHandlers;
    private Dictionary<OperationType, Action<NetworkStream, byte[]>> _tcpOperationHandlers;

    public GameClientHandler()
    {
        InitializeUdpOperationHandlers();
        InitializeTcpOperationHandlers();
    }

    private void InitializeTcpOperationHandlers()
    {
        _tcpOperationHandlers = new Dictionary<OperationType, Action<NetworkStream, byte[]>>
        {
            {OperationType.PlayerLobbyInfoResponse, PlayerLobbyResponse},
            // {OperationType.GameOverResponse, GameOverResponse},
        };
    }

    private void InitializeUdpOperationHandlers()
    {
        _udpOperationHandlers = new Dictionary<OperationType, Action<PlayerPositionUpdate>>
        {
            {OperationType.PlayerPositionUpdate, PlayerMoveResponse},
        };
    }

    private void GameOverResponse(NetworkStream stream, byte[] data)
    {
        var gameOverResponse = MessagePackSerializer.Deserialize<GameOverResponse>(data);
        // UnityMainThreadDispatcher.Instance().Enqueue(() =>
        // {
            // UIManager.Instance.GameOver(gameOverResponse.WinnerPlayerId);
        // });
    }


    private void PlayerLobbyResponse(NetworkStream stream, byte[] arg2)
    {
        EndPointPack endPointPack = new EndPointPack
        {
            OperationTypeId = (int)OperationType.EndPoint,
            PlayerId = Global.PlayerId
        };
        var serializedEndPointPack = MessagePackSerializer.Serialize(endPointPack);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                GameClient.Instance._UdpConnection.SendData(serializedEndPointPack);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }


    private void PlayerMoveResponse(PlayerPositionUpdate response)
    {
        PlayerManager.Instance.UpdateOrCreatePlayer(response.PlayerId, response.X, response.Y);
        Debug.WriteLine("Received data : " + response.PlayerId.ToString() + "," + response.X.ToString() + "," + response.Y.ToString());
    }

    public Action<PlayerPositionUpdate> GetUdpOperationHandler(OperationType operationType)
    {
        if (_udpOperationHandlers.TryGetValue(operationType, out Action<PlayerPositionUpdate> handler))
        {
            return handler;
        }
        else
        {
            Console.WriteLine($"Handler not found for operation type: {operationType}");
            return null;
        }
    }

    public Action<NetworkStream, byte[]> GetTcpOperationHandler(OperationType operationType)
    {
        if (_tcpOperationHandlers.TryGetValue(operationType, out Action<NetworkStream, byte[]> handler))
        {
            return handler;
        }
        else
        {
            Console.WriteLine($"Handler not found for operation type: {operationType}");
            return null;
        }
    }




}