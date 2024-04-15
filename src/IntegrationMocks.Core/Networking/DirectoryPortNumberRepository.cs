using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IntegrationMocks.Core.Networking;

public class DirectoryPortNumberRepository : IPortNumberRepository
{
    private readonly string _directoryPath;

    public DirectoryPortNumberRepository(string directoryPath)
    {
        _directoryPath = directoryPath;

        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }

    public HashSet<int> GetAll()
    {
        return Directory.GetFiles(_directoryPath, "*", SearchOption.TopDirectoryOnly)
            .Select(FilePathToValue)
            .ToHashSet();
    }

    public bool Add(int value)
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

    public void Remove(int value)
    {
        File.Delete(ValueToFilePath(value));
    }

    private string ValueToFilePath(int value)
    {
        return Path.Combine(_directoryPath, value.ToString());
    }

    private static int FilePathToValue(string filePath)
    {
        return int.Parse(Path.GetFileName(filePath));
    }
}
