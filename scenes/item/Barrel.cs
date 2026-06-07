using Godot;
using System;

public partial class Barrel : StaticBody2D
{


	private Area2D DamageReceiver;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DamageReceiver = GetNode<Area2D>("DamageReceiver");
		DamageReceiver.Connect("DamageReceived",new Callable(this,nameof(OnReceiverCompleted)));
		DamageReceiver.Connect("TempEx",new Callable(this,nameof(TempComp)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	

	public void OnReceiverCompleted(int temp)
	{
		GD.Print(temp);
		// QueueFree();
	}

	public void TempComp(Area2D temp)
	{
		GD.Print($"{temp} temp");
		// QueueFree();
	}

}
