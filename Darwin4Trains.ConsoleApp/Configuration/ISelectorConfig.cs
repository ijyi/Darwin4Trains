namespace Darwin4Trains.ConsoleApp.Configuration
{
  /// <summary>
  /// The Darwin4Trains selector config so we can create strongly typed versions of the config settings.
  /// </summary>
  public interface ISelectorConfig
  {
    /// <summary>
    /// Gets or sets a value indicating whether you want to request a Darwin4Trains snapshot.
    /// </summary>
    bool IsSnapshotEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether you want to listen for Darwin4Trains messages.
    /// </summary>
    bool IsListenerEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selector is enabled to filter Darwin4Trains messages.
    /// </summary>
    bool IsSelectorEnabled { get; set; }

    /// <summary>
    /// Gets or sets the snapshot filter to filter for specific Darwin4Trains messages during a snapshot.
    /// </summary>
    string SnapshotFilter { get; set; }

    /// <summary>
    /// Gets or sets the selector filter to filter specific messages coming out of Darwin4Trains.
    /// </summary>
    string SelectorFilter { get; set; }

    /// <summary>
    /// Gets or sets the destination name.
    /// </summary>
    string SnapshotDestination { get; set; }
  }
}
