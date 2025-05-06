//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {

    private Vector _position;
    private Vector _velocity;
    private bool _isMoving;

    private readonly object _positionLock = new();
    private readonly object _velocityLock = new();

    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      _position = initialPosition;
      _velocity = initialVelocity;
      Radius = 15.0;
      Mass = 1.0;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;


    public IVector Position
    {
      get
      {
        lock (_positionLock)
          return _position;
      }
    }

    public IVector Velocity
    {
      get
      {
        lock (_velocityLock)
          return _velocity;
      }
      set
      {
        lock (_velocityLock)
          _velocity = (Vector)value;
      }
    }

    public double Radius { get; }

    public double Mass { get; }

    public bool IsMoving
    {
      get => _isMoving;
      set => _isMoving = value;
    }

    public void StartThread()
    {
      Thread thread = new Thread(Move);
      thread.IsBackground = true;
      thread.Name = $"BallThread_{Guid.NewGuid().ToString()[..4]}";
      thread.Start();

    }

    private async void Move()
    {
      _isMoving = true;
      while (_isMoving)
      {
        lock (_positionLock)
        {
          _position = new Vector(
            _position.x + _velocity.x * 0.5,
            _position.y + _velocity.y * 0.5
          );
        }

        RaiseNewPositionChangeNotification();

        double speed;
        lock (_velocityLock)
        {
          speed = Math.Sqrt(_velocity.x * _velocity.x + _velocity.y * _velocity.y);
        }

        int delay = (int)Math.Clamp(1000 / (speed * 40), 10, 30);
        await Task.Delay(delay);
      }
    }


    #endregion IBall

    #region private

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    //public void Move(IVector delta)
    //{
    //   var d = (Vector)delta;
    //   //_position.x += d.x;
    //   //_position.y += d.y;
    //  //Position = new Vector(Position.x + delta.x, Position.y + delta.y);
    //  _position = new Vector(_position.x + d.x, _position.y + d.y);
    //  RaiseNewPositionChangeNotification();
    //}

    #endregion private
  }
}