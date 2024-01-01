using Godot;

public partial class World : Node3D
{
    
    public float Height {get; set;}
    public float Width {get; set;}
    StaticBody3D Ceiling { get; set; }
    StaticBody3D Floor { get; set; }
    StaticBody3D LeftWall { get; set; }
    StaticBody3D RightWall { get; set; }
    public Node3D toPlayContainer {get; set;}
    public AudioStreamPlayer audioPlayer;
    public Camera3D camera  {get; set;}
    private bool dragging = false;

    [Signal]
    public delegate void DropEventHandler(Vector3 position);
    [Signal]
    public delegate void MoveEventHandler(Vector3 position);
    

    public override void _Ready()
    {
        Name = "World";

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
			Position = new Vector3(0, -Height/2 + 0.25f, 0f), 
			Name = "Floor",
		};
		var ceiling = new StaticBody3D(){
			Position = new Vector3(0, Height/2 -3f, 0f), 
			Name = "Ceiling",
		};
		var leftWall = new StaticBody3D(){
			Position = new Vector3(-Width/2, 0, 0),
			Name = "LeftWall",
		// Scale = wallSize, 
		};

		var rightWall = new StaticBody3D(){
			Position = new Vector3(Width/2, 0, 0),
			Name = "RightWall",
		// Scale = wallSize,
		};
        		StaticBody3D background = new StaticBody3D(){
			Name = "Background",
			Position = new Vector3(0f, 0f, -10f),
		};
		MeshInstance3D backgroundMesh = new MeshInstance3D(){
			Mesh = new PlaneMesh(){
				Size = new Vector2(Width, Height),
				Orientation = PlaneMesh.OrientationEnum.Z,
				Material = new StandardMaterial3D(){
					AlbedoTexture = GD.Load<Texture2D>("res://Assets/Textures/Images/background.jpg"),
				}
			},
		};

		CollisionShape3D backgroundShape = new CollisionShape3D(){
			Shape = new BoxShape3D(){
				Size = new Vector3(Width, Height, 0.5f)
			},
		};

        toPlayContainer = new Node3D(){
			Name = "ToPlayContainer",
			Position = new Vector3(
				(-Width / 2) + 4f, // X position
				(Height / 2) - 2f, // Y position
				-4f
			),
			Scale= Vector3.One * 0.7f,
		};

		AudioStream audioStream = (AudioStream)GD.Load("res://Assets/Sound/background.mp3");
		audioPlayer = new AudioStreamPlayer(){
			Name = "AudioPlayer",
			Stream = audioStream,
			Autoplay = true,
		};

		background.AddChild(backgroundShape);
		background.AddChild(backgroundMesh);
		leftWall.AddChild(colliderL);
		rightWall.AddChild(colliderR);
		floor.AddChild(floorCollider);
		floor.AddChild(floorMesh);
		ceiling.AddChild(ceilingMesh);
		ceiling.AddChild(ceilingCollider);
        AddChild(background);
		AddChild(floor);
		AddChild(leftWall);
		AddChild(rightWall);
		AddChild(ceiling);
        AddChild(toPlayContainer);
        AddChild(audioPlayer);

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
				// EmitSignal("drop", castPosition(mouse.Position));
				EmitSignal(SignalName.Drop, castPosition(mouse.Position));
            }
        }
    }
    
    if(@event is InputEventMouseMotion motion && dragging)
    {
        //Hacer drag
        EmitSignal(SignalName.Move, castPosition(motion.Position));
    }
   }


    Vector3 castPosition(Vector2 mousePos) {
      float rayLength = 100f;
      Vector3 from = camera.ProjectRayOrigin(mousePos);
      Vector3 to = from + camera.ProjectRayNormal(mousePos) * rayLength;
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
}