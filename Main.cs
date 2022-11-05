#pragma warning disable CA1050
using Godot;
using System;

public partial class Main : Node {

    [Export]
    public PackedScene MobScene;
    public int Score;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Randomize();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public void GameOver() {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GetNode<Hud>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    public void NewGame() {
        Score = 0;

        GetNode<AudioStreamPlayer>("Music").Play();

        GetTree().CallGroup("mobs", "queue_free");
        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<Hud>("HUD");
        hud.UpdateScore(Score);
        hud.ShowMessage("Get Ready!");
    }

    public void OnScoreTimerTimeout() {
        Score++;
        GetNode<Hud>("HUD").UpdateScore(Score);
    }

    public void OnStartTimerTimeout() {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    public void OnMobTimerTimeout() {
        var mob = MobScene.Instantiate<Mob>();

        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randi();

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity.
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }
}
