namespace IntegrationMocks.Core.Resources;

public class DirectoryStringRepository : IStringRepository
{
    private readonly string _directoryPath;

    public DirectoryStringRepository(string directoryPath)
    {
        _directoryPath = directoryPath;

        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }

    public HashSet<string> GetAll()
    {
        return Directory.GetFiles(_directoryPath, "*", SearchOption.TopDirectoryOnly)
            .Select(FilePathToValue)
            .ToHashSet();
    }

    public bool Add(string value)
    {
        try
        {
            File.Open(ValueToFilePath(value), FileMode.CreateNew, FileAccess.Write, FileShare.None).Dispose();
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    public void Remove(string value)
    {
        File.Delete(ValueToFilePath(value));
    }

    private string ValueToFilePath(string value)
    {
        if (value.Any(x => x is '/' or '\\' or ':'))
        {
            throw new NotSupportedException($"Value '{value}' is not supported.");
        }

        return Path.Combine(_directoryPath, value);
    }

    private static string FilePathToValue(string filePath)
    {
        return Path.GetFileName(filePath);
    }
}
