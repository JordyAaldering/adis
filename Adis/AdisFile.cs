using System;
using System.Collections.Generic;
using System.IO;

namespace Adis;

public class AdisFile
{
    private Dictionary<int, AdisDefinition> definitions = new();
    private Dictionary<int, List<AdisEvent>> adisEvents = new();

    public void Read(TextReader reader)
    {
        AdisDefinition? lastDefinition = null;

        while (reader.ReadLine() is { } line)
        {
            var lineType = (LineType)line[0];
            switch (lineType)
            {
                case LineType.Definition:
                    lastDefinition = AdisDefinition.FromString(line);
                    definitions.Add(lastDefinition.EventNumber, lastDefinition);
                    // add new definition
                    break;
                case LineType.Value:
                    var lineStatus = (LineStatus)line[1];
                    int eventNumber = int.Parse(line.AsSpan(2, 6));
                    var adisEvent = new AdisEvent(lastDefinition, eventNumber, lineStatus);
                    adisEvent.Read(line);
                    break;
            }
        }
    }

    public void Write(TextWriter writer)
    {
        foreach (var (definitionNumber, events) in adisEvents)
        {
            var def = definitions[definitionNumber];
            writer.WriteLine(def.ToString());

            foreach (var adisEvent in events)
            {
                writer.WriteLine(adisEvent.ToString());
            }
        }

        writer.Write((char)LineType.EndOfLogicalFile);
        writer.WriteLine((char)LineStatus.Normal);
        writer.Write((char)LineType.PhysicalEndOfFile);
        writer.WriteLine((char)LineStatus.Normal);
    }
}
