using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.AccessSynchronization.Test
{
    [TestClass]
    public class RelativesUnlockTests
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
        public void RelativeUnlock_Shared_Upgradeable(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockWithRelativesAs(
                x => x.UnlockUpgradeable,
                x =>
                {
                    Assert.IsTrue(child.IsShareUnlocked);

                    Assert.IsFalse(child.IsUpgradeableUnlocked);
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
        public void RelativeUnlock_Shared_Exclusive(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockWithRelativesAs(
                x => x.UnlockExclusive,
                x =>
                {
                    Assert.IsTrue(child.IsShareUnlocked);

                    Assert.IsFalse(child.IsUpgradeableUnlocked);
                    Assert.IsFalse(child.IsExclusevelyUnlocked);

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
        public void RelativeUnlock_Upgradeable_Shared(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockUpgradeableWithRelativesAs(
                x => x.Unlock,
                x =>
                {
                    Assert.IsTrue(child.IsUpgradeableUnlocked);

                    Assert.IsFalse(child.IsShareUnlocked);
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
        public void RelativeUnlock_Upgradeable_Exclusive(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockUpgradeableWithRelativesAs(
                x => x.UnlockExclusive,
                x =>
                {
                    Assert.IsTrue(child.IsUpgradeableUnlocked);

                    Assert.IsFalse(child.IsShareUnlocked);
                    Assert.IsFalse(child.IsExclusevelyUnlocked);

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
        public void RelativeUnlock_Exclusive_Shared(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockExclusiveWithRelativesAs(
                x => x.Unlock,
                x =>
                {
                    Assert.IsTrue(child.IsExclusevelyUnlocked);

                    Assert.IsFalse(child.IsShareUnlocked);
                    Assert.IsFalse(child.IsUpgradeableUnlocked);

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
        public void RelativeUnlock_Exclusive_Upgradeable(bool useResolver)
        {
            var (root, parent, child) = getObjects(useResolver);

            child.UnlockExclusiveWithRelativesAs(
                x => x.UnlockUpgradeable,
                x =>
                {
                    Assert.IsTrue(child.IsExclusevelyUnlocked);

                    Assert.IsFalse(child.IsShareUnlocked);
                    Assert.IsFalse(child.IsUpgradeableUnlocked);

                    Assert.IsTrue(parent.IsUpgradeableUnlocked);

                    Assert.IsFalse(parent.IsShareUnlocked);
                    Assert.IsFalse(parent.IsExclusevelyUnlocked);

                    Assert.IsTrue(root.IsUpgradeableUnlocked);

                    Assert.IsFalse(root.IsShareUnlocked);
                    Assert.IsFalse(root.IsExclusevelyUnlocked);
                });
        }
    }
}
