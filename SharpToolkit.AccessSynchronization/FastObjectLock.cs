using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SharpToolkit.AccessSynchronization
{
    internal sealed class FastObjectLock : IObjectLock
    {
        private readonly ReaderWriterLockSlim @lock;

        public FastObjectLock()
        {
            this.@lock = new ReaderWriterLockSlim();
        }

        public bool IsShareLocked       => this.@lock.IsReadLockHeld;
        public bool IsExclusivelyLocked => this.@lock.IsWriteLockHeld;
        public bool IsUpgradeableLocked => this.@lock.IsUpgradeableReadLockHeld;

        public void TakeSharedLock(ILockStateInternal state) => 
            this.@lock.EnterReadLock();

        public void ExitSharedLock()  => this.@lock.ExitReadLock();

        public void TakeExclusiveLock(ILockStateInternal state) => 
            this.@lock.EnterWriteLock();

        public void ExitExclusiveLock()  => this.@lock.ExitWriteLock();

        public void TakeUpgradeableLock(ILockStateInternal state) => 
            this.@lock.EnterUpgradeableReadLock();

        public void ExitUpgradeableLock()  => this.@lock.ExitUpgradeableReadLock();
    }
}
