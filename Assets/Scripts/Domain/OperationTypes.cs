public enum OperationType
{
    // Authentication Operations
    LoginRequest = 100,
    LoginResponse = 101,
    LogoutRequest = 102,
    LogoutResponse = 103,
    AuthenticationResponse = 104,
    SignUpRequest = 105,
    SignUpResponse = 106,
    // Matchmaking Operations
    JoinLobbyRequest = 200,
    JoinLobbyResponse = 201,
    LeaveLobbyRequest = 202,
    MatchmakingResponse = 203,
    CreateLobbyRequest = 204,
    CreateLobbyResponse = 205,
    NotifyGameStart = 206,

    PlayerPositionUpdate = 90,
    StartGameSession = 92,
    HeartbeatPing = 94,
    // Lobby Management
    PlayerLobbyInfo = 96,
    PlayerLobbyInfoResponse = 98,

    UpdateLobbyStatus = 300,
    StartGameCountdown = 301,

    // Server Management
    ServerStatusUpdate = 400,
    AllocateGameServer = 401,
    DisconnectedPlayerResponse = 402,

    // Heartbeat and Monitoring
    HeartbeatResponse = 501,

    // Multi-Server Operations
    ServerTransferRequest = 600,
    ServerTransferResponse = 601,
    EndPoint = 602,
    GameOverRequest = 603,
    GameOverResponse = 604,
    GetTopLeaderboardEntries = 456,
    GetTopLeaderboardEntriesResponse = 457,
    

    PlayerJoinedLobbyRequest = 1001,
    GameEndInfo = 1002,

    // Other Operations
    CustomOperation = 700 // For future expansion or custom operations
}
