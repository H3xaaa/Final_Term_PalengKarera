using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NameInput : MonoBehaviourPunCallbacks
{
    [Header("Name Input UI")]
    public TMP_InputField nameInputField;
    public TMP_Text displayText;

    [Header("Lobby UI")]
    public TMP_InputField input_Join;
    public TextMeshProUGUI roomCodeText;
    public List<TextMeshProUGUI> playerSlots;
    public GameObject startGameButton;

    private void Start()
    {
        startGameButton.SetActive(false);
        nameInputField.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(string newText)
    {
        displayText.text = newText + "'s Lobby";
    }

    public void SaveName()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            PhotonNetwork.NickName = nameInputField.text; // Assign player name
            Debug.Log("Player Name Set: " + PhotonNetwork.NickName);
        }
    }

    public void CreateRoom()
    {
        string roomCode = GenerateRoomCode();
        roomCodeText.text = "Room Code: " + roomCode;

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 5 };
        PhotonNetwork.CreateRoom(roomCode, roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(input_Join.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        UpdatePlayerList();
        CheckHost();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i < players.Length)
                playerSlots[i].text = players[i].NickName;
            else
                playerSlots[i].text = "Waiting...";
        }
    }

    void CheckHost()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Gameplay");
        }
    }

    string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] codeArray = new char[6];
        for (int i = 0; i < 6; i++)
        {
            codeArray[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(codeArray);
    }
}
