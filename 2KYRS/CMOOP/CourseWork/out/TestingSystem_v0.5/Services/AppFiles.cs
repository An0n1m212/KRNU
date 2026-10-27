using System.IO;

namespace TestingSystem.Services;

/// <summary>Хранение пользовательских файлов (аватары, медиа вопросов, работы учеников) в %AppData%\TestingSystem.</summary>
public static class AppFiles
{
    /// <summary>Расширения, допустимые для загрузки учеником (исполняемые файлы запрещены — учитель открывает работы).</summary>
    public static readonly string[] AllowedUploadExtensions =
        { ".pdf", ".doc", ".docx", ".txt", ".md", ".png", ".jpg", ".jpeg", ".zip", ".rar", ".7z", ".cs", ".py", ".java", ".cpp", ".sql" };

    public const long MaxUploadBytes = 10 * 1024 * 1024;

    public static string Root => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestingSystem");

    /// <summary>Копирует файл в подпапку приложения под уникальным именем и возвращает новый путь.</summary>
    public static string CopyIn(string subFolder, string sourcePath, string prefix = "f")
    {
        string dir = Path.Combine(Root, subFolder);
        Directory.CreateDirectory(dir);
        string dest = Path.Combine(dir, $"{prefix}_{DateTime.Now.Ticks}{Path.GetExtension(sourcePath)}");
        File.Copy(sourcePath, dest, true);
        return dest;
    }

    public static bool IsAllowedUpload(string path)
        => AllowedUploadExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
}
