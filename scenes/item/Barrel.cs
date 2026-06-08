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

	private State currentState;
	//桶的状态
	enum State
	{
		idle,
		destroyed,
	}
	private Timer time;

	private bool isMoveing = false;

	public override void _Ready()
	{
		damageRece = GetNode<DamageReceiver>("DamageReceiver");
		damageRece.DamageCompleted += OnDamageCompleted;
		currentState = State.idle;
		time = GetNode<Timer>("DestoryedTtimer");
		barrelAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{

		BarrelDestroyed(delta);
		BarrelAnimationHandler();


	}
	public void BarrelDestroyed(double delta)
	{
		if (currentState == State.destroyed && isMoveing == false)
		{
			time.WaitTime = 1;
			time.Start();
			GD.Print("开始移动");
			isMoveing = true;
		}
		if (isMoveing == true)
		{
			Position += velocity * (float)delta;
			GD.Print(Position);
			if (time.TimeLeft <= 0)
			{
				isMoveing = false;
				currentState = State.idle;
				QueueFree();
				GD.Print("jeishu移动");
			}
		}

	}

	public void BarrelAnimationHandler()
	{
		if (currentState == State.destroyed)
		{
			barrelAnimation.Play("destoryed");
		}
		
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public void OnDamageCompleted(int damage, Vector2 vector2)
	{
		//判断是从左还是从右打得
		GD.Print(vector2, GlobalPosition);
		if (vector2.X < GlobalPosition.X)
		{

			velocity = Vector2.Right * knockBack;
			velocity += Vector2.Up * knockBack;
			currentState = State.destroyed;
		}
		else if (vector2.X > GlobalPosition.X)
		{

			velocity = Vector2.Left * knockBack;
			currentState = State.destroyed;
		}
		GD.Print(vector2);
	}
}
