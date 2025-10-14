using System.IO;

namespace openPlot.Tests;

internal static class TestPathHelper
{
    /// <summary>
    /// Sobe diretórios a partir do BaseDirectory até encontrar a pasta do projeto (<projectName>)
    /// contendo o .csproj correspondente.
    /// </summary>
    public static string GetProjectPath(string projectName)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            var candidate = Path.Combine(current.FullName, projectName);
            var csproj = Path.Combine(candidate, $"{projectName}.csproj");

            if (Directory.Exists(candidate) && File.Exists(csproj))
                return candidate;

            current = current.Parent;
        }

        throw new InvalidOperationException(
            $"Project folder for '{projectName}' not found from BaseDirectory '{AppContext.BaseDirectory}'.");
    }
}
