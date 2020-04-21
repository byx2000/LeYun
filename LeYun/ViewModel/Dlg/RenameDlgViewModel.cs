using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.ViewModel.Dlg
{
    class RenameDlgViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string NewName { get; set; }
        public bool IsCancel { get; set; } = true;

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public RenameDlgViewModel()
        {
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
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
