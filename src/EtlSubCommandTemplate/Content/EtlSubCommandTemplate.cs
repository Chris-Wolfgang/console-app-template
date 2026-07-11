// NOTE: This class depends on Wolfgang.Etl.Abstractions
// TODO Please run the following in your project folder:
//   dotnet add package Wolfgang.Etl.Abstractions
//
// NOTE: If your application was created from the Wolfgang Console App template this
// command is registered automatically at startup (AutoRegisterCommandsConvention).
// In an app without that convention, add [Subcommand(typeof(EtlSubCommandTemplate))] to Program.

using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Wolfgang.Etl.Abstractions;

namespace {DefaultNamespace}.Command;

[Command
(
    // TODO You can override the default name for the command by uncommenting the line below
    // Name = "{alternative-name}"
    
    // TODO Add a description for your command
    Description = "{command-description}",

    // TODO Specify response file handling. Default is disabled. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.ResponseFileHandling.html
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated

)]
internal class EtlSubCommandTemplate
{


    /// <summary>
    /// The maximum number of items to process. If not specified, all items will be processed.
    /// </summary>
    [Option(Description = "The maximum number of items to process. If not specified, all items will be processed")]
    [Range(0, int.MaxValue, ErrorMessage = "Maximum item count cannot be less than 0.")]
    public int? MaxItemCount { get; set; }



    /// <summary>
    /// The number of items to skip before starting to process. This is useful for pagination or skipping initial items.
    /// If not specified, no items will be skipped.
    /// </summary>
    [Option(Description = "The number of items to skip before starting to process. If not specified, no items will be skipped")]
    [Range(0, int.MaxValue, ErrorMessage = "Skip item count cannot be less than 0.")]
    public int? SkipItemCount { get; set; }


    // TODO Add additional command options here



    /// <summary>
    /// This method is called when the command is executed.
    /// </summary>
    /// <returns>
    /// A value of 0 indicates success. A value greater than 0 indicates failure
    /// </returns>
    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        ILogger<EtlSubCommandTemplate> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug("Starting {Command}", GetType().Name);

        try
        {
            // TODO: Validate the command options here if necessary

            // TODO Build the pipeline and run it. Uncomment and fill in the types
            // below. A Progress<Report> reporter surfaces item counts as it runs.
            // For graceful Ctrl+C / host shutdown, have your extractor/transformer/
            // loader implementations observe cancellationToken - e.g. pass it into
            // their constructors and check it while producing or consuming items.
            // (The ETL abstraction methods themselves don't take a token, which is
            // why ExecuteEtlAsync below doesn't forward one.)
            //
            // To create the typed loggers below, add an `ILoggerFactory loggerFactory`
            // parameter to this method - McMaster injects it by type.
            //
            // var progress = new Progress<Report>(report =>
            // {
            //     logger.LogDebug("Current count: {Count}", report.CurrentItemCount);
            //     console.WriteLine($"Current count: {report.CurrentItemCount}");
            // });
            //
            // var extractor = new MyExtractor< , Report>(loggerFactory.CreateLogger<MyExtractor< , Report>>())
            // {
            //     MaximumItemCount = MaxItemCount ?? int.MaxValue,
            //     SkipItemCount = SkipItemCount ?? 0
            // };
            // var transformer = new MyTransformer< , , Report>(loggerFactory.CreateLogger<MyTransformer< , , Report>>());
            // var loader = new MyLoader< , Report>(loggerFactory.CreateLogger<MyLoader< , Report>>());
            //
            // await ExecuteEtlAsync(extractor, transformer, loader, progress);

            await Task.Yield(); // Simulate doing work - remove once the ETL pipeline above is uncommented
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation("ETL process completed successfully.");
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled error: {Message}", e.Message);
            console.WriteLine(e);
            return ExitCode.ApplicationError;
        }

        logger.LogDebug("Completed {Command}", GetType().Name);

        return ExitCode.Success;
    }


    internal static Task ExecuteEtlAsync<TSource, TDestination, TProgress>
    (
        IExtractWithProgressAsync<TSource,TProgress> extractor,
        ITransformAsync<TSource, TDestination> transformer,
        ILoadAsync<TDestination> loader,
        IProgress<TProgress> reporter
    ) where TDestination : notnull where TSource : notnull
    {
        var item = extractor.ExtractAsync(reporter);
        var transformedItems = transformer.TransformAsync(item);
        return loader.LoadAsync(transformedItems);
    }
}