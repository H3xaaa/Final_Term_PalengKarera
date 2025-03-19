using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    public enum BuffType { SpeedBuff, SpeedDebuff, StaminaRecovery, StaminaDeplete}
    public BuffType buffType;
    public float duration = 20f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                Buff newBuff = CreateBuff(buffType, duration);
                if (player != null)
                {
                    if (newBuff != null)
                    {
                        player.ApplyBuff(newBuff);
                    }
                }

                Destroy(gameObject);
            }
        }
    }
    Buff CreateBuff(BuffType type, float duration)
    {
        switch (type)
        {
            case BuffType.SpeedBuff:
                return new Buff("Speed Buff", duration,
                    player => player.normalSpeed += 10f,
                    player => player.normalSpeed -= 10f);

            case BuffType.SpeedDebuff:
                return new Buff("Speed Debuff", duration,
                    player => player.normalSpeed -= 10f,
                    player => player.normalSpeed += 10f);
            
            case BuffType.StaminaRecovery:
                return new Buff("Stamina Recovery", float.PositiveInfinity,
                    player => player.staminaMax = 100f,
                    player => { });
            
            case BuffType.StaminaDeplete:
                return new Buff("Stamina Deplete", float.PositiveInfinity,
                    player => player.staminaMax = 0,
                    player => { });

            default:
                return null;
        }
    }
}
