using Godot;
using System;

public partial class Igroboss : Character
{
    [Export]
    public int player_target_distance;

    protected CharacterBody2D player;

    public ulong time_last_attack_boss;

    [Export]
    public ulong time_bettwen_attacks;

    [Export]

    public ulong time_duration_recover;

    public ulong time_start_recover;
    public Vector2 Knockback_force { get; private set; }


    public override void _Ready()
    {
        base._Ready();
        player = GetNode<CharacterBody2D>("../Player");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        damageEmitter.Monitoring = isAttacking();
        Knockback_force = Knockback_force.MoveToward(Vector2.Zero, (float)delta * 50);
    }

    public override bool isAttacking()
    {
        return currentState == State.fly;
    }

    public override bool CanGetHurt()
    {
        return true;
    }
    public override bool CanPunch()
    {
        var target = GetPlayerTarget();
        if (Time.GetTicksMsec() - time_last_attack_boss < time_duration_last_attack)
        {
            return false;
        }
        return base.CanPunch();
    }
    public override void HandleInput()
    {
        if (player != null && CanMove())
        {
            if (CanPunch() && rayCast.IsColliding())
            {
                currentState = State.fly;
                Velocity = heading * flyPwoer;
            }
            else if (IsPlayertWinthinRange())
            {
                Velocity = Vector2.Zero;
                currentState = State.idle;
            }
            else
            {
                var target = GetPlayerTarget();
                var movement = (target - Position).Normalized();

                Velocity = (movement + Knockback_force) * speed;
                currentState = State.walk;
            }
        }


    }
    public override void HandleGrounded()
    {
        if (currentState == State.grounded && currentHealth > 0)
        {
            currentState = State.Recover;
            time_start_recover = Time.GetTicksMsec();
        }
        if (currentState == State.Recover && Time.GetTicksMsec() - time_start_recover > time_duration_recover)
        {
            currentState = State.idle;
            time_last_attack_boss = Time.GetTicksMsec();

        }

    }

    public override void completedIdleAction()
    {
        if (currentState == State.hurt)
        {
            currentState = State.Recover;
            return;
        }
        base.completedIdleAction();
    }

    public override void OnEmitCompleted(Node2D temp)
    {
        temp.EmitSignal(DamageReceiver.SignalName.DamageCompleted,
        damage,
        heading,
        (int)DamageReceiver.HitType.KNOCKDOWN);
        time_last_attack_boss = Time.GetTicksMsec();
        currentState = State.idle;
    }

    public override void OnReceiverCompleted(int damageTemp, Vector2 direction, int hitTypeInt)
    {
        if (!IsVulnerable())
        {
            Knockback_force = direction * Knockback;
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damageTemp, 0, max_health);
        if (currentHealth <= 0)
        {
            currentState = State.deadth;
        }
        else
        {
            currentState = State.hurt;
            Velocity = Vector2.Zero;
        }
    }

    // public override void OnWallHit(Node2D wall)
    // {
    //     if (wall is AnimatableBody2D)
    //     {
    //         currentState = State.Recover;
    //         heightSpeed = knockDown;
    //         Velocity = -Velocity / 2;
    //     }
    // }




    public bool IsVulnerable()
    {
        return currentState == State.Recover;
    }

    public bool IsPlayertWinthinRange()
    {
        var target = GetPlayerTarget();
        return (Position - target).Length() < 1;
    }



    public Vector2 GetPlayerTarget()
    {

        var target = player.Position;
        if (Position.X > player.Position.X)
        {
            target.X += player_target_distance;
        }
        else
        {
            target.X -= player_target_distance;
        }
        return target;
    }

    public override void setHeading()
    {
        if (player == null || !CanMove())
        {
            return;
        }
        if (Position.X > player.Position.X)
        {
            heading = Vector2.Left;
        }
        else
        {
            heading = Vector2.Right;
        }
    }

}
