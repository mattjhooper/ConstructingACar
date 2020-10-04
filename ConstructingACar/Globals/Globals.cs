using System;
namespace ConstructingACar.Global   
{ 
    public static class Globals
    {
        public const double DEFAULT_FUEL_LEVEL = 20;
        public const double IDLE_FUEL_CONSUMPTION = 0.0003;
        public const double MAXIMUM_FUEL_LEVEL = 60;
        public const double RESERVE_FUEL_LEVEL = 5;
        public const int DEFAULT_ACCELERATION = 10;
        public const int MIN_ACCELERATION = 5;
        public const int MAX_ACCELERATION = 20;
        public const int MIN_SPEED = 0;
        public const int MAX_SPEED = 250;
        public const int FREEWHEEL_DECELERATION = 1;
        public const int MAX_BRAKE = 10;

        public const double DEFAULT_CONSUMPTION = 4.8 / 100;

        public static double GetConsumption(int speed)
        {
            double consumption = 0;
            switch (speed)
            {
                case var expression when (1 <= speed && speed <= 60):
                    consumption = 0.0020;
                    break;
                case var expression when (61 <= speed && speed <= 100):
                    consumption = 0.0014;
                    break;
                case var expression when (101 <= speed && speed <= 140):
                    consumption = 0.0020;
                    break;
                case var expression when (141 <= speed && speed <= 200):
                    consumption = 0.0025;
                    break;
                case var expression when (201 <= speed && speed <= 250):
                    consumption = 0.0030;
                    break;
                default:
                    consumption = 0;
                    break;
            }

            //Console.WriteLine($"GetConsumption. speed: {speed}, consumption: {consumption}");
            return consumption;
        }
    }
}