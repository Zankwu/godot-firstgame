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
		//获取槽位
		enemySlots = new List<EnemySlot>();
		var enemySlotsTemp = GetNode<Node2D>("EnemySlots");
		foreach (EnemySlot a in enemySlotsTemp.GetChildren())
		{
			enemySlots.Add(a);
			GD.Print("join~");
		}
	}


	public override void HandleInput()
	{
		var direction = Input.GetVector("left", "right", "up", "down");
		Velocity = direction * speed;
		if (CanPunch() && Input.IsActionJustPressed("attack"))
		{
			currentState = State.punch;
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

		//是否有可用槽位
		List<EnemySlot> availableSlots =
		enemySlots.FindAll(slot => slot.SlotIsFree());
		//没有返回空
		if(availableSlots.Count <= 0)
		{
			return null;
		}
		//选出最短的槽位
		availableSlots.Sort((a, b) =>
		{
			float distA = (basciEnemy.GlobalPosition - a.GlobalPosition).Length();
			float distB = (basciEnemy.GlobalPosition - b.GlobalPosition).Length();
			return distA.CompareTo(distB);
		});
		//最近的位置设置敌人
		availableSlots[0].setEnemy(basciEnemy);
		return availableSlots[0];
	}

	public void FreeSlot(BasicEnemy enemy)
	{
		var target_slots = enemySlots.FindAll(slot => slot.occupant == enemy);

		if (target_slots.Count() == 1)
		{
			target_slots[0].FreeSlot();
		}
	}
}
