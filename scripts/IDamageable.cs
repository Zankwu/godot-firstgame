using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;


public interface IDamageable
{
    void TakeDamage(int damage, Vector2 position);
}
