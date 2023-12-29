
using System.Collections.Generic;
using Godot;

public partial class Main : Node3D
{
	// Called when the node enters the scene tree for the first time.
	RandomNumberGenerator rng = new RandomNumberGenerator();
	List<Jewel> activeItems = new List<Jewel>();
	List<Jewel> toDropItems = new List<Jewel>();
	List<Jewel> toPlayItems = new List<Jewel>();
	Jewel activeItem;
	Camera3D mainCamera;
	private bool spawning = false;
	public int MAX_LEVEL = 10;
	private static Dictionary<int, PackedScene> scenes;
	private static Dictionary<int, CompressedTexture3D> particleCollisionTextures;
	Vector3 placePos = Vector3.Zero;
	Vector3 minPOs;
	Vector3 maxPos;
	Vector3 lastPos = Vector3.Zero;

	int score = 0;
	bool dragging = false;

	Label scoreLabel;

	AudioStreamPlayer audioPlayer;

	Node3D toPlayContainer;
	public override void _Ready()
	{
		mainCamera = GetNode<Camera3D>("Camera3D");
		Rect2 viewport = GetViewport().GetVisibleRect();
		minPOs = mainCamera.ProjectPosition(viewport.Position, 1f);
		maxPos = mainCamera.ProjectPosition(viewport.Size, 1f);
		float width = Mathf.Abs(maxPos.X - minPOs.X);
		float height = Mathf.Abs(maxPos.Y - minPOs.Y);
		lastPos = new Vector3(0f, minPOs.Y - 5.5f, 0f);
		GD.Print("MinPos: " + minPOs + " MaxPos: " + maxPos + " Viewport size: " + viewport.Size + " Viewport position: " + viewport.Position);
		scenes = new Dictionary<int, PackedScene>()
		{
			{1, ResourceLoader.Load<PackedScene>("res://Scenes/square.tscn")},
			{2, ResourceLoader.Load<PackedScene>("res://Scenes/circle.tscn")},
			{3, ResourceLoader.Load<PackedScene>("res://Scenes/heart.tscn")},
			{4, ResourceLoader.Load<PackedScene>("res://Scenes/square.tscn")},
			{5, ResourceLoader.Load<PackedScene>("res://Scenes/circle.tscn")},
			{6, ResourceLoader.Load<PackedScene>("res://Scenes/heart.tscn")},
			{7, ResourceLoader.Load<PackedScene>("res://Scenes/square.tscn")},
			{8, ResourceLoader.Load<PackedScene>("res://Scenes/circle.tscn")},
			{9, ResourceLoader.Load<PackedScene>("res://Scenes/heart.tscn")},
			{10, ResourceLoader.Load<PackedScene>("res://Scenes/square.tscn")},
		};
		particleCollisionTextures = new Dictionary<int, CompressedTexture3D>()
{
			{1, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/square.exr")},
			{2, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/circle.exr")},
			{3, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/heart.exr")},
			{4, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/square.exr")},
			{5, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/circle.exr")},
			{6, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/heart.exr")},
			{7, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/square.exr")},
			{8, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/circle.exr")},
			{9, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/heart.exr")},
			{10, GD.Load<CompressedTexture3D>("res://Assets/Textures/3D/square.exr")},
		};
		AudioStream audioStream = (AudioStream)GD.Load("res://Assets/Sound/background.mp3");
		audioPlayer = new AudioStreamPlayer(){
			Name = "AudioPlayer",
			Stream = audioStream,
			Autoplay = true,
			// loop
			
		};
		// set background size

		StaticBody3D background = new StaticBody3D(){
			Name = "Background",
			Position = new Vector3(0f, 0f, -10f),
		};
		MeshInstance3D backgroundMesh = new MeshInstance3D(){
			Mesh = new PlaneMesh(){
				Size = new Vector2(width, height),
				Orientation = PlaneMesh.OrientationEnum.Z,
				Material = new StandardMaterial3D(){
					AlbedoTexture = GD.Load<Texture2D>("res://Assets/Textures/Images/background.jpg"),
				}
			},
		};

		CollisionShape3D backgroundShape = new CollisionShape3D(){
			Shape = new BoxShape3D(){
				Size = new Vector3(width, height, 0.5f)
			},
		};
		background.AddChild(backgroundShape);
		background.AddChild(backgroundMesh);
		AddChild(background);


		// score group
		scoreLabel = GetTree().GetFirstNodeInGroup("score") as Label;
		scoreLabel.AddThemeFontSizeOverride("font_size", 30);
		scoreLabel.AddThemeColorOverride("font_color", Colors.Green);

		var particlesCollisionBox3D = new GpuParticlesCollisionBox3D(){
			Size = new Vector3(50f, 0.5f, 50f),
		};

		var colliderL = new CollisionShape3D(){
			Shape = new BoxShape3D(){
				Size = new Vector3(0.5f, 50f, 50f)
			}
		};
		var colliderR = new CollisionShape3D(){
			Shape = new BoxShape3D(){
				Size = new Vector3(0.5f, 50f, 50f)
			}
		};
		var floorCollider = new CollisionShape3D(){
			Shape = new BoxShape3D(){
				Size = new Vector3(100f, 0.5f, 5f)
			}
		};
		var floorMesh = new MeshInstance3D(){
			Mesh = new QuadMesh(){
				Size = new Vector2(100f, 0.5f),
				Material = new StandardMaterial3D(){
					AlbedoTexture = GD.Load<Texture2D>("res://Assets/Textures/Images/floor.png"),
					Uv1Scale = new Vector3(20f, 1, 1),
					Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				},
			},
			Position = Vector3.Back * 2f,
		};
		var ceilingCollider = new CollisionShape3D(){
			Shape = new BoxShape3D(){
				Size = new Vector3(100f, 0.5f, 5f)
			}
		};
		var ceilingMesh = new MeshInstance3D(){
			Mesh = new QuadMesh(){
				Size = new Vector2(100f, 0.5f),
				Material = new StandardMaterial3D(){
					AlbedoTexture = GD.Load<Texture2D>("res://Assets/Textures/Images/ceiling.png"),
					Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
					Uv1Scale = new Vector3(10f, 1, 1),
				}
			}
		};
		var floor = new StaticBody3D(){
			Position = new Vector3(0, maxPos.Y + 0.25f, 0f), 
			Name = "Floor",
		};
		var ceiling = new StaticBody3D(){
			Position = new Vector3(0, minPOs.Y -3f, 0f), 
			Name = "Ceiling",
		};
		var leftWall = new StaticBody3D(){
			Position = new Vector3(minPOs.X, 0, 0),
			Name = "LeftWall",
		// Scale = wallSize, 
		};

		var rightWall = new StaticBody3D(){
			Position = new Vector3(maxPos.X, 0, 0),
			Name = "RightWall",
		// Scale = wallSize,
		};

		toPlayContainer = new Node3D(){
			Name = "ToPlayContainer",
			Position = new Vector3(
				minPOs.X + 2f, // X position
				minPOs.Y - 2f, // Y position
				-4f
			),
			Scale= Vector3.One * 0.5f,
		};

		leftWall.AddChild(colliderL);
		rightWall.AddChild(colliderR);
		floor.AddChild(floorCollider);
		floor.AddChild(floorMesh);
		floor.AddChild(particlesCollisionBox3D);
		ceiling.AddChild(ceilingMesh);
		ceiling.AddChild(ceilingCollider);
		AddChild(floor);
		AddChild(leftWall);
		AddChild(rightWall);
		AddChild(ceiling);
		AddChild(audioPlayer);
		AddChild(toPlayContainer);
		resPawn();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// remove activeItems that have been dropped
		foreach (var jewel in toDropItems){
			jewel.QueueFree();
		}
		toDropItems.Clear();
		// if any of activeItems not is added as child add it
		foreach (var jewel in activeItems)
		{
		    if (!jewel.IsInsideTree())
		    {
		        AddChild(jewel);
		    }
		}

		// if any of toPlay not is added as child add it
		foreach (var jewel in toPlayItems)
		{
		    if (!toPlayContainer.IsAncestorOf(jewel))
		    {
		        toPlayContainer.AddChild(jewel);
		    }
		}
		// toPlayItems.Clear();
		scoreLabel.Text = "Score: " + score;

	}

	public override void _Input(InputEvent @event)
   {
    if(@event is InputEventMouseButton mouse)
    {
        if (mouse.Pressed)
        {
            //Empezó drag
            dragging = true; 
        }
        else
        {
            //Terminó drag
            dragging = false;
            
            if (!dragging)
            {
                //Clic símple, llamar Play()
				placeToPositionX(mouse.Position);
                Play();
            }
        }
    }
    
    if(@event is InputEventMouseMotion motion && dragging)
    {
        //Hacer drag
        placeToPositionX(motion.Position);
    }
   }


   void placeToPositionX(Vector2 pos){
		Vector3 castPos = castPosition(pos);
		lastPos = new Vector3(castPos.X, lastPos.Y, lastPos.Z);
		if(activeItem != null && activeItem.Freeze) activeItem.Position = lastPos;
   }


    Vector3 castPosition(Vector2 mousePos) {
      float rayLength = 100f;
      Vector3 from = mainCamera.ProjectRayOrigin(mousePos);
      Vector3 to = from + mainCamera.ProjectRayNormal(mousePos) * rayLength;
      PhysicsDirectSpaceState3D space = GetWorld3D().DirectSpaceState;
      PhysicsRayQueryParameters3D rayQuery = new PhysicsRayQueryParameters3D
      {
          From = from,
          To = to,
          CollideWithAreas = true
      };
      var result = space.IntersectRay(rayQuery);
	  if (!result.ContainsKey("position")) return Vector3.Zero;
      Vector3 newPosition = (Vector3)result["position"];
      return newPosition;
  }

    private void resPawn()
    {
		if (spawning) return;
		// ensure we have at least 4 to play
		while (toPlayItems.Count < 4){
			toPlayItems.Add(SpawnJewel());
		}
		activeItem = toPlayItems[0];
		activeItem.PlayLaser();
		if (toPlayContainer.IsAncestorOf(activeItem)){
			toPlayContainer.RemoveChild(activeItem);
		}
		toPlayItems.RemoveAt(0);
		activeItem.Position = lastPos;
		activeItem.isActive = true;
		for (int i = 0; i < toPlayItems.Count; i++){
			var item = toPlayItems[i];
			item.Position = new Vector3(
				i, // X position
				0f, // Y position
				0f - i
			);
			item.isActive = false;
		}
		activeItems.Add(activeItem);
		
	}


	Jewel SpawnJewel(){

		int level =rng.RandiRange(1, MAX_LEVEL - 3);
		var scene = scenes[level];
		var texture3D = particleCollisionTextures[level];
		Jewel jewel = scene.Instantiate<Jewel>();
		jewel.Level = level;
		jewel.texture3D = texture3D;
		jewel.Freeze = true;
		jewel.Scale = Vector3.One * getScaleFactor(level);
		jewel.Merge += Merge;
		return jewel;
    }

	private float getScaleFactor(int level){
		return Mathf.Lerp(1f, 2.5f, (level - 1f) / (MAX_LEVEL - 1f)); 
	}

	void Play(){
		if(activeItem == null) return;
		activeItem.GravityScale = 5f;
		activeItem.Freeze = false;
		activeItem.isActive = false;
		activeItem.DestroyLaser();
		activeItem = null;
		GetTree().CreateTimer(0.5f).Timeout += ()=>{
			resPawn();
		};
	}

    private void Merge(Jewel current, Jewel target)
    {
		if (current.Level < MAX_LEVEL){
			current.merging = true;
			target.merging = true;
			// calculate new position between current and target
			Vector3 newPos = new Vector3(
				Mathf.Lerp(current.Position.X, target.Position.X, 0.5f),
				Mathf.Lerp(current.Position.Y, target.Position.Y, 0.5f),
				0f
			);
			int level = current.Level + 1;
			PackedScene scene = scenes[level];
			Jewel jewel = scene.Instantiate<Jewel>();
			jewel.Level = level;
			jewel.Position = newPos;
			jewel.Scale = Vector3.One * getScaleFactor(level);
			
			// remove jewel from list
			activeItems.Remove(current);
			activeItems.Remove(target);
			toDropItems.Add(current);
			toDropItems.Add(target);
			AddChild(jewel);
			jewel.PlayMerge();
			score += current.Level;
			jewel.Merge += Merge;

		}
    }

}
