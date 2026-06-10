using Godot;
using System;

public partial class BasicEnemy : Character
{

	[Export]
	public CharacterBody2D player;
	public override void _Ready()
	{
		base._Ready();

		// 获取兄弟节点 Player
		player = GetNode<CharacterBody2D>("../Player");


	}

	public override void HandleInput()
	{
		
		if (player != null)
		{
			var direction = (player.Position - Position).Normalized();
			Velocity = speed * direction;
		}
	}
}
