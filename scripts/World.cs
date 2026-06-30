using Godot;
using System;

public partial class World : Node2D
{

	[Export]
	private CharacterBody2D player;

	[Export]
	private Camera2D camera;
	// Called when the node enters the scene tree for the first time.

	public bool is_lock_camera = false;
	public override void _Ready()
	{
		StageManager.Instance.CheckPointStart += OncheckPointStart;
		StageManager.Instance.CheckPointEnd += OncheckPointEnd;
	}

    private void OncheckPointEnd()
	{
		is_lock_camera = false;
	}


    private void OncheckPointStart()
	{
		is_lock_camera = true;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		//TODO 相机跟着对象走，不往回走
		if(player.Position.X > camera.Position.X && !is_lock_camera)
		{
			camera.Position = new Vector2(player.Position.X,camera.Position.Y);
		}
	}
}
