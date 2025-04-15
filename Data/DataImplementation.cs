﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {
      MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
    }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
    {
      //Scale = scale;
      //TableHeight = tableHeight * scale;
      //TableWidth = tableWidth * scale;
      //BallDiameter = diameter * scale;

      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
      Random random = new Random();
      for (int i = 0; i < numberOfBalls; i++)
      {
        //Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
        Vector startingPosition = new(random.Next(0, 390), random.Next(0, 390));
        //Ball newBall = new(startingPosition, startingPosition);
        //upperLayerHandler(startingPosition, newBall);
        //BallsList.Add(newBall);
        //Vector startingPosition = new(random.Next(0, (int)tableWidth), random.Next(0, (int)tableHeight));
        Ball newBall = new(startingPosition, startingPosition);
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          MoveTimer.Dispose();
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    //private bool disposedValue;
    private bool Disposed = false;
    //private double TableWidth;
    //private double TableHeight;
    //private double BallDiameter;
    //private double Scale;

    private readonly Timer MoveTimer;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];

    //private internal void Move(object? x)
    internal void Move(object? x)
    {
        //double radius = BallDiameter;
        double radius = 30.0;
        double TableWidth = 400.0;
        double TableHeight = 400.0;

      foreach (Ball item in BallsList)
      {
        //item.Move(
        //    new Vector(
        //        (RandomGenerator.NextDouble() - 0.5) * 10, 
        //        (RandomGenerator.NextDouble() - 0.5) * 10)
        //    );

        IVector pos = item.Position;
        IVector vel = new Vector(
            (RandomGenerator.NextDouble() - 0.5) * 10,
            (RandomGenerator.NextDouble() - 0.5) * 10);

        double newX = pos.x + vel.x;
        double newY = pos.y + vel.y;

        if (newX < radius || newX > TableWidth - radius)
        {
          vel = new Vector(-vel.x, vel.y);
          newX = Clamp(newX, radius, TableWidth - radius);
        }

        if (newY < radius || newY > TableHeight - radius)
        {
          vel = new Vector(vel.x, -vel.y);
          newY = Clamp(newY, radius, TableHeight - radius);
        }

        item.Velocity = vel;
        Vector delta = new Vector(newX - pos.x, newY - pos.y);
        item.Move(delta);


      }
    }

    private double Clamp(double value, double min, double max)
    {
      if (value < min)
        return min;
      if (value > max)
        return max;
      return value;
    }

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}