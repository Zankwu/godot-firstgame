using Godot;
using System;
using System.Collections.Generic;

public partial class Acters : Node2D
{
	public Dictionary<Collectible.Type, string> preScence = new() {
		{
			Collectible.Type.knife,"res://scenes/item/knife.tscn"
		},
	};


	public override void _Ready()
	{
		EntityManager.Instance.SpawnCollectibles += OnCollectibleSpawn;
	}



	public void OnCollectibleSpawn(int intType, int intState,
	Vector2 collectiblePosition, Vector2 direction, float height)
	{
		var type = (Collectible.Type)intType;
		var state = (Collectible.State)intState;
		GD.Print($"message: 获得到飞刀了");
		PackedScene packed = ResourceLoader.Load<PackedScene>(preScence[(Collectible.Type)type]);
		Collectible collec = packed.Instantiate() as Collectible;
		collec.currentState = state;
		collec.GlobalPosition = collectiblePosition;
		collec.direction = direction;
		collec.height = height;
		AddChild(collec);
	}

	public void OnShotSpawn()
	{
	}
}
