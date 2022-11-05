#pragma warning disable CA1050
using Godot;
using System;

public partial class Player : Area2D {

    [Export]
    public int Speed = 400; // How fast the player will move (pixel/sec)

    [Signal]
    public delegate void HitEventHandler();

    public Vector2 ScreenSize;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        ScreenSize = GetViewportRect().Size;
        Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        var velocity = Move();

        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0) {
            velocity = velocity.Normalized() * Speed;
            animatedSprite2D.Play();
        } else {
            animatedSprite2D.Stop();
        }

        // TODO: Looks like a bug to me
        Position += velocity * Convert.ToSingle(delta);
        Position = new Vector2(
            x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
            y: Mathf.Clamp(Position.y, 0, ScreenSize.y));

        playAnimations(velocity, animatedSprite2D);
    }

    public void OnPlayerBodyEntered(PhysicsBody2D body) {
        Hide();
        EmitSignal(nameof(Hit));
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }

    public void Start(Vector2 position) {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    private Vector2 Move() {
        var velocity = Vector2.Zero;

        if (Input.IsActionPressed("move_right")) {
            velocity.x += 1;
        }
        if (Input.IsActionPressed("move_left")) {
            velocity.x -= 1;
        }
        if (Input.IsActionPressed("move_down")) {
            velocity.y += 1;
        }
        if (Input.IsActionPressed("move_up")) {
            velocity.y -= 1;
        }
        return velocity;
    }

    private void playAnimations(Vector2 velocity, AnimatedSprite2D animatedSprite2D) {
        if (velocity.x != 0) {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipV = false;
            animatedSprite2D.FlipH = velocity.x < 0;
        } else if (velocity.y != 0) {
            animatedSprite2D.Animation = "up";
            animatedSprite2D.FlipV = velocity.y > 0;
        }
    }
}