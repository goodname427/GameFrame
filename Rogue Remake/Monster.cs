using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;
using System.Threading.Tasks;

namespace Rogue_Remake
{
    /// <summary>
    /// 怪物的标签
    /// </summary>
    public enum Flag { Mean, Flying, Regeneration, Greedy, Invisible }//效用：卑鄙的怪物可以在没有挑衅的情况下攻击；飞行怪物更难击中；回血怪物可以恢复生命值；贪婪的怪物试图在你进入房间时捡起金子；隐身怪物看不见
    public enum MonsterStatus { Sleeping, Aggressive, Standing };
    public partial class Monster : GameObject
    {
        public delegate void Skill();
        public string Name { get; }//怪物姓名
        public int Treasure { get; }//怪物携带宝藏的几率
        public Flag[] Flags { get; }//怪物标签
        public int Exp { get; }//杀死怪物所获得的经验
        public (int cur, int max) HP;//怪物血量
        public int AC { get; }//怪物护甲，护甲越低，越难击中
        public (int min, int max) DamageRange { get; }//伤害范围
        public event Skill SpecialSkill;//特殊技能

        (int min, int max) HPRange;//生成血量的范围
        (int min, int max)[] DamageRanges;//生成的伤害范围

        Pathfinder pathFinder;
        public MonsterStatus status = MonsterStatus.Standing;

        //MoveObj<Monster> moveObj = new MoveObj<Monster>();

        Monster(string name, int treasure, int exp, (int min, int max) hpRange, int ac, params Flag[] flags) : base(-1, -1, '\0', null)//生成模板
        {
            Name = name;
            Treasure = treasure;
            Exp = exp;
            HPRange = hpRange;
            AC = ac;
            Flags = flags;
        }
        void SetDamage(params (int min, int max)[] damageRanges)
        {
            DamageRanges = damageRanges;
        }//设置生成的伤害范围

        Monster(string name, int x, int y, Map map, int treasure, int exp, int hp, int ac, (int min, int max) damageRange, params Flag[] flags) : base(x, y, name[0], map)//生成实例 
        {
            Name = name;
            Treasure = treasure;
            Exp = exp;
            HP = (hp, hp);
            AC = ac;
            DamageRange = damageRange;
            Flags = flags;
            pathFinder = new Pathfinder(Program.player, (x, y), Program.System.Screen, PathFindDirect.EightDirect, default, Floor.IWall, Program.player.Image, '\0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z');//初始化自动寻路器
            pathFinder.Preferences.Add(Floor.IDoor);
            pathFinder.Preferences.Add(Floor.IRoad);
            //MoveInit();//初始化移动参数
        }
        public static Monster Summon(int x, int y, Map map, Monster tempMonster)//按照模板生成一个怪物
        {
            Random r = new Random();
            var monster = new Monster(tempMonster.Name, x, y, map, tempMonster.Treasure, tempMonster.Exp, r.Next(tempMonster.HPRange.min, tempMonster.HPRange.max), tempMonster.AC, tempMonster.DamageRanges[r.Next(tempMonster.DamageRanges.Length)], tempMonster.Flags);
            return monster;
        }
        #region 移动
        public async Task Move()//怪物移动
        {
            Explore();
            if (status == MonsterStatus.Aggressive)//如果怪物处于极具攻击性时会追击玩家
            {
                await pathFinder.Find(false);//使用A星算法计算路径
                Program.mutex.WaitOne();//排他锁，与玩家确定位置
                if (pathFinder.CurPos == Position && pathFinder.IsAround(Program.player.Position, out _)) Attack();//攻击
                if (pathFinder.CurPos != Program.player.Position)
                {
                    Position = pathFinder.CurPos;
                }
                Program.mutex.ReleaseMutex();
            }
            else
            {

            }
        }
        public void Explore()//发现周围是否有玩家
        {
            if (status == MonsterStatus.Standing && GetDistance(Program.player.Position) < 10)//当怪物属于静置状态时并且当玩家靠近时会进入攻击状态
            {
                status = MonsterStatus.Aggressive;
            }
        }
        #endregion
        #region 战斗
        void Attack()
        {
            BattleManager.Attack(this, Program.player);
        }
        public void Died()
        {
            int index = Array.IndexOf(Program.Build[Program.layer].Monsters, this);
            Program.Build[Program.layer].Monsters.SetValue(null, index);
            Program.System.Screen.RemoveRendObj(this, 1);
        }
        #endregion
    }
    public partial class Monster//储存模板
    {
        public static void MonsterInitialize()//怪物信息初始化
        {
            Aquator = new Monster("Aquator", 0, 20, (5, 8), 2, Flag.Mean);
            Aquator.SetDamage((0, 0));
            Bat = new Monster("Bat", 0, 1, (1, 8), 3, Flag.Flying);
            Bat.SetDamage((1, 2));
            Centaur = new Monster("Centaur", 15, 17, (4, 8), 4);
            Centaur.SetDamage((1, 2), (1, 5), (1, 5));
            Dragon = new Monster("Dragon", 100, 5000, (8, 10), -1);
            Dragon.SetDamage((1, 8), (1, 8), (3, 10));
            Emu = new Monster("Emu", 0, 2, (1, 8), 7, Flag.Mean);
            Emu.SetDamage((1, 2));
            Griffin = new Monster("Griffin", 20, 2000, (8, 13), 2, Flag.Mean, Flag.Flying, Flag.Regeneration);
            Griffin.SetDamage((3, 4), (3, 5));
            Hobgoblin = new Monster("Hobgoblin", 0, 3, (1, 8), 5, Flag.Mean);
            Hobgoblin.SetDamage((1, 8));
            IceMonster = new Monster("Ice Monster", 0, 5, (1, 8), 9);
            IceMonster.SetDamage((0, 0));
            Jabberwock = new Monster("Jabberwock", 70, 3000, (8, 15), 6);
            Jabberwock.SetDamage((2, 12), (2, 4));
            Kestrel = new Monster("Kestrel", 0, 1, (1, 8), 7, Flag.Mean, Flag.Flying);
            Kestrel.SetDamage((1, 4));
            Orc = new Monster("Orc", 15, 5, (1, 8), 6, Flag.Greedy);
            Orc.SetDamage((1, 8));
            Snake = new Monster("Snake", 0, 2, (1, 8), 5, Flag.Mean);
            Snake.SetDamage((1, 3));

            AllMonsters = new Monster[] { Bat, Kestrel, Emu, Snake, Hobgoblin, IceMonster, Orc, Zombie, Rattlesnake, Leprechaun, Quagga, Centaur, Aquator, Nymph, Yeti, Wraith, FlytrapVenus, Xeroc, Phantom, Troll, UrVile, Medusa, Vampire, Griffin, Jabberwock, Dragon };
        }
        #region 怪物模板
        public static Monster Aquator;
        public static Monster Bat;
        public static Monster Centaur;
        public static Monster Dragon;
        public static Monster Emu;
        public static Monster FlytrapVenus;
        public static Monster Griffin;
        public static Monster Hobgoblin;
        public static Monster IceMonster;
        public static Monster Jabberwock;
        public static Monster Kestrel;
        public static Monster Leprechaun;
        public static Monster Medusa;
        public static Monster Nymph;
        public static Monster Orc;
        public static Monster Phantom;
        public static Monster Quagga;
        public static Monster Rattlesnake;
        public static Monster Snake;
        public static Monster Troll;
        public static Monster UrVile;
        public static Monster Vampire;
        public static Monster Wraith;
        public static Monster Xeroc;
        public static Monster Yeti;
        public static Monster Zombie;
        //所有怪兽，越靠后越强大
        public static Monster[] AllMonsters;
        #endregion
    }

}
