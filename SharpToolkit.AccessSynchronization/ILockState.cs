using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.AccessSynchronization
{
    public interface ILockState { }

    internal interface ILockStateInternal : ILockState
    {
        TResult Unlock<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn);
        TResult Unlock<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn);
        TResult UnlockUpgradeable<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn);
        TResult UnlockUpgradeable<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn);
        TResult UnlockExclusive<TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, Func<TResult> fn);
        TResult UnlockExclusive<T, TResult>(LinkedList<ILockStateInternal> lockList, IObjectLock @lock, T obj, Func<T, TResult> fn);
    }
}
