﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PnP.Core.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using PnP.Core.Model;

namespace PnP.Core.Test.Security
{
    [TestClass]
    public class RoleAssignmentsTests
    {
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Configure mocking default for all tests in this class, unless override by a specific test
            //TestCommon.Instance.Mocking = false;
        }

        [TestMethod]
        public async Task GetWebRoleAssignments()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                context.Web.Load(w => w.RoleAssignments);
                Assert.IsTrue(context.Web.RoleAssignments.Length > 0);
            }
        }
    }
}
