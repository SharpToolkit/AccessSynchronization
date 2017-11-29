using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpToolkit.AccessSynchronization.Test
{
    [TestClass]
    // Tests that object relatives are unlocked correctly.
    public class TransitiveUnlockTests
    {
        public (Locked<Root>, Locked<Parent>, Locked<Child>) getObjects(bool useResolver)
        {


            if (useResolver)
            {
                var root = Fixtures.GetRoot(new TestResolver());
                var parent = Fixtures.GetParent(root, new TestResolver());
                var child = Fixtures.GetChild(parent, new TestResolver());

                return (root, parent, child);
            }

            else
            {
                var root = Fixtures.GetRoot();
                var parent = Fixtures.GetParent(root);
                var child = Fixtures.GetChild(parent);

                return (root, parent, child);
            }
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_Param_Shared(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.Unlock(x =>
            {
                Assert.IsTrue(child.IsShareUnlocked);

                Assert.IsFalse(child.IsUpgradeableUnlocked);
                Assert.IsFalse(child.IsExclusevelyUnlocked);

                Assert.IsTrue(parent.IsShareUnlocked);

                Assert.IsFalse(parent.IsUpgradeableUnlocked);
                Assert.IsFalse(parent.IsExclusevelyUnlocked);

                Assert.IsTrue(root.IsShareUnlocked);

                Assert.IsFalse(root.IsUpgradeableUnlocked);
                Assert.IsFalse(root.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_NoParam_Shared(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.Unlock(() =>
            {
                Assert.IsTrue(child.IsShareUnlocked);

                Assert.IsFalse(child.IsUpgradeableUnlocked);
                Assert.IsFalse(child.IsExclusevelyUnlocked);

                Assert.IsTrue(parent.IsShareUnlocked);

                Assert.IsFalse(parent.IsUpgradeableUnlocked);
                Assert.IsFalse(parent.IsExclusevelyUnlocked);

                Assert.IsTrue(root.IsShareUnlocked);

                Assert.IsFalse(root.IsUpgradeableUnlocked);
                Assert.IsFalse(root.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_Param_Upgradeable(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockUpgradeable(x =>
            {
                Assert.IsTrue(child.IsUpgradeableUnlocked);

                Assert.IsFalse(child.IsShareUnlocked);
                Assert.IsFalse(child.IsExclusevelyUnlocked);

                Assert.IsTrue(parent.IsUpgradeableUnlocked);

                Assert.IsFalse(parent.IsShareUnlocked);
                Assert.IsFalse(parent.IsExclusevelyUnlocked);

                Assert.IsTrue(root.IsUpgradeableUnlocked);

                Assert.IsFalse(root.IsShareUnlocked);
                Assert.IsFalse(root.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_NoParam_Upgradeable(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockUpgradeable(() =>
            {
                Assert.IsTrue(child.IsUpgradeableUnlocked);

                Assert.IsFalse(child.IsShareUnlocked);
                Assert.IsFalse(child.IsExclusevelyUnlocked);

                Assert.IsTrue(parent.IsUpgradeableUnlocked);

                Assert.IsFalse(parent.IsShareUnlocked);
                Assert.IsFalse(parent.IsExclusevelyUnlocked);

                Assert.IsTrue(root.IsUpgradeableUnlocked);

                Assert.IsFalse(root.IsShareUnlocked);
                Assert.IsFalse(root.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_Param_Exclusive(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockExclusive(x =>
            {
                Assert.IsTrue(child.IsExclusevelyUnlocked);

                Assert.IsFalse(child.IsShareUnlocked);
                Assert.IsFalse(child.IsUpgradeableUnlocked);

                Assert.IsTrue(parent.IsExclusevelyUnlocked);

                Assert.IsFalse(parent.IsShareUnlocked);
                Assert.IsFalse(parent.IsUpgradeableUnlocked);

                Assert.IsTrue(root.IsExclusevelyUnlocked);

                Assert.IsFalse(root.IsShareUnlocked);
                Assert.IsFalse(root.IsUpgradeableUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_NoParam_Exclusive(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockExclusive(() =>
            {
                Assert.IsTrue(child.IsExclusevelyUnlocked);

                Assert.IsFalse(child.IsShareUnlocked);
                Assert.IsFalse(child.IsUpgradeableUnlocked);

                Assert.IsTrue(parent.IsExclusevelyUnlocked);

                Assert.IsFalse(parent.IsShareUnlocked);
                Assert.IsFalse(parent.IsUpgradeableUnlocked);

                Assert.IsTrue(root.IsExclusevelyUnlocked);

                Assert.IsFalse(root.IsShareUnlocked);
                Assert.IsFalse(root.IsUpgradeableUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_Param_UngradedExclusive(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockUpgradeable(x =>
            {
                child.UnlockExclusive(y =>
                {
                    Assert.IsTrue(child.IsExclusevelyUnlocked);
                    Assert.IsTrue(child.IsUpgradeableUnlocked);

                    Assert.IsFalse(child.IsShareUnlocked);

                    Assert.IsTrue(parent.IsExclusevelyUnlocked);
                    Assert.IsTrue(parent.IsUpgradeableUnlocked);

                    Assert.IsFalse(parent.IsShareUnlocked);

                    Assert.IsTrue(root.IsExclusevelyUnlocked);
                    Assert.IsTrue(root.IsUpgradeableUnlocked);

                    Assert.IsFalse(root.IsShareUnlocked);
                });
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Transitive_NoParam_UngradedExclusive(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockUpgradeable(() =>
            {
                child.UnlockExclusive(() =>
                {
                    Assert.IsTrue(child.IsExclusevelyUnlocked);
                    Assert.IsTrue(child.IsUpgradeableUnlocked);

                    Assert.IsFalse(child.IsShareUnlocked);

                    Assert.IsTrue(parent.IsExclusevelyUnlocked);
                    Assert.IsTrue(parent.IsUpgradeableUnlocked);

                    Assert.IsFalse(parent.IsShareUnlocked);

                    Assert.IsTrue(root.IsExclusevelyUnlocked);
                    Assert.IsTrue(root.IsUpgradeableUnlocked);

                    Assert.IsFalse(root.IsShareUnlocked);
                });
            });
        }
    }
}
