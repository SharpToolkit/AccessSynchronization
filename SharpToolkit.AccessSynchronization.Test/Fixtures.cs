using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpToolkit.AccessSynchronization.Test
{
    class Fixtures
    {
        public static Locked<Root> GetRoot()
        {
            return new Root(Enumerable.Empty<LockedObject>()).LockedObject;
        }

        public static Locked<Root> GetRoot(ILockResolver resolver)
        {
            return new Root(Enumerable.Empty<LockedObject>() , resolver).LockedObject;
        }

        public static Locked<Parent> GetParent(Locked<Root> root)
        {
            return new Parent(root.Yield()).LockedObject;
        }

        public static Locked<Parent> GetParent(Locked<Root> root, ILockResolver resolver)
        {
            return new Parent(root.Yield(), resolver).LockedObject;
        }

        public static Locked<Child> GetChild(Locked<Parent> parent)
        {
            return new Child(parent.Yield()).LockedObject;
        }

        public static Locked<Child> GetChild(Locked<Parent> parent, ILockResolver resolver)
        {
            return new Child(parent.Yield(), resolver).LockedObject;
        }

        public static (Locked<Root>, List<Locked<Parent>>, List<Locked<Child>>) GetHierarchyGraph()
        {
            var root = GetRoot();

            var parents =
                Enumerable
                .Range(0, 9)
                .Select(x => new Parent(new[] { root }).LockedObject)
                .ToList();

            var children =
                parents
                .SelectMany(x =>
                {
                    return
                        Enumerable
                            .Range(0, 9)
                            .Select(y => new Child(new[] { x }).LockedObject);
                })
                .ToList();

            return (root, parents, children);
        }

        public static (Locked<Root>, List<Locked<Parent>>, List<Locked<Child>>) GetHierarchyGraph(ILockResolver resolver)
        {
            var root = GetRoot(resolver);

            var parents =
                Enumerable
                .Range(0, 9)
                .Select(x => new Parent(new[] { root }, resolver).LockedObject)
                .ToList();

            var children =
                parents
                .SelectMany(x =>
                {
                    return
                        Enumerable
                            .Range(0, 9)
                            .Select(y => new Child(new[] { x }, resolver).LockedObject);
                })
                .ToList();

            return (root, parents, children);
        }
    }
}
