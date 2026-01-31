using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMapManager : MonoBehaviour
{
    private HashSet<Vector2Int> revealedRooms = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> visitedRooms = new HashSet<Vector2Int>();

    public GameObject roomIconPrefab;
    private static readonly Color hiddenColor = new Color(0.15f, 0.15f, 0.15f);

    public Vector2 roomSpacing = new Vector2(16, 16);

    private Dictionary<Vector2Int, Image> icons =
        new Dictionary<Vector2Int, Image>();

    void Start()
    {
        GenerateMap();
        UpdateCurrentRoom(Vector2Int.zero);
    }

    void GenerateMap()
    {
        // 1. Find bounds
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var coord in WorldMap.Rooms.Keys)
        {
            if (coord.x < minX) minX = coord.x;
            if (coord.x > maxX) maxX = coord.x;
            if (coord.y < minY) minY = coord.y;
            if (coord.y > maxY) maxY = coord.y;
        }

        // 2. Compute map center
        Vector2 mapCenter = new Vector2(
            (minX + maxX) * 0.5f,
            (minY + maxY) * 0.5f
        );

        // 3. Create icons centered
        foreach (var kvp in WorldMap.Rooms)
        {
            Vector2Int coord = kvp.Key;

            GameObject icon = Instantiate(roomIconPrefab, transform);
            Image img = icon.GetComponent<Image>();

            Vector2 centeredCoord = (Vector2)coord - mapCenter;

            Vector2 pos = new Vector2(
                centeredCoord.x * roomSpacing.x,
                centeredCoord.y * roomSpacing.y
            );

            icon.GetComponent<RectTransform>().anchoredPosition = pos;

            img.color = hiddenColor;
            icons.Add(coord, img);
        }
    }
    public void RevealAdjacentRooms(Vector2Int coord)
    {
        RevealRoom(coord);

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = coord + dir;
            if (WorldMap.Rooms.ContainsKey(neighbor))
            {
                RevealRoom(neighbor);
            }
        }
    }

    public void RevealRoom(Vector2Int coord)
    {
        if (!WorldMap.Rooms.ContainsKey(coord))
            return;

        if (revealedRooms.Contains(coord))
            return;

        revealedRooms.Add(coord);

        Image img = icons[coord];
        img.color = GetRoomColor(coord);
    }

    public void UpdateCurrentRoom(Vector2Int room)
    {
        // Mark room as visited
        visitedRooms.Add(room);

        // Reveal current + adjacent
        RevealAdjacentRooms(room);

        foreach (var kvp in icons)
        {
            Vector2Int coord = kvp.Key;
            Image img = kvp.Value;

            // Hidden rooms
            if (!revealedRooms.Contains(coord))
            {
                img.color = hiddenColor;
                continue;
            }

            Color baseColor = GetRoomColor(coord);

            // Current room
            if (coord == room)
            {
                img.color = Color.white;
            }
            // Visited before
            else if (visitedRooms.Contains(coord))
            {
                img.color = LightenColor(baseColor);
            }
            // Revealed but not visited
            else
            {
                img.color = baseColor;
            }
        }
    }

    Color LightenColor(Color c, float amount = 0.5f)
    {
        return Color.Lerp(c, Color.white, amount);
    }
    Color GetRoomColor(Vector2Int coord)
    {
        RoomInfo info = WorldMap.Rooms[coord];

        return info.roomType switch
        {
            RoomType.Spawn => new Color(0.94f, 0.63f, 0.33f),
            RoomType.Enemy => Color.red,
            RoomType.Tribe => info.tribe switch
            {
                TribeType.TribeA => Color.green,
                TribeType.TribeB => new Color(0.6f, 0f, 0.8f),
                TribeType.TribeC => Color.blue,
                _ => Color.white
            },
            RoomType.Boss => Color.yellow,
            _ => Color.lightGray
        };
    }

}
