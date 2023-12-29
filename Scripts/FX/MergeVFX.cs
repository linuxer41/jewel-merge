using Godot;

public partial class MergeVFX : GpuParticles3D
{
	public int Level {get; set; } = 1;
	public Color colorA {get; set; } = Colors.Red;
	public Color colorB {get; set; } = Colors.Green;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawPass1 = new QuadMesh(){
			Material = new StandardMaterial3D(){
				AlbedoTexture = GD.Load<CompressedTexture2D>($"res://Assets/Textures/Sprites/{Level}.png"),
				AlbedoColor = colorB,
				VertexColorUseAsAlbedo = true,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				BlendMode = BaseMaterial3D.BlendModeEnum.Add,
			},
			Size = Vector2.One * 1f,
		};

		OneShot = true;
		Amount = 180;
		Lifetime = 1.2f;
		ProcessMaterial = new ParticleProcessMaterial(){
			LifetimeRandomness = 1f,
			Spread = 180f,
			InitialVelocityMax = 5f,
			InitialVelocityMin = 0.2f,
			CollisionMode = ParticleProcessMaterial.CollisionModeEnum.Rigid,
			CollisionBounce = 0.2f,
			EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Ring,
			EmissionRingAxis = new Vector3(0f, 0f, 1f),
			EmissionRingRadius = 1.5f,
			EmissionRingInnerRadius = 0.5f,
			Gravity = Vector3.Zero,
			ParticleFlagDisableZ = true,
			RadialVelocityMin = 0.5f,
			RadialVelocityMax = 3f,
			ScaleMin = 0.3f,
			ScaleMax = 2.5f,
			Color = Colors.White,
			ColorRamp = new GradientTexture2D(){
				Gradient = new Gradient(){
					Colors = new Color[]{colorA, colorB},
					Offsets = new float[]{0.1f, 0.55f},
					InterpolationMode = Gradient.InterpolationModeEnum.Cubic,
				},
				
			},
		};
		Emitting = true;
		SetDisableScale(true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
