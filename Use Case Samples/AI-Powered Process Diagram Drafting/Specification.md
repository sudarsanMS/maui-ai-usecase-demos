# AI-Powered Process Diagram Drafting

## Purpose

This sample helps a user turn a short natural-language process description into an editable flowchart in Syncfusion .NET MAUI Diagram. The AI proposes a draft only. The user reviews the draft before the app accepts it.

## Business goal

Help business analysts, process owners, and operations teams turn written process notes into a visual flowchart faster.

## Success criteria

* A user can enter a process description and generate a draft diagram.
* The app validates the AI response before it renders anything.
* The user can edit the diagram after generation.
* The app never executes a workflow.
* The app never accepts, publishes, or applies a draft without user confirmation.

## In scope

* Natural-language process input
* AI extraction of workflow structure
* Editable diagram rendering with Syncfusion .NET MAUI Diagram
* Start, process, decision, and end nodes
* Connectors with branch labels
* Accept, regenerate, and discard actions
* Edit support after generation
* Validation of AI responses before render
* Warning display for ambiguous or incomplete extraction

## Out of scope

* BPMN generation
* UML generation
* Swimlanes
* Workflow execution
* Workflow optimization
* Auto publishing
* Multi-user collaboration
* External workflow integrations

## User experience

### Desktop layout

The page uses a two-panel layout.

* Left panel: process description input, example selector, and Generate Draft button.
* Right panel: diagram surface and review actions.

After generation, the input section collapses to give the diagram more space.

### Mobile layout

The page uses a stacked layout.

* Description input
* Generate button
* Diagram surface
* Review actions

## User flow

1. The user enters a process description or selects an example.
2. The user selects Generate Draft.
3. The app sends the text to the AI extraction service.
4. The app receives structured JSON.
5. The app validates the JSON.
6. If validation passes, the app renders an editable diagram draft.
7. The user edits the diagram if needed.
8. The user accepts, regenerates, or discards the draft.

## AI behavior

### AI must

* Extract only the workflow described in the input.
* Detect process steps, decisions, outcomes, and branches.
* Return concise labels.
* Return structured JSON that matches the response contract.
* Return warnings when the input is vague or ambiguous.

### AI must not

* Invent steps.
* Make business decisions.
* Generate coordinates.
* Execute workflows.
* Replace user edits without confirmation.
* Auto-accept or auto-publish a draft.

## AI response contract

The service returns JSON in this shape:

```json
{
  "title": "Purchase Request Approval",
  "nodes": [
    {
      "id": "N1",
      "type": "start",
      "label": "Start"
    },
    {
      "id": "N2",
      "type": "process",
      "label": "Submit purchase request"
    },
    {
      "id": "N3",
      "type": "decision",
      "label": "Amount below ₹50,000?"
    }
  ],
  "connectors": [
    {
      "id": "C1",
      "sourceId": "N1",
      "targetId": "N2",
      "label": ""
    },
    {
      "id": "C2",
      "sourceId": "N3",
      "targetId": "N4",
      "label": "Yes"
    }
  ],
  "warnings": []
}
```

## Supported node types

* start
* process
* decision
* end

## Mapping rules

* Start -> Terminator
* Process -> Rectangle
* Decision -> Diamond
* End -> Terminator
* Flow -> Directional connector
* Branch -> Connector label

## Layout rules

* The diagram uses a top-to-bottom flow.
* Connectors use orthogonal routing.
* The AI does not generate coordinates.
* The application calculates coordinates.

## Validation rules

The app validates the AI response before it renders the diagram.

### Structural rules

* Node IDs are unique.
* Connector IDs are unique.
* Connector source references are valid.
* Connector target references are valid.
* Exactly one start node exists.
* At least one end node exists.
* Only supported node types are allowed.
* Node labels are not empty.
* Connector labels are not empty when the branch needs a label.
* No connector points to the same node as its source.

### Result rules

* Invalid responses do not replace the current diagram.
* Ambiguous branches show warnings.
* Service failures preserve the current state.

## Size limits

* Description length: 1500 characters
* Nodes: 20
* Decisions: 6
* Connectors: 30
* Node label length: 60 characters
* Connector label length: 30 characters

## Editing requirements

Users can:

* Move nodes
* Edit labels
* Add nodes
* Delete nodes
* Reconnect nodes
* Edit branch labels
* Undo
* Redo
* Fit to view

## Regeneration behavior

If the diagram contains user edits and the user selects Regenerate, the app shows this confirmation message:

> Regenerating will replace your current diagram edits. Do you want to continue?

The app only regenerates after the user confirms.

## Error handling

* Empty input disables Generate.
* Unclear input requests more detail.
* Invalid AI responses keep the current diagram.
* Ambiguous branches show warnings.
* Service failure preserves the current diagram.
* Limits exceeded prompts the user to shorten the input.

## Architecture

```text
ProcessDiagramPage
└── ProcessDiagramViewModel
    ├── IWorkflowExtractionService
    ├── WorkflowResponseValidator
    └── DiagramDraftBuilder
```

### ProcessDiagramPage

Owns the screen layout and binds to the view model. It does not contain provider-specific AI logic.

### ProcessDiagramViewModel

Manages input, generation, regeneration, acceptance, discard flow, and error state.

### IWorkflowExtractionService

Builds the prompt, calls the AI provider, and parses the JSON response.

### WorkflowResponseValidator

Validates schema, graph rules, and business limits before the diagram renders.

### DiagramDraftBuilder

Converts the validated workflow model into Syncfusion Diagram objects and applies shapes and connectors.

## Non-functional requirements

* The UI must stay responsive while the AI request runs.
* The app must preserve the current diagram when a request fails.
* The app must not expose provider-specific details in the UI layer.
* The app must keep AI output validation deterministic and testable.
* The feature must be maintainable as a single-screen showcase.

## Acceptance criteria

* Given a valid description, the app generates a draft diagram that matches the described workflow.
* Given an invalid response, the app does not overwrite the current diagram.
* Given a user edit, the app asks for confirmation before regeneration.
* Given ambiguous input, the app shows a warning.
* The user can continue editing after generation.
* The app never executes the described workflow.
