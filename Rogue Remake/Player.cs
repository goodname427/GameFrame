using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;
using System.Threading;
namespace Rogue_Remake
{
    public enum Fighter//战斗等级
    {
        NoStatus, Apprentice, JourneyMan, Adventurer, Fighter, Warrior
    }
    class Player : GameObject
    {
        public int Level;//等级
        public (int cur, int max) Hits;//伤害（当前/最大）
        public (int str, int mag) Strength;//力量（当前/最大）
        public int Gold;//金币
        public int Armor;//护甲
        public (int cur, int max) Exp;//玩家当前的经验
        public Fighter Fighter;//战斗地位？职业?也许是
        //玩家信息
        public string PlayerInfo { get => $" 等级:{Level} 血量:{Hits.cur}({Hits.max}) 力量:{Strength.str}({Strength.mag}) 护甲:{Armor} 经验:{Exp.cur}/{Exp.max}"; }

        public Movement moveObj = new Movement();
        public Player(Map map) : base(0, 0, '@', map)
        {
            Level = 1;
            Hits = (12, 12);
            Strength = (16, 16);
            Gold = 0;
            Armor = 5;
            Fighter = Fighter.NoStatus;
            Exp = (0, 8);
            MoveInit();
        }
        #region 移动模块
        public void MoveInit()
        {
            moveObj.Init(this);
            moveObj.Barriers.AddRange(new char[] { Floor.IWall, '\0', '?', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' });
            moveObj.SetParam(false, false, false);
            moveObj.Moved += (button, GB) =>
            {
                Program.Build[Program.layer].ChangeFogs(this);
                //Program.MonsterAct();
                if (button == ConsoleKey.F && GB.Map[GB.Position] == Floor.IStair)
                {
                    Program.layer++;
                    Program.System.Screen.ResetReRendChar();
                    Program.Build.CreatStair();
                    Program.ChangeMap(Program.layer);
                }
                if (button == ConsoleKey.E)//作弊功能，向前跳跃两格
                {
                    Position = Move<GameObject>.DirectMove(Position, Direction);
                    Position = Move<GameObject>.DirectMove(Position, Direction);
                }
                Program.mutex.ReleaseMutex();//释放
                if (button == ConsoleKey.Q)
                {
                    Program.System.Screen.Pause();//暂停屏幕
                    Program.testOp1.Show();
                }

            };
            moveObj.Moving += (button, GB) =>
              {
                  Program.mutex.WaitOne();//互斥锁，防止怪物不与玩家交错
                  Program.Build[Program.layer].ResumeRoadFog(this);
              };
            moveObj.OnTouch += (barrier, Pos) =>
              {
                  if (barrier <= 'Z' && barrier >= 'A')
                  {
                      Monster monster = null;
                      foreach (var m in Program.Build[Program.layer].Monsters)
                      {
                          if (m != null && m.Position == Pos) { monster = m; break; }
                      }
                      if (monster != null)
                      {
                          BattleManager.Attack(this, monster);
                      }
                  }
              };
        }
        public void Move()
        {
            moveObj.ManualMoveS();
        }
        #endregion
        #region 战斗模块

        #endregion
    }
}
