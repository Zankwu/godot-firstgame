using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Character : CharacterBody2D
{
	protected Dictionary<State, string> stateAnima = new()
	{
		{State.idle,"idle"},
		{State.walk,"walk"},
		{State.takeOff,"takeOff"},
		{State.jump,"jump"},
		{State.land,"land"},
		{State.jumpKick,"jumpKick"},
		{State.hurt,"hurt"},
		{State.fall,"fall"},
		{State.grounded,"grounded"},
		{State.deadth,"grounded"},
		{State.fly,"fly"},
		{State.PrepAttack,"idle"},
		{State.throwKnife,"throw"},
		{State.pickup,"pickup"},
		{State.shot,"shot"},
	};
	public enum State
	{
		idle, walk, Attack, takeOff, jump, land, jumpKick, hurt, fall, grounded, deadth, fly,
		PrepAttack, throwKnife, pickup, shot

	}
	[Export]
	public int health;
	public int currentHealth;
	public int currentDamage;

	[Export]
	public bool hasKnife;

	[Export]
	public bool hasGun;
	[Export]
	public bool canRespawnKnife;
	//朝向
	public Vector2 heading = Vector2.Right;
	[Export]
	public int damage;
	[Export]
	public float speed;
	[Export]
	public float jumpPwoer;
	[Export]
	public float flyPwoer;
	public float height = 0;
	public float heightSpeed;


	[Export]
	public int attackPowerDamage;
	//击退值
	[Export]
	public float Knockback = 50f;
	[Export]
	private float knockDown;
	[Export]
	private float durationGrounded;
	[Export]
	private bool canRespawn;
	public bool canCombo;
	private ulong timeSinceGrounded = Time.GetTicksMsec();


	public string[] attackAnimations;
	public int attackIndex = 0;

	protected State currentState = State.idle;



	//伤害发射器
	protected Area2D damageEmitter;
	private Area2D chainDamageEmitter;

	// 伤害接受器
	protected DamageReceiver damageReceiver;


	//玩家精灵
	protected Sprite2D playerSprite2D;
	protected Sprite2D knifeSprite2D;
	protected Sprite2D gunSprite;
	protected RayCast2D rayCast;
	//玩家动画
	protected AnimationPlayer animationPlayer;
	protected CollisionShape2D collisionShape;

	protected Node2D weaponPosition;

	protected Area2D sensor;

	protected const int GRAVITY = 600;

	[Export]
	public int knfieRespownTime;

	public ulong lastThrowKnifeTime;


	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerSprite2D = GetNode<Sprite2D>("CharacterSprite");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		damageEmitter = GetNode<Area2D>("DamageEmitter");
		chainDamageEmitter = GetNode<Area2D>("ChainDamageEmitter");
		damageReceiver = GetNode<DamageReceiver>("DamageReceiver");
		knifeSprite2D = GetNode<Sprite2D>("KnifeSprite");
		gunSprite = GetNode<Sprite2D>("GunSprite");
		rayCast = GetNode<RayCast2D>("RayCast2D");
		sensor = GetNode<Area2D>("CollectibleSensor");
		weaponPosition = GetNode<Node2D>("KnifeSprite/WeaponPosition");

		//碰到后调用OnEmitCompleted
		damageEmitter.AreaEntered += OnEmitCompleted;
		damageReceiver.DamageCompleted += OnReceiverCompleted;
		chainDamageEmitter.BodyEntered += OnWallHit;
		chainDamageEmitter.AreaEntered += ChainReaction;
		lastThrowKnifeTime = Time.GetTicksMsec();


		//跳跃力度
		jumpPwoer = 150;
		currentHealth = health;

	}



	public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		HandleMove(delta);
		HandleGrounded();
		HandleDeadth(delta);
		HandleKnifeRespawns();
		HandleAnimationChange();
		FlipSprites();
		HandlePrepAttack();
		damageEmitter.Monitoring = isAttacking();
		damageReceiver.Monitorable = CanGetHurt();
		knifeSprite2D.Visible = hasKnife;
		gunSprite.Visible = hasGun;
		collisionShape.Disabled = currentState == State.grounded ||
		currentState == State.fall || currentState == State.fly;
		playerSprite2D.Position = Vector2.Up * height;
		knifeSprite2D.Position = Vector2.Up * height;
		gunSprite.Position = Vector2.Up * height;
		MoveAndSlide();
		setHeading();
	}

    private bool isAttacking()
    {
        return currentState == State.Attack || currentState == State.jumpKick;
    }

    public virtual void HandleKnifeRespawns()
	{
		if (!hasKnife)
		{
			//每隔2秒钟重新生成刀具
			if (canRespawnKnife && Time.GetTicksMsec() - lastThrowKnifeTime > (ulong)knfieRespownTime)
			{
				hasKnife = true;
			}
		}

	}

	public virtual void HandlePrepAttack()
	{

	}

	private void HandleDeadth(double delta)
	{
		if (currentHealth <= 0 || (!canRespawn && currentHealth <= 0))
		{
			Velocity = Vector2.Zero;
			currentState = State.deadth;
			Modulate = new Color(Modulate, Modulate.A - (float)delta);
			if (Modulate.A <= 0)
			{
				QueueFree();

			}
		}
	}


	private void HandleGrounded()
	{
		if (currentState == State.grounded && (Time.GetTicksMsec() - timeSinceGrounded > durationGrounded))
		{
			currentState = State.land;
		}
	}

	public virtual void HandleInput()
	{

	}

	public void HandleMove(double delta)
	{
		if (currentState == State.Attack)
		{
			Velocity = Vector2.Zero;
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
					timeSinceGrounded = Time.GetTicksMsec();
				}
				else
					currentState = State.land;
				Velocity = Vector2.Zero;
			}
			else
			{
				heightSpeed -= GRAVITY * (float)delta;
			}



		}
	}


	public void HandleAnimationChange()
	{
		if (currentState == State.Attack)
		{
			animationPlayer.Play(attackAnimations[attackIndex]);

		}
		else
		{
			animationPlayer.Play(stateAnima[currentState]);

		}
	}
	public virtual void setHeading()
	{

	}
	public void FlipSprites()
	{

		if (heading == Vector2.Right)
		{
			playerSprite2D.FlipH = false;
			knifeSprite2D.Scale = new Vector2(1, knifeSprite2D.Scale.Y);
			gunSprite.Scale = new Vector2(1, gunSprite.Scale.Y);

			damageEmitter.Scale = new Vector2(1, damageEmitter.Scale.Y);
			rayCast.Scale = new Vector2(1, rayCast.Scale.Y);
		}
		else
		{

			playerSprite2D.FlipH = true;
			damageEmitter.Scale = new Vector2(-1, damageEmitter.Scale.Y);
			gunSprite.Scale = new Vector2(-1, gunSprite.Scale.Y);

			rayCast.Scale = new Vector2(-1, rayCast.Scale.Y);
			knifeSprite2D.Scale = new Vector2(-1, knifeSprite2D.Scale.Y);

		}
	}
	public virtual bool CanPickUp()
	{
		var areas = sensor.GetOverlappingAreas();
		if (areas.Count != 0)
		{
			Collectible collectType = areas[0] as Collectible;
			if (!hasKnife && collectType.currentType == Collectible.Type.knife)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void PickedUp()
	{
		if (CanPickUp())
		{
			var areas = sensor.GetOverlappingAreas();
			hasKnife = true;
			areas[0].QueueFree();
			currentState = State.idle;
		}
	}


	public bool CanGetHurt()
	{
		return currentState == State.idle
		|| currentState == State.walk || currentState == State.takeOff
		|| currentState == State.land || currentState == State.PrepAttack;
	}

	public virtual bool CanPunch()
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

	public void CompletedThrowAcition()
	{
		currentState = State.idle;
		hasKnife = false;
		var knife_globalpositon = new Vector2(weaponPosition.GlobalPosition.X,GlobalPosition.Y);

		EntityManager.Instance.EmitSignal(EntityManager.SignalName.SpawnCollectibles, 
		(int)Collectible.Type.knife, 
		(int)Collectible.State.fly, 
		knife_globalpositon, 
		heading, 
		-weaponPosition.Position.Y);

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
		canCombo = true;
		currentDamage = damage;
		if (currentState == State.jumpKick)
		{
			hitType = DamageReceiver.HitType.KNOCKDOWN;
		}
		if (attackIndex == attackAnimations.Count() - 1)
		{
			hitType = DamageReceiver.HitType.POWER;
			currentDamage = attackPowerDamage;

		}
		temp.EmitSignal("DamageCompleted", currentDamage, direction, (int)hitType);
	}

	public virtual void OnReceiverCompleted(int damageTemp, Vector2 direction, int hitTypeInt)
	{
		if (CanGetHurt())
		{
			if (hasKnife)
			{
				hasKnife = false;
				lastThrowKnifeTime = Time.GetTicksMsec();
			}
			canRespawnKnife = false;
			GD.Print($"message: {canRespawnKnife}");
			currentHealth -= damageTemp;
			GD.Print($"currentHealth{currentHealth}");
			if (currentHealth <= 0 || hitTypeInt == 1)
			{
				currentState = State.fall;
				heightSpeed = knockDown;
				Velocity = Knockback * direction;
			}
			else if (hitTypeInt == (int)DamageReceiver.HitType.POWER)
			{
				currentState = State.fly;
				Velocity = flyPwoer * direction;

			}
			else
			{
				currentState = State.hurt;
				Velocity = Knockback * direction;
			}
		}
	}

	private void OnWallHit(Node2D wall)
	{

		if (wall is AnimatableBody2D)
		{
			currentState = State.fall;
			heightSpeed = knockDown;
			Velocity = -Velocity / 2;
		}

	}
	private void ChainReaction(Area2D receiver)
	{
		// 排除自身的 DamageReceiver
		if (receiver.GetParent() == this) return;

		var direction = Position.X - receiver.GlobalPosition.X < 0 ? Vector2.Right : Vector2.Left;
		receiver.EmitSignal("DamageCompleted", damage, direction, (int)DamageReceiver.HitType.KNOCKDOWN);
	}


}
