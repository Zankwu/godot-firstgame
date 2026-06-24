using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Character
{


	public List<EnemySlot> enemySlots;

	public override void _Ready()
	{

		base._Ready();
		attackAnimations = ["punch", "powerAttack", "kick", "roundKick"];

		enemySlots = new List<EnemySlot>();
		//获取槽位
		var Slots = GetNode<Node2D>("EnemySlots");
		GD.Print(Slots);
		foreach (EnemySlot s in Slots.GetChildren())
		{
			enemySlots.Add(s);
		}


	}


	public override void HandleInput()
	{
		base.HandleInput();
		if (CanMove())
		{
			var direction = Input.GetVector("left", "right", "up", "down");

			Velocity = direction * speed;
		}

		if (CanPunch() && Input.IsActionJustPressed("attack"))
		{
			currentState = State.Attack;
			if (CanPickUp())
			{
				currentState = State.pickup;
			}
			else if (hasKnife)
			{
				currentState = State.throwKnife;
			}
			else if (hasGun)
			{
				
				Shoot();
			}
			else if (canCombo && (Time.GetTicksMsec() - time_since_last_attack < time_duration_last_attack))
			{

				attackIndex++;
				canCombo = false;
				time_since_last_attack = Time.GetTicksMsec();
			}
			else
			{
				attackIndex = 0;
				time_since_last_attack = Time.GetTicksMsec();
			}

			attackIndex = attackIndex % attackAnimations.Count();
		}

		if (CanJump() && Input.IsActionJustPressed("jump"))
		{
			attackIndex = 0;
			heightSpeed = jumpPwoer;
			currentState = State.takeOff;
		}
		if (CanJumpKick() && Input.IsActionJustPressed("attack"))
		{
			currentState = State.jumpKick;

		}
	}

	//返回最近的slot
	public EnemySlot ReserveSlot(BasicEnemy basciEnemy)
	{
		var avaliableSlots = enemySlots.FindAll(e => e.SlotIsFree());
		if (avaliableSlots.Count <= 0)
		{
			return null;
		}
		avaliableSlots.Sort((a, b) =>
		{
			float distA = (basciEnemy.GlobalPosition - a.GlobalPosition).Length();
			float distB = (basciEnemy.GlobalPosition - b.GlobalPosition).Length();
			return distA.CompareTo(distB);
		});
		avaliableSlots[0].setSlot(basciEnemy);
		return avaliableSlots[0];
	}

	public void FreeSlot(BasicEnemy enemy)
	{
		var target_slots = enemySlots.FindAll(slot => slot.occupant == enemy);

		if (target_slots.Count() == 1)
		{
			target_slots[0].FreeSlot();
		}
	}

	public override void setHeading()
	{
		if (CanMove())
		{
			if (Velocity.X > 0)
			{
				heading = Vector2.Right;
			}
			else if (Velocity.X < 0)
			{
				heading = Vector2.Left;
			}
		}

	}

	public override void Shoot()
	{
		base.Shoot();
		ammo_left -= 1;
	}

}
