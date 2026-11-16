using System.ClientModel;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using AIProcessDiagramDrafting.Models;

namespace AIProcessDiagramDrafting.Services;

public sealed class AzureOpenAIWorkflowExtractionService : AzureBaseService, IWorkflowExtractionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly ChatCompletionOptions CompletionOptions = new()
    {
        Temperature = 0f,
        MaxOutputTokenCount = 2048
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

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage($"""
                Process description:
                {processDescription}

                Draft a complete workflow that reflects this process and uses the important details from the description.
                Prefer domain-specific actions, branch labels, and outcomes over generic placeholders.
                Return only the JSON object.
                """)
        };

        string responseText;
        try
        {
            var completion = await chatClient.CompleteChatAsync(messages, CompletionOptions, cancellationToken);
            responseText = string.Concat(completion.Value.Content.Select(content => content.Text ?? string.Empty));
        }
        catch (RequestFailedException ex)
        {
            throw new InvalidOperationException(
                $"Azure OpenAI request failed ({ex.Status}: {ex.ErrorCode}). Check the endpoint, deployment name, and API key.",
                ex);
        }
        catch (ClientResultException ex)
        {
            throw new InvalidOperationException(
                $"Azure OpenAI request failed ({ex.Status}). Check the endpoint, deployment name, and API key.",
                ex);
        }

        return DeserializeWorkflowResponse(responseText);
    }

    private static WorkflowResponse DeserializeWorkflowResponse(string responseText)
    {
        if (TryDeserializeWorkflowResponse(responseText, out var workflow))
        {
            return workflow;
        }

        if (TryExtractJsonObject(responseText, out var json) && TryDeserializeWorkflowResponse(json, out workflow))
        {
            return workflow;
        }

        throw new InvalidOperationException(
            "Azure OpenAI returned content that could not be parsed as a workflow JSON object.");
    }

    private static bool TryDeserializeWorkflowResponse(string json, out WorkflowResponse workflow)
    {
        try
        {
            workflow = JsonSerializer.Deserialize<WorkflowResponse>(json, JsonOptions)
                ?? throw new JsonException("The response deserialized to null.");
            return true;
        }
        catch (JsonException)
        {
            workflow = null!;
            return false;
        }
    }

    private static bool TryExtractJsonObject(string text, out string json)
    {
        var startIndex = text.IndexOf('{');
        if (startIndex < 0)
        {
            json = string.Empty;
            return false;
        }

        var depth = 0;
        var inString = false;
        var escaped = false;

        for (var index = startIndex; index < text.Length; index++)
        {
            var current = text[index];

            if (inString)
            {
                if (escaped)
                {
                    escaped = false;
                }
                else if (current == '\\')
                {
                    escaped = true;
                }
                else if (current == '"')
                {
                    inString = false;
                }

                continue;
            }

            if (current == '"')
            {
                inString = true;
                continue;
            }

            if (current == '{')
            {
                depth++;
            }
            else if (current == '}')
            {
                depth--;
                if (depth == 0)
                {
                    json = text[startIndex..(index + 1)];
                    return true;
                }
            }
        }

        json = string.Empty;
        return false;
    }
}
