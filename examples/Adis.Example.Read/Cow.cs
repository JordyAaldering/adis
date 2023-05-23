using System;

namespace Adis.Example.Read;

public class Cow
{
    public int Number { get; private set; }

    public string Name { get; private set; } = "";

    public DateTime BirthDate { get; private set; }

    public DateTime? PregnancyCheckDate { get; private set; }

    public float MilkYield { get; private set; }

    public static Cow FromAdis(AdisEvent adisEvent)
    {
        return new Cow
        {
            Number = adisEvent.GetData<int>(1),
            Name = adisEvent.GetData<string>(2) ?? "",
            BirthDate = adisEvent.GetData<DateTime>(3),
            PregnancyCheckDate = adisEvent.GetData<DateTime?>(4),
            MilkYield = adisEvent.GetData<float>(5),
        };
    }

    public override string ToString()
    {
        return $"Number: {Number:D5} Name: {Name} BirthDate: {BirthDate:yyyy/MM/dd} "
            + $"CheckDate: {BirthDate:yyyy/MM/dd HH:mm:ss} Yield: {MilkYield:F3}";
    }
}
