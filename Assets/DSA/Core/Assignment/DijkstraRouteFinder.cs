using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 4.

    /// <summary>
    /// Finds minimum-total-cost routes through a Frontier Dispatch world
    /// using Dijkstra's shortest-path algorithm.
    ///
    /// This class is intentionally incomplete in the assignment starter.
    /// It should eventually use the student's prepared world representation
    /// and MinPriorityQueue implementation.
    /// </summary>
    public sealed class DijkstraRouteFinder
    {
        private readonly WorldData _world;

        /// <summary>
        /// Constructor. Creates a route finder operating on the supplied world.
        /// </summary>
        public DijkstraRouteFinder(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        /// <summary>
        /// Finds a minimum-total-travel-cost route from the start location
        /// to the goal location.
        ///
        /// A successful RouteResult should contain the route as location IDs,
        /// including both the start and goal locations.
        /// </summary>
        public RouteResult FindRoute(int startLocationId, int goalLocationId)
        {
            if (!_world.TryGetLocationById(startLocationId, out _))
            {
                throw new ArgumentException("Start location does not exist.");
            }

            if (!_world.TryGetLocationById(goalLocationId, out _))
            {
                throw new ArgumentException("Goal location does not exist.");
            }

            if (startLocationId == goalLocationId)
            {
                return new RouteResult(
                    true,
                    new List<int> { startLocationId },
                    0,
                    0
                );
            }

            var frontier = new MinPriorityQueue<int>();
            var distances = new Dictionary<int, int>();
            var previous = new Dictionary<int, int>();
            var visited = new HashSet<int>();

            distances[startLocationId] = 0;
            frontier.Enqueue(startLocationId, 0);

            int expandedCount = 0;

            while (frontier.Count > 0)
            {
                int currentLocationId = frontier.Dequeue();

                if (visited.Contains(currentLocationId))
                {
                    continue;
                }

                visited.Add(currentLocationId);
                expandedCount++;

                if (currentLocationId == goalLocationId)
                {
                    break;
                }

                int currentCost = distances[currentLocationId];

                foreach (Connection connection in _world.GetOutgoingConnections(currentLocationId))
                {
                    int neighbourId = connection.ToLocationId;

                    if (visited.Contains(neighbourId))
                    {
                        continue;
                    }

                    int newCost = currentCost + connection.TravelCost;

                    if (!distances.ContainsKey(neighbourId) ||
                        newCost < distances[neighbourId])
                    {
                        distances[neighbourId] = newCost;
                        previous[neighbourId] = currentLocationId;

                        frontier.Enqueue(neighbourId, newCost);
                    }
                }
            }

            if (!visited.Contains(goalLocationId))
            {
                return new RouteResult(
                    false,
                    new List<int>(),
                    0,
                    expandedCount
                );
            }

            var route = new List<int>();

            int currentId = goalLocationId;

            while (currentId != startLocationId)
            {
                route.Add(currentId);
                currentId = previous[currentId];
            }

            route.Add(startLocationId);

            route.Reverse();

            return new RouteResult(
                true,
                route,
                distances[goalLocationId],
                expandedCount
            );
        }
    }
}
