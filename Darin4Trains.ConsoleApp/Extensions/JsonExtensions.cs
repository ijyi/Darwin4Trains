namespace Darin4Trains.ConsoleApp.Extensions
{
  using System.Linq;
  using System.Text;

  public static class JsonExtensions
  {
    /// <summary>
    /// The indent string.
    /// </summary>
    private const string indentString = "    ";

    public static string FormatJson(this string str, bool addLines = false, bool removeType = true)
    {
      var indent = 0;
      var quoted = false;
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; i++)
      {
        var ch = str[i];
        switch (ch)
        {
          case '{':
          case '[':
            sb.Append(ch);
            if (!quoted && addLines)
            {
              sb.AppendLine();
              Enumerable.Range(0, ++indent).ForEach(item => sb.Append(indentString));
            }

            break;
          case '}':
          case ']':
            if (!quoted && addLines)
            {
              sb.AppendLine();
              Enumerable.Range(0, --indent).ForEach(item => sb.Append(indentString));
            }

            sb.Append(ch);
            break;
          case '"':
            sb.Append(ch);
            var escaped = false;
            var index = i;
            while (index > 0 && str[--index] == '\\')
            {
              escaped = !escaped;
            }

            if (!escaped)
            {
              quoted = !quoted;
            }

            break;
          case ',':
            sb.Append(ch);
            if (!quoted && addLines)
            {
              sb.AppendLine();
              Enumerable.Range(0, indent).ForEach(item => sb.Append(indentString));
            }

            break;
          case ':':
            sb.Append(ch);
            if (!quoted)
            {
              sb.Append(" ");
            }

            break;
          default:
            sb.Append(ch);
            break;
        }
      }

      var result = sb.ToString();
      var lines = result.Split('\r').ToList();
      if (removeType)
      {
        lines.RemoveAll(x => x.Contains("$type"));
      }

      return string.Join("\r", lines);
    }
  }
}
