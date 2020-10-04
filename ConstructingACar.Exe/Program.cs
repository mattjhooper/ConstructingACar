using System;
using ConstructingACar;

namespace ConstructingACar.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Constructing a Car");
            var car = new Car();

            car.EngineStart();

            car.Accelerate(10);

            car.BrakeBy(10);

            car.EngineStop();
            car.EngineStart();

            Console.WriteLine($"ActualConsumptionByDistance: {car.onBoardComputerDisplay.ActualConsumptionByDistance}");
        }
    }
}
