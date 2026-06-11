using Godot;
using System;

public partial class DamageReceiver : Area2D
{
  [Signal]
  public delegate void DamageCompletedEventHandler(int damage,Vector2 direction);
}
