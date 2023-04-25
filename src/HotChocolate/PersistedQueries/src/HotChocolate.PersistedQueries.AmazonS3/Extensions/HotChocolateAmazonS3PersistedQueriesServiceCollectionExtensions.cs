using System;
using System.Linq;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate.Execution;
using HotChocolate.PersistedQueries.AmazonS3;

namespace HotChocolate;

/// <summary>
/// Provides utility methods to setup dependency injection.
/// </summary>
public static class HotChocolateFileSystemPersistedQueriesServiceCollectionExtensions
{
    /// <summary>
    /// Adds a Amazon S3 read and write query storage to the
    /// services collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to which the services are added.
    /// </param>
    /// <param name="bucket">
    /// The bucket that shall be used to store queries.
    /// </param>
    public static IServiceCollection AddAmazonS3QueryStorage(
        this IServiceCollection services,
        string? bucket = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return services
            .AddReadOnlyAmazonS3QueryStorage(bucket)
            .AddSingleton<IWriteStoredQueries>(
                sp => sp.GetRequiredService<AmazonS3QueryStorage>());
    }

    /// <summary>
    /// Adds a Amazon S3 read-only query storage to the
    /// services collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to which the services are added.
    /// </param>
    /// <param name="bucket">
    /// The bucket that shall be used to read queries from.
    /// </param>
    public static IServiceCollection AddReadOnlyAmazonS3QueryStorage(
        this IServiceCollection services,
        string? bucket = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return services
            .RemoveService<IReadStoredQueries>()
            .RemoveService<IWriteStoredQueries>()
            .AddSingleton(c => new AmazonS3QueryStorage(
                c.GetService<IAmazonS3>() ??
                c.GetApplicationService<IAmazonS3>(), bucket))
            .AddSingleton<IReadStoredQueries>(
                sp => sp.GetRequiredService<AmazonS3QueryStorage>());
    }

    private static IServiceCollection RemoveService<TService>(
        this IServiceCollection services)
    {
        var serviceDescriptor = services.FirstOrDefault(t => t.ServiceType == typeof(TService));

        if (serviceDescriptor is not null)
        {
            services.Remove(serviceDescriptor);
        }

        return services;
    }
}
