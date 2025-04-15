using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint; // Set this in the Inspector
    private GameObject spawnedPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            Vector3 spawnPosition;
            Quaternion spawnRotation;

            if (spawnPoint != null)
            {
                spawnPosition = spawnPoint.position;
                spawnRotation = spawnPoint.rotation;
            }
            else
            {
                spawnPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
                spawnRotation = Quaternion.identity;
            }

            spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
            Debug.Log("Spawned player at: " + spawnPosition);

            // Assign joystick to the PlayerController
            PlayerController playerController = spawnedPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                GameObject uiCanvas = GameObject.Find("UI_Canvas");
                if (uiCanvas)
                {
                    Transform inGamePanel = uiCanvas.transform.Find("In-Game");
                    if (inGamePanel)
                    {
                        MobileJoystick joystick = inGamePanel.transform.Find("Joystick")?.GetComponent<MobileJoystick>();
                        playerController.SetJoystick(joystick);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Player Prefab is not assigned in Spawner.");
        }
    }
}
