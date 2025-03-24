using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int balance = 300;
    public Text balanceText;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateBalanceUI();
    }

    public void UpdateBalanceUI()
    {
        balanceText.text = "₱" + balance.ToString();
    }
}
