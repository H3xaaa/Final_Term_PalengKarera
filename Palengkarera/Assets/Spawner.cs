using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject spawnedPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
            spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

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
