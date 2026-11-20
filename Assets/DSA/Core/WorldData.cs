using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public sealed class WorldData
    {
        private readonly Location[] _locations;
        private readonly Connection[] _connections;

        private readonly Dictionary<int, Location> _locationsById;
        private readonly Dictionary<int, List<Connection>> _outgoingConnections;
        public WorldData(IEnumerable<Location> locations, IEnumerable<Connection> connections)
        {
            if (locations == null) throw new ArgumentNullException(nameof(locations));
            if (connections == null) throw new ArgumentNullException(nameof(connections));
            _locations = new List<Location>(locations).ToArray();
            _connections = new List<Connection>(connections).ToArray();

            _locationsById = new Dictionary<int, Location>();
            _outgoingConnections = new Dictionary<int, List<Connection>>();

            foreach (Location location in _locations)
            {
                _locationsById.Add(location.Id, location);
                _outgoingConnections.Add(location.Id, new List<Connection>());
            }

            foreach (Connection connection in _connections)
            {
                _outgoingConnections[connection.FromLocationId].Add(connection);
            }
        }

        public IReadOnlyList<Location> Locations => _locations;
        public IReadOnlyList<Connection> Connections => _connections;

        public bool TryGetLocationById(int id, out Location? location)
        {
            return _locationsById.TryGetValue(id, out location);
        }

        public IReadOnlyList<Connection> GetOutgoingConnections(int locationId)
        {
            if (_outgoingConnections.TryGetValue(locationId, out List<Connection>? connections))
            {
                return connections;
            }

            return Array.Empty<Connection>();
        }
    }
}
