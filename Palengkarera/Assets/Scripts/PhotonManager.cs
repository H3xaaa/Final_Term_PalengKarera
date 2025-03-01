using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon
        Debug.Log("Connecting to Photon...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinLobby(); // Join a lobby after connecting
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby! Waiting for player input...");
        // No longer auto-joining a room, wait for UI interaction
    }

    public void CreateOrJoinRoom() // Call this when the player presses "Start Game"
    {
        PhotonNetwork.JoinRandomRoom(); // Try to join a random room
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms available, creating a new room...");
        PhotonNetwork.CreateRoom("Room_" + Random.Range(1000, 9999), new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room! Waiting for the host to start...");

        // Only the host (first player in room) can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are the host! Press Start to begin.");
        }
    }

    public void StartGame() // Call this when the host presses "Start Game"
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }
}
