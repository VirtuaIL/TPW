//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using System;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using TP.ConcurrentProgramming.Data;


namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
  {
    #region ctor

    public BusinessLogicImplementation() : this(null)
    {
       
    }

    internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
    {
      layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
    }

    #endregion ctor

    //public override Dimensions GetDimensions => BusinessLogicAbstractAPI.GetDimensions;

    #region BusinessLogicAbstractAPI

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      layerBellow.Dispose();
      MoveTimer?.Dispose();
            BallsList.Clear();
      Disposed = true;
    }

    public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      BallsList.Clear();

      layerBellow.Start(numberOfBalls, GetDimensions.TableWidth, GetDimensions.TableHeight ,(startingPosition, databall) =>
      {
        Ball logicBall = new Ball(databall);
        BallsList.Add(logicBall);
          upperLayerHandler(
            new Position(startingPosition.x, startingPosition.y), logicBall);
      });
      MoveTimer?.Dispose();
      MoveTimer = new Timer(_ => MoveBalls(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));

        }

    private void MoveBalls()
    {
        double radius = GetDimensions.BallDimension;

        foreach (Ball ball in BallsList)
        {

            IVector pos = ball.Position;
            //IVector vel = ball.Velocity;

            Vector vel = new(
                (RandomGenerator.NextDouble() - 0.5) * 10 * Scale / 10.0,
                (RandomGenerator.NextDouble() - 0.5) * 10 * Scale / 10.0
            );

            //Debug.WriteLine($"pos: {pos}");


            double newX = pos.x + vel.x;
            double newY = pos.y + vel.y;

            if (newX < radius || newX > GetDimensions.TableWidth - radius)
            {
                vel = new Vector(-vel.x, vel.y);
                newX = Clamp(newX, radius, GetDimensions.TableWidth - radius);
            }

            if (newY < radius || newY > GetDimensions.TableHeight - radius)
            {
                vel = new Vector(vel.x, -vel.y);
                newY = Clamp(newY, radius, GetDimensions.TableHeight - radius);
            }

            ball.SetVelocity(vel);
            ball.SetPosition(new Vector(newX, newY));

            }

    }
    private double Clamp(double value, double min, double max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    #endregion BusinessLogicAbstractAPI

    #region private

    private bool Disposed = false;

    private readonly UnderneathLayerAPI layerBellow;

    private Timer? MoveTimer;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    //testy z DataImplementation
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

    #endregion TestingInfrastructure
    }
}