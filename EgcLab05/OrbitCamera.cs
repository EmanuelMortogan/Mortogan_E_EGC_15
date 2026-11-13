using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Egc;

public class OrbitCamera : Camera
{
    public Vector3 Target { get; private set; }
    public float Distance { get; private set; }
    public float Azimuth { get; private set; }
    public float Elevation { get; private set; }

    public const float MaxElevation = 89.0f;
    public const float MinDistance = 2.0f;
    public const float MaxDistance = 30.0f;
    public float MouseSensitivity = 0.3f;
    public float RotationSpeed = 90.0f;
    public float DistanceStep = 0.5f;

    private static readonly Vector3 WorldUp = Vector3.UnitY;
    private static readonly Vector3 StartTarget = new(0.0f, 1.2f, -2.0f);
    private const float StartDistance = 8.0f;
    private const float StartAzimuth = 90.0f;
    private const float StartElevation = 20.0f;

    public OrbitCamera()
    {
        Reset();
    }

    public override string Name => "orbită";

    public override Vector3 Position
    {
        get
        {
            float azimuth = MathHelper.DegreesToRadians(Azimuth);
            float elevation = MathHelper.DegreesToRadians(Elevation);
            Vector3 direction = new(
                MathF.Cos(elevation) * MathF.Cos(azimuth),
                MathF.Sin(elevation),
                MathF.Cos(elevation) * MathF.Sin(azimuth));
            return Target + Distance * direction;
        }
    }

    public override Vector3 Front => (Target - Position).Normalized();

    public override Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, WorldUp);

    public override void Update(KeyboardState keyboard, MouseState mouse, Vector2 mouseDelta, float dt)
    {
        if (mouse.IsButtonDown(MouseButton.Left))
        {
            Azimuth += mouseDelta.X * MouseSensitivity;
            Elevation += mouseDelta.Y * MouseSensitivity;
        }

        if (keyboard.IsKeyDown(Keys.Left))  Azimuth -= RotationSpeed * dt;
        if (keyboard.IsKeyDown(Keys.Right)) Azimuth += RotationSpeed * dt;
        if (keyboard.IsKeyDown(Keys.Up))    Elevation += RotationSpeed * dt;
        if (keyboard.IsKeyDown(Keys.Down))  Elevation -= RotationSpeed * dt;

        Elevation = Math.Clamp(Elevation, -MaxElevation, MaxElevation);

        Distance = Math.Clamp(Distance - mouse.ScrollDelta.Y * DistanceStep, MinDistance, MaxDistance);
    }

    public override void Reset()
    {
        Target = StartTarget;
        Distance = StartDistance;
        Azimuth = StartAzimuth;
        Elevation = StartElevation;
        FieldOfView = 60.0f;
    }

    public override IEnumerable<string> Describe()
    {
        Vector3 position = Position;
        yield return $"Țintă          : ({Target.X,6:F2}, {Target.Y,6:F2}, {Target.Z,6:F2})";
        yield return $"Distanță       : {Distance,6:F2}";
        yield return $"Azimut / elev. : {Azimuth,7:F1}° / {Elevation,6:F1}°";
        yield return $"Poziție        : ({position.X,6:F2}, {position.Y,6:F2}, {position.Z,6:F2})";
    }
}
