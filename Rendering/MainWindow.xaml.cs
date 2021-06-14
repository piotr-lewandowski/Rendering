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

        private int _movementCounter = 400;

        private void MoveScene(float a)
        {
            var c1 = CubesImage.Figures[0] as Cube; 
            var translation = new Vector3(0, 0, 0.01f);
            if (_movementCounter >= 200)
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
            _movementCounter = (_movementCounter + 1) % 400;

            c1.RotationQuaternion *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1,1,0)), 2f.ToRadians());


            var c2 = CubesImage.Figures[1] as Cube;
            c2.RotationQuaternion *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1f.ToRadians());
            
            var c3 = CubesImage.Figures[2] as Cube;
            c3.RotationQuaternion *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, 1f.ToRadians());

            CubesImage.InvalidateVisual();
        }

        #region EventHandlers

        private void Fov_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => CubesImage.Fov = (float) e.NewValue;

        private static void OnTimedEvent(object source, ElapsedEventArgs e) => _progress.Report(0);

        private void ChangeCamera1(object sender, RoutedEventArgs e) => CubesImage.CurrentCamera = CubesImage.Cameras[0];

        private void ChangeCamera2(object sender, RoutedEventArgs e) => CubesImage.CurrentCamera = CubesImage.Cameras[1];

        private void ChangeCamera3(object sender, RoutedEventArgs e) => CubesImage.CurrentCamera = CubesImage.Cameras[2];

        private void ChangeCamera4(object sender, RoutedEventArgs e) => CubesImage.CurrentCamera = CubesImage.Cameras[3];

        private void ShadingPhong(object sender, RoutedEventArgs e) => CubesImage.ShadingMode = ShadingMode.Phong;

        private void ShadingGouraud(object sender, RoutedEventArgs e) => CubesImage.ShadingMode = ShadingMode.Gouraud;

        private void ShadingConstant(object sender, RoutedEventArgs e) => CubesImage.ShadingMode = ShadingMode.Constant;

        private void EnableCulling(object sender, RoutedEventArgs e) => CubesImage.BackFaceCulling = true;

        private void DisableCulling(object sender, RoutedEventArgs e) => CubesImage.BackFaceCulling = false;

        private void EnableFog(object sender, RoutedEventArgs e) => CubesImage.Fog = true;

        private void DisableFog(object sender, RoutedEventArgs e) => CubesImage.Fog = false;

        private void EnableLight1(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[0];
            CubesImage.ActiveLightSources.Add(source);
        }

        private void DisableLight1(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[0];
            CubesImage.ActiveLightSources.Remove(source);
        }

        private void EnableLight2(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[1];
            CubesImage.ActiveLightSources.Add(source);
        }

        private void DisableLight2(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[1];
            CubesImage.ActiveLightSources.Remove(source);
        }

        private void EnableLight3(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[2];
            CubesImage.ActiveLightSources.Add(source);
        }

        private void DisableLight3(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[2];
            CubesImage.ActiveLightSources.Remove(source);
        }

        private void EnableLight4(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[3];
            CubesImage.ActiveLightSources.Add(source);
        }

        private void DisableLight4(object sender, RoutedEventArgs e)
        {
            var source = CubesImage.LightSources[3];
            CubesImage.ActiveLightSources.Remove(source);
        }

        #endregion
    }
}
