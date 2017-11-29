using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.AccessSynchronization.Test
{
    [TestClass]
    public class LockStatesTests
    {
        private Locked<Root> getRoot(bool useResolver)
        {
            if (useResolver)
                return Fixtures.GetRoot(new TestResolver());
            else
                return Fixtures.GetRoot();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_Default(bool useResolver)
        {
            var obj = getRoot(useResolver);

            Assert.IsFalse(obj.IsShareUnlocked);
            Assert.IsFalse(obj.IsUpgradeableUnlocked);
            Assert.IsFalse(obj.IsExclusevelyUnlocked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_Unlock_Param_Void(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.Unlock(x => 
            {
                Assert.IsTrue(obj.IsShareUnlocked);

                Assert.IsFalse(obj.IsUpgradeableUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_Unlock_Param_Result(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.Unlock(x =>
            {
                Assert.IsTrue(obj.IsShareUnlocked);

                Assert.IsFalse(obj.IsUpgradeableUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);

                return 0;
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_Unlock_NoParam_Void(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.Unlock(() =>
            {
                Assert.IsTrue(obj.IsShareUnlocked);

                Assert.IsFalse(obj.IsUpgradeableUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_Unlock_NoParam_Result(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.Unlock(() => 
            {
                Assert.IsTrue(obj.IsShareUnlocked);

                Assert.IsFalse(obj.IsUpgradeableUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);

                return 0;
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockUpgradeable_Param_Void(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockUpgradeable(x =>
            {
                Assert.IsTrue(obj.IsUpgradeableUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockUpgradeable_Param_Result(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockUpgradeable(x =>
            {
                Assert.IsTrue(obj.IsUpgradeableUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);

                return 0;
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockUpgradeable_NoParam_Void(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockUpgradeable(() =>
            {
                Assert.IsTrue(obj.IsUpgradeableUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockUpgradeable_NoParam_Result(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockUpgradeable(() =>
            {
                Assert.IsTrue(obj.IsUpgradeableUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsExclusevelyUnlocked);

                return 0;
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockExclusive_Param_Void(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockExclusive(x =>
            {
                Assert.IsTrue(obj.IsExclusevelyUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsUpgradeableUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockExclusive_Param_Result(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockExclusive(x =>
            {
                Assert.IsTrue(obj.IsExclusevelyUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsUpgradeableUnlocked);

                return 0;
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockExclusive_NoParam_Void(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockExclusive(() =>
            {
                Assert.IsTrue(obj.IsExclusevelyUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsUpgradeableUnlocked);
            });
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void LockedState_UnlockExclusive_NoParam_Result(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.UnlockExclusive(() =>
            {
                Assert.IsTrue(obj.IsExclusevelyUnlocked);

                Assert.IsFalse(obj.IsShareUnlocked);
                Assert.IsFalse(obj.IsUpgradeableUnlocked);

                return 0;
            });
        }
    }
}
