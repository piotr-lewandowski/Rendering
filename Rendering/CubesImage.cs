using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rendering.Figures;
using Rendering.LightSource;

namespace Rendering;

public class CubesImage : Canvas
{
    public bool Fog { get; set; }
    public bool BackFaceCulling { get; set; }
    public ShadingMode ShadingMode { get; set; }
    public LightSource.LightSource[] LightSources { get; set; }
    public List<LightSource.LightSource> ActiveLightSources { get; set; }
    public Camera[] Cameras { get; set; }
    public Camera CurrentCamera { get; set; }
    public Figure[] Figures { get; set; }
    public Matrix4x4 Projection { get; set; }
    public Matrix4x4 View => CurrentCamera.ViewMatrix;
    private float _fov;
    private float _aspectRatio;

    public float Fov
    {
        get => _fov;
        set
        {
            _fov = value;
            UpdateProjectionMatrix();
        }
    }

    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            UpdateProjectionMatrix();
        }
    }

    public WriteableBitmap Bitmap { get; set; }
    public float[,] ZIndex { get; set; }
    public int[,] ColorsArray { get; set; }

    public CubesImage()
    {
        BackFaceCulling = false;
        Fog = false;
        Figures = new Figure[]
        {
            new Cube(this, Colors.BurlyWood)
            {
                TranslationVector = new Vector3(0, 1, 1)
            },
            new Cube(this, Colors.Crimson)
            {
                TranslationVector = new Vector3(0, 2.5f, 1)
            },
            new Cube(this, Colors.Orange)
            {
                TranslationVector = new Vector3(0, -1.5f, 1)
            },
            new Sphere(this, Colors.BlueViolet)
            {
                TranslationVector = new Vector3(2, 0, 0)
            },
            new Square(this, Colors.ForestGreen)
            {
                RotationQuaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathF.PI / 2),
                TranslationVector = new Vector3(0, 0, 6),
                Scale = 6
            }
        };
        Cameras = new[]
        {
            new Camera(new Vector3(5, 0.5f, 0.5f), new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0, 1)),
            new Camera(new Vector3(5, 0.5f, 0.5f), new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0, 1)),
            new Camera(new Vector3(5, 5f, 0.5f), new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0, 1)),
            new Camera(new Vector3(5, -5f, 0.5f), new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0, 1))
        };
        CurrentCamera = Cameras[0];
        ShadingMode = ShadingMode.Constant;
        LightSources = new LightSource.LightSource[]
        {
            new PointLight(Colors.White, new Vector3(-10, -10, 10)),
            new DirectionalLight(Colors.White, new Vector3(0, -5, 10)),
            new DirectionalLight(Colors.White, new Vector3(0, 5, -10)),
            new SpotLight(Colors.White, new Vector3(1, 1, -10), new Vector3(0, 0, 1), 30)
        };
        ActiveLightSources = new List<LightSource.LightSource>();
    }

    private void UpdateProjectionMatrix() => Projection = Utility.ProjectionMatrix(Fov, AspectRatio);

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
                var pBackBuffer = Bitmap.BackBuffer;

                for (var j = 0; j < height; ++j)
                {
                    for (var i = 0; i < width; ++i)
                    {
                        pBackBuffer += 4;

                        *(int*)pBackBuffer = ColorsArray[i, j];
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
        var width = (int)size.Width;
        var height = (int)size.Height;

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

        AspectRatio = (float)(size.Width / size.Height);
        return size;
    }
}