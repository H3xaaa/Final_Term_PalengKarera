using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public CameraController cameraController; // 👈 Assign this in the Inspector!

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), Quaternion.identity);

        if (player.GetComponent<PhotonView>().IsMine) // Ensure it's the local player
        {
            cameraController.SetTarget(player.transform); // 👈 Attach camera to the spawned player
        }
    }
}
