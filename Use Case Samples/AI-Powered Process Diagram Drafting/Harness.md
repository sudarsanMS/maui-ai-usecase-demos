# Harness

## Purpose

This harness defines how the team should verify the AI-powered diagram drafting flow in a production-quality sample app.

## Test strategy

Use layered checks.

* Unit tests validate prompt building, JSON parsing, and response validation.
* View model tests validate state changes, regeneration confirmation, and error handling.
* UI tests validate the user flow from input to rendered draft.
* Manual checks confirm that editing remains possible after generation.

## Testable seams

### IWorkflowExtractionService

Mock this interface in tests. Return known JSON payloads, invalid payloads, slow responses, and failures.

### WorkflowResponseValidator

Test with valid graphs, duplicate IDs, missing start nodes, missing end nodes, unsupported node types, and self-referencing connectors.

### DiagramDraftBuilder

Test that validated nodes map to the correct Syncfusion shapes and that connectors are created with the expected labels and routing settings.

### ProcessDiagramViewModel

Test generate, regenerate, accept, discard, undo, redo, and state preservation.

## Functional harness scenarios

### Scenario 1: Valid purchase approval flow

Input:

> An employee submits a purchase request. The manager reviews it. If the amount is below ₹50,000 the manager approves it. Otherwise Finance reviews it. Approved requests create a purchase order. Rejected requests notify the employee.

Expected result:

* The app returns a draft with a start node, process steps, a decision node, and end nodes.
* The branch labels indicate the approved and alternate paths.
* The diagram renders top to bottom.
* The user can edit the rendered diagram.

### Scenario 2: Empty input

Input:

* Empty text

Expected result:

* Generate stays disabled.
* No AI request is sent.

### Scenario 3: Ambiguous description

Input:

> Handle the request as needed.

Expected result:

* The AI service returns a warning or the validator rejects the draft.
* The app asks for more detail.
* The existing diagram remains unchanged.

### Scenario 4: Invalid AI response

Input:

* A response with duplicate node IDs or an unsupported node type

Expected result:

* The app rejects the response.
* The current diagram is preserved.
* The user sees a validation error message.

### Scenario 5: Regenerate with edits

Input:

* A diagram with user edits
* The user selects Regenerate

Expected result:

* The app shows the confirmation message.
* The app regenerates only after confirmation.
* Cancel keeps the edited diagram.

### Scenario 6: Service failure

Input:

* A network or provider failure

Expected result:

* The app preserves the current diagram.
* The user sees a failure message.

## Validation checklist

The harness should confirm the following before release:

* One start node exists.
* At least one end node exists.
* Node and connector IDs are unique.
* Connectors reference valid nodes.
* Node and connector counts stay within limits.
* Labels stay within limits.
* The diagram remains editable after generation.
* Regeneration requires explicit confirmation when edits exist.
* The UI never auto-accepts or auto-executes a workflow.

## Sample assertions

### Validator assertions

* `IsValidGraph` returns false when a connector references a missing node.
* `IsValidGraph` returns false when two nodes share the same ID.
* `IsValidGraph` returns false when the graph has more than one start node.
* `IsValidGraph` returns false when the graph has no end node.

### View model assertions

* `GenerateDraftAsync` does not replace the current diagram when validation fails.
* `RegenerateAsync` shows a confirmation step when edits exist.
* `DiscardAsync` clears the draft state.
* `AcceptAsync` marks the draft as accepted without changing the workflow definition.

### Builder assertions

* Start and end nodes map to terminator shapes.
* Process nodes map to rectangles.
* Decision nodes map to diamonds.
* Connector labels are copied to the diagram model.

## Manual review checklist

* Enter a sample process description.
* Generate the draft.
* Confirm the diagram is readable and editable.
* Move a node.
* Edit a label.
* Add a node.
* Delete a node.
* Reconnect nodes.
* Regenerate and confirm the warning appears.
* Discard and confirm the draft clears.

## Release gate

The sample is ready when:

* Validation tests pass.
* View model tests pass.
* UI flow tests pass.
* Manual review confirms the diagram remains editable.
* The app preserves state after invalid or failed AI responses.
