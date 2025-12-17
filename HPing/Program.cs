using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using HPing.Utils;
using Terminal.Gui.Configuration;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace HPing;


class Program {
    
    [RequiresUnreferencedCode ("Calls Terminal.Gui.Application.Init(IConsoleDriver, String)")]
    [RequiresDynamicCode ("Calls Terminal.Gui.Application.Init(IConsoleDriver, String)")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ArgumentOptions))]
    static void Main(string[] args) {

        var am = new ArgumentManager(args);
        if (am.HasHelpArgument()) {
            Help.PrintHelp();
            return;
        }
        
        
        #region The code in this region is not intended for use in a native Aot self-contained. It's just here to make sure there is no functionality break with localization in Terminal.Gui using self-contained

        if (Equals(Thread.CurrentThread.CurrentUICulture, CultureInfo.InvariantCulture) && Application.SupportedCultures!.Count == 0)
        {
            // Only happens if the project has <InvariantGlobalization>true</InvariantGlobalization>
            Debug.Assert (Application.SupportedCultures.Count == 0);
        }
        else
        {
            Debug.Assert (Application.SupportedCultures!.Count > 0);
            Debug.Assert (Equals (CultureInfo.CurrentCulture, Thread.CurrentThread.CurrentUICulture));
        }

        #endregion
        
        
        // Override the default configuration for the application to use the Light theme
        //ConfigurationManager.RuntimeConfig = """{ "Theme": "Light" }""";
        ConfigurationManager.Enable(ConfigLocations.All);
        

        Application.Run<SingleView>().Dispose();

        // Before the application exits, reset Terminal.Gui for clean shutdown
        Application.Shutdown();
    }
}