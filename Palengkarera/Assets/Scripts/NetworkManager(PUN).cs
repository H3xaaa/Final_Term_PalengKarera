using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public CameraController cameraController; // Assign in Inspector
    public Transform spawnLocation;         // Set in Inspector for player spawn position
    private GameObject localPlayer;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene changes
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
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (spawnLocation != null)
        {
            spawnPosition = spawnLocation.position;
            spawnRotation = spawnLocation.rotation;
        }
        else
        {
            spawnPosition = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            spawnRotation = Quaternion.identity;
        }

        // Instantiate the player using PhotonNetwork
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, spawnRotation);
        Debug.Log("Player spawned at: " + spawnPosition);

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
