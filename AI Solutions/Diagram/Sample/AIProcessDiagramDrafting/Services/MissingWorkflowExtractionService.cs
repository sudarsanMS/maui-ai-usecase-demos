using AIProcessDiagramDrafting.Models;

namespace AIProcessDiagramDrafting.Services;

public sealed class MissingWorkflowExtractionService : IWorkflowExtractionService
{
    public Task<WorkflowResponse> ExtractAsync(string processDescription, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(
            "Azure OpenAI is not configured for this sample. Set AZURE_OPENAI_ENDPOINT, AZURE_OPENAI_DEPLOYMENT, and AZURE_OPENAI_API_KEY to enable AI-based diagram drafting. AI_PROCESS_DIAGRAM_FAKE_MODE is only for explicit test or harness runs.");
    }
}
