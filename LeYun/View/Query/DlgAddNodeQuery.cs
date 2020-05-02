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
    class DlgAddNodeQuery : IAddNodeQuery
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Demand { get; set; }

        public bool Begin()
        {
            NodeInfoDlg dlg = new NodeInfoDlg();
            NodeInfoDlgViewModel vm = new NodeInfoDlgViewModel();
            vm.Title = "添加节点";
            dlg.DataContext = vm;
            dlg.ShowDialog();
            if (vm.Result)
            {
                X = vm.X;
                Y = vm.Y;
                Demand = vm.Demand;
                return true;
            }
            return false;
        }
    }
}
