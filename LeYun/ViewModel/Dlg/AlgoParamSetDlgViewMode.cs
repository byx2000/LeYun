using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.ViewModel.Dlg
{
    class AlgoParamSetDlgViewMode : DlgViewModelBase
    {
        public int GenerationCount { get; set; }
        public double WTime { get; set; }
        public double WDis { get; set; }
        public double WCar { get; set; }
    }
}
