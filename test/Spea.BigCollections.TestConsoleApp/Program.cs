using BigCollections;
using System.Text.Json;

namespace BigCollections.TestConsoleApp
{
  internal class Program
  {
    static void Main(string[] args)
    {
      var a = new BigArray<int>(10);

      var s = JsonSerializer.Serialize(a);
      var options = new JsonSerializerOptions();
      options.Converters.Add(new BigArrayJsonConverter<int>());
      var o = JsonSerializer.Deserialize<BigArray<int>>(s, options);
    }
  }
}
