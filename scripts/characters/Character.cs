using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Character : CharacterBody2D
{
	[Export]
	public Player player;
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
		{State.PrepShot,"idle"},
		{State.Recover,"recover"},
	};
	public enum State
	{
		idle, walk, Attack, takeOff, jump, land, jumpKick, hurt, fall, grounded, deadth, fly,
		PrepAttack, throwKnife, pickup, shot, PrepShot, Recover
	}


	public enum CharacterType
	{
		PLAYER, PUNK, GOON, THUG, BOUNCER
	}

	[Export]
	public CharacterType Type;

	[Export]
	public bool auto_destroyed_on_drop;

	[Export]
	public float time_duration_last_attack;
	public float time_since_last_attack;

	[Export]
	public int max_health;
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
	public float knockDown;
	[Export]
	public float durationGrounded;
	[Export]
	public bool canRespawn;
	public bool canCombo;
	public ulong timeSinceGrounded = Time.GetTicksMsec();


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
	private Collectible.Type collectibleType;

	[Export]
	public int ammo_left;
	public int ammo_max = 3;


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
		currentHealth = max_health;

	}



	public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		HandleMove(delta);
		HandleGrounded();
		HandleDeadth(delta);
		HandleKnifeRespawns();
		HandleAnimationChange();
		HandlePrepShoot();
		FlipSprites();
		HandlePrepAttack();
		HandlerAirTime(delta);

		damageEmitter.Monitoring = isAttacking();
		damageReceiver.Monitorable = CanGetHurt();
		knifeSprite2D.Visible = hasKnife;
		gunSprite.Visible = hasGun;
		collisionShape.Disabled = currentState == State.grounded ||
		currentState == State.fall || currentState == State.fly;
		playerSprite2D.Position = Vector2.Up * height;
		knifeSprite2D.Position = Vector2.Up * height;
		gunSprite.Position = Vector2.Up * height;
		chainDamageEmitter.Monitoring = currentState == State.fly;

		MoveAndSlide();
		setHeading();
	}

	public virtual bool isAttacking()
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

	public virtual void HandlePrepShoot()
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
				EntityManager.Instance.EmitSignal(EntityManager.SignalName.OnEnemyDeath);

				QueueFree();
			}
		}
	}


	public virtual void HandleGrounded()
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

	}

	private void HandlerAirTime(double delta)
	{
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
			if (!hasKnife && !hasGun)
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
			Collectible collectible = areas[0] as Collectible;
			if (collectible.currentType == Collectible.Type.knife)
			{
				hasKnife = true;

			}
			else if (collectible.currentType == Collectible.Type.gun)
			{
				hasGun = true;
				ammo_left = ammo_max;
			}
			else if (collectible.currentType == Collectible.Type.food)
			{
				GD.Print($"currentHealth: {currentHealth}");

				currentHealth = max_health;
				GD.Print($"currentHealth: {currentHealth}");
			}

			collectible.QueueFree();
			currentState = State.idle;
		}
	}


	public virtual bool CanGetHurt()
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


	public virtual void completedIdleAction()
	{
		currentState = State.idle;
	}

	public void CompletedThrowAcition()
	{
		currentState = State.idle;
		hasKnife = false;
		var knife_globalpositon = new Vector2(weaponPosition.GlobalPosition.X, GlobalPosition.Y);

		EntityManager.Instance.EmitSignal(EntityManager.SignalName.SpawnCollectibles,
		(int)Collectible.Type.knife,
		(int)Collectible.State.fly,
		knife_globalpositon,
		heading,
		-weaponPosition.Position.Y,
		auto_destroyed_on_drop);

	}

	public virtual void Shoot()
	{
		var weapon_root_position = new Vector2(weaponPosition.GlobalPosition.X, Position.Y);

		if (ammo_left > 0)
		{
			currentState = State.shot;
			Velocity = Vector2.Zero;
			var target_point = heading * (this.GlobalPosition.X + GetViewport().GetVisibleRect().Size.X);
			var target = rayCast.GetCollider();
			if (target != null)
			{
				target_point = rayCast.GetCollisionPoint();
				if (target is Character)
				{
					var against = target as Character;
					against.OnReceiverCompleted(8, heading, (int)DamageReceiver.HitType.KNOCKDOWN);
				}
			}
			var distance = target_point.X - weaponPosition.GlobalPosition.X;
			EntityManager.Instance.EmitSignal(EntityManager.SignalName.SpawnShot,
			weapon_root_position,
			distance,
			-weaponPosition.Position.Y
			);
		}
		else
		{
			EntityManager.Instance.EmitSignal(EntityManager.SignalName.SpawnCollectibles,
			(int)Collectible.Type.gun,
			(int)Collectible.State.fly,
			weapon_root_position,
			heading,
			-weaponPosition.Position.Y,
			auto_destroyed_on_drop
			);
			hasGun = false;
		}


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
	public virtual void OnEmitCompleted(Node2D temp)
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
			attackIndex = 0;
			if (hasKnife)
			{
				hasKnife = false;
				lastThrowKnifeTime = Time.GetTicksMsec();
				collectibleType = Collectible.Type.knife;
				EntityManager.Instance.EmitSignal(EntityManager.SignalName.SpawnCollectibles,
							(int)collectibleType,
							(int)Collectible.State.fall,
							GlobalPosition,
							Vector2.Zero,
							-weaponPosition.Position.Y,
							auto_destroyed_on_drop
							);
			}
			if (hasGun)
			{
				hasGun = false;
				collectibleType = Collectible.Type.gun;
				EntityManager.Instance.EmitSignal(EntityManager.SignalName.SpawnCollectibles,
							(int)collectibleType,
							(int)Collectible.State.fall,
							GlobalPosition,
							Vector2.Zero,
							-weaponPosition.Position.Y,
							auto_destroyed_on_drop
							);

			}


			canRespawnKnife = false;
			currentHealth -= damageTemp;
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

	public virtual void OnWallHit(Node2D wall)
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
		receiver.EmitSignal(DamageReceiver.SignalName.DamageCompleted, damage, direction, (int)DamageReceiver.HitType.KNOCKDOWN);
	}


}
