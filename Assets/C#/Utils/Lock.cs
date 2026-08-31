using System.Collections.Generic;
using ITween;

/// <summary>
/// Lock for stopping services as needed
/// </summary>
public class Lock
{
    private HashSet<object> _lockHolders = new HashSet<object>();

    public void RegisterLockHolder(object owner)
    {
        _lockHolders.Add(owner);
    }

    public void UnregisterLockHolder(object owner)
    {
        _lockHolders.Remove(owner);
    }

    public void LockUntilNextFrame(object owner)
    {
        RegisterLockHolder(owner);
        UnlockNextFrame(owner);
    }
    
    public void UnlockNextFrame(object owner)
    {
        Delay.WaitForNextFrame(() =>
        {
            UnregisterLockHolder(owner);
        });
    }

    public static implicit operator bool(Lock l) => l._lockHolders.Count > 0;
}
