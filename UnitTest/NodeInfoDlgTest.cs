using System;
using LeYun.ViewModel.Dlg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class NodeInfoDlgTest
    {
        [TestMethod]
        public void OkTest()
        {
            NodeInfoDlgViewModel vm = new NodeInfoDlgViewModel();
            vm.OkCommand.Execute(null);
            Assert.IsTrue(vm.Result);
        }

        [TestMethod]
        public void CancelTest()
        {
            NodeInfoDlgViewModel vm = new NodeInfoDlgViewModel();
            vm.CancelCommand.Execute(null);
            Assert.IsFalse(vm.Result);
        }
    }
}
