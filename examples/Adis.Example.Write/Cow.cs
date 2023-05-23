using System;

namespace Adis.Example.Write;

public class Cow
{
    public int Number { get; set; }

    public string Name { get; set; } = "";

    public DateTime BirthDate { get; set; }

    public DateTime? PregnancyCheckDate { get; set; }

    public double MilkYield { get; set; }

    public AdisEvent ToAdisEvent(AdisDefinition definition)
    {
        var adisEvent = definition.CreateAdisEvent();
        adisEvent.SetData(1, Number);
        adisEvent.SetData(2, Name);
        adisEvent.SetData(3, BirthDate);
        adisEvent.SetData(4, PregnancyCheckDate);
        adisEvent.SetData(5, MilkYield);
        return adisEvent;
    }
}
