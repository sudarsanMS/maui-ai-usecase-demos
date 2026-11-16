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
        Turn each business description into a process diagram that is specific to that description.
        Do not reuse a generic sample flow or a fixed template.

        Return JSON only.
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
        - Model approvals, validations, retries, exceptions, escalations, and hand-offs as explicit steps when the description implies them.
        - Use Decision nodes only when the description implies a branch such as if/else, approved/rejected, valid/invalid, pass/fail, or similar.
        - Keep node labels concise, but specific to the process.
        - The title should summarize the process in natural language.
        - Stay within the app limits: no more than 20 nodes, 6 decision nodes, and 30 connectors.
        - If the description is vague, infer the most likely workflow and add a warning that notes the assumption.
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

                Draft a workflow that reflects this process and is meaningfully different from other descriptions.
                Prefer domain-specific actions and branches over generic placeholders.
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
