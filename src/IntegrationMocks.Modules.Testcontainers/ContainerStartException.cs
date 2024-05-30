using System;
using System.Text;

namespace IntegrationMocks.Modules.Testcontainers;

public class ContainerStartException : Exception
{
    public ContainerStartException(string message, string stdOut, string stdErr, Exception innerException)
        : base(message, innerException)
    {
        StdOut = stdOut;
        StdErr = stdErr;
    }

    public string StdOut { get; }

    public string StdErr { get; }

    public override string ToString()
    {
        var details = new StringBuilder()
            .AppendLine("STDOUT:")
            .AppendLine(StdOut)
            .AppendLine("STDERR:")
            .AppendLine(StdErr)
            .ToString();
        return ToString(this, details);
    }

    private static string ToString(Exception exception, string details)
    {
        var sb = new StringBuilder(exception.GetType().ToString())
            .Append(": ")
            .Append(exception.Message)
            .AppendLine()
            .Append(details);

        if (exception.InnerException != null)
        {
            sb
                .Append(" ---> ")
                .AppendLine(exception.InnerException.ToString())
                .Append("   --- End of inner exception stack trace ---");
        }

        if (exception.StackTrace != null)
        {
            sb.AppendLine().Append(exception.StackTrace);
        }

        return sb.ToString();
    }
}
