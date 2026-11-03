# REPORT.md - Individual Assignment

Keep this concise and specific to your implementation.

## 1. Initial analysis

- How are locations and connections represented in the baseline?
Locations and connections are represented as read-only lists in WorldData.cs

- Which operations appear frequent?
For loops that look through data in WorldData.cs are core to both methods supplied in WorldQueries.cs.

- Which operation(s) do you predict may scale poorly?
Both methods in WorldQueries.cs scale linearly and are O(n) in the worst-case scenario. While both get quite expensive as the data sample grows large, the method FindOutgoingConnections catches my eye as /especially/ inefficient. I surmise that there are not-so-difficult ways to pre-initialize the data to make it considerably cheaper. 

- What will you investigate first?
I will investigate FindOutgoingConnections first and sketch out how I can make it more efficient.

## 2. Data representation

Describe important changes/retained choices, operations affected, and time/memory trade-offs.

## 3. Algorithms and adaptation

Explain your route finder at a useful high level and how the travel-budget query adapts the same ideas.

## 4. Complexity

| Operation | Your representation/algorithm | Complexity | Short explanation |
| --- | --- | --- | --- |
| Find location by ID | | | |
| Get outgoing connections | | | |
| Priority queue enqueue | | | |
| Priority queue dequeue-min | | | |
| Route finding | | | |

## 5. Correctness and testing

Important tests/edge cases and what they demonstrate.

## 6. Performance investigation

Record reproducible before/after measurements for loading/preparation, repeated runtime work, and relevant allocation behavior. Explain what changed and why.

## 7. Development reflection

Describe one bug, wrong assumption, failed approach, or surprising result and how you investigated it.

## 8. Tools and AI assistance

Briefly describe relevant assistance, if any.

## 9. VG extension

Only if attempting VG: heuristic, traversal constraints, correctness, and Dijkstra/A* comparison.
