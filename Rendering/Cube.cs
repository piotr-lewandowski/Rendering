using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Timers;
using System.Linq;
using Rendering.PolygonFill;

//model:
//pozycja
//obrót
//skala

namespace Rendering
{
    class Cube
    {
        public Func<float, Matrix4x4> ModelFunction;
        public float _angle = 0f;
        public Vector3[] Points;
        public Matrix4x4 Model;
        private CubesImage _cubesImage;
        private Color[] ColorsArray = {
            Colors.Black, Colors.Gray, Colors.Gold, Colors.Red,
            Colors.Blue, Colors.Orange, Colors.Firebrick, Colors.Silver,
            Colors.Cyan, Colors.Pink, Colors.Coral, Colors.DarkRed
        };

        public Cube(CubesImage image)
        {
            _cubesImage = image;

            Points = new[] {
                new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(0,1,0), new Vector3(0,1,1),
                new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(1,1,0), new Vector3(1,1,1)  };

            Model = SupportMatrices.ModelMatrix;
        }
        public Cube(CubesImage image, Func<float, Matrix4x4> model)
        {
            _cubesImage = image;

            Points = new[] {
                new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(0,1,0), new Vector3(0,1,1),
                new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(1,1,0), new Vector3(1,1,1)  };

            ModelFunction = model;
            Model = ModelFunction(0);
        }

        public void Rotate(float angle)
        {
            Model = ModelFunction(angle);
        }

        public void Draw()
        {
            var resultPoints = new List<Vector3>();
            foreach (var point in Points)
            {
                var matrix = _cubesImage.Projection * _cubesImage.View * Model;
                var vector = new Vector4(point, 1);

                var resultVector = SupportMatrices.Multiply(matrix, vector);
                resultVector /= resultVector.W;

                var resultPoint = new Vector3(
                    (float)(resultVector.X * _cubesImage.ActualWidth + _cubesImage.ActualWidth) / 2,
                    (float)(resultVector.Y * _cubesImage.ActualHeight + _cubesImage.ActualHeight) / 2,
                    resultVector.Z);

                resultPoints.Add(resultPoint);
            }
            DrawFaces(resultPoints);
        }

        private void DrawFaces(List<Vector3> resultPoints)
        {
            Fill(resultPoints[0], resultPoints[3], resultPoints[1], ColorsArray[0]);
            Fill(resultPoints[0], resultPoints[2], resultPoints[3], ColorsArray[0]);
            Fill(resultPoints[4], resultPoints[7], resultPoints[5], ColorsArray[1]);
            Fill(resultPoints[4], resultPoints[6], resultPoints[7], ColorsArray[1]);
            Fill(resultPoints[1], resultPoints[3], resultPoints[7], ColorsArray[2]);
            Fill(resultPoints[1], resultPoints[7], resultPoints[5], ColorsArray[2]);
            Fill(resultPoints[0], resultPoints[2], resultPoints[6], ColorsArray[3]);
            Fill(resultPoints[0], resultPoints[6], resultPoints[4], ColorsArray[3]);
            Fill(resultPoints[2], resultPoints[6], resultPoints[7], ColorsArray[4]);
            Fill(resultPoints[2], resultPoints[7], resultPoints[3], ColorsArray[4]);
            Fill(resultPoints[0], resultPoints[4], resultPoints[5], ColorsArray[5]);
            Fill(resultPoints[0], resultPoints[5], resultPoints[1], ColorsArray[5]);
        }

        private void Fill(Vector3 point1, Vector3 point2, Vector3 point3, Color color)
        {
            var polygon = new Polygon(new[] { point1, point2, point3 });

            polygon.Fill(_cubesImage, color);
        }
    }
}
