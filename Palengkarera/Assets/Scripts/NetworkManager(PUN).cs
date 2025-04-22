using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles Photon networking, scene loading, intro animation, fade, and player spawning.
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Scene References")]
    public CameraController firstPersonCameraController;
    public CameraController thirdPersonCameraController;
    public Transform spawnLocation;
    public GameObject introAnimationObject;
    public float introWaitTime = 2f;

    [Header("Fade Image Before Spawn")]
    public Image fadeImage;
    public float fadeOutDuration = 1f;

    [Header("Objects to Enable BEFORE Spawn")]
    public List<GameObject> objectsToEnableBeforeSpawn;

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public Transform customSpawnPoint;

    private GameObject localPlayer;

    private void Start()
    {
        Debug.Log("NetworkManager Start method called.");
        StartCoroutine(PlayIntroAndSpawn());

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
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");

        if (scene.name == "Gameplay")
        {
            Debug.Log("Starting intro and spawning...");
            StartCoroutine(PlayIntroAndSpawn());
        }
    }

    private IEnumerator PlayIntroAndSpawn()
    {
        Debug.Log("Intro animation started.");
        if (introAnimationObject != null)
        {
            introAnimationObject.SetActive(true);
        }

        yield return new WaitForSeconds(introWaitTime);

        if (introAnimationObject != null)
        {
            introAnimationObject.SetActive(false);
        }

        foreach (GameObject obj in objectsToEnableBeforeSpawn)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Enabled: {obj.name}");
            }
        }

        SpawnPlayer();

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
                yield return null; // Smooth fade
            }

            fadeImage.color = new Color(color.r, color.g, color.b, 0f);
            fadeImage.gameObject.SetActive(false);
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned.");
            return;
        }

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (customSpawnPoint != null)
        {
            spawnPosition = customSpawnPoint.position;
            spawnRotation = customSpawnPoint.rotation;
        }
        else if (spawnLocation != null)
        {
            spawnPosition = spawnLocation.position;
            spawnRotation = spawnLocation.rotation;
        }
        else
        {
            spawnPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
            spawnRotation = Quaternion.identity;
        }

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
        Debug.Log("Player spawned at: " + spawnPosition);

        if (player.GetComponent<PhotonView>().IsMine)
        {
            localPlayer = player;

            if (firstPersonCameraController != null)
                firstPersonCameraController.SetTarget(localPlayer.transform);

            if (thirdPersonCameraController != null)
                thirdPersonCameraController.SetTarget(localPlayer.transform);

            GameObject uiCanvas = GameObject.Find("UI_Canvas");
            if (uiCanvas)
            {
                Transform inGamePanel = uiCanvas.transform.Find("In-Game");
                if (inGamePanel)
                {
                    MobileJoystick joystick = inGamePanel.transform.Find("Joystick")?.GetComponent<MobileJoystick>();
                    if (joystick != null)
                    {
                        PlayerController playerController = localPlayer.GetComponent<PlayerController>();
                        if (playerController != null && playerController.GetComponent<MobileJoystick>() == null)
                        {
                            playerController.SetJoystick(joystick);
                            Debug.Log("Joystick assigned to player.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Joystick not found in In-Game panel.");
                    }
                }
                else
                {
                    Debug.LogWarning("In-Game panel not found under UI_Canvas.");
                }
            }
            else
            {
                Debug.LogWarning("UI_Canvas not found.");
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
