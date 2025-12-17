using Terminal.Gui.Configuration;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace HPing;

public class MainWindow : Window {
    
    public static string UserName { get; set; }

    public MainWindow ()
    {
        Title = $"HPing - ({Application.QuitKey} 退出)";

        
    }

    public override void EndInit ()
    {
        base.EndInit ();
        // Set the theme to "Anders" if it exists, otherwise use "Default"
        ThemeManager.Theme = ThemeManager.GetThemeNames ().FirstOrDefault (x => x == "Anders") ?? "Default";
    }
    
}