using LeYun.View.Dlg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.ViewModel
{
    class AboutPageViewModel : ViewModelBase
    {
        // 命令
        public DelegateCommand ViewTeamCommand { get; }
        public DelegateCommand ViewHelpCommand { get; }
        public DelegateCommand ViewUpdateLogCommand { get; }
        public DelegateCommand ViewBusinessCommand { get; }

        public AboutPageViewModel()
        {
            ViewTeamCommand = new DelegateCommand(ViewTeam);
            ViewHelpCommand = new DelegateCommand(ViewHelp);
            ViewUpdateLogCommand = new DelegateCommand(ViewUpdateLog);
            ViewBusinessCommand = new DelegateCommand(ViewBusiness);
        }

        private void ViewBusiness(object obj)
        {
            MsgBox.Show("商务合作");
        }

        // 查看更新日志
        private void ViewUpdateLog(object obj)
        {
            UpdateLogDlg dlg = new UpdateLogDlg();
            dlg.ShowDialog();
        }

        // 查看使用帮助
        private void ViewHelp(object obj)
        {
            MsgBox.Show("使用帮助");
        }

        // 查看开发团队
        private void ViewTeam(object obj)
        {
            MsgBox.Show("开发团队");
        }
    }
}
