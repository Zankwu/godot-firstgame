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
				var direciton = (playerSlot.GlobalPosition - this.GlobalPosition).Normalized();
				if ((playerSlot.GlobalPosition - this.GlobalPosition).Length() < 1)
				{
					Velocity = Vector2.Zero;
				}
				else
				{
					Velocity = direciton * speed;

				}
			}
		}
	}
	public override void OnReceiverCompleted(int damageTemp, Vector2 direction,int hitTypeInt)
	{
		base.OnReceiverCompleted(damageTemp,direction,hitTypeInt);
		GD.Print(currentHealth);
		if(currentHealth <= 0)
		{
			playerSlot.FreeSlot();
			
		}
	}

}
