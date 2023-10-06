using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;
namespace Rogue_Remake
{
   static class BattleManager//用于管理战斗
    {
        public static void Battle(GameObject source,GameObject target)//处理玩家与怪兽的战斗
        {
            Attack(source,target);
            Settlement(source,target);
        }

        public static void Attack(GameObject source, GameObject target)//伤害处理
        {
            if(source is Monster)
            {
                var monster = (Monster)source;
                var player = (Player)target;
                Program.RemindInfo = $"{monster.Name}攻击了你";
                player.Hits.cur --;
            }
            else
            {
                var monster = (Monster)target;
                var player = (Player)source;
                Program.RemindInfo = $"你攻击了{monster.Name}";
                monster.HP.cur -= player.Strength.str;
                if(monster.HP.cur<1)
                {
                    monster.Died();
                    Program.RemindInfo = $"你击败了{monster.Name}";
                }

            }
        }

        public static void Settlement(GameObject source, GameObject target)//结算
        {

        }
    }
}
