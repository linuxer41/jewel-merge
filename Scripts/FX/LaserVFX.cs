using Godot;
using System;

public partial class LaserVFX : GpuParticles3D
{
	public Color color {get; set; } = Colors.Red;
    public override void _Ready()
    {
        ParticleProcessMaterial processMaterial = new ParticleProcessMaterial
        {
            ParticleFlagAlignY = true,
            ParticleFlagDisableZ = true,
            Direction = new Vector3(0, -1, 0),
            Spread = 0.0f,
            InitialVelocityMin = 9.8f,
            InitialVelocityMax = 9.8f,
            Gravity = new Vector3(0, 0, 0)
        };


        // Crea el Mesh
        StandardMaterial3D material = new StandardMaterial3D
        {
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            BlendMode = BaseMaterial3D.BlendModeEnum.Add,
            AlbedoColor = color,
        };

        SphereMesh mesh = new SphereMesh
        {
            Material = material,
            Radius = 0.1f
        };

        DrawPass1 = mesh; 
        Amount = 150;
        Lifetime = 3.0f;
        SpeedScale = 2.0f;
        FixedFps = 60;
        CollisionBaseSize = 1.0f; ;
        VisibilityAabb = new Aabb(-1.00001f, -45.59f, -1.00001f,  2.00002f, 46.5893f, 2.00002f);;
        LocalCoords = true;
        ProcessMaterial = processMaterial;
        DrawPass1 = DrawPass1;
        SetDisableScale(true);
        Position = new Vector3(0, -0.5f, 0);
    }
}
