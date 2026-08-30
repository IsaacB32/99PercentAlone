
/// <summary>
/// Interface for animation callsbacks
/// </summary>
public interface ICallbacks
{
    /// <summary>
    /// Callback right before the animation is started
    /// </summary>
    void OnBeforeAnimate() {}

    /// <summary>
    /// Callback upon finishing the animation but before completing it
    /// </summary>
    void OnAfterAnimate() {}

    /// <summary>
    /// Callback once the animation is complete
    /// </summary>
    void OnAnimateComplete() {}
}