using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private PlayerManager _playerManager;
    public TextMeshProUGUI CountDownText;
    public bool IsGamePlaying { get; set; }

    private PlayerMovement _playerMovement;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void CountThreeSeconds()
    {
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        CountDownText.gameObject.SetActive(true);
        CountDownText.text = "3";
        yield return new WaitForSeconds(1f);
        CountDownText.text = "2";
        yield return new WaitForSeconds(1f);
        CountDownText.text = "1";
        yield return new WaitForSeconds(1f);
        CountDownText.text = "GO!";
        IsGamePlaying = true;
        _playerMovement.IsGamePlaying = true;
        yield return new WaitForSeconds(1f);
        CountDownText.gameObject.SetActive(false);
    }

    void OnApplicationQuit()
    {
        if (Global.LobbyId != null && Global.PlayerId != -1)
            GameClient.Instance._TcpConnection.SendLobbyDisconnectRequest();
    }

    public void ResetGame()
    {
        IsGamePlaying = false;
        _playerMovement.IsGamePlaying = false;
        _playerMovement.ResetPosition();
        CountDownText.gameObject.SetActive(true);
        CountDownText.text = "Waiting Other Players To Start";
        _playerManager.ResetPlayers();
    }

    public void EndGame()
    {
        IsGamePlaying = false;
        _playerMovement.IsGamePlaying = false;
        ResetPlayerMoveInput();
    }

    public void ResetPlayerMoveInput()
    {
        _playerMovement.ResetMoveInput();
    }

}