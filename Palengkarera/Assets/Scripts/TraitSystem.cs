using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitSystem : MonoBehaviour
{
    public static List<Trait> inGameTrait = new List<Trait>
    {
        new Trait("Athletic", "Players gains 20% movement speed."),
        new Trait("Locked In", "Players reduced stamina consumption.")
    };
}
