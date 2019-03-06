﻿using IonServer.Content;
using System;
using System.Diagnostics;
using System.Threading;

namespace IonServer.Engine.Core.GameLogic
{
    public static class GameStarter
    {
        private static bool isRunning = false;

        public static void Start()
        {
            Console.WriteLine("Starting Game Logic");
            isRunning = true;

            //Run the start function
            Game.Start();

            //Start the timekeeper
            Game.Time = new Stopwatch();
            Game.Time.Start();

            StartUpdateLoop();
        }

        public static void Stop()
        {
            Console.WriteLine("Stopping Game Thread");
            StopUpdateLoop();

            Program._gameLogicThread.Join(); //Wait for last update to finish running and for the _gameLogicThread to terminate.

            Console.WriteLine("Game Thread Stopped");
            Console.WriteLine("Finishing Game Logic");
            Game.Stop();
        }

        //Starts the update loop for the game. This is where the _gameLogicThread will be occupied.
        private static void StartUpdateLoop()
        {
            long timeAtStart;
            long timeAtEnd;
            long timeToWait;

            long timeBetweenUpdates = 1000L/Game.UpdatesPerSecond;

            Stopwatch timer = new Stopwatch();
            while (isRunning)
            {
                timeAtStart = Game.Time.ElapsedMilliseconds; //Records time at start of Update
                
                //Latency from code below is accounted for,
                Game.Update();
                timeBetweenUpdates = 1000L / Game.UpdatesPerSecond; //Redefine timeBetweenUpdates every update so that it can be changed.
                //Stop no latency zone

                timeAtEnd = Game.Time.ElapsedMilliseconds; //Records time at end of Update

                timeToWait = timeBetweenUpdates - (timeAtEnd - timeAtStart); //Subtracts the time it took to run Game.Update() from the normal interval between updates.
                
                Thread.Sleep((int)timeToWait);
            }
        }

        //Stops the game loop.
        private static void StopUpdateLoop()
        {
            Console.WriteLine("Stopping Update Loop");
            isRunning = false;
        }
    }
}