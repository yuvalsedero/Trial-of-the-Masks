using UnityEngine;
using System.Collections.Generic;

public static class WorldMap
{
    public static readonly Dictionary<Vector2Int, RoomInfo> Rooms =
        new Dictionary<Vector2Int, RoomInfo>
        {
            { new Vector2Int(0, 0), new RoomInfo(RoomType.Spawn) },

            { new Vector2Int(5, 5), new RoomInfo(1, 0) },
            { new Vector2Int(1, 1), new RoomInfo(1, 0) },
            { new Vector2Int(0, 2), new RoomInfo(2, 0) },

            { new Vector2Int(-1, 2), new RoomInfo(RoomType.Tribe, TribeType.TribeA) },

            { new Vector2Int(-1, 3), new RoomInfo(2, 0) },
            { new Vector2Int(-1, 4), new RoomInfo(2, 0) },
            { new Vector2Int(-1, 5), new RoomInfo(0, 1) },

            { new Vector2Int(-2, 5), new RoomInfo(1, 1) },
            { new Vector2Int(-3, 5), new RoomInfo(1, 2) },
            { new Vector2Int(-3, 4), new RoomInfo(0, 5) },

            { new Vector2Int(0, 5), new RoomInfo(1, 1) },
            { new Vector2Int(1, 5), new RoomInfo(2, 1) },

            { new Vector2Int(1, 6), new RoomInfo(RoomType.Tribe, TribeType.TribeB) },

            { new Vector2Int(2, 6), new RoomInfo(2, 1) },
            { new Vector2Int(3, 6), new RoomInfo(3, 0) },
            { new Vector2Int(3, 5), new RoomInfo(3, 1) },

            { new Vector2Int(3, 4), new RoomInfo(RoomType.Tribe, TribeType.TribeC) },

            { new Vector2Int(4, 4), new RoomInfo(2, 2) },
            { new Vector2Int(5, 4), new RoomInfo(3, 2) },

            { new Vector2Int(0, 1), new RoomInfo(RoomType.Boss) },
        };
}
