namespace task04;
public class Fighter : ISpaceship
{
    public int Speed => 100;
    public int FirePower => 25;

    public int Direction { get; private set; }
    public int Position { get; private set; }

    public void MoveForward() => Position += Speed;

    public void Rotate(int angle) => Direction = ((Direction + angle) % 360 + 360) % 360;

    public void Fire() { /* выстрел фотонной ракетой мощностью FirePower */ }
}