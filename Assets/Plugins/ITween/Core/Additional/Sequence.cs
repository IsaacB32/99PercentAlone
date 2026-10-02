using System;
using System.Collections;

namespace ITween
{
    using Internal;
    
    public class Sequence : ITween, IDisposable
    {
        public int IDKey { get; }
        
        public bool IsAlive { get; }
        public bool IsRunning { get; }
        public bool IsPaused { get; }
        
        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Kill(bool ignoreFlags)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}
