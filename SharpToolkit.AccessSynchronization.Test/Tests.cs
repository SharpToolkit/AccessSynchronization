using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace SharpToolkit.AccessSynchronization.Test
{
    [TestClass]
    public class Tests
    {
        Locked<Root> root;
        List<Locked<Parent>> parents;
        List<Locked<Child>> children;

        private void postCheck()
        {
            Assert.IsFalse(this.children.Any(x => x.IsShareUnlocked));
            Assert.IsFalse(this.parents.Any(x => x.IsShareUnlocked));
            Assert.IsFalse(this.root.IsShareUnlocked);
        }

        [TestInitialize]
        public void TestInit()
        {
            (this.root, this.parents, this.children) = Fixtures.GetHierarchyGraph();
        }

        [TestMethod]
        public void Unlock_Root()
        {
            this.root.Unlock(x => { Assert.IsTrue(x.LockedObject.IsShareUnlocked); });

            postCheck();
        }

        [TestMethod]
        public void Unlock_Parent()
        {
            this.parents.First().Unlock(x => 
            {
                Assert.IsTrue(x.LockedObject.IsShareUnlocked);
                Assert.IsTrue(this.root.IsShareUnlocked);

                Assert.IsTrue(this.parents.Count(y => y.IsShareUnlocked) == 1);
                Assert.IsFalse(this.children.Any(y => y.IsShareUnlocked));
            });

            postCheck();
        }

        [TestMethod]
        public void Unlock_Child()
        {
            this.children.First().Unlock(x =>
            {
                Assert.IsTrue(x.LockedObject.IsShareUnlocked);

                Assert.IsTrue(this.parents.Count(y => y.IsShareUnlocked) == 1);

                Assert.IsTrue(this.root.IsShareUnlocked);
            });

            postCheck();
        }

        [TestMethod]
        public void Return_Simple()
        {
            var result = this.root.Unlock(x => 1234);

            Assert.AreEqual(1234, result);

            postCheck();
        }

        [TestMethod]
        public void Return_Transitive()
        {
            var result = this.children.Random().Unlock(x => 1234);

            Assert.AreEqual(1234, result);

            postCheck();
        }
    }
}
