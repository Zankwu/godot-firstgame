using Godot;
using System;

public partial class BasicEnemy : Character
{

	[Export]
	public Player player;

	private EnemySlot playerSlot = null;
	public override void _Ready()
	{
		base._Ready();

		// 获取兄弟节点 Player
		player = GetNode<Player>("../Player");
	}

	public override void HandleInput()
	{

		if (player != null && CanMove())
		{
			if (playerSlot == null)
			{
				playerSlot = player.ReserveSlot(this);
			}
			if (playerSlot != null)
			{
				var direction = (playerSlot.GlobalPosition - GlobalPosition).Normalized();
				Velocity = direction * speed;
			}
		}
	}
}
