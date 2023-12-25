using Godot;
using Godot.Collections;

public partial class Jewel : RigidBody3D
{
    private MeshInstance3D mesh;
    private CollisionShape3D shape;
    public int Level {get; set; }
    public bool isActive {get; set; } = false;
    public bool merging {get; set; } = false;
    AudioStreamPlayer audioPlayer;

    public Texture3D texture3D;

    private int collisionTimes = 0;

    public Vector3 TargetVelocity {get; set; } = Vector3.Zero;

	[Signal]
	public delegate void MergeEventHandler(Jewel current, Jewel target);
	private static Dictionary<int, Color> levelColors = new Dictionary<int, Color>()
	{
		{1, Color.FromHtml("#a30000")}, // Rojo rubí
		{2, Color.FromHtml("#00b300")}, // Verde esmeralda
		{3, Color.FromHtml("#1b00b3")}, // Azul zafiro 
		{4, Color.FromHtml("#b3b300")}, // Amarillo ámbar
		{5, Color.FromHtml("#b300b3")}, // Púrpura amatista
		{6, Color.FromHtml("#00b3b3")}, // Azul celeste
		{7, Color.FromHtml("#b37f00")}, // Naranja topacio
		{8, Color.FromHtml("#7f00b3")}, // Púrpura tanzanita
		{9, Color.FromHtml("#00b37f")}, // Verde oliva
		{10, Color.FromHtml("#b35f00")}  // Naranja ámbar 
	};

    Texture3D collisionTexture;


    public override void _Ready()
    {
		ShaderMaterial  shaderMaterial = new ShaderMaterial(){
			
        };
        audioPlayer = new AudioStreamPlayer(){
			Name = "AudioPlayer" + Name,
		};
        AudioStream audio = (AudioStream)GD.Load("res://Assets/Sound/punch.mp3");
		audioPlayer.Stream = audio;
		Color color = levelColors[Level];
        shaderMaterial.Shader = ResourceLoader.Load<Shader>("res://Assets/Shaders/crystal.gdshader");
        GetChild<MeshInstance3D>(0).MaterialOverride = shaderMaterial;
		BodyEntered += OnBodyEntered;
        shaderMaterial.SetShaderParameter("Color", color);
        AxisLockLinearZ = true;
        ContactMonitor = true;
		MaxContactsReported = 20;
        GravityScale = 1f;
        SetPhysicsProcess(true);
        AddChild(audioPlayer);

        PhysicsMaterialOverride = new PhysicsMaterial(){
            Bounce = 0.1f
        };
        GpuParticlesCollisionSdf3D gpuParticlesCollisionSdf3D = new GpuParticlesCollisionSdf3D(){
            Texture = texture3D,
            Size = Vector3.One * 2f,
            Position = new Vector3(0, 0.9f, 0),
        };
        AddChild(gpuParticlesCollisionSdf3D);

    }
    private void OnBodyEntered(Node body)
    {
        if(!merging){
           	if (body is Jewel jewel) {
                collisionTimes++;
                if(jewel.Level == Level) {
                    GD.Print("Jewel: Body entered: " + body.Name);
                    if(!Freeze && !jewel.Freeze){
                        EmitSignal(SignalName.Merge, this, body);
                    }
                }
            } else if( body.Name == "Floor") {
                collisionTimes++;
            }
        }
        if(collisionTimes == 1){
            audioPlayer.Play();
            GravityScale = 1f;
        }

    }


    public override void _Process(double delta)
    {
		// var coll = GetCollidingBodies();
        // ApplyCentralImpulse(new Vector3(0, -9.81f, 0) * (float)delta);

        // // Comprobamos si el balón ha tocado el suelo.
        // if (GetContactCount() > 0)
        // {
        // }

        
    }

    // public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    // {
    //     state.Transform.Basis = state.Transform.Basis.Rotated(Vector3.Up, 0.5f * (float)state.Step);
    // } 

    public override void _PhysicsProcess(double delta){
        // RotateY(0.5f * (float)delta);
        if (Freeze){
            if(isActive){
                // RotateX(0.5f * (float)delta);
                // GlobalRotate(Vector3.Back, 0.5f * (float)delta);
                RotateY(0.5f * (float)delta);
            } else{
                RotateY(0.5f * (float)delta);
            }
            
        }
        // MoveAndCollide(TargetVelocity * (float)delta);
    }
}