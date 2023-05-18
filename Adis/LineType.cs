namespace Adis;

public enum LineType
{
    Definition = 'D',
    Value = 'V',
    Comment = 'C',
    Search = 'S',
    Request = 'R',
    File = 'F',
    Include = 'I',
    Output = 'O',
    Terminate = 'T',
    EndOfLogicalFile = 'E',
    PhysicalEndOfFile = 'Z',
}
