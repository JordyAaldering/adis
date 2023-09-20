using System;
using System.Collections.Generic;
using System.IO;

namespace Adis;

public class AdisFile
{
    private readonly Dictionary<int, AdisDefinition> adisDefinitions = new();
    private readonly Dictionary<int, List<AdisEvent>> adisEvents = new();
    private readonly List<AdisRequest> adisRequests = new();

    public AdisFile()
    {
    }

    public static AdisFile FromReader(TextReader reader, IFormatProvider? formatProvider = null)
    {
        var adisFile = new AdisFile();
        formatProvider ??= new DefaultFormatProvider();
        AdisDefinition? lastDefinition = null;

        while (reader.ReadLine() is { } line)
        {
            var lineType = (LineType)line[0];
            switch (lineType)
            {
                case LineType.Definition:
                    lastDefinition = AdisDefinition.FromLine(line);
                    adisFile.AddDefinition(lastDefinition);
                    break;
                case LineType.Value:
                    if (lastDefinition == null)
                    {
                        throw new NullReferenceException("Value row found before definition");
                    }

                    var adisEvent = AdisEvent.FromLine(line, lastDefinition, formatProvider);
                    adisFile.AddEvent(lastDefinition, adisEvent);
                    break;
                case LineType.Request:
                    var adisRequest = AdisRequest.FromLine(line);
                    adisFile.AddRequest(adisRequest);
                    break;
                case LineType.EndOfLogicalFile:
                case LineType.PhysicalEndOfFile:
                    return adisFile;
            }
        }

        return adisFile;
    }

    public void AddDefinition(AdisDefinition adisDefinition)
    {
        adisDefinitions.Add(adisDefinition.EventNumber, adisDefinition);
        adisEvents.Add(adisDefinition.EventNumber, new List<AdisEvent>());
    }

    public void AddEvent(AdisDefinition definition, AdisEvent adisEvent)
    {
        adisEvents[definition.EventNumber].Add(adisEvent);
    }

    public AdisDefinition GetDefinition(int eventNumber)
    {
        return adisDefinitions[eventNumber];
    }

    public List<AdisEvent> GetEvents(int eventNumber)
    {
        return adisEvents[eventNumber];
    }

    public void AddRequest(AdisRequest adisRequest)
    {
        adisRequests.Add(adisRequest);

    }

    public void Write(TextWriter writer)
    {
        foreach (var (definitionNumber, events) in adisEvents)
        {
            var def = adisDefinitions[definitionNumber];
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
