using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles Photon networking, player spawning, and optional intro animation.
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Scene References")]
    public CameraController cameraController; // Camera that follows the player
    public Transform spawnLocation; // Optional: where the player will spawn
    public GameObject introAnimationObject; // Object to show during intro
    public float introWaitTime = 2f; // Time to wait before spawning player

    private GameObject localPlayer;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(
            "TestRoom",
            new Photon.Realtime.RoomOptions { MaxPlayers = 4 },
            null
        );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
        StartCoroutine(PlayIntroAndSpawn()); // Play intro before spawning
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");

        if (scene.name == "Gameplay")
        {
            StartCoroutine(PlayIntroAndSpawn()); // Play intro again on scene load
        }
    }

    /// <summary>
    /// Plays an optional intro animation, waits, then spawns the player.
    /// </summary>
    private IEnumerator PlayIntroAndSpawn()
    {
        // Enable intro animation if assigned
        if (introAnimationObject != null)
        {
            introAnimationObject.SetActive(true);
            Debug.Log("Intro animation started.");
        }

        // Wait for the defined intro time
        yield return new WaitForSeconds(introWaitTime);

        // Disable the intro animation after wait (optional)
        if (introAnimationObject != null)
        {
            introAnimationObject.SetActive(false);
            Debug.Log("Intro animation ended.");
        }

        // Proceed with spawning the player
        SpawnPlayer();
    }

    /// <summary>
    /// Spawns the networked player prefab and sets up camera and joystick.
    /// </summary>
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
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
