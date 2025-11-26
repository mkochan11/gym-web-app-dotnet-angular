namespace GymWebApp.ApplicationCore.Extensions;

public static class StringExtensions
{
    public static List<int> ToIntList(this string? stringList)
    {
        if (string.IsNullOrEmpty(stringList))
            return new List<int>();

        return stringList.Split(',')
                      .Select(id => int.TryParse(id, out var result) ? result : (int?)null)
                      .Where(id => id.HasValue)
                      .Select(id => id!.Value)
                      .ToList();
    }
}