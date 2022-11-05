#pragma warning disable CA1050
using Godot;
using System;

public partial class Mob : RigidBody2D {
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var animSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animSprite2D.Playing = true;

        string[] mobTypes = animSprite2D.Frames.GetAnimationNames();
        animSprite2D.Animation = mobTypes[GD.Randi() % mobTypes.Length];
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public void OnVisibleOnScreenNotifier2DScreenExited() {
        QueueFree();
    }
}
