using Godot;
using Godot.Collections;

public partial class Jewel : RigidBody3D
{
    private MeshInstance3D mesh;
    private CollisionShape3D shape;
    public int Level {get; set; }
    public bool isActive {get; set; } = false;
    public bool merging {get; set; } = false;
    AudioStreamPlayer dropAudioPlayer;
	AudioStreamPlayer collisionAudioPlayer;
	AudioStreamPlayer mergeAudioPlayer;

    public Texture3D texture3D;

    private int collisionTimes = 0;

    public Vector3 TargetVelocity {get; set; } = Vector3.Zero;

    public LaserVFX laserVFX;
    public MergeVFX mergeVFX;

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
    Dictionary<string, AudioStream> audioStreams;


    public override void _Ready()
    {
        audioStreams = new Dictionary<string, AudioStream>()
		{
			{"merge", (AudioStream)GD.Load("res://Assets/Sound/success.mp3")},
			{"drop", (AudioStream)GD.Load("res://Assets/Sound/drop.wav")},
            {"collision", (AudioStream)GD.Load("res://Assets/Sound/punch.mp3")},
		};
		ShaderMaterial  shaderMaterial = new ShaderMaterial(){
			
        };
        mergeAudioPlayer = new AudioStreamPlayer(){
			Name = "MergeAudioPlayer",
            Stream = audioStreams["merge"],
		};
        dropAudioPlayer = new AudioStreamPlayer(){
            Name = "DropAudioPlayer",
            Stream = audioStreams["drop"],
        };
        collisionAudioPlayer = new AudioStreamPlayer(){
            Name = "CollisionAudioPlayer",
            Stream = audioStreams["collision"],
        };
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
        AddChild(mergeAudioPlayer);
        AddChild(dropAudioPlayer);
        AddChild(collisionAudioPlayer);

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
            collisionAudioPlayer.Play();
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

    public override void _PhysicsProcess(double delta){
        // RotateY(0.5f * (float)delta);
        if (Freeze){
            if(isActive){
                RotateX(0.5f * (float)delta);
                // GlobalRotate(Vector3.Back, 0.5f * (float)delta);
                // GetChild<MeshInstance3D>(0).RotateObjectLocal(Vector3.Back, 0.5f * (float)delta);
            } else{
                GetChild<MeshInstance3D>(0).RotateY(0.5f * (float)delta);
            }
            
        }
        // MoveAndCollide(TargetVelocity * (float)delta);
    }

    public void PlayMerge(){
        mergeVFX = new MergeVFX(){
            Level = Level,
            color = levelColors[Level],
        };
        AddChild(mergeVFX);
        mergeAudioPlayer.Play();
        // timer for release
        GetTree().CreateTimer(2.5).Timeout += ()=>{
            mergeVFX.QueueFree();
        };
    }
    public void PlayLaser(){
        laserVFX = new LaserVFX(){
            color = levelColors[Level],
        };
        AddChild(laserVFX);
    }

    public void DestroyLaser(){
        dropAudioPlayer.Play();
        laserVFX.QueueFree();
    }
}