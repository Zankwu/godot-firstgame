using Godot;
using System;
using System.Diagnostics;

public partial class Character : CharacterBody2D
{
	[Export]
	public int health;
	[Export]
	public int damage;
	[Export]
	public float speed;

	private enum State
	{
		idle,
		walk
	}

	private State currentState = State.idle;

	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		GD.Print(animationPlayer);
	}


	public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		HandleMove();
		HandleAnimationChange();
		MoveAndSlide();

	}

	public void HandleInput()
	{
		var direction = Input.GetVector("left","right","up","down");
		Velocity = direction * speed;
	}

	public void HandleMove()
	{
		if (Velocity == Vector2.Zero)
		{
			currentState = State.idle;
		}else
			currentState = State.walk;
	}


	public void HandleAnimationChange()
	{
		if(currentState == State.idle)
		{
			animationPlayer.Play("idle");
		}else if(currentState == State.walk)
		{
			animationPlayer.Play("walk");
		}
	}




}
