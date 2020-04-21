using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.ViewModel.Dlg
{
    class AlgoParamSetDlgViewMode : ViewModelBase
    {
        public int GenerationCount { get; set; }
        public double WTime { get; set; }
        public double WDis { get; set; }
        public double WCar { get; set; }

        public bool IsCancel { get; private set; }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public AlgoParamSetDlgViewMode()
        {
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
            IsCancel = true;
        }

        private void Cancel(object obj)
        {
            IsCancel = true;
            ((Window)obj).Close();
        }

        private void Ok(object obj)
        {
            IsCancel = false;
            ((Window)obj).Close();
        }
    }
}
