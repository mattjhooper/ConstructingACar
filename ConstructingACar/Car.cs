﻿using System;
using System.Collections.Generic;
using System.Linq;
using static ConstructingACar.Global.Globals;

namespace ConstructingACar
{
    public class Car : ICar
    {
        public IDrivingInformationDisplay drivingInformationDisplay; // car #2

        public IFuelTankDisplay fuelTankDisplay;

        public IOnBoardComputerDisplay onBoardComputerDisplay; // car #3

        private IDrivingProcessor drivingProcessor; // car #2

        private readonly IEngine engine;

        private readonly IFuelTank fuelTank;

        private readonly int _maxAcceleration;

        private IOnBoardComputer onBoardComputer; // car #3

        public Car() : this(DEFAULT_FUEL_LEVEL, DEFAULT_ACCELERATION)
        {
        }

        public Car(double fuelLevel) : this(fuelLevel, DEFAULT_ACCELERATION)
        {
        }

        public Car(double fuelLevel, int maxAcceleration) // car #2
        {
            Console.WriteLine($"New Car. fuelLevel: {fuelLevel}, maxAcceleration: {maxAcceleration}");
            fuelTank = new FuelTank(fuelLevel);
            drivingProcessor = new DrivingProcessor();
            engine = new Engine(fuelTank, drivingProcessor);
            fuelTankDisplay = new FuelTankDisplay(fuelTank);
            drivingInformationDisplay = new DrivingInformationDisplay(drivingProcessor);
            _maxAcceleration = Math.Clamp(maxAcceleration, MIN_ACCELERATION, MAX_ACCELERATION);
            onBoardComputer = new OnBoardComputer(drivingProcessor, fuelTank);
            onBoardComputerDisplay = new OnBoardComputerDisplay(onBoardComputer);
        }

        public bool EngineIsRunning
        {
            get { return engine.IsRunning; }
        }

        private void ElapseSecond()
        {
            if (EngineIsRunning)
            {
                onBoardComputer.ElapseSecond();    
            }
        }

        public void BrakeBy(int speed) // car #2
        {
            Console.WriteLine($"BrakeBy {speed}");
             if (EngineIsRunning)
            {
                drivingProcessor.ReduceSpeed(drivingProcessor.ActualSpeed - Math.Min(speed, MAX_BRAKE));
                ElapseSecond();
            }
        }

        public void Accelerate(int speed) // car #2
        {
            Console.WriteLine($"Accelerate {speed}");
            if (engine.IsRunning)
            {
                if (speed < drivingProcessor.ActualSpeed)
                {
                    FreeWheel();
                }
                else
                {
                    var newSpeed = drivingProcessor.ActualSpeed + _maxAcceleration > speed ? Math.Max(speed, drivingProcessor.ActualSpeed) : drivingProcessor.ActualSpeed + _maxAcceleration;
                    drivingProcessor.IncreaseSpeedTo(newSpeed);

                    engine.Consume(drivingProcessor.ActualConsumption);
                }
                ElapseSecond();
            }
        }

        public void EngineStart()
        {
            Console.WriteLine($"Car EngineStart called");
            //onBoardComputer.TripReset();
            onBoardComputer.ElapseSecond();   
            engine.Start();
        }

        public void EngineStop()
        {
            Console.WriteLine($"Car EngineStop called");
            engine.Stop();
            onBoardComputer.ElapseSecond();   
            onBoardComputer.TripReset();
        }

        public void FreeWheel() // car #2
        {
            Console.WriteLine($"FreeWheel");
            drivingProcessor.ReduceSpeed(drivingProcessor.ActualSpeed - FREEWHEEL_DECELERATION);

            if (drivingProcessor.ActualSpeed == 0)
            {
                RunningIdle();
            }
            else
            {
                ElapseSecond();
            }
        }

        public void Refuel(double liters)
        {
            Console.WriteLine($"Car Refuel called with {liters} liters");
            fuelTank.Refuel(liters);
            //onBoardComputer.ElapseSecond();
        }

        public void RunningIdle()
        {
            // Console.WriteLine($"Car is RunningIdle");
            engine.Consume(drivingProcessor.ActualConsumption);
            ElapseSecond();
        }
    }

    public class DrivingInformationDisplay : IDrivingInformationDisplay // car #2
    {
        private readonly IDrivingProcessor _drivingProcessor;

        public DrivingInformationDisplay(IDrivingProcessor drivingProcessor)
        {
            _drivingProcessor = drivingProcessor;
        }

        public int ActualSpeed { get { return _drivingProcessor.ActualSpeed; } }
    }

    public class DrivingProcessor : IDrivingProcessor // car #2
    {
        private bool _isEngineRunning;
        private bool _accelerating;
        public int ActualSpeed { get; private set; } = 0;

        public double ActualConsumption
        {
            get
            {
                if (_isEngineRunning)
                {
                    if (ActualSpeed == 0)
                    {
                        return IDLE_FUEL_CONSUMPTION;
                    }
                    if (_accelerating)
                    {
                        return GetConsumption(ActualSpeed);
                    }
                }
                return 0;
            }
        }

        public void EngineStart()
        {
            _isEngineRunning = true;
        }

        public void EngineStop()
        {
            _isEngineRunning = false;
        }

        public void IncreaseSpeedTo(int speed)
        {
            //Console.WriteLine($"IncreaseSpeedTo {speed}");
            ActualSpeed = Math.Min(speed, MAX_SPEED);
            _accelerating = true;
        }

        public void ReduceSpeed(int speed)
        {
            //Console.WriteLine($"ReduceSpeed {speed}");
            ActualSpeed = Math.Max(speed, MIN_SPEED);
            _accelerating = false;
        }
    }

    public class Engine : IEngine
    {
        private bool _isRunning;
        private IFuelTank _fuelTank;
        private IDrivingProcessor _drivingProcessor;

        public Engine(IFuelTank fuelTank, IDrivingProcessor drivingProcessor)
        {
            _isRunning = false;
            _fuelTank = fuelTank;
            _drivingProcessor = drivingProcessor;
        }

        public bool IsRunning { get { return _isRunning; } }

        public void Consume(double liters)
        {
            //Console.WriteLine($"Engine Consume called with {liters} liters");
            if (_isRunning)
            {
                _fuelTank.Consume(liters);
                if (_fuelTank.FillLevel == 0)
                {
                    Stop();
                }
            }
        }

        public void Start()
        {
            //Console.WriteLine($"Engine Start called");
            if (_fuelTank.FillLevel > 0)
            {
                _isRunning = true;
                _drivingProcessor.EngineStart();
            }
        }

        public void Stop()
        {
            //Console.WriteLine($"Engine Stop called");
            _isRunning = false;
            _drivingProcessor.EngineStop();
        }
    }

    public class FuelTank : IFuelTank
    {
        private double _fillLevel;

        public FuelTank(double fuelLevel)
        {
            //Console.WriteLine($"New FuelTank with fuel level: {fuelLevel}");
            _fillLevel = Math.Min(Math.Max(0, fuelLevel), MAXIMUM_FUEL_LEVEL);
        }

        public double FillLevel { get { return _fillLevel; } }

        public bool IsOnReserve { get { return _fillLevel < RESERVE_FUEL_LEVEL; } }

        public bool IsComplete { get { return _fillLevel == MAXIMUM_FUEL_LEVEL; } }

        public void Consume(double liters)
        {
            //Console.WriteLine($"FuelTank Consume called with {liters} liters");
            _fillLevel = Math.Max(0, _fillLevel - liters);
        }

        public void Refuel(double liters)
        {
            //Console.WriteLine($"FuelTank Refuel called with {liters} liters");
            _fillLevel = Math.Min(_fillLevel + liters, MAXIMUM_FUEL_LEVEL);
        }
    }

    public class FuelTankDisplay : IFuelTankDisplay
    {
        private IFuelTank _fuelTank;
        public FuelTankDisplay(IFuelTank fuelTank)
        {
            _fuelTank = fuelTank;
        }
        public double FillLevel { get { return Math.Round(_fuelTank.FillLevel, 2); } }

        public bool IsOnReserve { get { return _fuelTank.IsOnReserve; } }

        public bool IsComplete { get { return _fuelTank.IsComplete; } }
    }

    public class OnBoardComputer : IOnBoardComputer // car #3
    {
        private readonly IDrivingProcessor _drivingProcessor;
        private readonly IFuelTank _fuelTank;

        private int _tripSpeed = 0;
        private int _totalSpeed = 0;

        private double _tripConsumptionByTimeSum = 0;
        private double _totalConsumptionByTimeSum = 0;

        private double _tripConsumptionByDistanceSum = 0;
        private double _totalConsumptionByDistanceSum = 0;

        private readonly Queue<double> _consumptionForLast100Seconds;

        public OnBoardComputer(IDrivingProcessor drivingProcessor, IFuelTank fuelTank)
        {
            _drivingProcessor = drivingProcessor;
            _fuelTank = fuelTank;
            _consumptionForLast100Seconds = SetupDefaultConsumption();
        }

        public int TripRealTime { get; private set; } = 0;

        public int TripDrivingTime { get; private set; } = 0;

        public int TripDrivenDistance { get; private set; } = 0;

        public int TotalRealTime { get; private set; } = 0;

        public int TotalDrivingTime { get; private set; } = 0;

        public int TotalDrivenDistance { get; private set; } = 0;

        public double TripAverageSpeed { get { return CalculateAverageSpeed(TripDrivingTime, TripDrivenDistance); } }

        public double TotalAverageSpeed { get { return CalculateAverageSpeed(TotalDrivingTime, TotalDrivenDistance); } }

        public int ActualSpeed { get { return _drivingProcessor.ActualSpeed; } }

        public double ActualConsumptionByTime { get; private set; } = 0;

        public double ActualConsumptionByDistance { get; private set; } = double.NaN;

        public double TripAverageConsumptionByTime { get; private set; }

        public double TotalAverageConsumptionByTime { get; private set; }

        public double TripAverageConsumptionByDistance { get { return TripDrivingTime == 0 ? 0 : Math.Round(_tripConsumptionByDistanceSum / TripDrivingTime, 1); } }

        public double TotalAverageConsumptionByDistance { get { return TotalDrivingTime == 0 ? 0 : Math.Round(_totalConsumptionByDistanceSum / TotalDrivingTime, 1); } }

        public int EstimatedRange { get; private set; } = 0;

        private Queue<double> SetupDefaultConsumption()
        {
            var defaultQueue = new Queue<double>();

            for (int i = 0; i < 100; i++)
            {
                defaultQueue.Enqueue(DEFAULT_CONSUMPTION);
            }

            return defaultQueue;
        }

        private double CalculateAverageConsumptionByTime(int realTime, double consumptionByTimeSum)
        {
            return realTime == 0 ? 0 : consumptionByTimeSum / realTime;
        }

        private double CalculateAverageSpeed(int DrivingTime, int DrivenDistance)
        {
            return DrivingTime == 0 ? 0d : ((double)DrivenDistance / 100000) / ((double)DrivingTime / 3600);
        }

        private int CalculateEstimatedRange()
        {
            int EstimatedRange = 0;
            // remove the oldest consumption value and add the current one
            _consumptionForLast100Seconds.Dequeue();
            _consumptionForLast100Seconds.Enqueue(_drivingProcessor.ActualConsumption);
            var totalConsumption = _consumptionForLast100Seconds.Sum(x => x);

            double secondsOfFuelRemaining = 100 * _fuelTank.FillLevel / totalConsumption;

            EstimatedRange = (int)Math.Round((secondsOfFuelRemaining / 3600) * _drivingProcessor.ActualSpeed);

            // Console.WriteLine($"totalConsumption: {totalConsumption}, EstimatedRange: {EstimatedRange}");

            return EstimatedRange;
        }

        //private double _tripDistanceKM = 0;

        public void ElapseSecond()
        {
            TripRealTime++;
            TotalRealTime++;
            ActualConsumptionByTime = _drivingProcessor.ActualConsumption;
            int secondsRequiredToDrive100Km = 0;
            if (_drivingProcessor.ActualSpeed > 0)
            {
                TripDrivingTime++;
                TotalDrivingTime++;

                _tripSpeed += _drivingProcessor.ActualSpeed;
                _totalSpeed += _drivingProcessor.ActualSpeed;
                secondsRequiredToDrive100Km = 3600 * 100 / _drivingProcessor.ActualSpeed;
                int distanceInElapsedSecondInCentMetres = 100000 * _drivingProcessor.ActualSpeed / 3600;
                TripDrivenDistance += (distanceInElapsedSecondInCentMetres);
                TotalDrivenDistance += (distanceInElapsedSecondInCentMetres);

                double distanceInElapsedSecond = ((double)_drivingProcessor.ActualSpeed) / 3600;
                //_tripDistanceKM += distanceInElapsedSecond;

            }
            ActualConsumptionByDistance = TripDrivingTime == 0 ? double.NaN : Math.Round(secondsRequiredToDrive100Km * ActualConsumptionByTime, 1);
            _tripConsumptionByTimeSum += ActualConsumptionByTime;
            _totalConsumptionByTimeSum += ActualConsumptionByTime;
            if (!double.IsNaN(ActualConsumptionByDistance))
            {
                _tripConsumptionByDistanceSum += ActualConsumptionByDistance;
                _totalConsumptionByDistanceSum += ActualConsumptionByDistance;
            }

            EstimatedRange = CalculateEstimatedRange();
            TripAverageConsumptionByTime = CalculateAverageConsumptionByTime(TripRealTime, _tripConsumptionByTimeSum);
            TotalAverageConsumptionByTime = CalculateAverageConsumptionByTime(TotalRealTime, _totalConsumptionByTimeSum);

        }

        public void TripReset()
        {
            TripRealTime = 0;
            TripDrivingTime = 0;
            TripDrivenDistance = 0;
            _tripSpeed = 0;
            _tripConsumptionByTimeSum = 0;
            _tripConsumptionByDistanceSum = 0;
            ActualConsumptionByDistance = double.NaN;
            TripAverageConsumptionByTime = 0;
        }

        public void TotalReset()
        {
            //TripReset();
            TotalRealTime = 0;
            TotalDrivingTime = 0;
            TotalDrivenDistance = 0;
            _totalSpeed = 0;
            _totalConsumptionByTimeSum = 0;
            _totalConsumptionByDistanceSum = 0;
            TotalAverageConsumptionByTime = 0;
        }
    }

    public class OnBoardComputerDisplay : IOnBoardComputerDisplay // car #3
    {
        private readonly IOnBoardComputer _onBoardComputer;
        public OnBoardComputerDisplay(IOnBoardComputer onBoardComputer)
        {
            _onBoardComputer = onBoardComputer;
        }
         public int TripRealTime { get { Console.WriteLine("TripRealTime"); return _onBoardComputer.TripRealTime; } }

        public int TripDrivingTime { get { Console.WriteLine("TripDrivingTime"); return _onBoardComputer.TripDrivingTime; } }

        public double TripDrivenDistance { get { return Math.Round(((double)_onBoardComputer.TripDrivenDistance / 100000), 2); } }

        public int TotalRealTime { get { Console.WriteLine("TotalRealTime"); return _onBoardComputer.TotalRealTime; } }

        public int TotalDrivingTime {  get { Console.WriteLine("TotalRealTime"); return _onBoardComputer.TotalDrivingTime; } }

        public double TotalDrivenDistance { get { return Math.Round(((double)_onBoardComputer.TotalDrivenDistance / 100000), 2); } }

        public int ActualSpeed { get { return _onBoardComputer.ActualSpeed; } }

        public double TripAverageSpeed { get { return Math.Round(_onBoardComputer.TripAverageSpeed, 1); } }

        public double TotalAverageSpeed { get { return Math.Round(_onBoardComputer.TotalAverageSpeed, 1); } }

        public double ActualConsumptionByTime { get { return _onBoardComputer.ActualConsumptionByTime; } }

        public double ActualConsumptionByDistance { get { return _onBoardComputer.ActualConsumptionByDistance; } }

        public double TripAverageConsumptionByTime { get { Console.WriteLine("TripAverageConsumptionByTime"); return Math.Round(_onBoardComputer.TripAverageConsumptionByTime, 5); } }        

        public double TotalAverageConsumptionByTime { get { Console.WriteLine("TotalAverageConsumptionByTime"); return Math.Round(_onBoardComputer.TotalAverageConsumptionByTime, 5); } }

        public double TripAverageConsumptionByDistance { get { return Math.Round(_onBoardComputer.TripAverageConsumptionByDistance, 1); } }

        public double TotalAverageConsumptionByDistance { get { return Math.Round(_onBoardComputer.TotalAverageConsumptionByDistance, 1); } }

        public int EstimatedRange { get { return _onBoardComputer.EstimatedRange; } }

        public void TripReset()
        {
            Console.WriteLine("TripReset");
            _onBoardComputer.TripReset();            
        }

        public void TotalReset()
        {
            Console.WriteLine("TotalReset"); 
            _onBoardComputer.TotalReset();            
        }
    }
}
