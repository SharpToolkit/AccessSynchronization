namespace SharpToolkit.AccessSynchronization
{
    internal interface IObjectLock
    {
        bool IsExclusivelyLocked { get; }
        bool IsShareLocked { get; }
        bool IsUpgradeableLocked { get; }

        void TakeExclusiveLock(ILockStateInternal state);
        void TakeSharedLock(ILockStateInternal state);
        void TakeUpgradeableLock(ILockStateInternal state);

        void ExitExclusiveLock();
        void ExitSharedLock();
        void ExitUpgradeableLock();
    }
}