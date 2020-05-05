using LeYun.Model;
using LeYun.View;
using LeYun.View.Dlg;
using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace LeYun
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        // 应用程序启动
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 设置编辑框行为
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            // 初始化窗口和页面
            Splash splash = new Splash();
            MainWindow mainWindow = new MainWindow();
            GlobalData.InitPages();

            if (!DesignerProperties.GetIsInDesignMode(mainWindow))
            {
                // 读取设置
                GlobalData.ReadSettings();
            } 

            // 如果软件未激活，则弹出广告
            if (!GlobalData.IsActive)
            {
                AdDlg adDlg = new AdDlg();
                adDlg.ShowDialog();
            }

            // 显示启动页
            splash.ShowDialog();

            // 显示主窗口
            mainWindow.ShowDialog();

            // 退出程序
            Shutdown();
        }

        // 应用程序退出
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // 保存设置
            GlobalData.SaveSettings();
        }
    }
}
