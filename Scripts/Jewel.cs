using Godot;
using Godot.Collections;

public partial class Jewel : RigidBody3D
{
    public int Level {get; set; }
    public bool isActive {get; set; } = false;
    public bool merging {get; set; } = false;
    AudioStreamPlayer dropAudioPlayer;
	AudioStreamPlayer collisionAudioPlayer;
	AudioStreamPlayer mergeAudioPlayer;

    public Texture3D texture3D;
    

    private int collisionTimes = 0;

    public Vector3 TargetVelocity {get; set; } = Vector3.Zero;

    public Laser laser;

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

    // AnimationPlayer animationPlayer;
    AnimationTree animationTree;
    // AnimationPlayer animationPlayer;
    AnimationNodeStateMachinePlayback playback;


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
        // animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationTree.Active = true;
        playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		Color color = levelColors[Level];
        shaderMaterial.Shader = ResourceLoader.Load<Shader>("res://Assets/Shaders/crystal.gdshader");
        GetChild<MeshInstance3D>(0).MaterialOverride = shaderMaterial;
		BodyEntered += OnBodyEntered;
        shaderMaterial.SetShaderParameter("Color", color);
        AxisLockLinearZ = true;
        ContactMonitor = true;
        Freeze = true;
		MaxContactsReported = 20;
        SetPhysicsProcess(true);
        AddChild(mergeAudioPlayer);
        AddChild(dropAudioPlayer);
        AddChild(collisionAudioPlayer);
        
        GD.Print($"Jewel {Name} created");
    }

    private void OnBodyEntered(Node body)
    {
        // GD.Print($"Jewel {Name} collided with: {body.Name}" );
        bool isJewel = body is Jewel;
        bool isFloor = body.Name == "Floor";
        // animationPlayer.Stop();
        if(!merging && !body.IsQueuedForDeletion() && !IsQueuedForDeletion()) {

            if (body is Jewel jewel && jewel.Level == Level && !merging) {
                EmitSignal(SignalName.Merge, this, body);
            }
        }
        if (isJewel || isFloor) {
            collisionTimes++;
        }
        
        if(collisionTimes == 1){
            collisionAudioPlayer.Play();
            // playback.Stop();
            // animationTree.Active = false;
            GravityScale = 1f; 
        }
    }

    public void PlayMerge(){
        GravityScale = 1f;
        Freeze = false;
        FreezeMode = FreezeModeEnum.Kinematic;
        playback.Travel("merge");
        // AuraVFX auraVFX = new AuraVFX(){
        //     Level = Level,
        //     color = levelColors[Level],
        //     Position = new Vector3(0f, 0f, -1f),
           
        // };
        // MergeVFX mergeVFX = new MergeVFX(){
        //     Level = Level,
        //     colorA = levelColors[Level - 1],
        //     colorB = levelColors[Level],
           
        // };
        // AddChild(auraVFX);
        // AddChild(mergeVFX);
        GpuParticles3D auraVFX = GetNode<GpuParticles3D>("aura");
        GpuParticles3D starsVFX = GetNode<GpuParticles3D>("stars");
        auraVFX.Position = new Vector3(0f, 0f, -5f);
        // starsVFX.Position = new Vector3(0f, 0f, -5f);
        ((StandardMaterial3D)((QuadMesh)starsVFX.DrawPass1).Material).AlbedoColor = levelColors[Level];
        ((StandardMaterial3D)((QuadMesh)auraVFX.DrawPass1).Material).AlbedoColor = levelColors[Level];
        ((QuadMesh)auraVFX.DrawPass1).Size = Vector2.One * 2f;
        ((QuadMesh)starsVFX.DrawPass1).Size = Vector2.One * 2f;
        auraVFX.Emitting = true;
        starsVFX.Emitting = true;
        mergeAudioPlayer.Play();

        // timer for release
        GetTree().CreateTimer(0.3).Timeout += ()=>{
            // mergeVFX.QueueFree(2);
            // Freeze = false;
        };
    }
    public void PlayLaser(){
        playback.Travel("activate");
        laser = new Laser(){
            color = levelColors[Level],
        };
        AddChild(laser);
        // animationPlayer.Play("rotateZ");
    }

    public void Drop() 
    {
        // animationPlayer.Stop();
        Freeze = false;
        playback.Stop();
        animationTree.Active = false;
        laser.QueueFree();
        GravityScale = 5f;
        dropAudioPlayer.Play();
    }

}