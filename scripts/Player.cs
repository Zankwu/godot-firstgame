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
			GD.Print("xx");
			enemySlots.Add(s);
		}
	}


	public override void HandleInput()
	{
		var direction = Input.GetVector("left", "right", "up", "down");
		Velocity = direction * speed;
		if (CanPunch() && Input.IsActionJustPressed("attack"))
		{
			currentState = State.Attack;
			if (hasKnife)
			{
				currentState = State.throwKnife;
			}
			else if (canCombo)
			{
				attackIndex++;
				canCombo = false;
			}
			else
			{
				attackIndex = 0;
			}

			attackIndex = attackIndex % attackAnimations.Count();
		}

		if (CanJump() && Input.IsActionJustPressed("jump"))
		{
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
