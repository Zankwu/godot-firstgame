using Godot;
using System;

public partial class Character : CharacterBody2D
{
	[Export]
	public int health;
	[Export]
	public int damage;
	[Export]
	public float speed;

	public override void _Process(double delta)
	{
		
		var direction = Input.GetVector("left","right","up","down");
		Velocity = new Vector2(direction.X * speed,direction.Y*speed);
		MoveAndSlide();
	}

}
