using System.Diagnostics;

namespace Totoro;

public static class Development
{
    /// <summary>
    /// Will read the .env file if available and set all environment variables.
    /// The .env file must be in the same directory as the solution file.
    /// </summary>
    /// <param name="filename">The .env file name.</param>
    [Conditional("DEBUG")]
    public static void ReadEnvironmentVariables(string filename = ".env.local")
    {
        var root = TryGetSolutionPath();
        var path = Path.Combine(root, filename);

        if (!File.Exists(path))
        {
            return;
        }

        var items = File.ReadAllLines(path)
            .Select(line => line.Split('='))
            .Where(parts => parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
            .Select(values => new KeyValuePair<string, string>(values[0], values[1]));

        foreach (var (key, value) in items)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    #region Private Methods

    private static string TryGetSolutionPath()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? string.Empty;
    }

    #endregion
}
