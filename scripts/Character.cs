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
	private Sprite2D playerBody;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerBody = GetNode<Sprite2D>("CharacterSprite");
		GD.Print(animationPlayer);
		damageEmitter = GetNode<Area2D>("DamageEmitter");
		//碰到后调用OnEmitCompleted
		damageEmitter.AreaEntered += OnEmitCompleted;
	}

    

    public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		HandleMove();
		HandleAnimationChange();
		FlipSprites();
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

	public void FlipSprites()
	{
		
			if (Input.GetAxis("left", "right") > 0)
			{
				playerBody.FlipH=false;
				damageEmitter.Scale= new Vector2(1, damageEmitter.Scale.Y);
			}else if(Input.GetAxis("left", "right")<0)
			{
				damageEmitter.Scale= new Vector2(-1, damageEmitter.Scale.Y);
				playerBody.FlipH=true;
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


	public void OnEmitCompleted(Area2D temp)
	{
		GD.Print($"1碰到后调用{temp}");
		
		temp.EmitSignal("DamageCompleted", damage,GlobalPosition);
		
	}
}
