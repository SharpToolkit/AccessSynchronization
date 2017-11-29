using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharpToolkit.AccessSynchronization.Test
{
    class TestResolver : ILockResolver
    {
        private int timeout = 1000;
        private Action<ConcurrentDictionary<int, ThreadLocksTrack>> action;

        public TestResolver()
        {
        }

        public TestResolver(int timeout)
        {
            this.timeout = timeout;
        }

        public TestResolver(Action<ConcurrentDictionary<int, ThreadLocksTrack>> action)
        {
            this.action = action;
        }

        public int Timeout => this.timeout;

        public void Resolve(ConcurrentDictionary<int, ThreadLocksTrack> taken)
        {
            this.action?.Invoke(taken);

            throw new Exception();
        }
    }
}
