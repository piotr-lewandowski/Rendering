using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rendering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IProgress<float> _progress;

        public MainWindow()
        {
            InitializeComponent();

            _progress = new Progress<float>(RotateCube);
            var timer = new Timer(17);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void RotateCube(float a)
        {
            foreach (var cube in CubesImage.Cubes)
            {
                cube._angle = (cube._angle + 1f) % 360;
                cube.Rotate(cube._angle);
            }
            CubesImage.InvalidateVisual();
        }

        private void Fov_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CubesImage.Fov = (float) e.NewValue;
            CubesImage.InvalidateVisual();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _progress.Report(0);
        }
    }
}
