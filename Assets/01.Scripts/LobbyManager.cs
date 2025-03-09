using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] Button lobbyButton;
    private void Awake()
    {
        lobbyButton.onClick.AddListener(GameStart);
    }

    private void GameStart()
    {
        SceneManager.LoadScene("MainScene");
    }
}
