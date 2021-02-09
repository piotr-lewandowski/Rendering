using Rendering.PolygonFill;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rendering
{
    public class CubesImage : Canvas
    {
        public Figure[] Figures { get; set; }
        public Matrix4x4 Projection { get; set; }
        public Matrix4x4 View { get; set; }
        private float _fov;
        private float _aspectRatio;
        public float Fov
        {
            get => _fov;
            set
            {
                _fov = value;
                UpdateProjectionMatrix();
                InvalidateVisual();
            }
        }
        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                _aspectRatio = value;
                UpdateProjectionMatrix();
                InvalidateVisual();
            }
        }
        public WriteableBitmap Bitmap { get; set; }
        public float[,] ZIndex { get; set; }
        public int[,] ColorsArray { get; set; }

        public CubesImage()
        {
            Figures = new Figure[] { new Cube(this, SupportMatrices.ModelMatrix1(50)), new Cube(this, SupportMatrices.ModelMatrix2(0)) };
            View = SupportMatrices.ViewMatrix;
        }

        void UpdateProjectionMatrix()
        {
            Projection = SupportMatrices.ProjectionMatrix(Fov, AspectRatio);
        }

        protected override void OnRender(DrawingContext dc)
        {
            foreach (var figure in Figures)
            {
                figure.Draw();
            }

            unsafe
            {
                try
                {
                    Bitmap.Lock();
                    var width = Bitmap.PixelWidth;
                    var height = Bitmap.PixelHeight;
                    IntPtr pBackBuffer = Bitmap.BackBuffer;

                    for (int j = 0; j < height; ++j)
                    {
                        for (int i = 0; i < width; ++i)
                        {
                            pBackBuffer += 4;

                            *(int*) pBackBuffer = ColorsArray[i, j];
                        }
                    }

                    Bitmap.AddDirtyRect(new Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight));
                }
                finally
                {
                    Bitmap.Unlock();
                }
            }

            base.OnRender(dc);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            var width = (int) size.Width;
            var height = (int) size.Height;

            Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            ZIndex = new float[width, height];
            ColorsArray = new int[width, height];
            Background = new ImageBrush(Bitmap);

            for (var i = 0; i < width; ++i)
            {
                for (var j = 0; j < height; ++j)
                {
                    ZIndex[i, j] = float.MaxValue;
                    ColorsArray[i, j] = 0xFFFFFF;
                }
            }

            AspectRatio = (float) (size.Width / size.Height);
            return size;
        }
    }
}
