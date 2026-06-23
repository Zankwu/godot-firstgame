using Godot;
using System;
using System.Collections.Generic;

public partial class Acters : Node2D
{
	public const string SHOT_PREFAB = "res://scenes/item/shot.tscn";
	public Dictionary<Collectible.Type, string> preScence = new() {
			{Collectible.Type.knife,"res://scenes/item/knife.tscn"},
			{Collectible.Type.gun,"res://scenes/item/gun.tscn"},
			{Collectible.Type.food,"res://scenes/item/food.tscn"}
	};


	public override void _Ready()
	{
		EntityManager entityManager = EntityManager.Instance;
		entityManager.SpawnCollectibles += OnCollectibleSpawn;
		entityManager.SpawnShot += OnShotSpawn;
	}



	public void OnCollectibleSpawn(int intType, int intState,
	Vector2 collectiblePosition, Vector2 direction, float height, bool auto_destroyed)
	{
		var type = (Collectible.Type)intType;
		var state = (Collectible.State)intState;
		PackedScene packed = ResourceLoader.Load<PackedScene>(preScence[(Collectible.Type)type]);
		Collectible collec = packed.Instantiate() as Collectible;
		collec.currentState = state;
		collec.GlobalPosition = collectiblePosition;
		collec.direction = direction;
		collec.height = height;
		collec.auto_destroyed = auto_destroyed;
		CallDeferred("add_child", collec);
	}

	public void OnShotSpawn(Vector2 gun_root_position, float distance, float gun_height)
	{
		PackedScene shotPacked = ResourceLoader.Load<PackedScene>(SHOT_PREFAB);
		var shot = shotPacked.Instantiate() as Shot;
		AddChild(shot);
		shot.Position = gun_root_position;
		shot.initialize(distance, gun_height);

	}
}
