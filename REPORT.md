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

- Changes made in WorldData.cs
Introduced two dictionaries: _locationsById, which maps location IDs to their corresponding Location objects, and _outgoingConnections, which maps location IDs to lists of outgoing connections. Both dictionaries are populated once in the constructor, allowing other systems to reuse the prepared data. The original arrays were retained to keep that efficient sequential access.

- Changes made in WorldQueries.cs
Replaced the original sequential searches with dictionary lookups through WorldData. This improves location lookup from O(V) to O(1) average time and outgoing connections from O(E) to O(1) average time, with O(k) required to iterate through the returned connections. The trade-off is additional memory usage and an initial preparation cost, in exchange for faster repeated queries.

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

Added 2 tests during Phase 3
- Test 1: Dequeue maintains priority order after repeated removals
This tests SiftDown() more thoroughly. It makes sure the heap reorganises itself correctly after each removal, not just the first one.

- Test 2: Duplicate priorities are handled correctly
Verifies that equal priorities don't cause items to be lost or incorrectly ordered relative to items with different priorities.

Important tests/edge cases and what they demonstrate.

## 6. Performance investigation

Record reproducible before/after measurements for loading/preparation, repeated runtime work, and relevant allocation behavior. Explain what changed and why.

## 7. Development reflection

Describe one bug, wrong assumption, failed approach, or surprising result and how you investigated it.

## 8. Tools and AI assistance

Asked AI to suggest test ideas for Phase 3.
Briefly describe relevant assistance, if any.

## 9. VG extension

Only if attempting VG: heuristic, traversal constraints, correctness, and Dijkstra/A* comparison.
