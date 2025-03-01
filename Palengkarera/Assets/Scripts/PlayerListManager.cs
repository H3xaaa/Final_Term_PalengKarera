using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListManager : MonoBehaviourPunCallbacks
{
    public TMP_Text playerListText;

    void UpdatePlayerList()
    {
        playerListText.text = "Players in Lobby:\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
}
