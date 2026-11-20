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
Introduced two dictionaries: _locationsById, which maps location IDs to their corresponding Location objects, and _outgoingConnections, which maps location IDs to lists of outgoing connections. Both dictionaries are populated once in the constructor, allowing other systems to reuse the prepared data. The original arrays were retained to keep efficient sequential access.

- Changes made in WorldQueries.cs
Replaced the original sequential searches with dictionary lookups through WorldData. This improves location lookup from O(V) to O(1) and outgoing connections from O(E) to O(1), with O(k) required to iterate through the returned connections. The trade-off is additional memory usage and an initial preparation cost, in exchange for faster repeated queries.

## 3. Algorithms and adaptation

The route finder uses a min-priority queue to explore locations in order of their accumulated travel cost. It tracks the cheapest known cost to each location in a dictionary and uses a previous dictionary to reconstruct the least-cost route. The search ends when the goal is reached or no locations remain to explore. The reachability query reuses the same Dijkstra-based approach but explores all locations within a given travel budget instead of searching for one specific goal. It excludes routes that exceed the budget and returns a dictionary of reachable location IDs and their cheapest known costs. Since no route reconstruction is needed, the previous dictionary is left out.

## 4. Complexity

| Operation | Your representation/algorithm | Complexity | Short explanation |
| --- | --- | --- | --- |
| Find location by ID | Dictionary<int, Location> | O(1) | Uses the location ID as a key to retrieve the corresponding location directly. |
| Get outgoing connections | Dictionary<int, List<Connection>> | O(1) for lookup, O(k) to iterate | Retrieves the list of connections leaving a location without scanning every connection. |
| Priority queue enqueue | Binary min-heap (List<Entry>) | O(log n) | Adds the entry at the end and uses sift-up to restore the heap property. |
| Priority queue dequeue-min | Binary min-heap (List<Entry>) | O(log n) | Removes the root (smallest priority), moves the last entry to the root, and uses sift-down to restore the heap property. |
| Route finding | Dijkstra's algorithm with a min-priority queue | O((V + E) log V) | Explores locations by lowest accumulated travel cost, tracks the cheapest known distances, and reconstructs the route using a dictionary. |

## 5. Correctness and testing

Added 2 tests during Phase 3
- Test 1: Dequeue maintains priority order after repeated removals
This tests SiftDown() more thoroughly. It makes sure the heap reorganises itself correctly after each removal, not just the first one.

- Test 2: Duplicate priorities are handled correctly
Verifies that equal priorities don't cause items to be lost or incorrectly ordered relative to items with different priorities.

Added 2 tests during Phase 4
- Test 1: Finds and returns correct least cost route
Simple test that checks if a route is successfully found, the algorithm chooses the cheapest route, the returned cost is correct, and the actual route is reconstructed in the correct order.

- Test 2: Handling an unreachable destination
Checks if the algorithm correctly identifies when no route exists and the result communicates failure correctly.

Added 1 test during Phase 5
- Test 1: Budget actually limits reachable locations

Note: Added tests for Phase 4 & 5 use the sample world factory that uses location IDs instead of the world dataset factory which uses a grid. I don't know which way is the intended "correct" way, so I'll stick with this for now.

Modifications and additions during Phase 6:
- Modified the supplied --baseline test by adding a warm-up before measurement, changing the timing output to capture sub-milliseconds results more accurately, and using a more thorough query workload.

- Added 2 new benchmarks: --loading and --allocations. --loading records initialization performance, while --allocations measures managed memory allocations during repeated queries.

## 6. Performance investigation

Three benchmarks were used to compare the starter implementation with the modified one. The benchmarks tested repeated world queries, world preparation, and memory allocations.

- Runtime query performance
The repeated query workload performed 5,000 iterations on a world containing 2,500 locations and 9,800 connections. The starter implementation took approximately 51.5–53.3 ms per workload, while the modified implementation took approximately 0.107–0.108 ms. This improvement comes from replacing repeated linear searches through the location and connection collections with dictionary-based lookups. Outgoing connections are now retrieved directly from a prepared dictionary instead of scanning every connection.

- Loading and preparation performance
The starter implementation took approximately 0.670–0.692 ms to prepare the medium-sized world, compared with 1.329–1.360 ms for the modified implementation. The increased preparation time is caused by constructing the additional dictionaries and outgoing-connection lists in WorldData. This is a worthwhile trade-off, since the increased work during initialization leads to repeated queries being handled significantly faster.

- Memory allocations
Across 5,000 query iterations, the starter implementation allocated 440,000 bytes, while the modified implementation allocated 0 bytes during the measured workload. The reduction comes primarily from reusing outgoing-connection lists created during world preparation, rather than allocating a new list for every query. Both implementations produced the same checksum of 6267100. The additional dictionaries and lists still require memory, but their allocations occur during initialization and are therefore outside this measurement.

In short, the modified implementation shifts some work and memory usage to initialization in exchange for significantly faster runtime queries and fewer repeated allocations. The measurements indicate that this is a beneficial trade-off for a system where world queries are performed frequently.

## 7. Development reflection

During my initial performance measurements, the modified implementation appeared to complete the world-query workload in 0 ms. This made me sceptical of the results, as I expected the additional dictionary lookups to improve performance, but not to the point where the workload seemingly took no time at all. After investigating, I discovered that the benchmark used Stopwatch.ElapsedMilliseconds, which returns whole milliseconds and therefore rounds down when execution takes less than one millisecond. I changed the measurement to use Stopwatch.Elapsed.TotalMilliseconds to obtain more precise results. I also expanded the benchmark with warm-up runs and repeated measurements to make the comparison more reliable. This revealed that the implementation did not actually take 0 ms, but approximately 0.107–0.108 ms per workload. The experience taught me that benchmark results need to be examined critically and that the measurement method itself can affect how performance is interpreted.

## 8. Tools and AI assistance

Asked AI to suggest test ideas for Phase 3.
Asked AI to guide me through the Dijsktra implementation, without giving the code outright. AI provided pseuodecode for some parts.
Asked AI to help me formulate my thoughts for certain parts of this report. Also helped me with analysing complexities by giving me pointers and finding flaws in my assumptions.
Used help from AI while investigating test results and had it help me create new benchmarks without giving the code outright.

Aside from AI, used google and read the documentation to figure out syntax throughout the entire assignment.

## 9. VG extension

Only if attempting VG: heuristic, traversal constraints, correctness, and Dijkstra/A* comparison.
