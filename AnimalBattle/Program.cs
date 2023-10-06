using GameFrame;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace AnimalBattle//总游戏界面
{
    class Program : ISystem
    {
        static ISystem system = new Program();
        int ISystem.Step { get; set; } = 0;
        Process ISystem.Process { get; set; }
        Map ISystem.Map { get; set; }
        Screen ISystem.Screen { get; set; }
        Camera ISystem.Camera { get; set; }

        /// <summary>
        /// 玩家队伍
        /// </summary>
        public Animal[] PlayerTeam;
        public Animal[] ShopAnimal;
        public Prop[] ShopProp;
        /// <summary>
        /// 回合数
        /// </summary>
        public int Turn;
        /// <summary>
        /// 玩家血量
        /// </summary>
        public int HP;
        /// <summary>
        /// 玩家奖杯数
        /// </summary>
        public int Trophy;
        /// <summary>
        /// 金币数
        /// </summary>
        public int Gold;

        public event AnimalEvent BattleStart;
        public event AnimalEvent BattleEnd;



        void ISystem.GlobalInit()
        {

        }
        void ISystem.LocalInit()
        {
            #region 玩家数值刷新
            PlayerTeam = new Animal[5];
            ShopAnimal = new Animal[2];
            ShopProp = new Prop[1];
            HP = 10;
            Trophy = 0;
            Gold = 10;
            Turn = 0;
            #endregion
        }

        public void GameLoop()
        {
            object step = 1;
            PlayerTeam = GetRandomsAnimals(5);
            while ((int)step >= 1)
            {
                StartTurn();
                var opg = GetOptionOfShop(step);//获取商店选项
                while ((int)step == 1)
                {
                    opg.Show();
                }
                EndTurn();
                Battle(GetRandomsAnimals(5));
            }
        }
        public OptionGroup GetOptionOfShop(object step)
        {
            var shopOG = new OptionGroup("商店：", "结束回合");
            shopOG["结束回合"].OptionEvent += () =>
              {
                  step = 1;
              };
            return shopOG;
        }
        /// <summary>
        /// 刷新商店
        /// </summary>
        public void RefreshShop()
        {
            ShopAnimal = GetRandomsAnimals(Turn);//刷新队伍

        }
        /// <summary>
        /// 购买商店成员
        /// </summary>
        /// <param name="shopIndex"></param>
        public void BuyAnimal(int shopIndex, int teamIndex)
        {
            try
            {
                if (ShopAnimal[shopIndex] == null) return;//判断该位置是否有动物
                var target = ShopAnimal[shopIndex];
                if (target.Price > Gold) return;//判断价格
                if (PlayerTeam[teamIndex] != null && PlayerTeam[teamIndex].GetType() != target.GetType()) return;//判断是否有空位

                Gold -= target.Price;
                ShopAnimal[shopIndex] = null;
                if (PlayerTeam[teamIndex].GetType() == target.GetType())//如果为同种动物则升级原位置动物
                    PlayerTeam[teamIndex].AddEx(1);
                else
                    PlayerTeam[teamIndex] = target;
                target.Init(PlayerTeam);//动物初始化
            }
            catch (IndexOutOfRangeException) { }
        }
        /// <summary>
        /// 卖出队伍中成员
        /// </summary>
        /// <param name="index"></param>
        public void SellAnimal(int index)
        {
            var animal = PlayerTeam[index];
            try
            {
                if (animal == null) return;
                animal.BeSold(PlayerTeam, ShopAnimal, null);//调用事件
                animal.Dispose(PlayerTeam);//析构函数
                animal = null;//移出该动物
                Gold += animal.Price / 2;//增加金币
            }
            catch (IndexOutOfRangeException) { }
        }
        /// <summary>
        /// 回合开始
        /// </summary>
        public void StartTurn()
        {
            Gold++;
            RefreshShop();
        }
        /// <summary>
        /// 结束回合
        /// </summary>
        public void EndTurn()
        {
            Turn++;
            SavePlayerTeam();
            Battle(ReadATeam());
        }
        /// <summary>
        /// 存储玩家队伍
        /// </summary>
        public void SavePlayerTeam()
        {
            var teamPath = @"D:\ConsoleGame\Team\" + Turn + "\\";//按当前回合数存储
            if (!Directory.Exists(teamPath)) Directory.CreateDirectory(teamPath);
            string filename;
            do { filename = Path.GetRandomFileName().Substring(0, 8); }//随机名称
            while (!File.Exists(teamPath + filename));//判断重复

            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream file = new FileStream(filename, FileMode.Create);
            formatter.Serialize(file, PlayerTeam);//存储
        }
        /// <summary>
        /// 从文件中读取一个队伍,用于战斗
        /// </summary>
        public Animal[] ReadATeam()
        {
            var teamPath = @"D:\ConsoleGame\Team\" + Turn + "\\";
            if (!Directory.Exists(teamPath)) return GetRandomsAnimals(5);//如果还未储存队伍则返回随机队伍；
            var teams = Directory.GetFiles(teamPath);
            var teamFullName = teams[new Random().Next(teams.Length)];//随机获取队伍

            var formatter = new BinaryFormatter();
            using FileStream teamFile = new FileStream(teamFullName, FileMode.Open);
            return (Animal[])formatter.Deserialize(teamFile);//读取
        }
        /// <summary>
        /// 返回指定成员数量的随机动物队伍
        /// </summary>
        /// <param name="nums">成员数量</param>
        /// <param name="maxPoints">最高点数</param>
        /// <returns></returns>
        public Animal[] GetRandomsAnimals(int nums = 5, int minPoints = 1, int maxPoints = 1)
        {
            Animal[] animals = new Animal[nums];
            Random r = new Random();
            for (int i = 0; i < nums; i++)
            {
                Animal newAnimal;//创建新动物
                do
                {
                    var animalType = Animal.AnimalTypes[r.Next(Animal.AnimalTypes.Length)];//从动物类型数组中随机抽取一个类型
                    newAnimal = (Animal)animalType.GetConstructors().First().Invoke(null);//调用该类型的构造函数生成一个新动物
                }
                while (newAnimal.Point <= maxPoints && newAnimal.Point >= minPoints);//判断是否满足点数
                animals[i] = newAnimal;//添加动物
            }
            return animals;
        }
        /// <summary>
        /// 复制目标动物队伍
        /// </summary>
        /// <param name="animalTeam">目标队伍</param>
        /// <returns></returns>
        public Animal[] CloneAnimals(Animal[] animalTeam)
        {
            var cloneTeam = new Animal[animalTeam.Length];
            for (int i = 0; i < cloneTeam.Length; i++)
            {
                if (animalTeam[i] != null)
                {
                    cloneTeam[i] = animalTeam[i].Clone();//复制
                    cloneTeam[i].Team = cloneTeam;//设置队伍
                }
            }
            return cloneTeam;
        }
        /// <summary>
        /// 战斗
        /// </summary>
        public void Battle(Animal[] opponent)
        {
            BattleStart?.Invoke(null, null, null);//战斗开始
            var battleTeam = CloneAnimals(PlayerTeam);//复制玩家队伍
            while (true)
            {
                Animal pAnimal = null, oAnimal = null;//玩家队伍前排，敌人队伍前排
                for (var i = 0; i < 5; i++)//寻找双方队伍前排
                {
                    if (battleTeam[i] != null && pAnimal == null) pAnimal = battleTeam[i];
                    if (opponent[i] != null && oAnimal == null) oAnimal = opponent[i];
                    if (pAnimal != null && oAnimal != null) break;
                }
                if (pAnimal == null || oAnimal == null) break;//判断战斗是否结束
                pAnimal.HurtBy(oAnimal);//相互攻击
                oAnimal.HurtBy(pAnimal);
            }
            BattleEnd?.Invoke(null, null, null);//战斗结束
            if (battleTeam.All(a => a == null) && !opponent.All(a => a == null))//失败
            {
                if (--HP <= 0) Settlement(false);
            }
            else if (!battleTeam.All(a => a == null) && opponent.All(a => a == null))//胜利
            {
                if (++Trophy >= 10) Settlement(true);
            }
        }
        /// <summary>
        /// 游戏结束结算
        /// </summary>
        /// <param name="IsWinner"></param>
        public void Settlement(bool IsWinner)
        {

        }
        static void Main(string[] args)
        {

        }
    }
}
