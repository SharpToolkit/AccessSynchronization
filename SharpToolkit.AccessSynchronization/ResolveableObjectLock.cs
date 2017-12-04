using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SharpToolkit.AccessSynchronization
{
    internal sealed class ResolveableObjectLock : IObjectLock
    {
        // TODO: Add view id, to allow analysing more than one lock view.
        private static ConcurrentDictionary<int, ThreadLocksTrack> threadsLocksTrack;

        private readonly object target;
        private readonly ReaderWriterLockSlim @lock;

        private ILockResolver lockFailureResolver;

        static ResolveableObjectLock()
        {
            threadsLocksTrack = new ConcurrentDictionary<int, ThreadLocksTrack>();
        }

        public ResolveableObjectLock(object target, ILockResolver resolver)
        {
            this.target = target;
            this.@lock = new ReaderWriterLockSlim();

            this.lockFailureResolver = resolver;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ThreadLocksTrack getTrack(int threadId)
        {
            if (threadsLocksTrack.TryGetValue(threadId, out var track) == false)
            {
                track = new ThreadLocksTrack();

#if DEBUG
                if (threadsLocksTrack.TryAdd(threadId, track) == false)
                {
                    // TODO: remove when proven that no collisions can occur.
                    throw new InvalidOperationException("Thread id collision.");
                }
#else
                threadsLocksTrack.TryAdd(threadId, track);
#endif
            }

            return track;
        }

        public bool IsShareLocked       => this.@lock.IsReadLockHeld;
        public bool IsExclusivelyLocked => this.@lock.IsWriteLockHeld;
        public bool IsUpgradeableLocked => this.@lock.IsUpgradeableReadLockHeld;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void enterLock(ILockStateInternal state, Func<int, bool> acquireFunc)
        {
            var track = this.getTrack(Thread.CurrentThread.ManagedThreadId);

            track.Intent(this.target, state);

            while (acquireFunc(this.lockFailureResolver.Timeout) == false)
            {
                this.lockFailureResolver.Resolve(threadsLocksTrack);
            }

            track.Acquire(this.target);
        }

        public void TakeSharedLock(ILockStateInternal state)
        {
            enterLock(
                state,
                this.@lock.TryEnterReadLock);
        }

        public void ExitSharedLock()
        {
            this.@lock.ExitReadLock();

            this.getTrack(Thread.CurrentThread.ManagedThreadId).Pop(this.target);
        }

        public void TakeExclusiveLock(ILockStateInternal state)
        {
            enterLock(
                state,
                this.@lock.TryEnterWriteLock);
        }

        public void ExitExclusiveLock()
        {
            this.@lock.ExitWriteLock();

            this.getTrack(Thread.CurrentThread.ManagedThreadId).Pop(this.target);
        }

        public void TakeUpgradeableLock(ILockStateInternal state)
        {
            enterLock(
                state,
                this.@lock.TryEnterUpgradeableReadLock);
        }

        public void ExitUpgradeableLock()
        {
            this.@lock.ExitUpgradeableReadLock();

            this.getTrack(Thread.CurrentThread.ManagedThreadId).Pop(this.target);
        }
    }
}
