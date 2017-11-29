using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.AccessSynchronization
{
    // TODO: add serialization
    public class DeadlockException : Exception
    {
        private readonly int threadA;
        private readonly object intendedObjectA;
        private readonly ILockState intendedStateA;
        private readonly ILockState holdingStateA;
        private readonly int threadB;
        private readonly object intendedObjectB;
        private readonly ILockState intendedStateB;
        private readonly ILockState holdingStateB;

        public DeadlockException(
            int threadA, 
            object intendedObjectA,
            ILockState intendedStateA,
            ILockState holdingStateA,
            int threadB,
            object intendedObjectB,
            ILockState intendedStateB,
            ILockState holdingStateB) : base("A deadlock has occured.")
        {
            this.threadA = threadA;
            this.intendedObjectA = intendedObjectA;
            this.intendedStateA = intendedStateA;
            this.holdingStateA = holdingStateA;
            this.threadB = threadB;
            this.intendedObjectB = intendedObjectB;
            this.intendedStateB = intendedStateB;
            this.holdingStateB = holdingStateB;
        }

        public override string ToString()
        {
            var part1 = $"{this.Message}\n";
            var part2 = $"Thread {this.threadA} is trying to unlock object {this.intendedObjectA} in {this.intendedStateA} state, while holding {this.intendedObjectB} in {this.holdingStateA} state.\n";
            var part3 = $"Thread {this.threadB} is trying to unlock object {this.intendedObjectB} in {this.intendedStateB} state, while holding {this.intendedObjectA} in {this.holdingStateB} state.";

            return part1 + part2 + part3;
        }
    }
}
