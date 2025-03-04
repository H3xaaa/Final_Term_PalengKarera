using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public CameraController cameraController; // Assign in Inspector
    private GameObject localPlayer;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene change
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");
        if (scene.name == "Gameplay") // Ensure it runs only in gameplay scene
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), Quaternion.identity);

        if (player.GetComponent<PhotonView>().IsMine)
        {
            localPlayer = player;

            // Assign camera
            if (cameraController != null)
            {
                cameraController.SetTarget(localPlayer.transform);
                Debug.Log("Camera assigned to player.");
            }
            else
            {
                Debug.LogError("CameraController is not assigned in NetworkManager.");
            }

            // Assign Joystick Controls
            MobileJoystick joystick = FindObjectOfType<MobileJoystick>();
            if (joystick != null)
            {
                localPlayer.GetComponent<PlayerController>().SetJoystick(joystick);
                Debug.Log("Joystick assigned to player.");
            }
            else
            {
                Debug.LogError("No MobileJoystick found in scene.");
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe when destroyed
    }
}
