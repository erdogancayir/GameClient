using System;
using MessagePack;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _authPanel;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _gameEndInfoPanel;
    [SerializeField] private GameObject _createLobbyPanel;
    [SerializeField] private GameObject _gameoverPanel;
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private GameObject _leaderboardArea;
    [SerializeField] private GameObject _leaderboardCard;
    [SerializeField] private TMP_InputField _username;
    [SerializeField] private TMP_InputField _password;
    [SerializeField] public TMP_Text infoText;
    [SerializeField] public TMP_Text winnerInfoText;
    [SerializeField] private TMP_InputField _maxplayer;
    [SerializeField] private TMP_Text _gameResultText;
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LoginButton()
    {
        MasterClient.Instance.masterClientHandler.SendLoginRequest(_username.text, _password.text);
    }
    
    public void RegisterButton()
    {
        MasterClient.Instance.masterClientHandler.SendRegisterRequest(_username.text, _password.text);
    }
    
    public void JoinLobbyButton()
    {
        MasterClient.Instance.masterClientHandler.SendJoinLobbyRequest();
    }

    public void CreateLobbyButton()
    {
        int maxplayer;
        if (_maxplayer.text == "")
        {
            maxplayer = 2;
        }
        else
            maxplayer = int.Parse(_maxplayer.text);
        if (maxplayer < 2)
            maxplayer = 2;
        

        MasterClient.Instance.masterClientHandler.SendCreateLobbyRequest(maxplayer);
    }

    public void GoToCreateLobby()
    {
        _lobbyPanel.SetActive(false);
        _createLobbyPanel.SetActive(true);
    }
    
    public void LoginSuccess()
    {
        _authPanel.SetActive(false);
        _lobbyPanel.SetActive(true);
    }

    public void JoinLobbySuccess()
    {
        _mainPanel.SetActive(false);
    }

    public void MenuButton()
    {
        _authPanel.SetActive(false);
        _gameoverPanel.SetActive(false);
        _createLobbyPanel.SetActive(false);
        _mainPanel.SetActive(true);
        _lobbyPanel.SetActive(true);
        _gameEndInfoPanel.SetActive(false);
        GameManager.Instance.ResetGame();
    }

    public void GameOver(int winnerPlayerId)
    {
        bool isWinner = winnerPlayerId == Global.PlayerId;
        _gameoverPanel.SetActive(true);
        if (isWinner)
        {
            _gameResultText.color = Color.green;
            _gameResultText.text = "You Win!";
        }
        else
        {
            _gameResultText.color = Color.red;
            _gameResultText.text = "You Lose!";
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().IsGamePlaying = false;
    }

    public void GameEndInfo(string username)
    {
        _gameEndInfoPanel.SetActive(true);
        GameManager.Instance.EndGame();
        winnerInfoText.text = username + " won the game!";
    }

    public void OpenLeaderBoard()
    {
        _leaderboardPanel.SetActive(true);
        MasterClient.Instance.masterClientHandler.GetLeaderBoard();
    }

    public void LeaderBoard(List<GetTopLeaderboardResponsePack> list)
    {
        foreach (Transform child in _leaderboardArea.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in list)
        {
            var card = Instantiate(_leaderboardCard, _leaderboardArea.transform);
            card.transform.GetChild(0).GetComponent<TMP_Text>().text = item.Username;
            card.transform.GetChild(1).GetComponent<TMP_Text>().text = item.TotalPoints.ToString();
        }
    }

    public void GoLeaderboardToMenu()
    {
        _leaderboardPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }
}