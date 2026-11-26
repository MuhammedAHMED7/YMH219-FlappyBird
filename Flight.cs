namespace FlappyBird;

public enum Phase { Ready, Flying, Paused, Finished }

public sealed class Gate(float x, float center)
{
    public float X { get; internal set; } = x;
    public float Center { get; internal set; } = center;
    public bool Passed { get; internal set; }
}

public sealed class Flight
{
    public const float BirdX = 208, Radius = 14, GateWidth = 67, HalfGap = 91, Ground = 474;
    private readonly Random random = new();
    public List<Gate> Gates { get; } = [];
    public float Height { get; private set; }
    public float Velocity { get; private set; }
    public float Speed => 182 + Math.Min(Score, 15) * 3;
    public int Score { get; private set; }
    public Phase State { get; private set; }

    public Flight() => Reset();
    public void Reset()
    {
        Height = 244;
        Velocity = 0;
        Score = 0;
        State = Phase.Ready;
        Gates.Clear();
        Gates.Add(new(476, 230));
        Gates.Add(new(815, 260));
        Gates.Add(new(1154, 180));
    }

    public void Pause()
    {
        if (State == Phase.Flying) State = Phase.Paused;
    }

    public void Resume()
    {
        if (State == Phase.Paused) State = Phase.Flying;
    }

    public void Flap()
    {
        if (State is Phase.Paused or Phase.Finished) return;
        State = Phase.Flying;
        Velocity = -258;
    }

    public void Advance(float seconds)
    {
        if (State != Phase.Flying) return;
        Velocity += 690 * seconds;
        Height += Velocity * seconds;
        foreach (var g in Gates) g.X -= Speed * seconds;
        if (Height - Radius <= 0 || Height + Radius >= Ground ||
            Gates.Any(g => BirdX + Radius > g.X && BirdX - Radius < g.X + GateWidth &&
                (Height - Radius < g.Center - HalfGap || Height + Radius > g.Center + HalfGap)))
        {
            State = Phase.Finished;
            return;
        }
        foreach (var g in Gates)
        {
            if (!g.Passed && g.X + GateWidth < BirdX - Radius) { Score++; g.Passed = true; }
            if (g.X + GateWidth < 0)
            {
                g.X = Gates.Max(other => other.X) + 339;
                g.Center = random.Next(155, 322);
                g.Passed = false;
            }
        }
    }
}