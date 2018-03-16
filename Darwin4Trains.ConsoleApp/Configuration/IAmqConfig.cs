namespace Darwin4Trains.ConsoleApp.Configuration
{
  /// <summary>
  /// The AMQ config so we can create strongly typed versions of the config settings.
  /// </summary>
  public interface IAmqConfig
  {
    /// <summary>
    /// Gets or sets the connection uri for the Darwin4Trains AMQ server.
    /// </summary>
    string ConnectionUri { get; set; }

    /// <summary>
    /// Gets or sets the username for the Darwin4Trains AMQ server.
    /// </summary>
    string Username { get; set; }

    /// <summary>
    /// Gets or sets the password for the Darwin4Trains AMQ server.
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// Gets or sets the client id supplied for Darwin4Trains.
    /// </summary>
    string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the destination name.
    /// </summary>
    string DestinationName { get; set; }
  }
}
