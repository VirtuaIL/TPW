﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic
{
  public abstract class BusinessLogicAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region Layer API

    //public static readonly double Scale = 1.0;

    //public static readonly Dimensions GetDimensions = new(
    //    BallDimension: 30.0 * Scale,
    //    TableHeight: 400.0 * Scale,
    //    TableWidth: 400.0 * Scale
    //    );

    //public static readonly Dimensions GetDimensions = new(10.0, 10.0, 10.0);
    public static readonly Dimensions GetDimensions = new(
        BallDimension: 30.0,
        TableHeight: 400.0,
        TableWidth: 400.0
    );

    public abstract void Start(int numberOfBalls, double scale, Action<IPosition, IBall> upperLayerHandler);

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #endregion Layer API

    #region private

    private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());

    #endregion private
  }
  /// <summary>
  /// Immutable type representing table dimensions
  /// </summary>
  /// <param name="BallDimension"></param>
  /// <param name="TableHeight"></param>
  /// <param name="TableWidth"></param>
  /// <remarks>
  /// Must be abstract
  /// </remarks>
  public record Dimensions(double BallDimension, double TableHeight, double TableWidth);

  public interface IPosition
  {
    double x { get; init; }
    double y { get; init; }
  }

  public interface IBall 
  {
    event EventHandler<IPosition> NewPositionNotification;
  }
}