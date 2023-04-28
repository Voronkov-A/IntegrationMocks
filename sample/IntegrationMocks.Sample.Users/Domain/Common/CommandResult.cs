using System.ComponentModel.DataAnnotations;

namespace IntegrationMocks.Sample.Users.Domain.Common;

public readonly struct CommandResult<T>
{
    private readonly T? _value;
    private readonly List<string>? _errors;

    private CommandResult(T? value, List<string>? errors)
    {
        _value = value;
        _errors = errors;
    }

    public bool IsSucceeded => _errors == null;

    public IReadOnlyCollection<string> Errors => _errors ?? (IReadOnlyCollection<string>) Array.Empty<string>();

    public T GetOrThrow()
    {
        if (_errors == null)
        {
            return _value!;
        }

        throw new ValidationException(string.Join("\n", _errors));
    }

    public static CommandResult<T> Success(T value)
    {
        return new CommandResult<T>(value, null);
    }

    public static CommandResult<T> Fail(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();

        if (errorList.Count == 0)
        {
            throw new ArgumentException("Error list is empty.", nameof(errors));
        }

        return new CommandResult<T>(default, errorList);
    }
}

public static class CommandResult
{
    public static CommandResult<T> Success<T>(T value)
    {
        return CommandResult<T>.Success(value);
    }

    public static CommandResult<T> Fail<T>(IEnumerable<string> errors)
    {
        return CommandResult<T>.Fail(errors);
    }

    public static CommandResult<T> Fail<T>(string error)
    {
        return Fail<T>(new[] { error });
    }
}
