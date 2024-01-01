using System;
using System.Collections.Generic;
using Godot;

public partial class GameController
{
    private float height;
    private float width;
    private float depth;
    private Vector3 lastPos = Vector3.Zero;
    RandomNumberGenerator rng = new RandomNumberGenerator();
    List<Jewel> activeItems = new List<Jewel>();
	Jewel activeItem;

    Dictionary<int, PackedScene> objects;
    World world;
    Camera3D camera;

    public int score = 0;
    public int MAX_LEVEL = 10;
    public GameController(float height, float width, Camera3D camera, float depth = 1)  { 
        this.height = height;
        this.width = width;
        this.depth = depth;
        this.camera = camera;
        lastPos = new Vector3(0f, (height/2) - 5.5f, 0f);

        objects = new Dictionary<int, PackedScene>()
		{
			{1, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{2, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{3, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{4, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{5, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{6, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{7, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{8, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{9, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
			{10, ResourceLoader.Load<PackedScene>("res://Scenes/diamond.tscn")},
		};
        
    }
    public void LoadWorld(Node3D parent){
        world = new World(){
            Height = height,
            Width = width,
            camera = camera,
        };
        world.Move +=  MoveObject;
        world.Drop += DropObject;
        parent.AddChild(world);
        Spawn();
    }

    private void DropObject(Vector3 position)
    {
        if (activeItem != null) {
            activeItem.Position = new Vector3(position.X, activeItem.Position.Y, activeItem.Position.Z);
            activeItem.Drop();
            activeItem = null;
            world.GetTree().CreateTimer(0.5).Timeout += Spawn;
            //spawning = true;
        };
        
        
    }

    private void MoveObject(Vector3 position)
    {
        if (activeItem != null) activeItem.Position = new Vector3(position.X, activeItem.Position.Y, activeItem.Position.Z);
    }

    private const int MIN_JEWELS = 3;

    private void PositionJewels()  
{
    var numJewels = world.toPlayContainer.GetChildCount();

    for (int i = 0; i < numJewels; i++)
    {
       Jewel jewel = world.toPlayContainer.GetChild<Jewel>(i);

       // Posición en eje X
       var xPos = -i * 2.5f;  

       // Posición en eje Y
       var yPos = 0f;

       // Posición en eje Z
       // Las más nuevas hasta atrás  
       var zPos = -(numJewels - i);

       // Asignar posición final
       jewel.Position = new Vector3(xPos, yPos, zPos);

       // Desactivar
       jewel.isActive = false; 
    }
}

    private void Spawn()  
    {
        // Asegurar mínimo
        while (world.toPlayContainer.GetChildCount() < MIN_JEWELS) {
            world.toPlayContainer.AddChild(InstantiateJewel());
        }

        // Tomar la joya más antigua
        Jewel oldest = world.toPlayContainer.GetChild<Jewel>(0);
        activeItem = oldest;
        
        // Removerla y activarla
        activeItem.isActive = true;
        // activeItem.Reparent(world, false);
        world.toPlayContainer.RemoveChild(activeItem);
        world.AddChild(activeItem);
        
        activeItem.PlayLaser();
        activeItem.Position = lastPos;
        activeItems.Add(activeItem);

        // Instanciar una nueva joya al final  
        Jewel newJewel = InstantiateJewel();
        world.toPlayContainer.AddChild(newJewel);

        // Reordenar todas las joyas
        PositionJewels();
        
    }

    Jewel InstantiateJewel(int _level = 0){

		int level = _level == 0 ? rng.RandiRange(1, MAX_LEVEL - 5): _level;
		var obj = objects[level];
		Jewel jewel = obj.Instantiate<Jewel>();
		jewel.Level = level;
		jewel.Merge += Merge;
		return jewel;
    }

    private void Merge(Jewel current, Jewel target)
    {
		if (current.Level < MAX_LEVEL){
			current.merging = true;
			target.merging = true;
			// calculate new position between current and target
			// Vector3 newPos = new Vector3(
			// 	Mathf.Lerp(current.Position.X, target.Position.X, 0.5f),
			// 	Mathf.Lerp(current.Position.Y, target.Position.Y, 0.5f),
			// 	0f
			// );
			int level = current.Level + 1;
			Jewel jewel = InstantiateJewel(level);
			jewel.Position = target.Position;
			// remove jewel from list
			activeItems.Remove(current);
			activeItems.Remove(target);
            current.QueueFree();
            target.QueueFree();
			world.AddChild(jewel);
			jewel.PlayMerge();
			score += current.Level;

		}
    }
}