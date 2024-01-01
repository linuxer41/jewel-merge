using Godot;

public partial class AuraVFX : GpuParticles3D
{
	public int Level {get; set; } = 1;
	public Color color {get; set; } = Colors.Red;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawPass1 = new QuadMesh(){
			Material = new StandardMaterial3D(){
				AlbedoTexture = GD.Load<CompressedTexture2D>("res://Assets/Textures/Sprites/effect.png"),
				AlbedoColor = color,
				VertexColorUseAsAlbedo = true,
				BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				BlendMode = BaseMaterial3D.BlendModeEnum.Mix,
				ParticlesAnimHFrames = 4,
				ParticlesAnimVFrames = 4,
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				// ParticlesAnimLoop = true,
				

			},
			Size = Vector2.One * 6f,
		};

		OneShot = true;
		Amount = 1;
		Lifetime = 1f;
		ProcessMaterial = new ParticleProcessMaterial(){
			Spread = 0f,
			Gravity = Vector3.Zero,
			Color = color,
			AnimSpeedMax = 1f,
			AnimOffsetMax = 1f,
			Direction = new Vector3(0f, 0f, 0f),
		};
		Emitting = true;
	}
}
