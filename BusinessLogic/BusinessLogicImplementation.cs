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
using System.Numerics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
  {
    #region ctor

    public BusinessLogicImplementation() : this(null)
    { }

    internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
    {
      layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
    }

    #endregion ctor

    #region BusinessLogicAbstractAPI

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      layerBellow.Dispose();
      Disposed = true;
    }

    public override void Start(int numberOfBalls,Action<IPosition, IBall> upperLayerHandler)
    {
      //double Scale = scale;
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
      //layerBellow.Start(numberOfBalls, Scale ,GetDimensions.BallDimension ,GetDimensions.TableWidth, GetDimensions.TableHeight ,(startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.x), new Ball(databall)));
      layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.x), new Ball(databall)));
    }

    internal void WallCollision(Data.IBall ball)
{
    lock (_collisionLock)
    {
        //double tableWidth = 400;
        double tableWidth = GetDimensions.TableWidth;
        double tableHeight = GetDimensions.TableHeight;
        //double tableHeight = 400;

        double left = ball.Position.x - ball.Radius;
        double right = ball.Position.x + 1.5 * ball.Radius;
        double top = ball.Position.y - ball.Radius;
        double bottom = ball.Position.y + 1.5 * ball.Radius;

        // Odbicie w poziomie
        if (left <= 0 && ball.Velocity.x < 0 || right >= tableWidth && ball.Velocity.x > 0)
        {
            ball.Velocity = layerBellow.CreateVector(-ball.Velocity.x, ball.Velocity.y);
        }

        // Odbicie w pionie
        if (top <= 0 && ball.Velocity.y < 0 || bottom >= tableHeight && ball.Velocity.y > 0)
        {
            ball.Velocity = layerBellow.CreateVector(ball.Velocity.x, -ball.Velocity.y);
        }
    }
}


    internal void BallCollision(Data.IBall ball)
{
    lock (_collisionLock)
    {
        List<Data.IBall> balls = layerBellow.getAllBalls();
        foreach (var otherBall in balls)
        {
            if (otherBall != ball)
            {
                // Odległość między środkami
                double dx = ball.Position.x - otherBall.Position.x;
                double dy = ball.Position.y - otherBall.Position.y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                double minDistance = ball.Radius + otherBall.Radius;

                if (distance < minDistance && distance > 0.0)
                {
                    // Normalizacja wektora kolizji
                    double nx = dx / distance;
                    double ny = dy / distance;

                    // Różnica prędkości
                    double dvx = ball.Velocity.x - otherBall.Velocity.x;
                    double dvy = ball.Velocity.y - otherBall.Velocity.y;

                    // Iloczyn skalarny prędkości względem osi normalnej
                    double impactSpeed = dvx * nx + dvy * ny;

                    // Jeśli się zbliżają
                    if (impactSpeed < 0)
                    {
                        double m1 = ball.Mass;
                        double m2 = otherBall.Mass;

                        double impulse = (2 * impactSpeed) / (m1 + m2);

                        double v1x = ball.Velocity.x - impulse * m2 * nx;
                        double v1y = ball.Velocity.y - impulse * m2 * ny;
                        double v2x = otherBall.Velocity.x + impulse * m1 * nx;
                        double v2y = otherBall.Velocity.y + impulse * m1 * ny;

                        ball.Velocity = layerBellow.CreateVector(v1x, v1y);
                        otherBall.Velocity = layerBellow.CreateVector(v2x, v2y);
                    }
                }
            }
        }
    }
}




    #endregion BusinessLogicAbstractAPI

    #region private

    private bool Disposed = false;
    private readonly object _collisionLock = new object();
    private readonly UnderneathLayerAPI layerBellow;

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}