using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI roomCodeText; // Assign TMP Text to display room code
    public TMP_InputField nameInputField; // Assign TMP InputField for player name
    public TMP_InputField joinCodeInput; // Assign TMP InputField to enter room code
    public List<TextMeshProUGUI> playerNameTexts; // Assign 5 TMP Texts for player slots
    public GameObject startButton; // Assign "Start" button (visible only for host)

    private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();

    void Start()
    {
        startButton.SetActive(false); // Hide Start button initially
    }

    public void CreateLobby()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Please enter a name before creating a lobby!");
            return;
        }

        PhotonNetwork.NickName = playerName; // Set player's nickname
        string roomCode = GenerateRoomCode(); // Generate a 6-character code
        roomCodeText.text = roomCode; // Update the UI to display the generated code

        RoomOptions options = new RoomOptions { MaxPlayers = 5 };
        PhotonNetwork.CreateRoom(roomCode, options);
    }

    public void JoinLobby()
    {
        string playerName = nameInputField.text.Trim();
        string enteredCode = joinCodeInput.text.ToUpper();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Please enter a name before joining a lobby!");
            return;
        }
        if (enteredCode.Length != 6)
        {
            Debug.Log("Invalid room code!");
            return;
        }

        PhotonNetwork.NickName = playerName; // Set player's nickname
        PhotonNetwork.JoinRoom(enteredCode);
    }

    public override void OnJoinedRoom()
    {
        roomCodeText.text = "Lobby Code: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        startButton.SetActive(PhotonNetwork.IsMasterClient); // Show Start button only for host
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
        for (int i = 0; i < playerNameTexts.Count; i++)
        {
            if (i < players.Length)
            {
                string playerName = players[i].NickName;
                bool isReady = playerReadyStatus.ContainsKey(players[i].ActorNumber) && playerReadyStatus[players[i].ActorNumber];
                playerNameTexts[i].text = playerName + (isReady ? " (Ready)" : "");
            }
            else
            {
                playerNameTexts[i].text = "Waiting...";
            }
        }
    }

    public void ToggleReady()
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        bool isReady = playerReadyStatus.ContainsKey(playerID) && playerReadyStatus[playerID];
        playerReadyStatus[playerID] = !isReady;
        photonView.RPC("UpdateReadyStatus", RpcTarget.All, playerID, !isReady);
    }

    [PunRPC]
    void UpdateReadyStatus(int playerID, bool ready)
    {
        playerReadyStatus[playerID] = ready;
        UpdatePlayerList();
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && AllPlayersReady())
        {
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }

    bool AllPlayersReady()
    {
        foreach (var ready in playerReadyStatus.Values)
        {
            if (!ready) return false;
        }
        return true;
    }

    string GenerateRoomCode()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] codeArray = new char[6];
        for (int i = 0; i < 6; i++)
        {
            codeArray[i] = characters[Random.Range(0, characters.Length)];
        }
        return new string(codeArray);
    }
}
