using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;

//model:
//pozycja
//obrót
//skala

namespace Rendering
{
    public class Cube : Figure
    {
        public float _angle = 0f;

        private readonly Color[] ColorsArray =
        {
            Colors.Black, Colors.Gray, Colors.Gold, Colors.Red,
            Colors.Blue, Colors.Orange, Colors.Firebrick, Colors.Silver,
            Colors.Cyan, Colors.Pink, Colors.Coral, Colors.DarkRed
        };
        public Vector3[] Points { get; set; }
        public Vector3[] NormalVectors { get; }

        public Cube(CubesImage canvas) : base(canvas)
        {
            Points = new[]
            {
                new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
                new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1)
            };

            NormalVectors = new[]
            {
                new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1),
                new Vector3(0, 0, -1), new Vector3(0, 1, 0), new Vector3(0, -1, 0)
            };
        }

        public override void Draw()
        {
            var resultPoints = new List<Vector3>(Points.Length);
            var normalVectors = new List<Vector3>(NormalVectors.Length);
            foreach (var point in Points)
            {
                var matrix = Canvas.Projection * Canvas.View * Model;
                var vector = new Vector4(point, 1);

                var resultVector = SupportMatrices.Multiply(matrix, vector);
                resultVector /= resultVector.W;

                var resultPoint = new Vector3(
                    (float) (resultVector.X * Canvas.ActualWidth + Canvas.ActualWidth) / 2,
                    (float) (resultVector.Y * Canvas.ActualHeight + Canvas.ActualHeight) / 2,
                    resultVector.Z);

                resultPoints.Add(resultPoint);
            }

            var anchors = new[]
            {
                Points[0], Points[4], Points[1], Points[0], Points[2], Points[0]
            };

            for (var i = 0; i < normalVectors.Count; ++i) normalVectors[i] = normalVectors[i] + anchors[i];

            foreach (var normalVector in NormalVectors)
            {
                var matrix = Canvas.Projection * Canvas.View * Model;
                var vector = new Vector4(normalVector, 1);

                var resultVector = SupportMatrices.Multiply(matrix, vector);
                resultVector /= resultVector.W;

                var resultPoint = new Vector3(
                    (float) (resultVector.X * Canvas.ActualWidth + Canvas.ActualWidth) / 2,
                    (float) (resultVector.Y * Canvas.ActualHeight + Canvas.ActualHeight) / 2,
                    resultVector.Z);

                normalVectors.Add(resultPoint);
            }

            DrawFaces(resultPoints, normalVectors);
        }

        public void DrawFaces(List<Vector3> resultPoints, List<Vector3> normalVectors)
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
    }
}