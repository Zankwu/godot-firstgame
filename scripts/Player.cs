using Godot;
using System;

public partial class Player : Character 
{
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

}
