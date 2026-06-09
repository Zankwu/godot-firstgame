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

		if (player != null)
		{
			
	
		}
		else
		{
			GD.PrintErr("找不到 Player 节点！");
		}
	}

	public override void HandleInput()
	{
		GD.Print($"成功获取 Player！{player}");
		if (player != null)
		{
			var direction = (player.Position - Position).Normalized();
			Velocity = speed * direction;
		}
	}
}
