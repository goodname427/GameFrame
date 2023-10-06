using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;

namespace AnimalBattle
{
    public partial class Animal
    {
        /// <summary>
        /// 所有动物种类
        /// </summary>
        public static Type[] AnimalTypes =
        {
            typeof(Fish)
        };
        public class Example : Animal
        {
            public Example()
            {
                hp = 1;
                damage = 1;
                ExBar = (0, 2);
                maxLevel = 3;
                Price = 1;
                skillArg = new int[] { 1, 1 };
                Point = 1;
                name = "example";
                description = @"this is example.
                    it don't have any skill";
            }
            public override void Init(Animal[] team)
            {
                base.Init(team);
            }
            public override void Dispose(Animal[] team)
            {
                base.Dispose(team);
            }
            protected override void Upgrade()
            {
                base.Upgrade();
            }

            public override void Skill(Animal[] senders, Animal[] targets, object arg)
            {

            }
        }
        public class Fish : Animal
        {
            public Fish()
            {
                hp = 1;
                damage = 1;
                ExBar = (0, 2);
                maxLevel = 3;
                Price = 2;
                skillArg = new int[] { 1, 1 };
                Point = 1;
                name = "鱼";
                description = "";
            }

            public override void Init(Animal[] team)
            {
                base.Init(team);
                LevelUp += Skill;
            }
            public override void Dispose(Animal[] team)
            {
                base.Dispose(team);
                LevelUp -= Skill;
            }
            protected override void Upgrade()
            {
                base.Upgrade();
                skillArg = new int[] { Level, Level };
            }

            public override void Skill(Animal[] senders, Animal[] targets, object arg)
            {
                base.Skill(senders, targets, arg);
                int[] args = (int[])skillArg;
                foreach (var friend in Team)
                {
                    if (friend != this)
                    {
                        friend.HP += args[0];
                        friend.Damage += args[1];
                    }
                }
            }
        }
    }
}
