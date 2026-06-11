using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Character : CharacterBody2D
{
	[Export]
	public int health;
	public int currentHealth;
	[Export]
	public int damage;
	[Export]
	public float speed;
	[Export]
	public float jumpPwoer;
	public float height = 0;
	public float heightSpeed;

	//击退值
	[Export]
	public float Knockback = 50f;
	[Export]
	private float knockDown;

	public enum State
	{
		idle, walk, punch, takeOff, jump, land, jumpKick, hurt, fall, grounded
	}

	protected Dictionary<State, string> stateAnima = new()
	{
		{State.idle,"idle"},
		{State.walk,"walk"},
		{State.punch,"punch"},
		{State.takeOff,"takeOff"},
		{State.jump,"jump"},
		{State.land,"land"},
		{State.jumpKick,"jumpKick"},
		{State.hurt,"hurt"},
		{State.fall,"fall"},
		{State.grounded,"grounded"},
	};

	protected State currentState = State.idle;



	//伤害发射器
	protected Area2D damageEmitter;
	// 伤害接受器
	protected DamageReceiver damageReceiver;


	//玩家精灵
	protected Sprite2D playerSprite2D;
	//玩家动画
	protected AnimationPlayer animationPlayer;
	protected const int GRAVITY = 600;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerSprite2D = GetNode<Sprite2D>("CharacterSprite");
		GD.Print(animationPlayer);
		damageEmitter = GetNode<Area2D>("DamageEmitter");
		damageReceiver = GetNode<DamageReceiver>("damage_receiver");
		//碰到后调用OnEmitCompleted
		damageEmitter.AreaEntered += OnEmitCompleted;
		damageReceiver.DamageCompleted += OnReceiverCompleted;
		//跳跃力度
		jumpPwoer = 150;
		currentHealth = health;
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

		// if (Canpunch() && Input.IsActionJustPressed("attack"))
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
				currentState = State.walk
	;
		}

		if (currentState == State.jump || currentState == State.jumpKick || currentState == State.fall)
		{
			height += heightSpeed * (float)delta;
			if (height < 0)
			{
				
				height = 0;
				if (currentState == State.fall)
				{
					currentState = State.grounded;
				}
				else
					currentState = State.land;
			}
			else
			{
				heightSpeed -= GRAVITY * (float)delta;
			}
			playerSprite2D.Position = Vector2.Up * height;
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
			playerSprite2D
	.FlipH = false;
			damageEmitter.Scale = new Vector2(1, damageEmitter.Scale.Y);
		}
		else if (Input.GetAxis("left", "right") < 0)
		{
			damageEmitter.Scale = new Vector2(-1, damageEmitter.Scale.Y);
			playerSprite2D
	.FlipH = true;
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
		currentState = State.idle;
	}
	public void completedTakeOffAction()
	{
		currentState = State.jump;
	}
	public void completedJumpAction()
	{
		currentState = State.land;
	}

	public void completedLandAction()
	{
		currentState = State.idle;
	}

	//伤害发射器
	public void OnEmitCompleted(Node2D temp)
	{
		var hitType = DamageReceiver.HitType.NORMAL;
		var direction = Position.X - temp.GlobalPosition.X < 0 ? Vector2.Right : Vector2.Left;
		if (currentState == State.jumpKick)
		{
			hitType = DamageReceiver.HitType.KNOCKDOWN;
		}
		temp.EmitSignal("DamageCompleted", damage, direction, (int)hitType);
	}

	public virtual void OnReceiverCompleted(int damageTemp, Vector2 direction, int hitTypeInt)
	{
		currentHealth -= damageTemp;
		if (currentHealth <= 0 || hitTypeInt == 1)
		{
			currentState = State.fall;
			heightSpeed = knockDown;
		}
		else
		{
			currentState = State.hurt;
		}
		Velocity = Knockback * direction;
		if (currentHealth <= 0)
		{
			QueueFree();
		}
	}
}
