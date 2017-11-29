using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpToolkit.AccessSynchronization
{
    public class ThreadLocksTrack
    {
        public enum AcqusitionState { Intent, Acquired }

        public class Item
        {
            public ILockState LockState { get; }
            public AcqusitionState AcqusitionState { get; private set; }

            public Item(ILockState state)
            {
                this.LockState = state;
                this.AcqusitionState = AcqusitionState.Intent;
            }

            public void Acquire()
            {
                this.AcqusitionState = AcqusitionState.Acquired;
            }
        }

        private readonly Dictionary<object, Stack<Item>> objects;

        internal ThreadLocksTrack()
        {
            this.objects = new Dictionary<object, Stack<Item>>();
        }

        internal void Intent(object obj, ILockStateInternal state)
        {
            lock (this.objects)
            {
                if (this.objects.TryGetValue(obj, out var stack) == false)
                {
                    stack = new Stack<Item>();
                    this.objects.Add(obj, stack);
                }

                stack.Push(new Item(state));
            }
        }

        internal void Acquire(object obj)
        {
            lock (this.objects)
            {
                this.objects[obj].Peek().Acquire();
            }
        }

        internal void Pop(object obj)
        {
            lock (this.objects)
            {
                var list = this.objects[obj];

                list.Pop();
            }
        }

        public Dictionary<object, IEnumerable<Item>> Report
        {
            get
            {
                lock (this.objects)
                {
                    return this.objects.ToDictionary(
                        x => x.Key,
                        x => (IEnumerable<Item>)x.Value.ToLinkedList());
                }
            }
        }
    }
}
