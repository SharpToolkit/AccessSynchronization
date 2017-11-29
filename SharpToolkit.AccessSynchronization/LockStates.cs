using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.AccessSynchronization
{
    public class Locked : ILockStateInternal
    {
        TResult ILockStateInternal.Unlock<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            var state = new SharedUnlocked();

            lockList.AddLast(state);

                @lock.TakeSharedLock(state);

                    var r = fn();

                @lock.ExitSharedLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.Unlock<T, TResult>(
            LinkedList<ILockStateInternal> lockList,
            IObjectLock @lock, 
            T obj, 
            Func<T, TResult> fn)
        {
            var state = new SharedUnlocked();

            lockList.AddLast(state);

                @lock.TakeSharedLock(state);

                    var r = fn(obj);

                @lock.ExitSharedLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.UnlockUpgradeable<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            var state = new UpgradeableUnlocked();

            lockList.AddLast(state);

                @lock.TakeUpgradeableLock(state);

                    var r = fn();

                @lock.ExitUpgradeableLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.UnlockUpgradeable<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            var state = new UpgradeableUnlocked();

            lockList.AddLast(state);

                @lock.TakeUpgradeableLock(state);

                    var r = fn(obj);

                @lock.ExitUpgradeableLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.UnlockExclusive<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            var state = new ExclusiveUnlocked();

            lockList.AddLast(state);

                @lock.TakeExclusiveLock(state);

                    var r = fn();

                @lock.ExitExclusiveLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.UnlockExclusive<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            var state = new ExclusiveUnlocked();

            lockList.AddLast(state);

                @lock.TakeExclusiveLock(state);

                    var r = fn(obj);

                @lock.ExitExclusiveLock();

            lockList.RemoveLast();

            return r;
        }

        public override string ToString()
        {
            return "Locked";
        }
    }

    public class ExclusiveUnlocked : ILockStateInternal
    {
        TResult ILockStateInternal.Unlock<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            return fn();
        }

        TResult ILockStateInternal.Unlock<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            return fn(obj);
        }

        TResult ILockStateInternal.UnlockUpgradeable<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            return fn();
        }

        TResult ILockStateInternal.UnlockUpgradeable<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            return fn(obj);
        }

        TResult ILockStateInternal.UnlockExclusive<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            return fn();
        }

        TResult ILockStateInternal.UnlockExclusive<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            return fn(obj);
        }

        public override string ToString()
        {
            return "Exclusive";
        }
    }

    public class SharedUnlocked : ILockStateInternal
    {
        TResult ILockStateInternal.Unlock<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            return fn();
        }

        TResult ILockStateInternal.Unlock<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            return fn(obj);
        }

        TResult ILockStateInternal.UnlockUpgradeable<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            var state = new UpgradeableUnlocked();


            lockList.AddLast(state);

                @lock.TakeUpgradeableLock(state);

                    var r = fn();

                @lock.ExitUpgradeableLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.UnlockUpgradeable<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            var state = new UpgradeableUnlocked();

            lockList.AddLast(state);

                @lock.TakeUpgradeableLock(state);

                    var r = fn(obj);

                @lock.ExitUpgradeableLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.UnlockExclusive<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            // Will throw LockRecursionException
            @lock.TakeExclusiveLock(new ExclusiveUnlocked());

            return fn();
        }

        TResult ILockStateInternal.UnlockExclusive<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            // Will throw LockRecursionException
            @lock.TakeExclusiveLock(new ExclusiveUnlocked());

            return fn(obj);
        }

        public override string ToString()
        {
            return "Shared";
        }
    }

    public class UpgradeableUnlocked : ILockStateInternal
    {
        TResult ILockStateInternal.Unlock<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            return fn();
        }

        TResult ILockStateInternal.Unlock<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            return fn(obj);
        }

        TResult ILockStateInternal.UnlockUpgradeable<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            return fn();
        }

        TResult ILockStateInternal.UnlockUpgradeable<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            return fn(obj);
        }

        TResult ILockStateInternal.UnlockExclusive<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn)
        {
            var state = new ExclusiveUnlocked();

            lockList.AddLast(state);

                @lock.TakeExclusiveLock(state);

                    var r = fn();

                @lock.ExitExclusiveLock();

            lockList.RemoveLast();

            return r;
        }

        TResult ILockStateInternal.UnlockExclusive<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn)
        {
            var state = new ExclusiveUnlocked();

            lockList.AddLast(state);

                @lock.TakeExclusiveLock(state);

                    var r = fn(obj);

                @lock.ExitExclusiveLock();

            lockList.RemoveLast();

            return r;
        }

        public override string ToString()
        {
            return "Upgradeable";
        }
    }
}
