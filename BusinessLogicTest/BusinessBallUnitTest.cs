//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void MoveTestMethod()
    {
      DataBallFixture dataBallFixture = new DataBallFixture();
      Ball newInstance = new(dataBallFixture);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
      dataBallFixture.Move();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
    }

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
        public Data.IVector Velocity { get; set; } = new VectorFixture(0.0, 0.0);

        public Data.IVector Position => _position;

        public event EventHandler<Data.IVector>? NewPositionNotification;

        private Data.IVector _position = new VectorFixture(0.0, 0.0);

        public void SetPosition(Data.IVector newPosition)
        {
            _position = new VectorFixture(newPosition.x, newPosition.y);
            NewPositionNotification?.Invoke(this, _position);
        }

        internal void Move()
        {
            var newPos = new VectorFixture(_position.x + Velocity.x, _position.y + Velocity.y);
            SetPosition(newPos);
        }
    }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; init; }
      public double y { get; init; }
    }

    #endregion testing instrumentation
  }
}