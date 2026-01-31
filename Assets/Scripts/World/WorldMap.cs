using UnityEngine;
using System.Collections.Generic;

public static class WorldMap
{
    public static readonly Dictionary<Vector2Int, RoomInfo> Rooms =
        new Dictionary<Vector2Int, RoomInfo>
        {
            // Spawn / Hub
            { new Vector2Int(0, 0), new RoomInfo(RoomType.Spawn) },

            // Top cluster
            { new Vector2Int(1, -1), new RoomInfo(3, 0) }, // boars
            { new Vector2Int(2, -4), new RoomInfo(2, 1) }, // mixed
            { new Vector2Int(2, 2),  new RoomInfo(1, 2) }, // monkey heavy
            { new Vector2Int(3, 1),  new RoomInfo(0, 2) }, // monkeys

            { new Vector2Int(2, 1), new RoomInfo(3, 0) },

            // Tribe A
            { new Vector2Int(1, 1), new RoomInfo(RoomType.Tribe, TribeType.TribeA) },
            { new Vector2Int(3, 0), new RoomInfo(2, 0) },

            // Middle / Tribe B cluster
            { new Vector2Int(0, 1), new RoomInfo(RoomType.Tribe, TribeType.TribeB) },
            { new Vector2Int(3, -1), new RoomInfo(2, 1) },
            { new Vector2Int(2, -1), new RoomInfo(3, 0) },
            { new Vector2Int(2, -2), new RoomInfo(1, 1) },

            // Bottom / Tribe C
            { new Vector2Int(1, -3), new RoomInfo(2, 1) },
            { new Vector2Int(2, -3), new RoomInfo(1, 2) },
            { new Vector2Int(1, 2),  new RoomInfo(RoomType.Tribe, TribeType.TribeC) },

            // Boss
            { new Vector2Int(3, -3), new RoomInfo(RoomType.Boss) },
        };
}
