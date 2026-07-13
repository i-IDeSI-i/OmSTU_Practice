using Xunit;
using task04;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(25, fighter.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Cruiser_ShouldHaveMorePowerfulWeapons()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(cruiser.FirePower > fighter.FirePower);
    }

    [Fact]
    public void MoveForward_ShouldIncreasePositionBySpeed()
    {
        var fighter = new Fighter();
        fighter.MoveForward();
        Assert.Equal(100, fighter.Position);

        fighter.MoveForward();
        Assert.Equal(200, fighter.Position);
    }

    [Fact]
    public void Rotate_ShouldChangeDirection()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(90);
        Assert.Equal(90, cruiser.Direction);

        cruiser.Rotate(300);
        Assert.Equal(30, cruiser.Direction); // 390 % 360 = 30
    }

    [Fact]
    public void Rotate_WithNegativeAngle_NormalizesDirection()
    {
        var fighter = new Fighter();
        fighter.Rotate(-90);
        Assert.Equal(270, fighter.Direction);
    }
}