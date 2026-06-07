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
		walk,
		punch

	}

	private State currentState = State.idle;

	private AnimationPlayer animationPlayer;

	private Area2D damageEmitter;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		GD.Print(animationPlayer);
		damageEmitter =  GetNode<Area2D>("DamageEmitter");
		damageEmitter.Connect("area_entered", new Callable(this, nameof(OnEmitCompleted)));
		damageEmitter.Connect("area_entered",new Callable(this,nameof(tempEmit)));
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
		var direction = Input.GetVector("left", "right", "up", "down");
		Velocity = direction * speed;
		if (Input.IsActionJustPressed("attack") && CanAttack())
		{
			currentState = State.punch;

		}
	}

	public void HandleMove()
	{
		if (CanMove())
		{
			if (Velocity == Vector2.Zero)
			{
				currentState = State.idle;
			}
			else
				currentState = State.walk;
		}else
			Velocity = Vector2.Zero;

	}


	public void HandleAnimationChange()
	{
		if (currentState == State.idle)
		{
			animationPlayer.Play("idle");
		}
		else if (currentState == State.walk)
		{
			animationPlayer.Play("walk");
		}
		else if (currentState == State.punch)
		{

			animationPlayer.Play("punch");
		}
	}

	public bool CanAttack()
	{


		return currentState == State.idle || currentState == State.walk;


	}
	public bool CanMove()
	{
		return currentState == State.idle || currentState == State.walk;
	}


	public void completedAction()
	{
		currentState = State.idle;
	}


	public void OnEmitCompleted(DamageReceiver temp)
	{
		temp.EmitSignal("DamageReceived", damage);
		GD.Print(temp);
	}

	public void tempEmit(Area2D temp)
	{
		temp.EmitSignal("TempEx", temp);
		
	}

}
