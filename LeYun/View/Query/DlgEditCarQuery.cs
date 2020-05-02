using LeYun.Model;
using LeYun.View.Dlg;
using LeYun.ViewModel;
using LeYun.ViewModel.Dlg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.View.Query
{
    class DlgEditCarQuery : IEditCarQuery
    {
        public double NewWeightLimit { get; set; }
        public double NewDisLimit { get; set; }

        public bool Begin(Car car)
        {
            CarInfoDlg dlg = new CarInfoDlg();
            CarInfoDlgViewModel vm = new CarInfoDlgViewModel();
            vm.Title = "编辑车辆" + car.ID.ToString();
            vm.WeightLimit = car.WeightLimit;
            vm.DisLimit = car.DisLimit;
            dlg.DataContext = vm;
            dlg.ShowDialog();
            if (vm.Result)
            {
                NewWeightLimit = vm.WeightLimit;
                NewDisLimit = vm.DisLimit;
                return true;
            }
            return false;
        }
    }
}
