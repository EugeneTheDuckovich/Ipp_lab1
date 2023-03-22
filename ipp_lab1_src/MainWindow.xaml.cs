using System.IO;
using System.Windows;

namespace ipp_lab1_src;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private RectanglesDrawer _rectanglesDrawer;

    public MainWindow()
    {
        InitializeComponent();

        if (!Directory.Exists("log\\")) Directory.CreateDirectory("log\\");

        _rectanglesDrawer = new RectanglesDrawer(Dispatcher, canvas);
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _rectanglesDrawer.StartDrawing();
    }

}