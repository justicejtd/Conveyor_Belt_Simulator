using ProcP.Models;
using System.Diagnostics;
using System.Windows;

namespace ProcP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //[DllImport("Shell32.dll")]
        //public static extern int SHChangeNotify(uint eventId, uint flags, IntPtr item1, IntPtr item2);
        //private const int SHCNE_ASSOCCHANGED = 0x8000000;
        //private const int SHCNF_FLUSH = 0x1000;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var filePath = Process.GetCurrentProcess().MainModule.FileName;
            FileAssociations.EnsureAssociationsSet(
                 new FileAssociation
                 {
                     Extension = ".procp",
                     ProgId = "ProcP",
                     FileTypeDescription = "PROCP File",
                     ExecutableFilePath = filePath
                 });
        }

        //public static bool IsAssociated()
        //{
        //    return Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Currentversion\\Explorer\\FileExts\\.procp", false) == null;
        //}

        //public static void Associate()
        //{
        //    RegistryKey fileReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.procp");
        //    RegistryKey appReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\Applications\\ProcP.exe");
        //    RegistryKey appAssoc = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.procp");

        //    string iconPath = Environment.CurrentDirectory + "img\\simulation.ico";
        //    fileReg.CreateSubKey("DefaultIcon").SetValue("", iconPath);
        //    fileReg.CreateSubKey("PerceivedType").SetValue("", "XML");

        //    appReg.CreateSubKey("shell\\open\\command").SetValue("", "\"" + Assembly.GetExecutingAssembly().Location + "\" %1");
        //    appReg.CreateSubKey("shell\\edit\\command").SetValue("", "\"" + Assembly.GetExecutingAssembly().Location + "\" %1");
        //    appReg.CreateSubKey("DefaultIcon").SetValue("", iconPath);

        //    appAssoc.CreateSubKey("UserChoice").SetValue("Progid", "Applications\\ProcP.exe");
        //    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
        //}
    }
}
