using Godot;
using System;
using System.Collections.Generic;

public partial class UIcontroller : CanvasLayer
{

	public int currentHealth;

	[Export]
	public HealthBar playerHealthBar;

	[Export]
	public HealthBar enemyHealthBar;

	[Export]
	public TextureRect enemyIcon;

	public ulong time_last_healthbar = Time.GetTicksMsec();

	public ulong time_duration_healthbar = 1000;

	public  Dictionary<Character.CharacterType,Texture2D> EnemyIcons = new()
	{
		{Character.CharacterType.PUNK,GD.Load<Texture2D>("res://assets/art/ui/avatars/avatar-punk.png")},
		{Character.CharacterType.GOON,GD.Load<Texture2D>("res://assets/art/ui/avatars/avatar-goon.png")},
		{Character.CharacterType.BOUNCER,GD.Load<Texture2D>("res://assets/art/ui/avatars/avatar-boss.png")},
	};

	public UIcontroller()
	{
		

	}
	public override void _Ready()
	{
		enemyHealthBar.Visible = false;
		enemyIcon.Visible = false;

	}
	public override void _Process(double delta)
	{
		if (Time.GetTicksMsec() - time_last_healthbar > time_duration_healthbar)
		{
			enemyHealthBar.Visible = false;
			enemyIcon.Visible = false;
		}
	}

	public override void _EnterTree()
	{
		DamageManager.Instance.HealthChange += OnCharacterHealthChange;
	}
	private void OnCharacterHealthChange(Character.CharacterType type, int currentHealth, int max_health)
	{
		if (type == Character.CharacterType.PLAYER)
		{
			playerHealthBar.RefreshHealthBar(currentHealth, max_health);
		}
		else
		{
			time_last_healthbar = Time.GetTicksMsec();
			enemyIcon.Visible = true;
			enemyIcon.Texture = EnemyIcons[type];
			enemyHealthBar.Visible = true; ;
			enemyHealthBar.RefreshHealthBar(currentHealth, max_health);

		}



	}

}
