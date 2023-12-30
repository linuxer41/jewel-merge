using Godot;
using System;

public partial class LaserVFX : GpuParticles3D
{
	public Color color {get; set; } = Colors.Red;
    public override void _Ready()
    {
        ParticleProcessMaterial processMaterial = new ParticleProcessMaterial
        {
            Direction = new Vector3(0, 1, 0),
            Spread = 80f,
            InitialVelocityMin = 0.2f,
            InitialVelocityMax = 3f,
            Gravity = new Vector3(0, 0, 0)
        };


        // Crea el Mesh
        StandardMaterial3D material = new StandardMaterial3D
        {
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            AlbedoColor = new Color(color.R, color.G, color.B, 0.5f),
            EmissionEnabled = true,
            EmissionEnergyMultiplier = 10f,
            Emission = color,
            // lasser effect

        };

        BoxMesh mesh = new BoxMesh
        {
            Material = material,
            Size = new Vector3(0.1f, 0.1f, 0.1f)
        };
        Amount = 30;
        Lifetime = 0.2f;
        CollisionBaseSize = 1.0f;
        ProcessMaterial = processMaterial;
        DrawPass1 = mesh;
    }
}
