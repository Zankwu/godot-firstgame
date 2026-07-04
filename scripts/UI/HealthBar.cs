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

	public HealthBar()
	{

	}

	public override void _Ready()
	{
		
	}
	


	public void RefreshHealthBar(int currentHealth,int max_health)
	{
		white.Scale = new Vector2(max_health+2, white.Scale.Y);
		Red.Scale = new Vector2(max_health, Red.Scale.Y);
		bar.Scale = new Vector2(currentHealth, bar.Scale.Y);
	}

}
