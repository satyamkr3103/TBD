using System;
using System.Collections.Generic;

[Serializable]
public class AIModificationRequest
{
    public float totalEnergyCost; // The cost for the entire batch
    public List<SingleModificationRequest> items; // Can hold 1 or 100 items
}

[Serializable]
public class SingleModificationRequest
{
    public string action; // "place", "modify", or "remove"
    public string requestedSpawnName; // Custom prefab name
    public string shape; // "cube", "sphere", "cylinder"
    public PositionData position; 
    public string material;
    public string color;
    public float bounciness;
    public float scaleMultiplier;
    public bool isLadder; // Tells the game this object can be climbed
    public bool isBouncy; // True if it's a spring/trampoline
    public bool isHazard; // True if it's a spike/lava/danger
    public bool isConveyor; // True if it's a conveyor belt or speed boost
    public bool isWater; // True if it's water, liquid, or floating zone
}

[Serializable]
public class PositionData
{
    public float x;
    public float y;
}
