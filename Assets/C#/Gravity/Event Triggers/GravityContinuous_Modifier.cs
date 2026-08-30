using System;
using UnityEngine;

//=!= IDEA ONLY =!=
// see Modifier.cs

/// <summary>
/// Force Directional Gravity to be recalculated every frame
/// </summary>
public class GravityContinuous_Modifier : Modifier<GravityDirectionalTrigger>
{
    public override void Apply()
    {
        //check that it doesn't break anything ??
    }

    private void Update()
    {
        Target.SetGravityToUp();
    }
}
