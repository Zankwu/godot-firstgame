using Godot;
using System;

public partial class HealthBar : Control
{
	[Export]
	public TextureRect bar;

	[Export]
	public ColorRect white;
	[Export]
	public ColorRect Red;

	public int healthBarWidth;
	public HealthBar()
	{

	}

	public override void _Ready()
	{
		CallDeferred(nameof(ConnectDamageManager));
	}
	private void ConnectDamageManager()
	{
		if (DamageManager.Instance != null)
		{
			DamageManager.Instance.HealthChange += HandlerHealthChange;
		}
	}
	public override void _Process(double delta)
	{
		white.Scale = new Vector2(healthBarWidth+2, white.Scale.Y);
		Red.Scale = new Vector2(healthBarWidth, Red.Scale.Y);
		bar.Scale = new Vector2(healthBarWidth, bar.Scale.Y);
	}


	private void HandlerHealthChange(int temp)
	{
		healthBarWidth = temp;
	}

}
