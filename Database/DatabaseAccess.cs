namespace Codelet.Database
{
  using System;

  /// <summary>
  /// The database access mode.
  /// </summary>
  public enum DatabaseAccess
  {
    /// <summary>
    /// The read and write mode (regular access mode), allows both types of operations.
    /// </summary>
    ReadAndWrite,

    /// <summary>
    /// The read only mode, all attempts to commit data will end up with <see cref="InvalidOperationException"/>.
    /// </summary>
    ReadOnly,
  }
}