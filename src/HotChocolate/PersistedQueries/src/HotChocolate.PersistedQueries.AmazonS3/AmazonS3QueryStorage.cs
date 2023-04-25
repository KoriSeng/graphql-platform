using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using HotChocolate.Execution;
using HotChocolate.Utilities;
using HotChocolate.Language;

namespace HotChocolate.PersistedQueries.AmazonS3;

/// <summary>
/// An implementation of <see cref="IReadStoredQueries"/>
/// and <see cref="IWriteStoredQueries"/> that
/// uses the Amazon S3.
/// </summary>
public class AmazonS3QueryStorage : IReadStoredQueries
    , IWriteStoredQueries
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;


    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="s3">Amazon s3 client.</param>
    /// <param name="bucket">Bucket name.</param>
    public AmazonS3QueryStorage(IAmazonS3 s3, string bucket)
    {
        _s3 = s3 ?? throw new ArgumentNullException(nameof(s3));
        _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        if (!AmazonS3Util.DoesS3BucketExistV2Async(s3, bucket).Result)
        {
            throw new ArgumentNullException(nameof(bucket));
        }
    }

    /// <inheritdoc />
    public async Task<QueryDocument?> TryReadQueryAsync(string queryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queryId))
        {
            throw new ArgumentNullException(nameof(queryId));
        }

        var response = await _s3.GetObjectAsync(_bucket, Util.EncodeQueryId(queryId), cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return null;
        }

        var document = await BufferHelper.ReadAsync(
                response.ResponseStream,
                static (buffer, buffered) =>
                {
                    var span = buffer.AsSpan().Slice(0, buffered);
                    return Utf8GraphQLParser.Parse(span);
                },
                cancellationToken)
            .ConfigureAwait(false);
        return new QueryDocument(document);
    }

    /// <inheritdoc />
    public async Task WriteQueryAsync(string queryId, IQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queryId))
        {
            throw new ArgumentNullException(nameof(queryId));
        }

        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var stream = new MemoryStream();
        await query.WriteToAsync(stream, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);
        var request = new PutObjectRequest() { BucketName = _bucket, FilePath = Util.EncodeQueryId(queryId), InputStream = stream, };
        await _s3.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
