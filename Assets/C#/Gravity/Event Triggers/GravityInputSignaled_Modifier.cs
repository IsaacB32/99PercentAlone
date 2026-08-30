
/// <summary>
/// Reset Directional Gravity to Up when switching to player input 
/// </summary>
public class GravityInputSignaled_Modifier : Modifier<GravityDirectionalTrigger>
{
    public override void Apply() { }
    
    #region Subscribe

    private void OnEnable()
    {
        InputEngine.OnSwitchInputMap += OnSwitchInput;
    }
    
    private void OnDisable()
    {
        InputEngine.OnSwitchInputMap -= OnSwitchInput;
    }

    #endregion

    private void OnSwitchInput(InputMapType mapType)
    {
        if (mapType == InputMapType.Player)
        {
            Target.SetGravityToUp();
        }
    }
}
