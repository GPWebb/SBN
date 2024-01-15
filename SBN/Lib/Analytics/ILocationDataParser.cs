namespace SBN.Lib.Analytics
{
    public interface ILocationDataParser
    {
        LocationData Parse(string locationData);
    }
}