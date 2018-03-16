namespace Darin4Trains.ConsoleApp.Configuration
{
  using System.Diagnostics.CodeAnalysis;

  /// <inheritdoc cref="IAmqConfig" />
  public class AmqConfig : IAmqConfig
  {
    /// <summary>
    /// The configuration section in the appconfig.json file.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public static string ConfigSection => "AmqConfig";

    /// <inheritdoc />
    public string ConnectionUri { get; set; }

    /// <inheritdoc />
    public string Username { get; set; }

    /// <inheritdoc />
    public string Password { get; set; }

    /// <inheritdoc />
    public string ClientId { get; set; }

    /// <inheritdoc />
    public string DestinationName { get; set; }
  }
}
