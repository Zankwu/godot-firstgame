using Godot;
using System;

public partial class EnemySlot : Node2D
{


	public BasicEnemy occupant = null;


	public bool SlotIsFree()
	{
		return occupant == null;
	}

	public void setSlot(BasicEnemy enemy)
	{
		occupant = enemy;
	}

	public void FreeSlot()
	{
		occupant = null;
	}

}

