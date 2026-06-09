using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Character : CharacterBody2D
{
	[Export]
	public int health;
	[Export]
	public int damage;
	[Export]
	public float speed;
	[Export]
	public float jumpPwoer;

	public float height = 0;
	public float heightSpeed;

	public enum State
	{
		idle,
		walk,
		punch,
		takeOff,
		jump,
		land,
		jumpKick

	}

	protected  Dictionary<State, string> stateAnima = new()
	{
		{State.idle,"idle"},
		{State.walk,"walk"},
		{State.punch,"punch"},
		{State.takeOff,"takeOff"},
		{State.jump,"jump"},
		{State.land,"land"},
		{State.jumpKick,"jumpKick"},
	};

	protected State currentState = State.idle;

	protected  AnimationPlayer animationPlayer;

	protected  Area2D damageEmitter;
	//玩家精灵
	protected  Sprite2D playerBody;

	protected  const int GRAVITY = 600;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerBody = GetNode<Sprite2D>("CharacterSprite");
		GD.Print(animationPlayer);
		damageEmitter = GetNode<Area2D>("DamageEmitter");
		//碰到后调用OnEmitCompleted
		damageEmitter.AreaEntered += OnEmitCompleted;

		//跳跃力度
		jumpPwoer = 150;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		HandleMove(delta);
		HandleAnimationChange();
		FlipSprites();
		MoveAndSlide();
	}

	public virtual void HandleInput()
	{
		// var direction = Input.GetVector("left", "right", "up", "down");
		// Velocity = direction * speed;

		// if (CanPunch() && Input.IsActionJustPressed("attack"))
		// {
		// 	currentState = State.punch;
		// }

		// if (CanJump() && Input.IsActionJustPressed("jump"))
		// {
		// 		heightSpeed = jumpPwoer;
		// 		currentState = State.takeOff;
		// }
		// if (CanJumpKick() && Input.IsActionJustPressed("attack"))
		// {
		// 	currentState = State.jumpKick;

		// }
	}

	public void HandleMove(double delta)
	{
		if (currentState == State.punch)
		{
			Velocity = Vector2.Zero;
			GD.Print("1");
			return;
		}
		if (CanMove())
		{
			if (Velocity == Vector2.Zero)
			{
				currentState = State.idle;
			}
			else
				currentState = State.walk;
		}
		if (currentState == State.jump || currentState == State.jumpKick)
		{
			height += heightSpeed * (float)delta;
			if (height < 0)
			{
				currentState = State.land;
				height = 0;
			}
			else
			{
				heightSpeed -= GRAVITY * (float)delta;
			}


			playerBody.Position = Vector2.Up * height;
		}
	}


	public void HandleAnimationChange()
	{
		animationPlayer.Play(stateAnima[currentState]);
	}

	public void FlipSprites()
	{

		if (Input.GetAxis("left", "right") > 0)
		{
			playerBody.FlipH = false;
			damageEmitter.Scale = new Vector2(1, damageEmitter.Scale.Y);
		}
		else if (Input.GetAxis("left", "right") < 0)
		{
			damageEmitter.Scale = new Vector2(-1, damageEmitter.Scale.Y);
			playerBody.FlipH = true;
		}
	}
	public bool CanPunch()
	{
		return currentState == State.idle || currentState == State.walk;
	}

	public bool CanJumpKick()
	{
		return currentState == State.jump;
	}
	public bool CanMove()
	{
		return currentState == State.idle || currentState == State.walk;
	}
	public bool CanJump()
	{
		return currentState == State.idle || currentState == State.walk;
	}


	public void completedIdleAction()
	{
		GD.Print("✅ completedIdleAction 被调用！");
		currentState = State.idle;
	}
	public void completedTakeOffAction()
	{
		GD.Print("✅ completedTakeOffAction 被调用！");
		currentState = State.jump;
	}
	public void completedJumpAction()
	{
		GD.Print("✅ completedJumpAction 被调用！");
		currentState = State.land;
	}

	public void completedLandAction()
	{
		GD.Print("✅ completedLandAction 被调用！");
		currentState = State.idle;
	}


	public void OnEmitCompleted(Area2D temp)
	{
		GD.Print($"1碰到后调用{temp}");

		temp.EmitSignal("DamageCompleted", damage, GlobalPosition);

	}
}
