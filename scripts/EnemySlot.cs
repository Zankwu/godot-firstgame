using Godot;
using System;

public partial class EnemySlot : Node2D
{


	public BasicEnemy occupant = null;


	public bool SlotIsFree()
	{
		return occupant == null;
	}

	public void FreeSlot()
	{
		occupant = null;
	}

	public void setEnemy(BasicEnemy enemy)
	{
		occupant = enemy;
	}


}

