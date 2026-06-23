using Godot;
using System;

public partial class Shot : Line2D
{	
	[Export]
	public float shot_duration_time;

	public float duration_shot = 0;

	public float height = 0;
	public float shot_distance = 0;

	public ulong time_start = Time.GetTicksMsec();

	

	// Called when the node enters the scene tree for the first time.
	public void initialize(float distance,float gun_height)
	{
		height = gun_height;
		shot_distance = distance;
		AddPoint(new Vector2(0,-height),0);
		AddPoint(new Vector2(distance,-height),1);
		duration_shot = Mathf.Abs(shot_distance) * shot_duration_time / GetViewport().GetVisibleRect().Size.X;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var elapsed = Time.GetTicksMsec() - time_start;
		var progress = elapsed / duration_shot;
		var new_x = Mathf.Lerp(0,shot_distance,progress);
		SetPointPosition(0,new Vector2(new_x,-height));
		if(progress >= 1)
		{
			QueueFree();
		}
	}
}
