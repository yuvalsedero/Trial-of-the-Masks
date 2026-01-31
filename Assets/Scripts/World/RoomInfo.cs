public class RoomInfo
{
    public RoomType roomType;
    public TribeType tribe;

    // 👇 NEW: enemy composition
    public int boarCount;
    public int monkeyCount;

    // Default (non-enemy rooms)
    public RoomInfo(RoomType type, TribeType tribeType = TribeType.TribeA)
    {
        roomType = type;
        tribe = tribeType;
        boarCount = 0;
        monkeyCount = 0;
    }

    // 👇 Enemy room constructor
    public RoomInfo(int boars, int monkeys)
    {
        roomType = RoomType.Enemy;
        tribe = TribeType.TribeA;
        boarCount = boars;
        monkeyCount = monkeys;
    }
}
