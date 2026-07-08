
public interface IInputState
{
    private void GravityMovement() {}
    private void WeightlessMovement() {}
    private void MenuMovement() {}
}

public enum PlayerInputState
{
    Menu,
    Gravity,
    Weightless
} 
