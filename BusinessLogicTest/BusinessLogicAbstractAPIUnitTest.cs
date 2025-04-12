﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Reflection.Metadata.Ecma335;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BusinessLogicAbstractAPIUnitTest
  {
    [TestMethod]
    public void BusinessLogicConstructorTestMethod()
    {
      BusinessLogicAbstractAPI instance1 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
      BusinessLogicAbstractAPI instance2 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
      Assert.AreSame(instance1, instance2);
      instance1.Dispose();
      Assert.ThrowsException<ObjectDisposedException>(() => instance2.Dispose());
    }

    [TestMethod]
    public void GetDimensionsTestMethod()
    {
      double scale = BusinessLogicAbstractAPI.Scale;
      Assert.AreEqual<Dimensions>(new(3.0*scale, 40.0*scale, 40.0*scale), BusinessLogicAbstractAPI.GetDimensions);
    }
  }
}