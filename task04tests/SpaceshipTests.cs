using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
        Assert.Equal(70, cruiser.StockOfRockets);
        Assert.Equal(0, cruiser.Angle);
        Assert.Equal(0, cruiser.coord_X);
        Assert.Equal(0, cruiser.coord_Y);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(30, fighter.FirePower);
        Assert.Equal(120, fighter.StockOfRockets);
        Assert.Equal(0, fighter.Angle);
        Assert.Equal(0, fighter.coord_X);
        Assert.Equal(0, fighter.coord_Y);
    }

    [Fact]
    public void CruiserRocketsMustBeMorePowerfulThanFighterRockets()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(cruiser.FirePower > fighter.FirePower);
    }

    [Fact]
    public void Rotate90AndMoveShouldChangeY()
    {
        var fighter = new Fighter();
        fighter.Rotate(90);
        fighter.MoveForward();
        Assert.Equal(90, fighter.Angle);
        Assert.Equal(100, fighter.coord_Y);
        Assert.Equal(0, fighter.coord_X);
    }

    [Fact]
    public void RotateMinus240ShouldNormalizeAndMove()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(-240);
        cruiser.MoveForward();
        Assert.Equal(120, cruiser.Angle);
        Assert.Equal(-25, cruiser.coord_X);
        Assert.Equal(43, cruiser.coord_Y);
    }

    [Fact]
    public void Rotate108AndMove_ShouldChangeBothCoordinates()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(108);
        cruiser.MoveForward();
        Assert.Equal(108, cruiser.Angle);
        Assert.Equal(-15, cruiser.coord_X);
        Assert.Equal(48, cruiser.coord_Y);
    }

    [Fact]
    public void RocketsShouldShrinkCorrectlyWhenFired()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        fighter.Fire();
        cruiser.Fire();
        Assert.Equal(69, cruiser.StockOfRockets);
        Assert.Equal(117, fighter.StockOfRockets);
    }
}
