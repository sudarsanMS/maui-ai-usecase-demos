# Development Tasks

## Goal

Build a single-screen .NET MAUI sample that turns a short process description into an editable Syncfusion Diagram draft with explicit user review.

## Task 1: Set up the sample shell

* Create the new sample project area and page.
* Add the page title, input section, diagram surface, and review actions.
* Support desktop and mobile layout variants.
* Collapse the input section after generation.

## Task 2: Define the workflow contract

* Create the workflow response model.
* Add node, connector, and warning types.
* Enforce the supported node type list.
* Keep the model independent of any specific AI provider.

## Task 3: Implement AI extraction service abstraction

* Add `IWorkflowExtractionService`.
* Build the prompt from the user input.
* Send the request through the abstracted provider interface.
* Parse the JSON response into the workflow model.
* Return warnings when the AI reports ambiguity.

## Task 4: Implement response validation

* Create `WorkflowResponseValidator`.
* Validate schema rules.
* Validate graph rules.
* Validate business limits.
* Reject invalid responses before rendering.

## Task 5: Build diagram draft conversion

* Create `DiagramDraftBuilder`.
* Map supported nodes to Syncfusion shapes.
* Create directional connectors.
* Apply top-to-bottom layout settings.
* Apply orthogonal routing.
* Keep coordinate generation inside the app.

## Task 6: Implement view model state management

* Create `ProcessDiagramViewModel`.
* Manage generate, regenerate, accept, discard, undo, and redo actions.
* Preserve the current diagram when validation fails.
* Show the regeneration confirmation when edits exist.
* Collapse the input section after a successful generation.

## Task 7: Build editing support

* Allow move, edit, add, delete, and reconnect actions.
* Allow branch label editing.
* Add fit-to-view support.
* Preserve user edits until explicit regeneration or discard.

## Task 8: Add user feedback and error handling

* Disable Generate when the input is empty.
* Request more detail when the description is unclear.
* Show warnings for ambiguous branches.
* Preserve state on service failure.
* Request a shorter description when limits are exceeded.

## Task 9: Add acceptance and discard flow

* Add an Accept Draft action.
* Add a Discard action.
* Confirm before replacing edits during regeneration.
* Never auto-accept or auto-execute the workflow.

## Task 10: Add automated tests

* Add unit tests for validator rules.
* Add unit tests for prompt generation and JSON parsing.
* Add view model tests for state transitions.
* Add builder tests for diagram mapping.
* Add UI flow tests for the main user journey.

## Task 11: Verify the sample against the harness

* Run the validation scenarios.
* Confirm invalid responses never overwrite the current diagram.
* Confirm the diagram remains editable after generation.
* Confirm regeneration requires explicit user confirmation.
* Confirm the sample never executes the workflow.

## Delivery order

1. Shell and layout
2. Workflow contract
3. Extraction service abstraction
4. Validation
5. Diagram builder
6. View model state management
7. Editing support
8. Error handling
9. Acceptance and discard flow
10. Automated tests
11. Harness verification
