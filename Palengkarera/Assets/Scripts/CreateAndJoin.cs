using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    [Header("Player Name Input")]
    public TMP_InputField nameInputField;
    public TMP_Text displayText;

    [Header("Lobby UI")]
    public TMP_InputField roomNameInputField; // Custom room name input
    public TMP_InputField joinRoomInputField;
    public TextMeshProUGUI roomNameText;
    public List<TextMeshProUGUI> playerSlots; // UI Slots for player names
    public GameObject startGameButton;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Ensure player is connected to Photon
        PhotonNetwork.AutomaticallySyncScene = true; // Enable automatic scene sync
        startGameButton.SetActive(false);
        nameInputField.onValueChanged.AddListener(UpdateLobbyDisplay);
    }

    void UpdateLobbyDisplay(string newText)
    {
        displayText.text = newText + "'s Lobby";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("Connected to Master Server.");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Photon Lobby.");
    }

    public void SetPlayerName()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            PhotonNetwork.NickName = nameInputField.text;
            Debug.Log("Player Name Set: " + PhotonNetwork.NickName);
        }
    }

    public void CreateRoom()
    {
        SetPlayerName(); // Ensure player name is set

        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            Debug.LogWarning("Room name is empty! Please enter a room name.");
            return;
        }

        string roomName = roomNameInputField.text;
        roomNameText.text = "Room Name: " + roomName;

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 5 };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom()
    {
        SetPlayerName(); // Ensure player name is set

        if (string.IsNullOrEmpty(joinRoomInputField.text))
        {
            Debug.LogWarning("Room name is empty! Please enter a room name to join.");
            return;
        }

        PhotonNetwork.JoinRoom(joinRoomInputField.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        // Update room name for non-host players
        roomNameText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerList();
        CheckHost();

        // Inform other players that someone joined
        photonView.RPC("SyncLobbyUI", RpcTarget.OthersBuffered);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
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
                playerSlots[i].text = (i == 0) ? players[i].NickName + " (Host)" : players[i].NickName; // Mark Host
            else
                playerSlots[i].text = "Waiting...";
        }
    }

    void CheckHost()
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("LoadGameplayScene", RpcTarget.All);
        }
    }

    [PunRPC]
    void LoadGameplayScene()
    {
        PhotonNetwork.LoadLevel("Gameplay"); // Load scene for all players
    }

    [PunRPC]
    void SyncLobbyUI()
    {
        UpdatePlayerList(); // Ensure all players update their UI when someone joins
    }
}
