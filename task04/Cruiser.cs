namespace task04;
public class Cruiser : ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;
    public int Direction { get; private set; }
    public int Position { get; private set; }

    public void MoveForward() => Position += Speed;

    public void Rotate(int angle) => Direction = ((Direction + angle) % 360 + 360) % 360;

    public void Fire() { /* выстрел фотонной ракетой мощностью FirePower */ }
}