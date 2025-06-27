namespace task04;

public interface ISpaceship
{
    void MoveForward();      // Движение вперед
    void Rotate(int angle);  // Поворот на угол (градусы)
    void Fire();             // Выстрел ракетой
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
    int Angle { get; } // Угол поворота
    int coord_X { get; } // Координата X местонахождения корабля
    int coord_Y { get; } // Координата Y местонахождения корабля
    int StockOfRockets { get; } // Запас ракет
}

public class Cruiser : ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;
    public int StockOfRockets { get; private set; } = 70;
    public int Angle { get; private set; } = 0;  
    public int coord_X { get; private set; } = 0;  
    public int coord_Y { get; private set; } = 0;
    public void MoveForward()
    {
        switch (Angle)
        {
            case 0:   coord_X += Speed; break;   
            case 90:  coord_Y += Speed; break;   
            case 180: coord_X -= Speed; break;   
            case 270: coord_Y -= Speed; break;  
            default:  
                double radians = Angle * Math.PI / 180;
                coord_X += (int)Math.Round(Math.Cos(radians) * Speed);
                coord_Y += (int)Math.Round(Math.Sin(radians) * Speed);
                break;
        }
    }
    public void Rotate(int angle)
    {
        Angle = (Angle + angle) % 360;
        if (Angle < 0)
        {
            Angle += 360;
        }
    }
    public void Fire()
    {
        if (StockOfRockets <= 0)
        {
            return;
        }
        StockOfRockets--;
    }
}

public class Fighter : ISpaceship
{
    public int Speed => 100;
    public int FirePower => 30;
    public int StockOfRockets { get; private set; } = 120;
    public int Angle { get; private set; } = 0;
    public int coord_X { get; private set; } = 0;
    public int coord_Y { get; private set; } = 0;
    public void MoveForward()
    {
        switch (Angle)
        {
            case 0: coord_X += Speed; break;
            case 90: coord_Y += Speed; break;
            case 180: coord_X -= Speed; break;
            case 270: coord_Y -= Speed; break;
            default:
                double radians = Angle * Math.PI / 180;
                coord_X += (int)Math.Round(Math.Cos(radians) * Speed);
                coord_Y += (int)Math.Round(Math.Sin(radians) * Speed);
                break;
        }
    }
    public void Rotate(int angle)
    {
        Angle = (Angle + angle) % 360;
        if (Angle < 0)
        {
            Angle += 360;
        }
    }
    public void Fire()
    {
        if (StockOfRockets < 3)
        {
            return;
        }
        StockOfRockets -= 3;
    }
}
