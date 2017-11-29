using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpToolkit.Extensions.Diagnostics.Testing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpToolkit.AccessSynchronization.Test
{
    [TestClass]
    public class TrackingTests
    {
        public class Context
        {
            public AutoResetEvent ResetEvent { get; set; }
        }

        [TestInitialize]
        public void TestInit()
        {
            Assembly
                .GetAssembly(typeof(ThreadLocksTrack))
                .GetTypes()
                .Single(x => x.Name == "ResolveableObjectLock")
                .GetField("threadsLocksTrack", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, new ConcurrentDictionary<int, ThreadLocksTrack>());
        }

        [TestMethod]
        public void Track()
        {
            int firstThreadId = default;
            int secondThreadId = default;
            Dictionary<int, Dictionary<object, IEnumerable<(ILockState, ThreadLocksTrack.AcqusitionState)>>> result = null;

            Action<ConcurrentDictionary<int, ThreadLocksTrack>> act =
                x =>
                {
                    result = x.ToDictionary(y => y.Key, y =>

                        y.Value.Report.ToDictionary(z => z.Key, z => z.Value.Select(i => (i.LockState, i.AcqusitionState)))
                    );
                };

            var rig = new MultithreadedTestRig<Context>();

            var root = Fixtures.GetRoot(new TestResolver(act));
            var parent = Fixtures.GetParent(root, new TestResolver(act));
            var child = Fixtures.GetChild(parent, new TestResolver(act));

            var rootUnlocked   = root  .Unsafe(x => x);
            var parentUnlocked = parent.Unsafe(x => x);
            var childUnlocked  = child .Unsafe(x => x);

            var context = new Context() { ResetEvent = new AutoResetEvent(false) };

            rig.CreateThread(
                ctx =>
                {
                    firstThreadId = Thread.CurrentThread.ManagedThreadId;
                    child.Unlock(() =>
                    {
                        // Let the other thread to try unlock
                        ctx.ResetEvent.Set();

                        // Wait for other thread to reach finally block
                        ctx.ResetEvent.WaitOne();
                    });
                },
                cts => { },
                context);

            rig.CreateThread(
                ctx =>
                {
                    secondThreadId = Thread.CurrentThread.ManagedThreadId;
                    // Wait for other thread to unlock the branch
                    ctx.ResetEvent.WaitOne();

                    try
                    {

                        root.UnlockExclusive(() =>
                        {

                        });
                    }

                    finally
                    {
                        // Release other thread
                        ctx.ResetEvent.Set();
                    }
                },
                ctx => { },
                context);

            rig.StartThreads();

            rig.WaitForThreads();

            Assert.AreEqual(3, result[firstThreadId].Count);

            // Root checks
            Assert.AreEqual(1, result[firstThreadId][rootUnlocked].Count());
            Assert.AreEqual("Shared", result[firstThreadId][rootUnlocked].Single().Item1.ToString());
            Assert.AreEqual(ThreadLocksTrack.AcqusitionState.Acquired, result[firstThreadId][rootUnlocked].Single().Item2);

            // Parent checks
            Assert.AreEqual(1, result[firstThreadId][parentUnlocked].Count());
            Assert.AreEqual("Shared", result[firstThreadId][parentUnlocked].Single().Item1.ToString());
            Assert.AreEqual(ThreadLocksTrack.AcqusitionState.Acquired, result[firstThreadId][parentUnlocked].Single().Item2);

            // Child checks
            Assert.AreEqual(1, result[firstThreadId][childUnlocked].Count());
            Assert.AreEqual("Shared", result[firstThreadId][childUnlocked].Single().Item1.ToString());
            Assert.AreEqual(ThreadLocksTrack.AcqusitionState.Acquired, result[firstThreadId][childUnlocked].Single().Item2);

            // Second thread
            Assert.AreEqual(1, result[secondThreadId].Count);

            // Root checks
            Assert.AreEqual(1, result[secondThreadId][rootUnlocked].Count());
            Assert.AreEqual("Exclusive", result[secondThreadId][rootUnlocked].Single().Item1.ToString());
            Assert.AreEqual(ThreadLocksTrack.AcqusitionState.Intent, result[secondThreadId][rootUnlocked].Single().Item2);
        }

        [TestMethod]
        public void Deadlock()
        {
            var rig = new MultithreadedTestRig<Context>();

            var root = Fixtures.GetRoot(new ExceptionDeadlockResolver(1000));

            var parent1 = Fixtures.GetParent(root, new ExceptionDeadlockResolver(1000));
            var child1 = Fixtures.GetChild(parent1, new ExceptionDeadlockResolver(1000));

            var parent2 = Fixtures.GetParent(root, new ExceptionDeadlockResolver(1000));
            var child2 = Fixtures.GetChild(parent2, new ExceptionDeadlockResolver(1000));

            var context = new Context() { ResetEvent = new AutoResetEvent(false) };

            rig.CreateThread(
                ctx =>
                {
                    child1.Unlock(() =>
                    {
                        // Let the other thread to take lock
                        ctx.ResetEvent.Set();

                        // Wait for other thread to take lock
                        ctx.ResetEvent.WaitOne();

                        // FIGHT!
                        child2.UnlockExclusiveWithRelativesAs(
                            x => x.Unlock,
                            () =>
                            {

                            });
                    });
                
                },
                cts => { },
                context);
            // VS
            rig.CreateThread(
                ctx =>
                {
                    // Wait for other thread to unlock the branch
                    ctx.ResetEvent.WaitOne();

                    child2.Unlock(() =>
                    {
                        ctx.ResetEvent.Set();

                        // FIGHT!
                        child1.UnlockExclusiveWithRelativesAs(
                            x => x.Unlock,
                            () =>
                            {

                            });
                    });
                },                
                ctx => { },
                context);

            rig.StartThreads();

            rig.WaitForThreads();

            Assert.IsFalse(rig.Success);
            Assert.AreEqual(typeof(DeadlockException), rig.CaughtException.GetType());
        }
    }
}
