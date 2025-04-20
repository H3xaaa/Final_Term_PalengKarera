using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles Photon networking, player spawning, and optional intro animation.
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Scene References")]
    public CameraController firstPersonCameraController; // First-person camera controller
    public CameraController thirdPersonCameraController; // Third-person camera controller
    public Transform spawnLocation;
    public GameObject introAnimationObject;
    public float introWaitTime = 2f;

    [Header("Fade Image Before Spawn")]
    public Image fadeImage;
    public float fadeOutDuration = 1f;

    [Header("Objects to Enable BEFORE Spawn")]
    public List<GameObject> objectsToEnableBeforeSpawn;

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
        StartCoroutine(PlayIntroAndSpawn());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");

        if (scene.name == "Gameplay")
        {
            StartCoroutine(PlayIntroAndSpawn());
        }
    }

    private IEnumerator PlayIntroAndSpawn()
    {
        // Show intro if exists
        if (introAnimationObject != null)
        {
            introAnimationObject.SetActive(true);
            Debug.Log("Intro animation started.");
        }

        // Optional wait time before spawning
        yield return new WaitForSeconds(introWaitTime);

        if (introAnimationObject != null)
        {
            introAnimationObject.SetActive(false);
            Debug.Log("Intro animation ended.");
        }

        // Enable any additional pre-spawn objects
        foreach (GameObject obj in objectsToEnableBeforeSpawn)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Spawn the player first
        SpawnPlayer();

        // Fade out AFTER player has been spawned
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color color = fadeImage.color;
            float time = 0f;

            while (time < fadeOutDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, time / fadeOutDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                time += Time.deltaTime;
                yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, 0f);
            fadeImage.gameObject.SetActive(false);
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

        GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, spawnRotation);
        Debug.Log("Player spawned at: " + spawnPosition);

        if (player.GetComponent<PhotonView>().IsMine)
        {
            localPlayer = player;

            // Assign cameras to player
            if (firstPersonCameraController != null && thirdPersonCameraController != null)
            {
                // Assuming you want to switch between cameras after spawning
                firstPersonCameraController.SetTarget(localPlayer.transform);
                thirdPersonCameraController.SetTarget(localPlayer.transform);
                Debug.Log("First-person and third-person cameras assigned to player.");
            }
            else
            {
                Debug.LogError("One or both CameraControllers are not assigned in NetworkManager.");
            }

            // Assign joystick to player
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
