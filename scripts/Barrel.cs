using Godot;
using System;

public partial class Barrel : StaticBody2D
{


	private DamageReceiver damageRece;

	private AnimationPlayer barrelAnimation;

	// Called when the node enters the scene tree for the first time.
	private Vector2 tempVec;
	//桶的基本移动速度
	private float knockBack = 50;
	//速度
	private Vector2 velocity = Vector2.Zero;

	private Sprite2D barrelSprte2D;

	//高度
	private double height = 0.0;
	//下降速率
	private double heightSpeed = 0.0;
	//桶的状态
	private State currentState;
	//重力
	private const int GRAVITY = 600;
	enum State
	{
		idle,
		destroyed,
	}


	public override void _Ready()
	{
		damageRece = GetNode<DamageReceiver>("DamageReceiver");
		barrelSprte2D = GetNode<Sprite2D>("Sprite2D");
		damageRece.DamageCompleted += OnDamageCompleted;
		currentState = State.idle;
		barrelAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	public override void _Process(double delta)
	{
		BarrelDestroyed(delta);
		BarrelAnimationHandler();
		FallingHandle(delta);
	}
	public void BarrelDestroyed(double delta)
	{
		if (currentState == State.destroyed)
		{
			barrelSprte2D.Frame = 1;
		}
	}

	public void BarrelAnimationHandler()
	{
		if (currentState == State.destroyed)
		{
			// barrelAnimation.Play("destoryed");
		}

	}
	// TODO 接收到信号后调用
	public void OnDamageCompleted(int damage, Vector2 direction,int hitTypeInt)
	{
		if (currentState == State.idle)
		{
			currentState = State.destroyed;
			heightSpeed = knockBack * 2;
		}
		//判断是从左还是从右打得

		velocity = direction * knockBack;


	}

	//TODO 这是桶子移动操作
	public void FallingHandle(double delta)
	{

		if (currentState == State.destroyed)
		{
			height += heightSpeed * delta;
			if (height < 0)
			{
				height = 0;
				QueueFree();
			}
			else
			{
				heightSpeed -= GRAVITY * delta;

			}
			GD.Print($"heightSpeed:{heightSpeed},{height}");
			Position += velocity * (float)delta;
			barrelSprte2D.Position = Vector2.Up * (float)height;
		}
	}

}
