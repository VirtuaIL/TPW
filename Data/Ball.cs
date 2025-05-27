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
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private Vector _position;
        private Vector _velocity;
        private bool _isMoving;

        private readonly object _positionLock = new();
        private readonly object _velocityLock = new();

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private double _lastFrameTimeSeconds = 0;

        private const double MOVEMENT_SCALE_FACTOR = 50.0; // Współczynnik do skalowania ruchu

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

        private void Move()
        {
            _isMoving = true;
            _stopwatch.Start();
            _lastFrameTimeSeconds = _stopwatch.Elapsed.TotalSeconds; // Czas ostatniej klatki

            while (_isMoving)
            {
                
                double currentTimeSeconds = _stopwatch.Elapsed.TotalSeconds; // Czas bieżącej klatki

                double deltaTime = currentTimeSeconds - _lastFrameTimeSeconds; // Różnica czasu między klatkami

                _lastFrameTimeSeconds = currentTimeSeconds;

                
                deltaTime = Math.Min(deltaTime, 0.1); // Max 0.1 sekundy na klatkę, jak by była zbyt długa

                lock (_positionLock)
                {
                    _position = new Vector(
                      _position.x + _velocity.x * deltaTime * MOVEMENT_SCALE_FACTOR,
                      _position.y + _velocity.y * deltaTime * MOVEMENT_SCALE_FACTOR
                    );
                }

                RaiseNewPositionChangeNotification();

                int desiredFps = 60;
                double desiredFrameTimeSeconds = 1.0 / desiredFps; //~ 0.01667 sekundy na klatkę

                double timeSpentThisFrameSeconds = _stopwatch.Elapsed.TotalSeconds - currentTimeSeconds; // Czas spędzony na tej klatce
                double timeToWaitSeconds = desiredFrameTimeSeconds - timeSpentThisFrameSeconds; // Czas-delay który trzeba odczekać, aby osiągnąć stały FPS

                if (timeToWaitSeconds > 0)
                {
                    Thread.Sleep((int)(timeToWaitSeconds * 1000)); // bo musi być w ms
                }
                else
                {
                    Thread.Sleep(0);
                }

            }
            _stopwatch.Stop();
        }

        #endregion IBall

        #region private

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        #endregion private
    }
}