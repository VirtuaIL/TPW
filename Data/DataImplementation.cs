//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            //BallsList = new List<Ball>();
        }

        public override IVector CreateVector(double x, double y)
        {
            return new Vector(x, y);
        }

        public override List<IBall> getAllBalls()
        {
            return new List<IBall>(BallsList);
        }


        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();

            for (int i = 0; i < numberOfBalls; i++)
            {
                //Vector startingPosition = new(random.Next(0, 390), random.Next(0, 390));
                //Ball newBall = new(startingPosition, startingPosition);
                Vector startPos = new(random.Next(0, 350), random.Next(0, 350));
                Vector velocity = new(random.NextDouble() * 4 - 1, random.NextDouble() * 4 - 1);
                Ball newBall = new(startPos, velocity);

                BallsList.Add(newBall);
                upperLayerHandler(startPos, newBall);
                newBall.StartThread();
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
                    BallsList.Clear();
                }
                Disposed = true;
            }
        }

        public override void Dispose()
        {

            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private bool Disposed = false;
        private List<Ball> BallsList = [];
        

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
