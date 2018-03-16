namespace Darwin4Trains.ConsoleApp.Configuration
{
  /// <inheritdoc cref="ISelectorConfig" />
  public class SelectorConfig : ISelectorConfig
  {
    /// <summary>
    /// The config section so we can get it from the config file.
    /// </summary>
    public static string ConfigSection => "SelectorConfig";

    /// <inheritdoc />
    public bool IsSnapshotEnabled { get; set; }

    /// <inheritdoc />
    public bool IsListenerEnabled { get; set; }

    /// <inheritdoc />
    public bool IsSelectorEnabled { get; set; }

    /// <inheritdoc />
    public string SnapshotFilter { get; set; }

    /// <inheritdoc />
    public string SelectorFilter { get; set; }

    /// <inheritdoc />
    public string SnapshotDestination { get; set; }
  }
}
