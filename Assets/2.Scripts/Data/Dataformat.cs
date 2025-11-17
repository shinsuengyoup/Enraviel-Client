namespace Dataformat
{
    public class DefaultStat
    {
        private int health;
        private int minAtk;
        private int maxAtk;
        private int actionPoint;
        private int def;
        private int range;
        private int breakDmg;

        public DefaultStat()
        {
            health = 10;
            minAtk = 1;
            maxAtk = 3;
            actionPoint = 10;
            def = 1;
            range = 3;
            breakDmg = 10;
        }
    }


    public enum TileType
    {
        None = 1,
        Wall = 2,
        Slow = 3,   // 해당 타일에서는 행동력 소모 n배
        Tick = 4,   // 해당 타일에 진입 or 정체 시 대미지
        Spawn = 5,
        SpawnEnemy = 6,
    }
}


