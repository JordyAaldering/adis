using System;
using System.IO;

namespace Adis.Example.Write;

public class Program
{
    private static readonly Random rnd = new();

    private static void Main()
    {
        var adisFile = new AdisFile();
        var cowDefinition = CreateCowDefinition();
        adisFile.AddDefinition(cowDefinition);

        for (int i = 0; i < 50; i++)
        {
            var cow = RandomCow();
            var adisEvent = cow.ToAdisEvent(cowDefinition);
            adisFile.AddEvent(cowDefinition, adisEvent);
        }

        var writer = new StringWriter();
        adisFile.Write(writer);
        Console.Write(writer.ToString());
    }

    private static AdisDefinition CreateCowDefinition()
    {
        var cowDefinition = new AdisDefinition(123456);
        cowDefinition.AddColumnDefinition(1, 5);
        cowDefinition.AddColumnDefinition(2, 12);
        cowDefinition.AddColumnDefinition(3, 8);
        cowDefinition.AddColumnDefinition(4, 14);
        cowDefinition.AddColumnDefinition(5, 7, 3);
        return cowDefinition;
    }

    private static Cow RandomCow()
    {
        int number = rnd.Next(9999);
        var birthDate = new DateTime(2022, 1, 1).AddDays(rnd.Next(300));

        return new Cow()
        {
            Number = number,
            Name = $"Cow {number:D6}",
            BirthDate = birthDate.AddSeconds(rnd.Next(7200)),
            PregnancyCheckDate = rnd.Next(2) == 0 ? null : birthDate.AddDays(rnd.Next(10, 100)),
            MilkYield = rnd.NextDouble() * 100.0,
        };
    }
}
