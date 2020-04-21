using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.ViewModel
{
    // 对话框视图模型基类，适用于有“确定”和“取消”两个按钮的对话框
    // 点击“确定”按钮时，对话框关闭，IsCancel为false
    // 点击“取消”按钮时，对话框关闭，IsCancel为true
    // IsCancel默认为true，因此直接关闭对话框的效果与点击“取消”按钮一样
    // 对话框关闭后，可通过检查IsCancel的值得知用户是否按下“取消”按钮
    // 基类包含两个事件：OkCommand和CancelCommand
    // 使用时将OkCommand绑定到“确定”按钮，将CancelCommand绑定到“取消”按钮，并传递当前对话框作为命令参数
    class DlgViewModelBase : ViewModelBase
    {
        public bool IsCancel { get; set; } = true;
        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DlgViewModelBase()
        {
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Ok(object obj)
        {
            IsCancel = false;
            ((Window)obj).Close();
        }

        private void Cancel(object obj)
        {
            IsCancel = true;
            ((Window)obj).Close();
        }
    }
}
