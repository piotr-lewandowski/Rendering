using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using Rendering.Figures;

namespace Rendering
{
    public partial class MainWindow : Window
    {
        private static IProgress<float> _progress;

        public MainWindow()
        {
            InitializeComponent();

            _progress = new Progress<float>(MoveScene);
            var timer = new Timer(34);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private int _movementCounter = 600;

        private void MoveScene(float a)
        {
            var c1 = CubesImage.Figures[0] as Cube; 
            var translation = new Vector3(0, 0, 0.01f);
            if (_movementCounter >= 300)
            {
                c1.TranslationVector += translation;
                CubesImage.Cameras[0].Position += translation;
                CubesImage.Cameras[1].Target += translation;
            }
            else
            {
                c1.TranslationVector -= translation;
                CubesImage.Cameras[0].Position -= translation;
                CubesImage.Cameras[1].Target -= translation;
            }
            _movementCounter = (_movementCounter + 1) % 600;

            var c2 = CubesImage.Figures[1] as Cube;
            c2.RotationQuaternion *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1f.ToRadians());
            
            var c3 = CubesImage.Figures[2] as Cube;
            c3.RotationQuaternion *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, 1f.ToRadians());

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

        private void ChangeCamera1(object sender, RoutedEventArgs e)
        {
            CubesImage.CurrentCamera = CubesImage.Cameras[0];
        }
        private void ChangeCamera2(object sender, RoutedEventArgs e)
        {
            CubesImage.CurrentCamera = CubesImage.Cameras[1];
        }
        private void ChangeCamera3(object sender, RoutedEventArgs e)
        {
            CubesImage.CurrentCamera = CubesImage.Cameras[2];
        }
    }
}
