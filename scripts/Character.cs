using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Character : CharacterBody2D ,IDamageable
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

	protected Dictionary<State, string> stateAnima = new()
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

	protected AnimationPlayer animationPlayer;

	//伤害发射器
	protected Area2D damageEmitter;
	// 伤害接受器
	protected DamageReceiver damageReceiver;

	//玩家精灵
	protected Sprite2D playerBody;

	protected const int GRAVITY = 600;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerBody = GetNode<Sprite2D>("CharacterSprite");
		GD.Print(animationPlayer);
		damageEmitter = GetNode<Area2D>("DamageEmitter");
		damageReceiver = GetNode<DamageReceiver>("damage_receiver");
		//碰到后调用OnEmitCompleted
		damageEmitter.AreaEntered += OnEmitCompleted;

		damageEmitter.BodyEntered += OnEmitCompleted;

		damageReceiver.DamageCompleted += OnReceiverCompleted;
																
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


		if (temp.GetParent() is IDamageable damageable)
        {
            temp.EmitSignal("DamageCompleted", damage, GlobalPosition);
        }
	}

	public void OnReceiverCompleted(int damage, Vector2 vector2)
	{
		GD.Print($"1碰到后调用{damage}");

	}

    public void TakeDamage(int damage, Vector2 position)
    {
        throw new NotImplementedException();
    }

}
