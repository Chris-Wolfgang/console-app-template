using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

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
    [Range(0, int.MaxValue, ErrorMessage = "Current item count cannot be less than 0.")]
    public int? MaxItemCount { get; set; }



    /// <summary>
    /// The number of items to skip before starting to process. This is useful for pagination or skipping initial items.
    /// If not specified, no items will be skipped.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Current item count cannot be less than 0.")]
    public int? SkipItemCount { get; set; }


    // TODO Add additional command options here



    /// <summary>
    /// This method is called when the command is executed.
    /// </summary>
    /// <returns>
    /// A value of 0 indicatess success. A value greater than 0 indicates failure
    /// </returns>
    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        ILogger<SubCommandTemplate> logger
    )
    {
        logger.LogDebug("Starting {command}", GetType().Name);

        try
        {
            // TODO: Validate the command options here if necessary



            // Create a progress reporter to report the current count of items processed
            var progress = new Progress<Report>
            (
                report =>
                {
                    logger.LogDebug("Current count: {count}", report.CurrentCount);
                    console.WriteLine("Current count: {count}", report.CurrentCount);
                }
            );


            // Create instances of the extractor
            var extractor = new MyExtractor(logger)
            {
                MaxItemCount = MaxItemCount,
                SkipItemCount = SkipItemCount
            };

            // Create instances of the transformer
            var transformer = new MyTransformer(logger);

            // Create instances of the loader
            var loader = new MyLoader(logger);

            // Execute the ETL process asynchronously
            await ExecuteEtlAsycn(extractor, transformer, loader, console, progress);

            logger.LogInformation("ETL process completed successfully.");

            return ExitCode.Success;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, e.Message);
            console.WriteLine(e);
            return ExitCode.ApplicationError;
        }

        logger.LogDebug("Completed {command}", GetType().Name);

        return ExitCode.Success;
    }


    internal async Task ExecuteEtlAsync<IExtractor, ITransformer, ILoader>
    (
        IExtractWithProgressAsync extractor,
        ITransformAsync transformer,
        ILoadAsync loader,
        IProgress<> reporter
    )
    {
        var item extractor.ExtractAsync(reporter);
        var transformedItems = transformer.TransformAsync();
        await loader.LoadAsync(transformedItems);
    }
}