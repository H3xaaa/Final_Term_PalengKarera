using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Buff 
{
    public string name;
    public float duration;
    private Action<PlayerController> applyEffect;
    private Action<PlayerController> removeEffect;

    public Buff(string name, float duration, 
        Action<PlayerController> applyEffect, 
        Action<PlayerController> removeEffect)
    {
        this.name = name;
        this.duration = duration;
        this.applyEffect = applyEffect;
        this.removeEffect = removeEffect;
    }

    public void Apply(PlayerController player)
    {
        applyEffect(player);
        string durationText = float.IsInfinity(duration) ? "" : $" Duration: {duration}s";
        Debug.Log($"{name} Applied.{durationText}");
    }
    public void Remove(PlayerController player)
    {
        removeEffect(player);
        Debug.Log($"{name} Expired.");
    }
}
