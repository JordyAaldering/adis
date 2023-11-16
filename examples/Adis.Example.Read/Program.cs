using System;
using System.IO;

namespace Adis.Example.Read;

public class Program
{
    private static void Main()
    {
        var inputFile = new FileInfo(Path.Join(".", "Resources", "AdisFile.txt"));
        var reader = inputFile.OpenText();

        var adisFile = AdisFile.FromReader(reader);
        var cowEvents = adisFile.GetEvents(123456);

        Console.WriteLine($"ADIS file contains {cowEvents.Count} cows");
        foreach (var cowEvent in cowEvents)
        {
            var cow = Cow.FromAdis(cowEvent);
            Console.WriteLine(cow);
        }
    }
}
