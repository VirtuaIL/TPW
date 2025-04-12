//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    private readonly Data.IBall _ball;
    //private readonly Dimensions _dimensions;
    //private readonly double _radius;
    //private readonly Timer _timer;

    public Ball(Data.IBall ball)
    {
        _ball = ball;
        //_dimensions = dimensions;
        //_radius = dimensions.BallDimension / 2.0;

        _ball.NewPositionNotification += RaisePositionChangeEvent;

        //_timer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16)); // 60 FPS
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    #endregion private

    public Data.IVector Position => _ball.Position;
    public Data.IVector Velocity => _ball.Velocity;

    public void SetPosition(Data.IVector newPosition)
    {
        _ball.SetPosition(newPosition);
        RaisePositionChangeEvent(this, newPosition);
    }

        public void SetVelocity(Data.IVector newVelocity)
    {
        _ball.Velocity = newVelocity;
    }

        //private void Move()
        //{
        //    var pos = _ball.Position;
        //    var vel = _ball.Velocity;

        //    double newX = pos.x + vel.x;
        //    double newY = pos.y + vel.y;

        //    // Odbicie od lewej/prawej ściany
        //    if (newX < _radius || newX > _dimensions.TableWidth - _radius)
        //    {
        //        vel = new Vector(-vel.x, vel.y);
        //        newX = Clamp(newX, _radius, _dimensions.TableWidth - _radius);
        //    }

        //    // Odbicie od góry/dna
        //    if (newY < _radius || newY > _dimensions.TableHeight - _radius)
        //    {
        //        vel = new Vector(vel.x, -vel.y);
        //        newY = Clamp(newY, _radius, _dimensions.TableHeight - _radius);
        //    }

        //    _ball.Velocity = vel;
        //    _ball.SetPosition(new Vector(newX, newY));
        //}

        //private double Clamp(double value, double min, double max)
        //{
        //    return Math.Max(min, Math.Min(max, value));
        //}
    }
}