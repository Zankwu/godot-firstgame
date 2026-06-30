using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class CheckPoint : Node2D
{

	[Export]
	public Node2D enemies;

	public Array<EnemyData> enemy_data = [];

	public Area2D playerDetectionArea;
	public bool is_activity = false;

	[Export]
	public int max_enemies;

	public int enemies_alive;
	public override void _Ready()
    {
		CreateEnemyData();
        playerDetectionArea = GetNode<Area2D>("PlayerDetectionArea");
        playerDetectionArea.BodyEntered += OncheckPoint;
        EntityManager.Instance.OnEnemyDeath += HandleEnemyDeath;
    }

    private void CreateEnemyData()
    {
        foreach (Character temp in enemies.GetChildren())
        {
            enemy_data.Add(new EnemyData(temp.Type, temp.GlobalPosition));

            temp.QueueFree();
        }
    }

    private void HandleEnemyDeath()
	{
		enemies_alive -= 1;
		if (enemies_alive == 0 && enemy_data.Count == 0)
		{
			StageManager.Instance.EmitSignal(StageManager.SignalName.CheckPointEnd);
			QueueFree();
		}

	}


	private void OncheckPoint(Node2D area2D)
	{
		if (!is_activity)
		{
			is_activity = true;
			StageManager.Instance.EmitSignal(StageManager.SignalName.CheckPointStart);
			enemies_alive = 0;

		}

	}


	public override void _Process(double delta)
	{

		HandleChecnkPoint();
	}

	private void HandleChecnkPoint()
	{
		
		if (enemy_data.Count > 0 && is_activity && enemies_alive < max_enemies)
		{
			enemies_alive += 1;
			var enemy_temp = enemy_data[0];
			enemy_data.RemoveAt(0);
			EntityManager.Instance.EmitSignal(EntityManager.SignalName.SpawnEnemy, enemy_temp);
		}
	}
}
