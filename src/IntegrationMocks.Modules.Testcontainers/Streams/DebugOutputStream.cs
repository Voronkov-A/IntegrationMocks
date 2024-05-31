using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace IntegrationMocks.Modules.Testcontainers.Streams;

internal class DebugOutputStream : Stream
{
    private readonly Encoding _encoding;

    public DebugOutputStream(Encoding encoding)
    {
        _encoding = encoding;
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length =>
        throw new NotSupportedException($"{nameof(DebugOutputStream)} does not support seeking.");

    public override long Position
    {
        get => throw new NotSupportedException($"{nameof(DebugOutputStream)} does not support seeking.");
        set => throw new NotSupportedException($"{nameof(DebugOutputStream)} does not support seeking.");
    }

    public override void Flush()
    {
        Debug.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException($"{nameof(DebugOutputStream)} does not support reading.");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException($"{nameof(DebugOutputStream)} does not support seeking.");
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException($"{nameof(DebugOutputStream)} does not support seeking.");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        var str = _encoding.GetString(buffer, offset, count);
        Debug.Write(str);
    }
}
