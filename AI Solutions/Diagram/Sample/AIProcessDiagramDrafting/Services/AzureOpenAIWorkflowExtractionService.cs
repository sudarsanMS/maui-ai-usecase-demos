using System.Text.Json;
using Azure.AI.OpenAI;
using AIProcessDiagramDrafting.Models;

namespace AIProcessDiagramDrafting.Services;

public sealed class AzureOpenAIWorkflowExtractionService : AzureBaseService, IWorkflowExtractionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string SystemPrompt = """
        You are a workflow analyst and diagram drafting engine.
        Your job is to turn each business description into a workflow that is specific to that description.
        Do not reuse a generic sample flow or a fixed template.

        First, identify the important details from the description:
        - the trigger or starting event
        - the actor or system performing each action
        - the major business steps
        - any approvals, validations, exceptions, hand-offs, or retries
        - the final outcome or outcomes

        Then return JSON only.
        Do not include markdown, code fences, or explanations.
        The response must match this shape:
        {
          "title": "string",
          "nodes": [{ "id": "string", "type": "start|process|decision|end", "label": "string" }],
          "connectors": [{ "id": "string", "sourceId": "string", "targetId": "string", "label": "string" }],
          "warnings": ["string"]
        }

        Important rules:
        - Use the description's real nouns, actors, documents, systems, and outcomes.
        - Make the flow complete enough to be useful as a draft, not just a sentence-by-sentence summary.
        - Create a distinct node for every major step.
        - Use Decision nodes only when the description implies a real branch such as if/else, approved/rejected, valid/invalid, pass/fail, or similar.
        - Label branch connectors clearly, such as Yes/No, Approved/Rejected, Valid/Invalid, Success/Failure, or the named condition.
        - Keep node labels concise but specific to the process.
        - The title should summarize the process in natural language.
        - Stay within the app limits: no more than 20 nodes, 6 decision nodes, and 30 connectors.
        - If the description is vague or missing details, infer the most likely workflow and add a warning that notes the assumption.
        - Do not invent unrelated steps.
        - Do not generate coordinates.
        - Do not add unsupported workflow steps.
        """;

    public async Task<WorkflowResponse> ExtractAsync(string processDescription, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(processDescription);

        var client = CreateClient();
        var chatClient = client.GetChatClient(DeploymentName);

        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new OpenAI.Chat.SystemChatMessage(SystemPrompt),
            new OpenAI.Chat.UserChatMessage($"""
                Process description:
                {processDescription}

                Draft a complete workflow that reflects this process and uses the important details from the description.
                Prefer domain-specific actions, branch labels, and outcomes over generic placeholders.
                """)
        };

        var options = new OpenAI.Chat.ChatCompletionOptions
        {
            Temperature = 0f,
            ResponseFormat = OpenAI.Chat.ChatResponseFormat.CreateJsonObjectFormat(),
            MaxOutputTokenCount = 2048
        };

        var completion = await chatClient.CompleteChatAsync(messages, options, cancellationToken);
        var json = string.Concat(completion.Value.Content.Select(content => content.Text ?? string.Empty));

        return JsonSerializer.Deserialize<WorkflowResponse>(json, JsonOptions)
            ?? throw new InvalidOperationException("Azure OpenAI returned an empty workflow response.");
    }
}
