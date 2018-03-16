namespace Darin4Trains.ConsoleApp
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text;

  using Apache.NMS;
  using Apache.NMS.ActiveMQ;

  using Darin4Trains.ConsoleApp.Configuration;
  using Darin4Trains.ConsoleApp.Extensions;

  using Microsoft.Extensions.Configuration;

  using Newtonsoft.Json.Linq;

  public class Program
  {
    /// <summary>
    /// The darwin snapshot sequence.
    /// </summary>
    private const string DarwinSnapshotSequence = "DarwinSnapshotSequence";

    /// <summary>
    /// The message directory.
    /// </summary>
    private static readonly DirectoryInfo MessageDirectory = Directory.CreateDirectory($"C:\\temp\\Darwin4Trains\\Messages\\{DateTimeOffset.Now.ToUnixTimeMilliseconds()}");

    /// <summary>
    /// The main.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    public static void Main(string[] args)
    {
      // Ensure that the config has been loaded into strongly typed objects to use
      SetupConfig();

      var amqUri = new Uri(AmqConfiguration.ConnectionUri);
      Console.WriteLine("Connecting to: " + amqUri);

      var factory = new ConnectionFactory(amqUri) { AsyncSend = true };
      using (var connection = factory.CreateConnection(AmqConfiguration.Username, AmqConfiguration.Password))
      {
        connection.ClientId = AmqConfiguration.ClientId;
        using (var session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
        {
          if (SelectorConfiguration.IsSnapshotEnabled)
          {
            ConsumeSnapshot(session, connection);
          }

          if (SelectorConfiguration.IsListenerEnabled)
          {
            ConsumeDarwin4Trains(session, connection);
          }
        }
      }
    }

    private static void ConsumeDarwin4Trains(ISession session, IConnection connection)
    {
      using (var topic = session.GetTopic(AmqConfiguration.DestinationName))
      {
        using (var consumer = GetSessionConsumer(session, topic))
        {
          consumer.Listener += OnMessage;
          connection.Start();

          Console.WriteLine($"Listening to {AmqConfiguration.DestinationName}");
          Console.WriteLine("Press any key to quit...");
          Console.ReadKey();

          connection.Stop();
        }
      }
    }

    private static void ConsumeSnapshot(ISession session, IConnection connection)
    {
      // server destination to send requests
      var destination = session.GetQueue(SelectorConfiguration.SnapshotDestination);
      var producer = session.CreateProducer(destination);
      producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

      using (var queue = session.CreateTemporaryQueue())
      {
        using (var consumer = session.CreateConsumer(queue))
        {
          consumer.Listener += OnSnapshot;
          connection.Start();

          // request snapshot
          var textMessage = session.CreateTextMessage(SelectorConfiguration.SnapshotFilter);
          textMessage.NMSReplyTo = queue;
          textMessage.NMSCorrelationID = Guid.NewGuid().ToString();
          textMessage.NMSTimeToLive = new TimeSpan(0, 0, 10);
          textMessage.NMSTimestamp = DateTime.UtcNow;
          textMessage.NMSType = "snapshot";

          producer.Send(textMessage);

          Console.WriteLine($"Requesting Snapshot {textMessage.Text}");
          Console.WriteLine("Press any key to quit...");
          Console.ReadKey();

          connection.Stop();
        }
      }
    }

    public static AmqConfig AmqConfiguration { get; set; }

    public static SelectorConfig SelectorConfiguration { get; set; }

    /// <summary>
    /// The get session consumer.
    /// </summary>
    /// <param name="session">
    /// The session.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <returns>
    /// The <see cref="IMessageConsumer"/>.
    /// </returns>
    private static IMessageConsumer GetSessionConsumer(ISession session, ITopic topic)
    {
      if (SelectorConfiguration.IsSelectorEnabled && !string.IsNullOrWhiteSpace(SelectorConfiguration.SelectorFilter))
      {
        return session.CreateConsumer(topic, SelectorConfiguration.SelectorFilter);
      }

      return session.CreateConsumer(topic);
    }

    /// <summary>
    /// The on message.
    /// </summary>
    /// <param name="receivedMsg">The received message from AMQ.</param>
    private static void OnSnapshot(IMessage receivedMsg)
    {
      if (!(receivedMsg is ITextMessage message))
      {
        Console.WriteLine("No message received!");
      }
      else
      {
        var sequenceNumber = Convert.ToInt32(receivedMsg.Properties[DarwinSnapshotSequence]);

        var trainJson = message.Text.FormatJson(true, false);
        var fileName = $"{receivedMsg.NMSCorrelationID}\\Snapshot_{sequenceNumber}.json";
        var messageFile = new FileInfo(Path.Combine(MessageDirectory.FullName, fileName));
        //if (messageFile.Directory != null && !messageFile.Directory.Exists)
        //{
        //  messageFile.Directory.Create();
        //}

        using (FileStream stream = File.Open(Path.Combine(MessageDirectory.FullName, fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
          byte[] info = new UTF8Encoding(true).GetBytes(trainJson);
          stream.Write(info, 0, info.Length);
        }
      }
    }

    /// <summary>
    /// The on message.
    /// </summary>
    /// <param name="receivedMsg">The received message from AMQ.</param>
    private static void OnMessage(IMessage receivedMsg)
    {
      if (!(receivedMsg is ITextMessage message))
      {
        Console.WriteLine("No message received!");
      }
      else
      {
        var trainJson = message.Text;

        dynamic messageData = JObject.Parse(message.Text);
        var fileName = $"-Date_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.json";

        if (trainJson.Contains("TrainAmqViewModel"))
        {
          fileName = $"RID_{messageData["RID"]}-UID_{messageData["UID"]}-TrainAmqViewModel{fileName}";
        }
        else if (trainJson.Contains("StationFacilitiesViewModel"))
        {
          fileName = $"CRS_{messageData["CRS"]}-StationFacilitiesViewModel{fileName}";
        }
        else if (trainJson.Contains("TflAmqViewModel"))
        {
          fileName = $"TFL-TflAmqViewModel{fileName}";
        }
        else if (trainJson.Contains("HeartbeatAmqViewModel"))
        {
          fileName = $"Heartbeat-HeartbeatAmqViewModel{fileName}";
        }

        using (var stream = File.Open(Path.Combine(MessageDirectory.FullName, fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
          byte[] info = new UTF8Encoding(true).GetBytes(trainJson);
          stream.Write(info, 0, info.Length);
        }

        // File.WriteAllText(Path.Combine(MessageDirectory.FullName, fileName), trainJson);
        Console.WriteLine("Received message with ID:   " + message.NMSMessageId);
      }
    }

    private static void SetupConfig()
    {
      var builder = new ConfigurationBuilder().AddJsonFile("appconfig.json", optional: true, reloadOnChange: true);

      IConfigurationRoot configuration = builder.Build();

      AmqConfiguration = new AmqConfig();
      SelectorConfiguration = new SelectorConfig();

      configuration.GetSection(AmqConfig.ConfigSection).Bind(AmqConfiguration);
      configuration.GetSection(SelectorConfig.ConfigSection).Bind(SelectorConfiguration);
    }
  }
}
