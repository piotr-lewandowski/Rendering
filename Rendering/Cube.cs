using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Vector3[] NormalVectors { get; }

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

            Points = new[]
            {
                new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(0,1,0), new Vector3(0,1,1),
                new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(1,1,0), new Vector3(1,1,1)
            };

            NormalVectors = new[]
            {
                new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1),
                new Vector3(0, 0, -1), new Vector3(0, 1, 0), new Vector3(0, -1, 0)
            };

            ModelFunction = model;
            Model = ModelFunction(0);
        }

        public void Rotate(float angle)
        {
            Model = ModelFunction(angle);
        }

        public void Draw()
        {
            var resultPoints = new List<Vector3>(Points.Length);
            var normalVectors = new List<Vector3>(NormalVectors.Length);
            foreach (var point in Points)
            {
                var matrix = _cubesImage.Projection * _cubesImage.View * Model;
                var vector = new Vector4(point, 1);

                var resultVector = SupportMatrices.Multiply(matrix, vector);
                resultVector /= resultVector.W;

                var resultPoint = new Vector3(
                    (float) (resultVector.X * _cubesImage.ActualWidth + _cubesImage.ActualWidth) / 2,
                    (float) (resultVector.Y * _cubesImage.ActualHeight + _cubesImage.ActualHeight) / 2,
                    resultVector.Z);

                resultPoints.Add(resultPoint);
            }

            var anchors = new[]
            {
                Points[0], Points[4], Points[1], Points[0], Points[2], Points[0]
            };

            for (var i = 0; i < normalVectors.Count; ++i)
            {
                normalVectors[i] = normalVectors[i] + anchors[i];
            }

            foreach (var normalVector in NormalVectors)
            {
                var matrix = _cubesImage.Projection * _cubesImage.View * Model;
                var vector = new Vector4(normalVector, 1);

                var resultVector = SupportMatrices.Multiply(matrix, vector);
                resultVector /= resultVector.W;

                var resultPoint = new Vector3(
                    (float) (resultVector.X * _cubesImage.ActualWidth + _cubesImage.ActualWidth) / 2,
                    (float) (resultVector.Y * _cubesImage.ActualHeight + _cubesImage.ActualHeight) / 2,
                    resultVector.Z);

                normalVectors.Add(resultPoint);
            }

            DrawFaces(resultPoints, normalVectors);
        }

        private void DrawFaces(List<Vector3> resultPoints, List<Vector3> normalVectors)
        {
            Fill(resultPoints[0], resultPoints[3], resultPoints[1], ColorsArray[0], normalVectors[0]);
            Fill(resultPoints[0], resultPoints[2], resultPoints[3], ColorsArray[0], normalVectors[0]);
            
            Fill(resultPoints[4], resultPoints[5], resultPoints[6], ColorsArray[1], normalVectors[1]);
            Fill(resultPoints[5], resultPoints[7], resultPoints[6], ColorsArray[1], normalVectors[1]);
            
            Fill(resultPoints[1], resultPoints[3], resultPoints[7], ColorsArray[2], normalVectors[2]);
            Fill(resultPoints[1], resultPoints[7], resultPoints[5], ColorsArray[2], normalVectors[2]);
            
            Fill(resultPoints[0], resultPoints[4], resultPoints[2], ColorsArray[3], normalVectors[3]);
            Fill(resultPoints[2], resultPoints[4], resultPoints[6], ColorsArray[3], normalVectors[3]);
            
            Fill(resultPoints[2], resultPoints[6], resultPoints[3], ColorsArray[4], normalVectors[4]);
            Fill(resultPoints[3], resultPoints[6], resultPoints[7], ColorsArray[4], normalVectors[4]);
            
            Fill(resultPoints[0], resultPoints[1], resultPoints[5], ColorsArray[5], normalVectors[5]);
            Fill(resultPoints[0], resultPoints[5], resultPoints[4], ColorsArray[5], normalVectors[5]);
        }

        private void Fill(Vector3 point1, Vector3 point2, Vector3 point3, Color color, Vector3 normalVector)
        {
            var viewVector = new Vector3(3, 0.5f, 0.5f);

            var matrix = _cubesImage.Projection * _cubesImage.View * SupportMatrices.ModelMatrix;
            var vector = new Vector4(viewVector, 1);

            var resultVector = SupportMatrices.Multiply(matrix, vector);

            viewVector = new Vector3(
                (float) (resultVector.X * _cubesImage.ActualWidth + _cubesImage.ActualWidth) / 2,
                (float) (resultVector.Y * _cubesImage.ActualHeight + _cubesImage.ActualHeight) / 2,
                resultVector.Z);

            var n = Vector3.Cross(point1 - point2, point1 - point3);

            if (Vector3.Dot(point1-viewVector, n) > 0)
            {
                return;
            }

            var polygon = new Polygon(new[] { point1, point2, point3 }, normalVector);
            polygon.Fill(_cubesImage, color);
        }
    }
}
