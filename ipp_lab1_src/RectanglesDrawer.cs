using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ipp_lab1_src;

public class RectanglesDrawer
{
    private Thread _drawingThread;
    private AutoResetEvent _resetEvent;
    private Canvas _canvas;
    private Dispatcher _dispatcher;

    public RectanglesDrawer(Dispatcher dispatcher,Canvas canvas)
    {
        if (!Directory.Exists("log\\")) Directory.CreateDirectory("log\\");


        _resetEvent = new AutoResetEvent(false);

        _drawingThread = new Thread(Draw);
        _drawingThread.IsBackground = true;
        _drawingThread.Start();

        _canvas = canvas;

        _dispatcher = dispatcher;
    }

    public void StartDrawing() 
    {
        _resetEvent.Set();
    }

    private void Draw()
    {
        while (true)
        {
            _resetEvent.WaitOne();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _dispatcher.Invoke(() =>
            {
                _canvas.Children.Clear();
                Random random = new Random();
                for (int i = 0; i < _canvas.ActualWidth / 20; i++)
                {
                    for (int j = 0; j < _canvas.ActualHeight / 10; j++)
                    {

                        var red = (byte)random.Next(256);
                        var green = (byte)random.Next(256);
                        var blue = (byte)random.Next(256);
                        Color color = Color.FromRgb(red, green, blue);
                        Rectangle rectangle = new Rectangle()
                        {
                            Width = 20,
                            Height = 10,
                            Fill = new SolidColorBrush(color)

                        };
                        _canvas.Children.Add(rectangle);
                        Canvas.SetTop(rectangle, j * 10);
                        Canvas.SetLeft(rectangle, i * 20);
                    }
                }

                stopwatch.Stop();
                WriteExecutionTimeToFile(stopwatch.ElapsedMilliseconds, _canvas.ActualWidth, _canvas.ActualHeight);
            });
        }
    }

    private void WriteExecutionTimeToFile(long time, double width, double height)
    {
        using (var mutex = new Mutex(false, "log.txt"))
        {
            mutex.WaitOne(Timeout.Infinite, false);
            File.AppendAllText("log\\log.txt", $"size: {(int)width}/{(int)height}  time: {time}\n");
        }
    }
}
