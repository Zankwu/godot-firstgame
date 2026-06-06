using Godot;
using System;

public partial class World : Node2D
{

	[Export]
	private CharacterBody2D player;

	[Export]
	private Camera2D camera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(player.Position.X > camera.Position.X)
		{
			camera.Position = new Vector2(player.Position.X,camera.Position.Y);
		}
	}
}
