using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitHP
{
    class RibbonPanel : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            var asm = typeof(RibbonPanel).Assembly;

            var panelHP = application.CreateRibbonPanel("RevitHP");

            var btnFamilyBrowser = new PushButtonData("HitMe", "族库", asm.Location, typeof(BtnFamilyBrowser).FullName);
            btnFamilyBrowser.LargeImage = new BitmapImage(new Uri($"pack://application:,,,/{asm.FullName};component/Pixadex.png", UriKind.RelativeOrAbsolute));
            panelHP.AddItem(btnFamilyBrowser);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class BtnFamilyBrowser : IExternalCommand
    {
        public static ExternalEvent s_event = null;
       
        
    

        private static UIApplication s_app = null;

        public static UIDocument GetDoc()
        {
            if (s_app == null) return null;

            return s_app.ActiveUIDocument;
        }
        public static ExternalEvent GetEvent()
        {
            return s_event;
        }


        // 共享一个window
        private static MainWindow m_wnd = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if(m_wnd == null)
            {
                m_wnd = new MainWindow();
                var iop = new WindowInteropHelper(m_wnd);
                iop.Owner = Process.GetCurrentProcess().MainWindowHandle;
            }

            m_wnd.Show();
            if (m_wnd.WindowState == WindowState.Minimized) {
                m_wnd.WindowState = WindowState.Normal;
            }
            m_wnd.Activate();

            s_app = commandData.Application;
            if (s_event==null)
            {
             ExternalEventExample external = new ExternalEventExample();
            s_event = ExternalEvent.Create(external);
            }
           
            
            return Result.Succeeded;
        }
    }
}
