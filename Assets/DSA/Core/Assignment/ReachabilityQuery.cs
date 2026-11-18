using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 5. Adapt shortest-path reasoning to a travel-cost budget.

    /// <summary>
    /// Finds locations that can be reached from a starting location without
    /// exceeding a maximum total travel cost.
    ///
    /// This is an adaptation of shortest-path reasoning: instead of searching
    /// for one goal, the query determines the cheapest known cost to every
    /// location that fits within the supplied budget.
    ///
    /// This class is intentionally incomplete in the assignment starter.
    ///
    /// What do we mean by "query"? In this context, a query is simply a one-time search for information.
    /// </summary>
    public sealed class ReachabilityQuery
    {
        private readonly WorldData _world;

        /// <summary>
        /// Creates a reachability query operating on the supplied world.
        /// </summary>
        public ReachabilityQuery(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        /// <summary>
        /// Returns reachable location IDs mapped to their cheapest known
        /// travel cost from the starting location.
        ///
        /// Only locations whose cheapest cost is within maxTravelCost should
        /// be included. The starting location has cost 0.
        /// </summary>
        public IReadOnlyDictionary<int, int> FindReachable(int startLocationId, int maxTravelCost)
        {
            if (!_world.TryGetLocationById(startLocationId, out _))
            {
                throw new ArgumentException("Start location does not exist.");
            }

            if (maxTravelCost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTravelCost));
            }

            var frontier = new MinPriorityQueue<int>();
            var distances = new Dictionary<int, int>();
            var visited = new HashSet<int>();

            distances[startLocationId] = 0;
            frontier.Enqueue(startLocationId, 0);

            while (frontier.Count > 0)
            {
                int currentLocationId = frontier.Dequeue();

                if (visited.Contains(currentLocationId))
                {
                    continue;
                }

                int currentCost = distances[currentLocationId];

                if (currentCost > maxTravelCost)
                {
                    break;
                }

                visited.Add(currentLocationId);

                foreach (Connection connection in _world.GetOutgoingConnections(currentLocationId))
                {
                    int neighbourId = connection.ToLocationId;

                    if (visited.Contains(neighbourId))
                    {
                        continue;
                    }

                    int newCost = currentCost + connection.TravelCost;

                    if (newCost <= maxTravelCost &&
                        (!distances.ContainsKey(neighbourId) ||
                         newCost < distances[neighbourId]))
                    {
                        distances[neighbourId] = newCost;
                        frontier.Enqueue(neighbourId, newCost);
                    }
                }
            }

            return distances;
        }
    }
}
