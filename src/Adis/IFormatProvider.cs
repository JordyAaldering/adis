namespace Adis;

public interface IFormatProvider
{
    string Serialize<T>(T? value, int length, int resolution = 0);

    T? Deserialize<T>(string value, int length, int resolution = 0);
}
