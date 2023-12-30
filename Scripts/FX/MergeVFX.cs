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
				AlbedoTexture = GD.Load<CompressedTexture2D>("res://Assets/Textures/Sprites/spriteSheet.png"),
				AlbedoColor = colorB,
				VertexColorUseAsAlbedo = true,
				BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				BlendMode = BaseMaterial3D.BlendModeEnum.Add,
				ParticlesAnimHFrames = 4,
				ParticlesAnimVFrames = 3,
				ParticlesAnimLoop = true,
				

			},
			Size = Vector2.One * 2f,
		};

		OneShot = true;
		Amount = 60;
		Lifetime = 1.2f;
		ProcessMaterial = new ParticleProcessMaterial(){
			LifetimeRandomness = 1f,
			Spread = 180f,
			InitialVelocityMax = 10f,
			DampingMax = 10f,
			EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Ring,
			EmissionRingAxis = new Vector3(0f, 0f, 1f),
			EmissionRingRadius = 1.5f,
			EmissionRingInnerRadius = 0.5f,
			Gravity = Vector3.Zero,
			ScaleMin = 0.5f,
			ScaleMax = 1.5f,
			Color = Colors.White,
			ColorRamp = new GradientTexture2D(){
				Gradient = new Gradient(){
					Colors = new Color[]{Colors.White, colorA, colorB},
					Offsets = new float[]{0.1f, 0.5f, 0.9f},
					InterpolationMode = Gradient.InterpolationModeEnum.Cubic,
				},
				
			},
			AnimSpeedMax = 1f,
			AnimOffsetMax = 4f,
		};
		Emitting = true;
		SetDisableScale(true);
	}
}
