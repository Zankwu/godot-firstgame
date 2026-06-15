using Godot;
using System;

public partial class DamageReceiver : Area2D
{

  public enum HitType
  {
    NORMAL,KNOCKDOWN,POWER
  }

  [Signal]
  public delegate void DamageCompletedEventHandler(int damage,Vector2 direction,int hitType);
}
