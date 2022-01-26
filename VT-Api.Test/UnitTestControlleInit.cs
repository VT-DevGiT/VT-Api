using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace VT_Api.Test
{
    [TestClass]
    public class UnitTestControlleInit
    {

        [TestInitialize]
        public void Initialize()
        {
                
            VtController.InitApi();
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public void NullVtControllers()
        {
            var VtCtr = VtController.Get;
            Assert.IsNotNull(VtController.Get);

            var config = VtCtr.Configs;
            var autoRegister = VtCtr.AutoRegister;
            var miniegame = VtCtr.MinGames;
            var role = VtCtr.Role;
            var team = VtCtr.Team;
            var events = VtCtr.Events;
            var commands = VtCtr.Commands;

            Assert.IsNotNull(config);
            Assert.IsNotNull(autoRegister);
            Assert.IsNotNull(miniegame);
            Assert.IsNotNull(role);
            Assert.IsNotNull(team);
            Assert.IsNotNull(events);
            Assert.IsNotNull(commands);
        }
    }
}