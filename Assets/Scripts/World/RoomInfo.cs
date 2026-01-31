public class RoomInfo
{
    public RoomType roomType;
    public TribeType tribe;

    public int boarCount;
    public int monkeyCount;

    // Non-enemy rooms
    public RoomInfo(RoomType type, TribeType tribeType = TribeType.TribeA)
    {
        roomType = type;
        tribe = tribeType;
        boarCount = 0;
        monkeyCount = 0;
    }

    // Enemy rooms (KEEP YOUR NUMBERS)
    public RoomInfo(int boars, int monkeys)
    {
        roomType = RoomType.Enemy;
        tribe = TribeType.TribeA;
        boarCount = boars;
        monkeyCount = monkeys;
    }
}
