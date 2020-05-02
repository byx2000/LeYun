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
    class DlgAddCarQuery : IAddCarQuery
    {
        public double WeightLimit { get; set; }
        public double DisLimit { get; set; }

        public bool Begin()
        {
            CarInfoDlg dlg = new CarInfoDlg();
            CarInfoDlgViewModel vm = new CarInfoDlgViewModel();
            vm.Title = "添加车辆";
            dlg.DataContext = vm;
            dlg.ShowDialog();
            if (vm.Result)
            {
                WeightLimit = vm.WeightLimit;
                DisLimit = vm.DisLimit;
                return true;
            }
            return false;
        }  
    }
}
