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
				AlbedoTexture = GD.Load<CompressedTexture2D>("res://Assets/Textures/Sprites/stars.png"),
				AlbedoColor = colorB,
				VertexColorUseAsAlbedo = true,
				BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				BlendMode = BaseMaterial3D.BlendModeEnum.Mix,
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				ParticlesAnimHFrames = 4,
				ParticlesAnimVFrames = 3,
				// ParticlesAnimLoop = true,
				

			},
			Size = Vector2.One * 2f,
		};

		OneShot = true;
		Amount = 12;
		Lifetime = 2f;
		//Explosiveness =1;
		ProcessMaterial = new ParticleProcessMaterial(){
			Spread = 90f,
			InitialVelocityMin = 2f,
			InitialVelocityMax = 10f,
			DampingMax = 5f,
			EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Ring,
			EmissionRingAxis = new Vector3(0f, 0f, 1f),
			EmissionRingRadius = 1f,
			EmissionRingInnerRadius = 0.5f,
			Gravity = Vector3.Zero,
			Color = Colors.White,
			ColorRamp = new GradientTexture2D(){
				Gradient = new Gradient(){
					Colors = new Color[]{Colors.White, colorA, colorB},
					Offsets = new float[]{0.1f, 0.5f, 0.9f},
					InterpolationMode = Gradient.InterpolationModeEnum.Cubic,
				},
				
			},
			AnimOffsetMin = 1f,
			AnimOffsetMax = 1f,
			Direction = new Vector3(0f, 1f, 0f),
			AngularVelocityCurve = new CurveTexture(){
				Curve = new Curve(){
					MinValue = 0f,
					MaxValue = 360f,
					_Data = new Godot.Collections.Array{new Vector2(0, 1), 0.0, 0.0, 0, 0, new Vector2(1, 360), 0.0, 0.0, 0, 0},
				}
			},
			
			ScaleCurve = new CurveTexture(){
				Curve = new Curve(){
					MinValue = 0f,
					MaxValue = 1f,
					_Data = new Godot.Collections.Array{new Vector2(0, 1), 0.0, 0.0, 0, 0, new Vector2(1, 0.00105274f), 0.0, 0.0, 0, 0},
				}
			},

		};
		Emitting = true;
	}
}
