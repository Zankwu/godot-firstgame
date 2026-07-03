using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Door : Node2D
{
	
	[Signal]
	public delegate void OpenedEventHandler();

	public enum State
	{
		CLOSED, OPENNING, OPENED
	}

	public State currentState = State.CLOSED;

	[Export]
	public Sprite2D doorSprite;

	[Export]
	public float time_door_open_duration;

	[Export]
	public Array<BasicEnemy> enemies;

	public ulong time_door_open_start;

	public int doorSprite_height;
	public override void _Ready()
	{
		doorSprite_height = doorSprite.Texture.GetHeight();
		// OPEN();
	}

	public override void _Process(double delta)
	{
		if (currentState == State.OPENNING)
		{
			if (Time.GetTicksMsec() - time_door_open_start > time_door_open_duration)
			{
				currentState = State.OPENED;
				doorSprite.Position = Vector2.Up * doorSprite_height;
				EmitSignal("Opened");
			}
			else
			{
				var progress = (Time.GetTicksMsec() - time_door_open_start) / time_door_open_duration;
				doorSprite.Position = Vector2.Zero.Lerp(Vector2.Up * doorSprite_height, progress);
				
			}

		}

	}


	public void OPEN()
	{
		if (currentState == State.CLOSED)
		{
			currentState = State.OPENNING;
			time_door_open_start = Time.GetTicksMsec();
		}
	}



}
