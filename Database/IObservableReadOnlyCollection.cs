namespace Codelet.Database
{
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;

  /// <summary>
  /// A composite interface for read only observable collection.
  /// </summary>
  /// <typeparam name="T">The type of the items inside collection.</typeparam>
  public interface IObservableReadOnlyCollection<out T>
    : IReadOnlyCollection<T>,
      INotifyCollectionChanged,
      INotifyPropertyChanged,
      INotifyPropertyChanging
  {
  }
}