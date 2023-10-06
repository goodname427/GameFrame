using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalBattle
{
    public delegate void AnimalEvent(Animal[] senders, Animal[] targets, object arg);
    public abstract partial class Animal
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get => description; }
        /// <summary>
        /// 血量
        /// </summary>
        public int HP
        {
            get => hp;
            set
            {
                if (value <= 50) hp = value;
                else hp = 50;
            }
        }
        /// <summary>
        /// 伤害,上限为50
        /// </summary>
        public int Damage
        {
            get => damage; set
            {
                if (value <= 50) damage = value;
                else damage = 50;
            }
        }
        /// <summary>
        /// 价格,上限为50
        /// </summary>
        public int Price { get; private set; }
        /// <summary>
        /// 级别
        /// </summary>
        public int Point { get; private set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; private set; }
        /// <summary>
        /// 经验条
        /// </summary>
        public (int cur, int max) ExBar { get; private set; }
        /// <summary>
        /// 队伍
        /// </summary>
        public Animal[] Team { get; set; }
        /// <summary>
        /// 携带的道具
        /// </summary>
        public Prop Prop
        {
            get => prop;
            set
            {
                value.Init(this);
                if (!prop.IsSingeUse)//判断道具是否是一次性道具
                    prop = value;
            }
        }

        protected int hp;
        protected int damage;
        protected int maxLevel;//最大等级
        protected object skillArg;//技能参数
        protected Prop prop;
        protected string name;
        protected string description;

        public event AnimalEvent Hurt;
        public event AnimalEvent LevelUp;
        public event AnimalEvent Sell;
        public event AnimalEvent InvokeSkill;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="Team"></param>
        public virtual void Init(Animal[] team)
        {
            Team = team;
        }
        /// <summary>
        /// 解构
        /// </summary>
        /// <param name="team"></param>
        public virtual void Dispose(Animal[] team)
        {
            Team = null;
        }
        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="num"></param>
        public void AddEx(int num)
        {
            if (Level == maxLevel) return;
            ExBar = (ExBar.cur + num, ExBar.max);
            while (ExBar.cur > ExBar.max)
            {
                ExBar = (ExBar.cur - ExBar.max, ExBar.max);
                if (Level == maxLevel) return;//判断是否到达等级上限
                Upgrade();//升级
            }
        }
        /// <summary>
        /// 升级
        /// </summary>
        protected virtual void Upgrade()
        {
            LevelUp?.Invoke(new Animal[] { this }, null, null);//升级事件
            Level++;
        }
        /// <summary>
        /// 技能
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="arg"></param>
        public virtual void Skill(Animal[] senders, Animal[] targets, object arg)
        {
            InvokeSkill?.Invoke(senders, targets, arg);
        }
        /// <summary>
        /// 复制该动物
        /// </summary>
        /// <returns></returns>
        public Animal Clone()
        {
            var animal = (Animal)MemberwiseClone();
            animal.prop = prop.Clone();
            animal.prop.Ownner = animal;
            return animal;
        }
        /// <summary>
        /// 被伤害
        /// </summary>
        /// <param name="target">伤害来源</param>
        public void HurtBy(Animal target)
        {
            Hurt?.Invoke(new Animal[] { this }, new Animal[] { target }, null);
            hp -= target.damage;
        }
        //调用售卖事件
        public void BeSold(Animal[] senders, Animal[] targets, object arg) { Sell?.Invoke(senders, targets, arg); }
    }
    public abstract class Prop
    {
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get => price; }
        /// <summary>
        /// 是否是一次性道具
        /// </summary>
        public bool IsSingeUse { get => isSingeUse; }
        /// <summary>
        /// 道具主人
        /// </summary>
        public Animal Ownner { get; set; }

        protected int price;
        protected bool isSingeUse;
        /// <summary>
        /// 道具效果
        /// </summary>
        /// <param name="targets">作用目标</param>
        /// <param name="arg">参数</param>
        public abstract void Effect(Animal[] targets);
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="targets">作用目标</param>
        public abstract void Init(Animal targets);

        public Prop Clone()
        {
            return (Prop)MemberwiseClone();
        }
    }
}
