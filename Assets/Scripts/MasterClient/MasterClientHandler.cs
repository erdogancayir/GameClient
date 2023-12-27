using MessagePack;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class MasterClientHandler
{
    public Dictionary<OperationType, Action<NetworkStream, byte[]>> _handlers;

    public MasterClientHandler()
    {
        initalizeHandlers();
    }

    private void initalizeHandlers()
    {
        _handlers = new Dictionary<OperationType, Action<NetworkStream, byte[]>>
        {
            { OperationType.LoginResponse, OnLoginResponse },
            { OperationType.SignUpResponse, OnRegisterResponse },
            { OperationType.JoinLobbyResponse, JoinLobbyResponse },
            { OperationType.CreateLobbyResponse, CreateLobbyResponse },
            { OperationType.NotifyGameStart, LobbyStartResponse },
            { OperationType.DisconnectedPlayerResponse, HandleDisconnectedPlayer },
            { OperationType.GetTopLeaderboardEntriesResponse, HandleLeaderBoard },

            { OperationType.GameEndInfo, GameEndInfo },

        };
    }

    public void HandleLeaderBoard(NetworkStream stream, byte[] data)
    {
        var leaderboard = MessagePackSerializer.Deserialize<SendTopLeaderboardResponsePack>(data);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.LeaderBoard(leaderboard.TopEntries);
        });
    }

    public void GameEndInfo(NetworkStream stream, byte[] data)
    {
        var gameEndInfo = MessagePackSerializer.Deserialize<GameEndInfoPack>(data);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UIManager.Instance.GameEndInfo(gameEndInfo.Username);
        });
    }

    private void HandleDisconnectedPlayer(NetworkStream stream, byte[] data)
    {
        var handleDisconnectedPlayer = MessagePackSerializer.Deserialize<HandleDisconnectedPlayer>(data);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            PlayerManager.Instance.RemovePlayer(handleDisconnectedPlayer.PlayerId);
        });
    }

    #region TCP Response


    private void OnRegisterResponse(NetworkStream stream, byte[] data)
    {
        var registerResponse = MessagePackSerializer.Deserialize<SignUpResponse>(data);
        if (registerResponse.Success)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.infoText.text = $"Register is successful";
            });
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.infoText.text = $"Register is not successful!";
            });
        }
    }
    private void OnLoginResponse(NetworkStream stream, byte[] data)
    {
        var loginResponse = MessagePackSerializer.Deserialize<AuthenticationResponse>(data);
        if (loginResponse.Success)
        {
            Global.Token = loginResponse.Token;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.infoText.text = $"Login is successful";
                UIManager.Instance.LoginSuccess();
            });
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.infoText.text = $"Login is not successful";
            });
        }
    }

    private void JoinLobbyResponse(NetworkStream stream, byte[] data)
    {
        var joinLobbyResponse = MessagePackSerializer.Deserialize<JoinLobbyResponse>(data);
        if (joinLobbyResponse.Success)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Global.LobbyId = joinLobbyResponse.LobbyID;
                Global.PlayerId = joinLobbyResponse.PlayerID;
                SendJoinedLobbyInfo();
                UIManager.Instance.JoinLobbySuccess();
            });
        }
    }

    private void CreateLobbyResponse(NetworkStream stream, byte[] data)
    {
        var createLobbyResponse = MessagePackSerializer.Deserialize<CreateLobbyResponse>(data);
        if (createLobbyResponse.Success)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Global.LobbyId = createLobbyResponse.LobbyID;
                Global.PlayerId = createLobbyResponse.PlayerID;
                SendJoinedLobbyInfo();
                UIManager.Instance.JoinLobbySuccess();
            });
        }
        else
        {
        }
    }

    #endregion

    #region TCP Request

    public void GetLeaderBoard()
    {
        var leaderboardreq = new GetTopLeaderboardPack
        {
            OperationTypeId = (int)OperationType.GetTopLeaderboardEntries,
            TopLimit = 5
        };
        var data = MessagePackSerializer.Serialize(leaderboardreq);
        MasterClient.Instance.SendTcpData(data);
    }
    public void SendLoginRequest(string username, string password)
    {
        var authreq = new AuthenticationRequest
        {
            OperationTypeId = (int)OperationType.LoginRequest,
            Username = username,
            Password = password
        };
        var data = MessagePackSerializer.Serialize(authreq);
        MasterClient.Instance.SendTcpData(data);
    }

    public void SendRegisterRequest(string username, string password)
    {
        var authreq = new AuthenticationRequest
        {
            OperationTypeId = (int)OperationType.SignUpRequest,
            Username = username,
            Password = password
        };
        var data = MessagePackSerializer.Serialize(authreq);
        MasterClient.Instance.SendTcpData(data);
    }

    public void SendJoinLobbyRequest()
    {
        var joinLobbyRequest = new JoinLobbyRequest
        {
            OperationTypeId = (int)OperationType.JoinLobbyRequest,
            Token = Global.Token
        };
        var data = MessagePackSerializer.Serialize(joinLobbyRequest);
        MasterClient.Instance.SendTcpData(data);
    }

    public void SendCreateLobbyRequest(int maxplayer)
    {
        var createLobbyRequest = new CreateLobbyRequest
        {
            OperationTypeId = (int)OperationType.CreateLobbyRequest,
            MaxPlayers = maxplayer,
            Token = Global.Token,
        };
        var data = MessagePackSerializer.Serialize(createLobbyRequest);
        MasterClient.Instance.SendTcpData(data);
    }

    private void LobbyStartResponse(NetworkStream stream, byte[] data)
    {
        GameStartResponse response = MessagePackSerializer.Deserialize<GameStartResponse>(data);
        Global.LobbyId = response.LobbyID;
        Global.PlayerId = response.PlayerId;
        SendPlayerLobbyInfo();

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().CountThreeSeconds();
        });
    }

    public void SendPlayerLobbyInfo()
    {
        PlayerLobbyInfo playerLobbyInfo = new PlayerLobbyInfo
        {
            OperationTypeId = (int)OperationType.PlayerLobbyInfo,
            PlayerId = Global.PlayerId,
            LobbyId = Global.LobbyId
        };

        var serializedPlayerLobbyInfo = MessagePackSerializer.Serialize(playerLobbyInfo);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                GameClient.Instance._TcpConnection.SendData(serializedPlayerLobbyInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }

    public void SendJoinedLobbyInfo()
    {
        var joinedLobbyInfo = new PlayerJoinedLobbyRequest
        {
            OperationTypeId = (int)OperationType.PlayerJoinedLobbyRequest,
            PlayerID = Global.PlayerId,
            LobbyID = Global.LobbyId,
        };

        var serializedJoinedLobbyInfo = MessagePackSerializer.Serialize(joinedLobbyInfo);
        try
        {
            GameClient.Instance._TcpConnection.SendData(serializedJoinedLobbyInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
}