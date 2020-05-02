using System;
using LeYun.ViewModel.Dlg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class CarInfoDlgTest
    {
        [TestMethod]
        public void OkTest()
        {
            CarInfoDlgViewModel vm = new CarInfoDlgViewModel();
            vm.OkCommand.Execute(null);
            Assert.IsTrue(vm.Result);
        }

        [TestMethod]
        public void CancelTest()
        {
            CarInfoDlgViewModel vm = new CarInfoDlgViewModel();
            vm.CancelCommand.Execute(null);
            Assert.IsFalse(vm.Result);
        }
    }
}
