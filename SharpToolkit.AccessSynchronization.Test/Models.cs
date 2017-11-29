
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.AccessSynchronization.Test
{
    public abstract class Base<T>
        where T : Base<T>
    {
        private static int lastId;

        public readonly int id;

        public Locked<T> LockedObject { get; }

        public Base(IEnumerable<LockedObject> relatives)
        {
            LockedObject = new Locked<T>((T)this, relatives);

            lastId += 1;
            this.id = lastId;
        }

        public Base(IEnumerable<LockedObject> relatives, ILockResolver resolver)
        {
            LockedObject = new Locked<T>((T)this, relatives, resolver);

            lastId += 1;
            this.id = lastId;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}: {id}";
        }
    }

    public class Root : Base<Root>
    {
        public Root(IEnumerable<LockedObject> relatives) 
            : base(relatives)
        {
        }

        public Root(IEnumerable<LockedObject> relatives, ILockResolver resolver)
            : base(relatives, resolver)
        {
        }
    }

    public class Parent : Base<Parent>
    {
        public Parent(IEnumerable<LockedObject> relatives) 
            : base(relatives)
        {
        }

        public Parent(IEnumerable<LockedObject> relatives, ILockResolver resolver)
            : base(relatives, resolver)
        {
        }
    }

    public class Child : Base<Child>
    {
        public Child(IEnumerable<LockedObject> relatives) 
            : base(relatives)
        {
        }

        public Child(IEnumerable<LockedObject> relatives, ILockResolver resolver)
            : base(relatives, resolver)
        {
        }
    }
}
