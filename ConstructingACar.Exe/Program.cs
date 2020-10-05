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

            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.onBoardComputerDisplay.TripReset();
            Console.WriteLine($"TripAverageConsumptionByTime: {car.onBoardComputerDisplay.TripAverageConsumptionByTime}");

            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);                        
            car.onBoardComputerDisplay.TripReset();
            Console.WriteLine($"TripAverageConsumptionByTime: {car.onBoardComputerDisplay.TripAverageConsumptionByTime}");            
        }
    }
}
