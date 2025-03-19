using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trait 
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Trait(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
