using System.ComponentModel;

namespace TFPLibrary.Models;

public abstract class ParserBase
{
    internal readonly ProgressReportingModel ProgressModel;
    internal readonly IProgress<int> Progress;
    internal readonly CancellationToken Ct;
    internal ProcessingSteps ProcessingStep;

    protected ParserBase(ProgressReportingModel progressModel, IProgress<int> progress, CancellationToken ct)
    {
        ProgressModel = progressModel;
        Progress = progress;
        Ct = ct;
        ProcessingStep = ProcessingSteps.SplittingStep;
    }

    internal async Task ReportProgressAsync()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10), Ct);
        Progress.Report(ProcessingStep switch
        {
            ProcessingSteps.SplittingStep => ProgressModel.ReportProgress1(),
            ProcessingSteps.CountingStep => ProgressModel.ReportProgress2(),
            ProcessingSteps.ConvertingStep => ProgressModel.ReportProgress3(),
            ProcessingSteps.OrderingStep => ProgressModel.ReportProgress4(),
            _ => throw new InvalidEnumArgumentException()
        });
    }
}
