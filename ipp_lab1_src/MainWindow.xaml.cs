using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ipp_lab1_src
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread drawingThread;
        private AutoResetEvent _resetEvent;

        public MainWindow()
        {
            InitializeComponent();

            _resetEvent = new AutoResetEvent(false);
            
            drawingThread = new Thread(DrawRandomRectangles);
            drawingThread.IsBackground = true;
            drawingThread.Start();

            if (!Directory.Exists("log\\")) Directory.CreateDirectory("log\\");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StartDrawing();
        }

        private void StartDrawing()
        {
            // Запускаємо потік, який буде малювати прямокутники
            _resetEvent.Set();
        }

        private void DrawRandomRectangles()
        {
            // Метод, який малює різнобарвні прямокутники у випадкових місцях на canvas
            while (true)
            {
                _resetEvent.WaitOne();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Dispatcher.Invoke(() =>
                {
                    canvas.Children.Clear();
                    Random random = new Random();
                    for (int i = 0; i < canvas.ActualWidth / 20; i++)
                    {
                        for (int j = 0; j < canvas.ActualHeight / 10; j++)
                        {
                            
                            var red = (byte)random.Next(256);
                            var green = (byte)random.Next(256);
                            var blue = (byte)random.Next(256);
                            Color color = Color.FromRgb(red,green,blue);
                            Rectangle rectangle = new Rectangle()
                            {
                                Width = 20,
                                Height = 10,
                                Fill = new SolidColorBrush(color),
                                
                            };
                            canvas.Children.Add(rectangle);
                            Canvas.SetTop(rectangle,j*10);
                            Canvas.SetLeft(rectangle,i*20);
                        }
                    }
                });

                stopwatch.Stop();
                WriteExecutionTimeToFile(stopwatch.ElapsedMilliseconds, canvas.ActualWidth, canvas.ActualHeight);

                //Thread.Sleep(100); // Пауза між малюванням прямокутників
            }
        }

        private void WriteExecutionTimeToFile(long time, double width, double height)
        {
            using (var mutex = new Mutex(false, "log.txt"))
            {
                mutex.WaitOne(Timeout.Infinite,false);
                File.AppendAllText("log\\log.txt", $"size: {(int)width}/{(int)height}  time: {time}\n");
            }
        }

    }
}
