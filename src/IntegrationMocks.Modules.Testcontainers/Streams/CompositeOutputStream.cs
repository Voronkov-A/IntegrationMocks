using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IntegrationMocks.Modules.Testcontainers.Streams;

internal class CompositeOutputStream : Stream
{
    private readonly List<Stream> _children;

    public CompositeOutputStream(IEnumerable<Stream> children)
    {
        _children = children.ToList();
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => _children.All(x => x.CanWrite);

    public override long Length =>
        throw new NotSupportedException($"{nameof(CompositeOutputStream)} does not support seeking.");

    public override long Position
    {
        get => throw new NotSupportedException($"{nameof(CompositeOutputStream)} does not support seeking.");
        set => throw new NotSupportedException($"{nameof(CompositeOutputStream)} does not support seeking.");
    }

    public override void Flush()
    {
        foreach (var child in _children)
        {
            child.Flush();
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException($"{nameof(CompositeOutputStream)} does not support reading.");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException($"{nameof(CompositeOutputStream)} does not support seeking.");
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException($"{nameof(CompositeOutputStream)} does not support seeking.");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        foreach (var child in _children)
        {
            child.Write(buffer, offset, count);
        }
    }
}
