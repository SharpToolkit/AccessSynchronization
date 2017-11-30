using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SharpToolkit.AccessSynchronization
{
    public abstract class LockedObject
    {
        internal LockedObject() { }

        public abstract TResult Unlock           <TResult>(Func<TResult> fn);
        public abstract TResult UnlockUpgradeable<TResult>(Func<TResult> fn);
        public abstract TResult UnlockExclusive  <TResult>(Func<TResult> fn);
    }
        
    /// <summary>
    /// Represents an object in a locked state.
    /// </summary>
    /// <remarks>
    /// The semantic meaning of the <see cref="Locked{T}"/> class name is
    /// different from the 'lock' keyword meaning. While the object
    /// is locked it can't be accessed. It needs to be unlocked in order
    /// to be accessed.
    /// </remarks>
    /// <typeparam name="T">The type of the locked object.</typeparam>
    public sealed class Locked<T> : LockedObject
    {
        private readonly ThreadLocal<LinkedList<ILockStateInternal>> lockList;
        internal IObjectLock @lock;
        
        // convert to array
        private IEnumerable<LockedObject> relatives { get; }

        private T model { get; }


        public bool IsShareUnlocked       => this.@lock.IsShareLocked;
        public bool IsExclusevelyUnlocked => this.@lock.IsExclusivelyLocked;
        public bool IsUpgradeableUnlocked => this.@lock.IsUpgradeableLocked;

        public Locked(T model) : this(model, Enumerable.Empty<LockedObject>()) { }

        public Locked(
            T model,
            IEnumerable<LockedObject> lockables)
            : this(model, lockables, null) { }

        public Locked(
            T model, 
            IEnumerable<LockedObject> lockables, 
            ILockResolver resolver)
        {
            this.model = model;

            this.relatives = lockables;
            this.lockList = new ThreadLocal<LinkedList<ILockStateInternal>>(() => new LinkedList<ILockStateInternal>(new Locked().Yield()));

            if (resolver == null)
            {
                this.@lock = new FastObjectLock();
            }
            else
            {
                this.@lock = new ResolveableObjectLock(model, resolver);
            }
        }

        // TODO: Make with ILockState
        public TResult Unsafe<TResult>(Func<T, TResult> fn)
        {
            return fn(this.model);
        }

        public void Unsafe(Action<T> action)
        {
            action.AsFunc()(this.model);
        }

        private TResult unlockImpl<TResult>(
            Func<LockedObject, Func<Func<TResult>, TResult>> relativeUnlocker,
            Func<ILockStateInternal, TResult> thisUnlocker)
        {
            TResult recurseRelatives(LinkedList<LockedObject> ls)
            { 
                if (ls.Last == null)
                    return thisUnlocker(this.lockList.Value.Last.Value);

                else
                {
                    var last = ls.PopLast();

                    return relativeUnlocker(last)(
                        () =>
                        {
                            return recurseRelatives(ls);
                        }
                    );
                }
            }

            return recurseRelatives(this.relatives.ToLinkedList());
        }

        public override TResult Unlock<TResult>(Func<TResult> fn)
        {
            return unlockImpl(
                x => x.Unlock,
                x => x.Unlock(this.lockList.Value, this.@lock, fn));
        }

        public TResult UnlockWithRelativesAs<TResult>(
            Func<LockedObject, Func<Func<TResult>, TResult>> relativeUnlocker,
            Func<TResult> fn)
        {
            return unlockImpl(
                relativeUnlocker,
                x => x.Unlock(this.lockList.Value, this.@lock, fn));
        }

        public TResult Unlock<TResult>(Func<T, TResult> fn)
        {
            return unlockImpl(
                x => x.Unlock,
                x => x.Unlock(this.lockList.Value, this.@lock, this.model, fn));
        }

        public TResult UnlockWithRelativesAs<TResult>(
            Func<LockedObject, Func<Func<TResult>, TResult>> relativeUnlocker,
            Func<T, TResult> fn)
        {
            return unlockImpl(
                relativeUnlocker,
                x => x.Unlock(this.lockList.Value, this.@lock, this.model, fn));
        }

        public void Unlock(Action action)
        {
            Unlock(action.AsFunc());
        }

        public void UnlockWithRelativesAs(
            Func<LockedObject, Func<Func<int>, int>> relativeUnlocker,
            Action action)
        {
            unlockImpl(
                relativeUnlocker,
                x => x.Unlock(this.lockList.Value, this.@lock, action.AsFunc()));
        }

        public void Unlock(Action<T> action)
        {
            Unlock(action.AsFunc());
        }

        public void UnlockWithRelativesAs(
            Func<LockedObject, Func<Func<int>, int>> relativeUnlocker,
            Action<T> action)
        {
            unlockImpl(
                relativeUnlocker,
                x => x.Unlock(this.lockList.Value, this.@lock, this.model, action.AsFunc()));
        }

        public override TResult UnlockUpgradeable<TResult>(Func<TResult> fn)
        {
            return unlockImpl(
                x => x.UnlockUpgradeable,
                x => x.UnlockUpgradeable(this.lockList.Value, this.@lock, fn));
        }

        public TResult UnlockUpgradeableWithRelativesAs<TResult>(
            Func<LockedObject, Func<Func<TResult>, TResult>> relativeUnlocker,
            Func<TResult> fn)
        {
            return unlockImpl(
                relativeUnlocker,
                x => x.UnlockUpgradeable(this.lockList.Value, this.@lock, fn));
        }

        public TResult UnlockUpgradeable<TResult>(Func<T, TResult> fn)
        {
            return unlockImpl(
                x => x.UnlockUpgradeable,
                x => x.UnlockUpgradeable(this.lockList.Value, this.@lock, this.model, fn));
        }

        public TResult UnlockUpgradeableWithRelativesAs<TResult>(
            Func<LockedObject, Func<Func<TResult>, TResult>> relativeUnlocker,
            Func<T, TResult> fn)
        {
            return unlockImpl(
                relativeUnlocker,
                x => x.UnlockUpgradeable(this.lockList.Value, this.@lock, this.model, fn));
        }

        public void UnlockUpgradeable(Action action)
        {
            UnlockUpgradeable(action.AsFunc());
        }

        public void UnlockUpgradeableWithRelativesAs(
            Func<LockedObject, Func<Func<int>, int>> relativeUnlocker,
            Action action)
        {
            unlockImpl(
                relativeUnlocker,
                x => x.UnlockUpgradeable(this.lockList.Value, this.@lock, action.AsFunc()));
        }

        public void UnlockUpgradeable(Action<T> action)
        {
            UnlockUpgradeable(action.AsFunc());
        }

        public void UnlockUpgradeableWithRelativesAs(
            Func<LockedObject, Func<Func<int>, int>> relativeUnlocker,
            Action<T> action)
        {
            unlockImpl(
                relativeUnlocker,
                x => x.UnlockUpgradeable(this.lockList.Value, this.@lock, this.model, action.AsFunc()));
        }

        public override TResult UnlockExclusive<TResult>(Func<TResult> fn)
        {
            return unlockImpl(
                x => x.UnlockExclusive,
                x => x.UnlockExclusive(this.lockList.Value, this.@lock, fn));
        }

        public TResult UnlockExclusiveWithRelativesAs<TResult>(
            Func<LockedObject, Func<Func<TResult>, TResult>> relativeUnlocker,
            Func<TResult> fn)
        {
            return unlockImpl(
                relativeUnlocker,
                x => x.UnlockExclusive(this.lockList.Value, this.@lock, fn));
        }

        public TResult UnlockExclusive<TResult>(Func<T, TResult> fn)
        {
            return unlockImpl(
                x => x.UnlockExclusive,
                x => x.UnlockExclusive(this.lockList.Value, this.@lock, this.model, fn));
        }

        public TResult UnlockExclusiveWithRelativesAs<TResult>(
            Func<LockedObject, Func<Func<TResult>, TResult>> relativeUnlocker,
            Func<T, TResult> fn)
        {
            return unlockImpl(
                relativeUnlocker,
                x => x.UnlockExclusive(this.lockList.Value, this.@lock, this.model, fn));
        }

        public void UnlockExclusive(Action action)
        {
            UnlockExclusive(action.AsFunc());
        }

        public void UnlockExclusiveWithRelativesAs(
            Func<LockedObject, Func<Func<int>, int>> relativeUnlocker,
            Action action)
        {
            unlockImpl(
                relativeUnlocker,
                x => x.UnlockExclusive(this.lockList.Value, this.@lock, action.AsFunc()));
        }

        public void UnlockExclusive(Action<T> action)
        {
            UnlockExclusive(action.AsFunc());
        }

        public void UnlockExclusiveWithRelativesAs(
            Func<LockedObject, Func<Func<int>, int>> relativeUnlocker,
            Action<T> action)
        {
            unlockImpl(
                relativeUnlocker,
                x => x.UnlockExclusive(this.lockList.Value, this.@lock, this.model, action.AsFunc()));
        }
    }
}
