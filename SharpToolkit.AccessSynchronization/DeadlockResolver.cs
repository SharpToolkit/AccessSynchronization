using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace SharpToolkit.AccessSynchronization
{
    public sealed class ExceptionDeadlockResolver : ILockResolver
    {
        public int Timeout { get; private set; }

        public ExceptionDeadlockResolver(int timeout)
        {
            this.Timeout = timeout;
        }

        public void Resolve(ConcurrentDictionary<int, ThreadLocksTrack> taken)
        {
            var pairs =
                taken.SelectMany(
                    x => taken
                        .Where(y => x.Key != y.Key)
                        .Select(y => (subject: x.Value, target: y.Value, subjectThread: x.Key, targetThread: y.Key)));
                

            foreach (var pair in pairs)
            {
                checkAgainst(pair.subject, pair.target, pair.subjectThread, pair.targetThread);
            }
        }

        private void checkAgainst(ThreadLocksTrack subject, ThreadLocksTrack target, int subjectThread, int targetThread)
        {
            var (obj, locks) = 
                subject.Report
                .Where(
                    x => x.Value
                        .Select(y => y.AcqusitionState)
                        .Contains(ThreadLocksTrack.AcqusitionState.Intent))
                .Single();

            if (target.Report.ContainsKey(obj) == false)
                // Target doesn't contain intended object
                // so no deadlock can exist.
                return;
            

            var (tObj, tLocks) =
                target.Report
                .Where(
                    x => x.Value
                        .Select(y => y.AcqusitionState)
                        .Contains(ThreadLocksTrack.AcqusitionState.Intent))
                    .Single();

            if (obj == tObj)
                // The target also intents to unlock the object.
                return;

            if (subject.Report.ContainsKey(tObj) == false)
                // The taget intents to unlock object that is not locked by subject.
                return;

            throw new DeadlockException(
                subjectThread,
                obj,
                locks.Single(x => x.AcqusitionState == ThreadLocksTrack.AcqusitionState.Intent).LockState,
                subject.Report[tObj].Last().LockState,
                targetThread,
                tObj,
                tLocks.Single(x => x.AcqusitionState == ThreadLocksTrack.AcqusitionState.Intent).LockState,
                target.Report[obj].Last().LockState);
        }
    }
}
