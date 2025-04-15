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

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class DataImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        IEnumerable<IBall>? ballsList = null;
        newInstance.CheckBallsList(x => ballsList = x);
        Assert.IsNotNull(ballsList);
        int numberOfBalls = 0;
        newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
        Assert.AreEqual<int>(0, numberOfBalls);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataImplementation newInstance = new DataImplementation();
      bool newInstanceDisposed = false;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      IEnumerable<IBall>? ballsList = null;
      newInstance.CheckBallsList(x => ballsList = x);
      Assert.IsNotNull(ballsList);
      newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(0, x));
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
    }

    [TestMethod]
    public void StartTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        int numberOfCallbackInvoked = 0;
        int numberOfBalls2Create = 10;
        //double scale = 1.0;
        //double diameter = 10.0;
        //double tableWidth = 400.0;
        //double tableHeight = 400.0;
        newInstance.Start(
          numberOfBalls2Create,
          (startingPosition, ball) =>
          {
            numberOfCallbackInvoked++;
            Assert.IsTrue(startingPosition.x >= 0);
            Assert.IsTrue(startingPosition.y >= 0);
            Assert.IsNotNull(ball);
          });
        Assert.AreEqual<int>(numberOfBalls2Create, numberOfCallbackInvoked);
        newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(10, x));
      }
    }


    [TestMethod]
    public void MoveTestMethod()
    {
          using (DataImplementation dataImpl = new DataImplementation())
          {
            int numberOfBalls = 1;
            //double scale = 1.0;
            //double diameter = 10.0;
            double tableWidth = 400.0;
            double tableHeight = 400.0;
            

            dataImpl.Start(numberOfBalls, (startingPosition, ball) => { });

            List<IBall>? balls = null;
            dataImpl.CheckBallsList(x => balls = x.ToList());
            Assert.IsNotNull(balls);
            Assert.AreEqual(1, balls.Count);

            var oldPosition = balls[0].Position;

            Random rand = new Random();
                IVector delta = new Vector(
                       (rand.NextDouble() - 0.5) * 10,
                       (rand.NextDouble() - 0.5) * 10
                    );

            dataImpl.Move(delta);
            
            var newPosition = balls[0].Position;

            Assert.IsFalse(oldPosition.x == newPosition.x && oldPosition.y == newPosition.y, "Pozycja kulki powinna się zmienić po ruchu.");
    
            Assert.IsTrue(newPosition.x >= 0 && newPosition.x <= tableWidth, "Kulka nie powinna wyjść poza szerokość planszy.");
            Assert.IsTrue(newPosition.y >= 0 && newPosition.y <= tableHeight, "Kulka nie powinna wyjść poza wysokość planszy.");
          }
    }

  }
}