using System;
using HotChocolate;
using HotChocolate.Execution.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides utility methods to setup dependency injection.
/// </summary>
public static class HotChocolateAmazonS3PersistedQueriesRequestExecutorBuilderExtensions
{
    /// <summary>
    /// Adds a Amazon S3 read and write query storage to the
    /// services collection.
    /// </summary>
    /// <param name="builder">
    /// The service collection to which the services are added.
    /// </param>
    /// <param name="bucket">
    /// The bucket that shall be used to store queries.
    /// </param>
    public static IRequestExecutorBuilder AddFileSystemQueryStorage(
        this IRequestExecutorBuilder builder,
        string? bucket = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.ConfigureSchemaServices(
            s => s.AddAmazonS3QueryStorage(bucket));
    }

    /// <summary>
    /// Adds a Amazon S3 read-only query storage to the
    /// services collection.
    /// </summary>
    /// <param name="builder">
    /// The service collection to which the services are added.
    /// </param>
    /// <param name="bucket">
    /// The bucket that shall be used to read queries from.
    /// </param>
    public static IRequestExecutorBuilder AddReadOnlyFileSystemQueryStorage(
        this IRequestExecutorBuilder builder,
        string? bucket = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.ConfigureSchemaServices(
            s => s.AddReadOnlyAmazonS3QueryStorage(bucket));
    }
}
