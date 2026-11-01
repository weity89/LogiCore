namespace LogiCore.Domain.Repositories;

public static class RepositoryExtensions
{
    public static string ToReportTable<T>(this IEnumerable<T> collection, string title)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== {title} ===");
        foreach (var item in collection)
        {
            sb.AppendLine(item?.ToString() ?? "null");
        }
        sb.AppendLine("=================");
        return sb.ToString();
    }
}