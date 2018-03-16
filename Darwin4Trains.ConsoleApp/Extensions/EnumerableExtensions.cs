namespace Darwin4Trains.ConsoleApp.Extensions
{
  using System;
  using System.Collections.Generic;

  public static class EnumerableExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> doAction)
    {
      foreach (var collectionItem in collection)
      {
        doAction(collectionItem);
      }
    }
  }
}
