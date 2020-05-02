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
    class DlgEditNodeQuery : IEditNodeQuery
    {
        public double NewX { get; set; }
        public double NewY { get; set; }
        public double NewDemand { get; set; }

        public bool Begin(Node currentNode)
        {
            NodeInfoDlg dlg = new NodeInfoDlg();
            NodeInfoDlgViewModel vm = new NodeInfoDlgViewModel();
            vm.Title = "编辑节点" + currentNode.ID.ToString();
            vm.X = currentNode.X;
            vm.Y = currentNode.Y;
            vm.Demand = currentNode.Demand;
            dlg.DataContext = vm;
            dlg.ShowDialog();

            if (vm.Result)
            {
                NewX = vm.X;
                NewY = vm.Y;
                NewDemand = vm.Demand;
                return true;
            }
            return false;
        }
    }
}
