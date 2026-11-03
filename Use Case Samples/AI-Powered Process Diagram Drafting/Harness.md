# Harness

## Positive Scenarios

| Test ID | Scenario | Input | Steps | Expected Result | Pass Criteria |
|---|---|---|---|---|---|
| POS-01 | Simple linear workflow | “Employee submits a request. Manager reviews it. Manager approves it. The request is closed.” | Enter text, select Generate Draft, wait for AI response, render diagram | Start, process, and end sequence renders as a valid top-to-bottom flow | Diagram appears with one start node, sequential process nodes, and one end node; no validation errors |
| POS-02 | Workflow with one decision | “Employee submits a purchase request. Manager reviews it. If the amount is below ₹50,000, approve it. Otherwise, send it to Finance. Approved requests create a purchase order. Rejected requests notify the employee.” | Generate draft and inspect branches | Decision node renders with yes/no branch connectors and labeled outcomes | Decision diamond appears; connectors are labeled; both outcomes connect to valid end paths |
| POS-03 | Workflow with multiple decisions | “Customer submits a support case. Triage checks priority. If urgent, escalate to engineering. Otherwise, if billing-related, route to finance. Non-billing cases are handled by support.” | Generate draft and inspect output | Multiple decision points render in sequence with valid branching | Two decision nodes render correctly; each branch is connected and labeled appropriately |
| POS-04 | Workflow with branching approvals | “Employee submits travel expense. Manager reviews it. If under limit, approve. If over limit, Finance reviews it. Finance approves or rejects it.” | Generate draft and inspect graph | Branching approval workflow renders with one manager decision and finance review path | Diagram shows the review split, finance path, and terminal outcomes |
| POS-05 | Workflow with multiple end states | “A customer submits a case. If the request is valid, close the case. If invalid, notify the customer and archive it.” | Generate draft and inspect graph | Multiple terminal outcomes render as distinct end nodes | At least one end node exists for each final outcome; graph remains valid |

## Validation Tests

| Test ID | Scenario | Input | Steps | Expected Result | Pass Criteria |
|---|---|---|---|---|---|
| VAL-01 | Duplicate node IDs | Structured AI response with two nodes sharing the same id | Submit response to validator | Validation fails and diagram is not updated | Current diagram remains unchanged; duplicate ID error is shown |
| VAL-02 | Duplicate connector IDs | Structured AI response with two connectors sharing the same id | Submit response to validator | Validation fails and diagram is not updated | Current diagram remains unchanged; duplicate connector error is shown |
| VAL-03 | Missing start node | Response containing only process and end nodes | Validate before render | Validation fails | Diagram does not render; missing start error is shown |
| VAL-04 | Missing end node | Response containing start and process nodes only | Validate before render | Validation fails | Diagram does not render; missing end error is shown |
| VAL-05 | Unsupported node types | Response containing a node type outside start/process/decision/end | Validate before render | Validation fails | Unsupported node type is rejected and current diagram stays intact |
| VAL-06 | Empty labels | Response with an empty node label or blank whitespace label | Validate before render | Validation fails | Invalid label is rejected and no rendering occurs |
| VAL-07 | Broken connector references | Connector sourceId or targetId does not match any node | Validate before render | Validation fails | Diagram remains unchanged and reference error is shown |
| VAL-08 | Self-referencing connector | Connector sourceId equals targetId | Validate before render | Validation fails | Self-loop is rejected and current diagram remains unchanged |

## Error Tests

| Test ID | Scenario | Input | Steps | Expected Result | Pass Criteria |
|---|---|---|---|---|---|
| ERR-01 | Empty input | Blank text | Open screen with empty input | Generate Draft button is disabled | Button remains disabled until valid text is entered |
| ERR-02 | AI timeout | Valid process description, AI service times out | Generate draft and wait for timeout | Current diagram is preserved and timeout message appears | No existing diagram is lost; retry or failure state is shown |
| ERR-03 | Malformed JSON | AI returns invalid JSON text | Generate draft and parse response | Parsing fails and no diagram update occurs | Current diagram remains unchanged; retry path is shown |
| ERR-04 | Null response | AI returns no payload | Generate draft | Response is treated as invalid | Current diagram remains unchanged; error state is shown |
| ERR-05 | Partial response | AI returns JSON missing nodes or connectors | Validate response | Validation fails | No render occurs; user sees invalid response message |
| ERR-06 | Workflow exceeding limits | Description that causes output above node, connector, or character limits | Generate draft | System rejects the draft and requests a shorter description | No diagram change occurs; limits message is shown |

## User Experience Tests

| Test ID | Scenario | Input | Steps | Expected Result | Pass Criteria |
|---|---|---|---|---|---|
| UX-01 | Edit generated node | Generated diagram with a process node | Edit node label and reposition node | Diagram updates immediately in the editor | Edits persist within the current session and diagram remains valid |
| UX-02 | Delete generated node | Generated diagram with multiple nodes | Delete a non-required node | Diagram updates and connector relationships refresh | Deleted node is removed; invalid connectors are not left dangling |
| UX-03 | Reconnect workflow | Generated diagram with two candidate target nodes | Reconnect a connector to a different node | Connector endpoint changes and graph remains valid | New connection is visible and graph integrity is preserved |
| UX-04 | Undo behavior | Any edited diagram | Make an edit, then undo | Last change is reversed | Diagram returns to prior state after undo |
| UX-05 | Redo behavior | Any diagram after undo | Undo, then redo | Reverted edit is restored | Diagram returns to the redone state |
| UX-06 | Regeneration confirmation | Edited generated diagram | Select Regenerate | Confirmation prompt appears before replacement | No edits are lost until user confirms |
| UX-07 | Discard generated draft | Generated diagram with edits | Select Discard | Draft is removed and user returns to input-focused state | No draft remains on screen; input is preserved or reset per app behavior, but no auto-save occurs |