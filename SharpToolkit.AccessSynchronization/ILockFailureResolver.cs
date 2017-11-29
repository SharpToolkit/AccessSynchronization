using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.AccessSynchronization
{
    public interface ILockResolver
    {
        int Timeout { get; }

        void Resolve(ConcurrentDictionary<int, ThreadLocksTrack> taken);
    }

    public class NullLockFailureResolver : ILockResolver
    {
        public NullLockFailureResolver()
        {
        }

        public int Timeout => int.MaxValue;

        public void Resolve(ConcurrentDictionary<int, ThreadLocksTrack> taken)
        {
            
        }
    }
}
