using Godot;
using System;
using System.Linq;

public partial class BasicEnemy : Character
{

	// [Export]
	// public Player player;


	//近战攻击间隔
	[Export]
	public int bettwenDurationAttackMeleeTime;

	[Export]
	public int prepDurationAttackMeleeTime;

	//远程攻击间隔
	[Export]
	public int bettwenDurationAttackRangeTime;

	[Export]
	public int prepDurationAttackRangeTime;

	//近战攻击时间
	public ulong meleeLastSinceAttackTime = Time.GetTicksMsec();
	public ulong meleeReadySinceAttackTime = Time.GetTicksMsec();

	//远程攻击时间
	public ulong rangeLastSinceAttackTime = Time.GetTicksMsec(); //上次攻击时间
	public ulong rangeReadySinceAttackTime = Time.GetTicksMsec();

	public ulong durationTime;





	private EnemySlot playerSlot = null;
	public override void _Ready()
	{
		base._Ready();
		// 获取兄弟节点 Player
		player = GetNode<Player>("/root/World/acters/Player");
		attackAnimations = ["punch", "punch_ait"];


	}

	public override void HandleInput()
	{

		if (player != null && CanMove())
		{

			if (canRespawnKnife || hasKnife || hasGun)
			{
				AttackWithRange();
			}
			else
			{
				AttackWithMelee();
			}
		}
	}

	public void AttackWithRange()
	{
		// 获取相机和屏幕信息
		Camera2D camera = GetViewport().GetCamera2D();
		float screenWidth = GetViewport().GetVisibleRect().Size.X;

		// 计算屏幕左右边界（世界坐标）
		float screenLeftEdge = camera.GlobalPosition.X - screenWidth / 2;
		float screenRightEdge = camera.GlobalPosition.X + screenWidth / 2;

		// 计算左右目标点（Y 轴对齐玩家）
		Vector2 leftDestination = new Vector2(screenLeftEdge + 10, player.GlobalPosition.Y);
		Vector2 rightDestination = new Vector2(screenRightEdge - 10, player.GlobalPosition.Y);

		// 选择最近的目标
		Vector2 closestDestination;
		if ((leftDestination - GlobalPosition).Length() < (rightDestination - GlobalPosition).Length())
		{
			closestDestination = leftDestination;
		}
		else
		{
			closestDestination = rightDestination;
		}

		// 移动到目标
		if ((closestDestination - GlobalPosition).Length() < 1)
		{
			Velocity = Vector2.Zero;
		}
		else
		{
			Velocity = (closestDestination - GlobalPosition).Normalized() * speed;
		}
		if (CanRangeAttack() && hasKnife && rayCast.IsColliding())
		{
			currentState = State.throwKnife;
			rangeLastSinceAttackTime = Time.GetTicksMsec();
			lastThrowKnifeTime = Time.GetTicksMsec();
		}

		if (CanRangeAttack() && hasGun && rayCast.IsColliding())
		{
			currentState = State.PrepShot;
			rangeReadySinceAttackTime = Time.GetTicksMsec();
			Velocity = Vector2.Zero;
			HandlePrepShoot();
			// rangeLastSinceAttackTime = Time.GetTicksMsec();
		}
	}

	public void AttackWithMelee()
	{
		if (CanPickUp())
		{
			currentState = State.pickup;
			if (playerSlot != null)
			{
				player.FreeSlot(this);
			}
		}
		else if (playerSlot == null)
		{
			playerSlot = player.ReserveSlot(this);
		}
		if (playerSlot != null)
		{

			var direciton = (playerSlot.GlobalPosition - this.GlobalPosition).Normalized();
			if (isPlayerWithInRange())
			{
				if (CanPunch())
				{
					currentState = State.PrepAttack;
					meleeReadySinceAttackTime = Time.GetTicksMsec();
				}
				Velocity = Vector2.Zero;

			}
			else
			{
				Velocity = direciton * speed;
			}
		}
	}
	public override void HandlePrepShoot()
	{
		if (Time.GetTicksMsec() - (ulong)rangeReadySinceAttackTime > (ulong)prepDurationAttackRangeTime && currentState == State.PrepShot)
		{
			Shoot();
			rangeLastSinceAttackTime = Time.GetTicksMsec();
		}

	}
	public override void HandlePrepAttack()
	{

		if (Time.GetTicksMsec() - meleeReadySinceAttackTime > (ulong)prepDurationAttackMeleeTime && currentState == State.PrepAttack)
		{
			currentState = State.Attack;
			meleeLastSinceAttackTime = Time.GetTicksMsec();

			// 一行打乱
			attackAnimations = attackAnimations.OrderBy(x => Guid.NewGuid()).ToArray();
			// if (hasKnife)
			// {
			// 	currentState = State.throwKnife;
			// }
		}
	}

	public override void HandleKnifeRespawns()
	{
		base.HandleKnifeRespawns();
	}



	public override bool CanPunch()
	{
		if (Time.GetTicksMsec() - meleeLastSinceAttackTime < 2500)
		{
			return false;
		}
		return base.CanPunch();
	}

	public bool CanRangeAttack()
	{
		if (Time.GetTicksMsec() - rangeLastSinceAttackTime
			< (ulong)bettwenDurationAttackRangeTime)
		{
			return false;
		}
		return base.CanPunch();
	}



	public bool isPlayerWithInRange()
	{
		return (playerSlot.GlobalPosition - this.GlobalPosition).Length() < 1;
	}
	public override void setHeading()
	{
		// if(GlobalPosition.X > player.GlobalPosition.X)
		// {
		// 	heading = Vector2.Left;
		// }
		// else
		// {
		// 	heading = Vector2.Right;
		// }
		if (player == null || !CanMove())
		{
			return;
		}
		if (Position.X > player.Position.X)
		{
			heading = Vector2.Left;
		}
		else
		{
			heading = Vector2.Right;
		}
	}
	public override void OnReceiverCompleted(int damageTemp, Vector2 direction, int hitTypeInt)
	{
		base.OnReceiverCompleted(damageTemp, direction, hitTypeInt);
		GD.Print(currentHealth);
		GD.Print(currentState);
		if (currentHealth <= 0 || currentState == State.grounded)
		{

			player?.FreeSlot(this);

		}
	}



}
