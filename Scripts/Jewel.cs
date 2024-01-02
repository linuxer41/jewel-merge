using Godot;
using Godot.Collections;

public enum LocationEnum
{
    Spawn,
    Current,
    Active,
}

public enum OriginEnum
{
    Spawn,
    Merge,
}

public enum StateEnum
{
    Normal,
    Merging,
}
public partial class Jewel : RigidBody3D
{
    public int Level {get; set; } = 1;
    public float ScaleFactor {get; set; } = 1f;
    public LocationEnum Location {get; set; } = LocationEnum.Spawn;
    public OriginEnum Origin {get; set; } = OriginEnum.Spawn;
    public StateEnum State {get; set; } = StateEnum.Normal;
    public Vector3 NewPosition {get; set; } = Vector3.Zero;
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
        Scale = Vector3.One * ScaleFactor;
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationTree.Active = true;
        playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		Color color = levelColors[Level];
        shaderMaterial.Shader = ResourceLoader.Load<Shader>("res://Assets/Shaders/crystal.gdshader");
        GetChild<MeshInstance3D>(0).MaterialOverride = shaderMaterial;
		BodyEntered += OnBodyEntered;
        shaderMaterial.SetShaderParameter("Color", color);
        Freeze = true;
        AxisLockLinearZ = true;
        ContactMonitor = true;
		MaxContactsReported = 20;
        AddChild(mergeAudioPlayer);
        AddChild(dropAudioPlayer);
        AddChild(collisionAudioPlayer);
        SetPhysicsProcess(true);
    }

    private void OnBodyEntered(Node body)
    {
        // GD.Print($"Jewel {Name} collided with: {body.Name}" );
        bool isJewel = body is Jewel;
        bool isFloor = body.Name == "Floor";
        // GD.Print($"Jewel {Name} collided with: {body.Name}, isJewel: {isJewel}, isFloor: {isFloor}, lOCATION: {Location}, STATE: {State}" );
        // animationPlayer.Stop();
        if(Location.Equals(LocationEnum.Active) && !body.IsQueuedForDeletion() && !IsQueuedForDeletion()) {

            if (body is Jewel jewel && jewel.Level == Level && State.Equals(StateEnum.Normal) && !State.Equals(StateEnum.Merging)) {
                // State = StateEnum.Merging;
                EmitSignal(SignalName.Merge, this, body);
            }
        }
        if ((isJewel || isFloor) && Location.Equals(LocationEnum.Active)) {
            collisionTimes++;
        }
        
        if(collisionTimes == 1){
            GravityScale = 1;
            if(Origin != OriginEnum.Merge){
                collisionAudioPlayer.Play();
            }
            
        }
    }

    public void PlayMerge(){
        Freeze = false;
        playback.Travel("merge");
        GpuParticles3D auraVFX = GetNode<GpuParticles3D>("aura");
        GpuParticles3D starsVFX = GetNode<GpuParticles3D>("stars");
        auraVFX.Scale = Vector3.One * ScaleFactor;
        Color color = levelColors[Level];
        ((StandardMaterial3D)((QuadMesh)starsVFX.DrawPass1).Material).AlbedoColor = color;
        ((StandardMaterial3D)((QuadMesh)auraVFX.DrawPass1).Material).AlbedoColor = new Color(color.R, color.G, color.B, 0.5f);
        ((QuadMesh)auraVFX.DrawPass1).Size = Vector2.One * 2f;
        ((QuadMesh)starsVFX.DrawPass1).Size = Vector2.One * 2f;
        auraVFX.Emitting = true;
        starsVFX.Emitting = true;
        mergeAudioPlayer.Play();

        // timer for release
        GetTree().CreateTimer(0.3).Timeout += ()=>{
            // mergeVFX.QueueFree(2);
            // Freeze = false;

            State = StateEnum.Normal;
        };
    }
    public void Select(){
        Location = LocationEnum.Current;
        playback.Travel("activate");
        laser = new Laser(){
            color = levelColors[Level],
            Position = new Vector3(0f, -ScaleFactor + 1f, 0f),
        };
        AddChild(laser);
        // animationPlayer.Play("rotateZ");
    }

    public void Drop() 
    {
        // animationPlayer.Stop();
        Freeze = false;
        GravityScale = 5;
        Location = LocationEnum.Active;
        playback.Stop();
        animationTree.Active = false;
        laser.QueueFree();
        dropAudioPlayer.Play();
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        // Move to new position
        if (!NewPosition.Equals(Vector3.Zero)){
            state.Transform = new Transform3D(state.Transform.Basis, NewPosition);
            NewPosition = Vector3.Zero;
        }
    }
}