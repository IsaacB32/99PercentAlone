using UnityEngine;

public static class Layers
{
    public const int Default = 0;
    public const int TransparentFX = 1;
    public const int IgnoreSCRaycast = 2;
    public const int Wall = 3;
    public const int Water = 4;
    public const int UI = 5;
    public const int Ground = 6;
    public const int GravityTrigger = 7;
    public const int Interaction = 8;

    public static int ToLayerMask(int layer)
    {
        return 1 << layer;
    }
}
