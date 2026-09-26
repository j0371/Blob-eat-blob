using Godot;

namespace BlobEatBlob.Scripts;

public partial class NonPlayer : Blob
{
    // Fraction of Blob.Speed a roaming blob moves at, picked per blob so they don't all move in lockstep.
    [Export]
    private float _minSpeedFactor = 0.3f;

    [Export]
    private float _maxSpeedFactor = 0.6f;

    // Max turn rate in radians/second. The actual rate drifts randomly within ±this value.
    [Export]
    private float _maxTurnRate = 1.5f;

    // How quickly the turn rate changes direction. Higher = more wiggly paths.
    [Export]
    private float _turnJitter = 3f;

    // How quickly velocity eases toward the current heading.
    [Export]
    private float _steeringSmoothing = 4f;

    private readonly RandomNumberGenerator _random = new();

    private float _heading;

    private float _turnRate;

    private float _roamSpeed;

    public override void _Ready()
    {
        base._Ready();

        _random.Randomize();
        Grow(_random.RandiRange(0, 1));

        _heading = _random.RandfRange(0f, Mathf.Tau);
        _roamSpeed = Speed * _random.RandfRange(_minSpeedFactor, _maxSpeedFactor);
        Velocity = Vector2.FromAngle(_heading) * _roamSpeed;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Move((float)delta);
    }

    private Vector2 Move(float delta)
    {
        _turnRate = Mathf.Clamp(
            _turnRate + _random.RandfRange(-_turnJitter, _turnJitter) * delta,
            -_maxTurnRate,
            _maxTurnRate);
        _heading += _turnRate * delta;

        Vector2 targetVelocity = Vector2.FromAngle(_heading) * _roamSpeed;
        Velocity = Velocity.Lerp(targetVelocity, 1f - Mathf.Exp(-_steeringSmoothing * delta));

        // After bumping into something, turn to follow the new direction instead of pushing into it.
        if (MoveAndSlide())
        {
            _heading = Velocity.Angle();
        }

        return Velocity;
    }
}
