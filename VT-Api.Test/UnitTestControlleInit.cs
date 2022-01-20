using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VT_Api.Test
{
    [TestClass]
    public class UnitTestControlleInit
    {
        [TestMethod]
        public void NullVtController()
        {
            var VtCtr = VtController.Get;

            Assert.IsNotNull(VtCtr);
        }
    }
}