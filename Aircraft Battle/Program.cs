using System;
using System.Collections.Generic;
using GameFrame;
using System.Threading.Tasks;

namespace Aircraft_Battle
{
    public class Program : ISystem
    {
        public static ISystem system = new Program();
        Process ISystem.Process { get; set; } 
        int ISystem.Step { get; set; } = 0;
        Map ISystem.Map { get; set; }
        Screen ISystem.Screen { get; set; }
        Camera ISystem.Camera { get; set; }

        static Timer timer1 = new Timer(1000);//敌机刷新
        static Timer timer2 = new Timer(1200);//敌机移动   
        static Timer timer3 = new Timer(1500);//敌机射击          
        static Timer sp1 = new Timer(1000);//子弹射速 1档=1格/秒         
        static Timer sp2 = new Timer(500);//子弹射速          
        static Timer sp3 = new Timer(333);//子弹射速       
        static Timer sp4 = new Timer(250);//子弹射速         
        static Timer sp5 = new Timer(200);//子弹射速
        public static Timer[] bltSpdTmr = { null, sp1, sp2, sp3, sp4, sp5 };//子弹射速集                                   

        PlayerAircraft Player ;//玩家
        public static Aircraft[] Aircrafts = new Aircraft[100];//游戏内所有飞机
        public static Bullet[] Bullets = new Bullet[2500];//游戏内所有子弹

        public static int AIndex { get => Array.IndexOf(Aircrafts, null); }//访问空位
        public static int BIndex { get => Array.IndexOf(Bullets, null); }

        void ISystem.GlobalInit()//全局初始化
        {
            system.Map = new Map();
            system.Process=new Process(timer1, timer2, timer3, sp1, sp2, sp3, sp4, sp5);
            Player = new PlayerAircraft();
            timer1.Update += () =>//刷新
              {
                  int index = Array.IndexOf(Aircrafts, null);
                  if (index != -1)
                  { Aircrafts.SetValue(new OrdinaryFighter(), index); Aircrafts[index].Place(); }
              };
            timer2.Update += () =>//移动
              {
                  int max = AIndex;
                  for (int i = 1; i < max; i++)
                  {
                      Move<Aircraft>.AutoMove(Aircrafts[i], isCanOutMap: true);
                      if (Aircrafts[i].IsOutMap()) Aircrafts[i].Die();
                  }
              };
            timer3.Update += () =>//射击 
              {
                  for (int i = 1; i < 100; i++)
                  {
                      if (Aircrafts[i] != null) Aircrafts[i].Shoot();
                  }
              };
            system.Process.Update += () =>
            {
                Task.Run(() => Move<PlayerAircraft>.ManualMove(Player));
                // Player.Shoot();                
                system.Map.Update();

            };

            Move<PlayerAircraft>.ManualMoveP += (ConsoleKey? button, PlayerAircraft GB) =>
                  {
                      if (button == ConsoleKey.J) Player.Shoot();
                  };

            GameManager.SetGameTitle("Aircraft Battle");
            GameManager.SetHelp("Press \"W\" \"A\" \"S\" \"D\" to move\nPress \"Esc\" to Leave\nPress \"J\" to shoot");
            #region 选项类二 设置
            GameManager.SettingUI[0].OptionEvent += () =>
            {
                //system.SetData(ref Speed);
                //timer1.interval = (11 - Speed) * 50;
            };           
            #endregion 

            system.Step = 0;
        }
        void ISystem.LocalInit()//局部初始化
        {
            Player = new PlayerAircraft();//初始化玩家
            Aircrafts.SetValue(Player, 0);
            Player.Place();
            system.Map.Init(21,21);
        }
        public static void Main()
        {
            GameManager.GameStart(system);
        }
    }
}
