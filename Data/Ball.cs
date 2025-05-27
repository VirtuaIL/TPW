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

        public Guid Id { get; } = Guid.NewGuid();

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
            Radius = 15.0;
            Mass = 1.0;
            FileLogger.Log($"[Data] Ball {Id} created. Pos: ({_position.x:F2},{_position.y:F2}), Vel: ({_velocity.x:F2},{_velocity.y:F2})");
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
            //set
            //{
            //    lock (_velocityLock)
            //        _velocity = (Vector)value;
            //}
            set
            {
                IVector oldVelocity;
                bool changed = false;
                lock (_velocityLock)
                {
                    oldVelocity = _velocity;
                    // Zakładając, że Vector to rekord, .Equals() zadziała poprawnie dla porównania wartości
                    if (!_velocity.Equals(value))
                    {
                        _velocity = (Vector)value;
                        changed = true;
                    }
                }
                if (changed)
                {
                    // Użycie Id w logowaniu
                    FileLogger.Log($"[Data] Ball {Id} velocity changed. Old: ({oldVelocity.x:F2},{oldVelocity.y:F2}), New: ({_velocity.x:F2},{_velocity.y:F2})");
                }
            }
        }

        public double Radius { get; }
        public double Mass { get; }

        public bool IsMoving
        {
            get => _isMoving;
            //set => _isMoving = value;
            set
            {
                if (_isMoving != value)
                {
                    _isMoving = value;
                    // Użycie Id w logowaniu
                    FileLogger.Log($"[Data] Ball {Id} IsMoving set to: {_isMoving}");
                }
            }
        }

        public void StartThread()
        {
            //Thread thread = new Thread(Move); 
            //thread.IsBackground = true;
            //thread.Name = $"BallThread_{Guid.NewGuid().ToString()[..4]}";
            //thread.Start();
            Thread thread = new Thread(Move)
            {
                IsBackground = true,
                // Użycie części Id w nazwie wątku dla łatwiejszej identyfikacji
                Name = $"BallThread_{Id.ToString().Substring(0, 8)}"
            };
            thread.Start();
        }

        private void Move()
        {
            _isMoving = true;
            FileLogger.Log($"[Data] Ball {Id} thread starting.");
            _stopwatch.Start();
            _lastFrameTimeSeconds = _stopwatch.Elapsed.TotalSeconds; // Czas ostatniej klatki

            while (_isMoving)
            {
                
                double currentTimeSeconds = _stopwatch.Elapsed.TotalSeconds; // Czas bieżącej klatki

                double deltaTime = currentTimeSeconds - _lastFrameTimeSeconds; // Różnica czasu między klatkami

                _lastFrameTimeSeconds = currentTimeSeconds;

                
                deltaTime = Math.Min(deltaTime, 0.1); // Max 0.1 sekundy na klatkę, jak by była zbyt długa

                Vector oldPosition;
                bool positionChanged = false;

                lock (_positionLock)
                {
                    oldPosition = _position;
                    _position = new Vector(
                      _position.x + _velocity.x * deltaTime * MOVEMENT_SCALE_FACTOR,
                      _position.y + _velocity.y * deltaTime * MOVEMENT_SCALE_FACTOR
                    );
                    if (Math.Abs(oldPosition.x - _position.x) > 0.0001 || Math.Abs(oldPosition.y - _position.y) > 0.0001)
                    {
                        positionChanged = true;
                    }
                }

                if (positionChanged)
                {
                    // Użycie Id w logowaniu
                    FileLogger.Log($"[Data] Ball {Id} position. New: ({_position.x:F2},{_position.y:F2}), DeltaT: {deltaTime:F4}s");
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
            FileLogger.Log($"[Data] Ball {Id} thread stopped.");
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