using System;
using System.Collections.Generic;
using Godot;

public partial class GameController
{
    private float height;
    private float width;
    private float depth;
    private Vector3 lastPos = Vector3.Zero;
    private bool spawning = false;
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
        ResPawn();
    }

    private void DropObject(Vector3 position)
    {
        if (activeItem != null) {
            activeItem.Position = new Vector3(position.X, activeItem.Position.Y, activeItem.Position.Z);
            activeItem.Drop();
            activeItem = null;
            world.GetTree().CreateTimer(0.5).Timeout += ResPawn;
            //spawning = true;
        };
        
        
    }

    private void MoveObject(Vector3 position)
    {
        if (activeItem != null) activeItem.Position = new Vector3(position.X, activeItem.Position.Y, activeItem.Position.Z);
    }

    private void ResPawn()
    {
		if (spawning) return;
		// ensure we have at least 4 to play
		while (world.toPlayContainer.GetChildCount() < 3){
			world.toPlayContainer.AddChild(InstantiateJewel());
		}
        // take last an add one
		activeItem = world.toPlayContainer.GetChild<Jewel>(0);
        world.toPlayContainer.RemoveChild(activeItem);
        world.toPlayContainer.AddChild(InstantiateJewel());
		activeItem.PlayLaser();
		activeItem.Position = lastPos;
		activeItem.isActive = true;
		for (int i = 0; i < world.toPlayContainer.GetChildCount(); i++){
			var item = world.toPlayContainer.GetChild<Jewel>(i);
			item.Position = new Vector3(
				i, // X position
				0f, // Y position
				0f - i
			);
			item.isActive = false;
		}
		activeItems.Add(activeItem);
        world.AddChild(activeItem);
        spawning = false;
		
	}

    Jewel InstantiateJewel(int _level = 0){

		int level = _level == 0 ? rng.RandiRange(1, MAX_LEVEL - 3): _level;
		var obj = objects[level];
		Jewel jewel = obj.Instantiate<Jewel>();
		jewel.Level = level;
		jewel.Scale = Vector3.One * getScaleFactor(level);
		jewel.Merge += Merge;
		return jewel;
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
			Jewel jewel = InstantiateJewel(level);
			jewel.Position = newPos;
			
			// remove jewel from list
			activeItems.Remove(current);
			activeItems.Remove(target);
            current.QueueFree();
            target.QueueFree();
			world.AddChild(jewel);
			jewel.PlayMerge();
			score += current.Level;
			jewel.Merge += Merge;

		}
    }

    private float getScaleFactor(int level){
		return Mathf.Lerp(1f, 2.5f, (level - 1f) / (MAX_LEVEL - 1f)); 
	}
}