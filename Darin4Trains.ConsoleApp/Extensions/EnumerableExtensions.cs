using System;
using System.Collections.Generic;

namespace Darin4Trains.ConsoleApp.Extensions
{
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
