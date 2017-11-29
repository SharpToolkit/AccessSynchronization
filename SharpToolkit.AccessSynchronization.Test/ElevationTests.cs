using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SharpToolkit.AccessSynchronization.Test
{
    [TestClass]
    public class ElevationTests
    {
        private Locked<Root> getRoot(bool useResolver)
        {
            if (useResolver)
                return Fixtures.GetRoot(new TestResolver());
            else
                return Fixtures.GetRoot();
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Elevation_Shared_Shared(bool useResolver)
        {
            var reached = false;

            var obj = getRoot(useResolver);

            obj.Unlock(() =>
            {
                obj.Unlock(() =>
               {
                   reached = true;
               });
            });

            Assert.IsTrue(reached);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        [ExpectedException(typeof(LockRecursionException))]
        public void Elevation_Shared_Upgradeable(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.Unlock(() =>
            {
                obj.UnlockUpgradeable(() =>
               {
                   Assert.Fail();
               });
            });
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        [ExpectedException(typeof(LockRecursionException))]
        public void Elevation_Shared_Exclusive(bool useResolver)
        {
            var obj = getRoot(useResolver);

            obj.Unlock(() =>
            {
                obj.UnlockExclusive(() =>
               {
                   Assert.Fail();
               });
            });
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Elevation_Upgradeable_Shared(bool useResolver)
        {
            var reached = false;

            var obj = getRoot(useResolver);

            obj.UnlockUpgradeable(() =>
            {
                obj.Unlock(() =>
               {
                   reached = true;
               });
            });

            Assert.IsTrue(reached);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Elevation_Upgradeable_Upgradeable(bool useResolver)
        {
            var reached = false;

            var obj = getRoot(useResolver);

            obj.UnlockUpgradeable(() =>
            {
                obj.UnlockUpgradeable(() =>
               {
                   reached = true;
               });
            });

            Assert.IsTrue(reached);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Elevation_Upgradeable_Exclusive(bool useResolver)
        {
            var reached = false;

            var obj = getRoot(useResolver);

            obj.UnlockUpgradeable(() =>
            {
                obj.UnlockExclusive(() =>
                {
                    reached = true;
                });
            });

            Assert.IsTrue(reached);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Elevation_Exclusive_Shared(bool useResolver)
        {
            var reached = false;

            var obj = getRoot(useResolver);

            obj.UnlockExclusive(() =>
            {
                obj.Unlock(() =>
                {
                    reached = true;
                });
            });

            Assert.IsTrue(reached);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Elevation_Exclusive_Upgradeable(bool useResolver)
        {
            var reached = false;

            var obj = getRoot(useResolver);

            obj.UnlockExclusive(() =>
            {
                obj.UnlockUpgradeable(() =>
                {
                    reached = true;
                });
            });

            Assert.IsTrue(reached);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Elevation_Exclusive_Exclusive(bool useResolver)
        {
            var reached = false;

            var obj = getRoot(useResolver);

            obj.UnlockExclusive(() =>
            {
                obj.UnlockExclusive(() =>
                {
                    reached = true;
                });
            });

            Assert.IsTrue(reached);
        }
    }
}
