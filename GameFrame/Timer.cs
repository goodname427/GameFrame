using System;

namespace GameFrame
{
    public class Timer//计时器
    {
        public delegate void dele();
        public event dele Update;//每一个时间间隔执行一次

        public int LastTime { get; set; }//已经执行过的时间
        public int Interval { get; set; }//更新间隔
        public bool Enabled { get; set; }//是否打开

        public Timer(int interval) { Interval = interval; Enabled = true; }

        /// <summary>
        /// 重置事件
        /// </summary>
        public void ResetUpdate()
        {
            Update = null;
        }
        /// <summary>
        /// 执行计时器
        /// </summary>
        public void Execute()
        {
            if (Enabled) Update?.Invoke();
        }

    }
    [Obsolete]
    public class Timing : Timer//定时器
    {
        DateTime End;//定时器结束时间
        DateTime Now;//目前时间
        Process Process;//所使用的进程
        public Timing(int interval, Process process) : base(interval)
        {
            Process = process;
            process.Update += Countdown;
        }
        public void Countdown()
        {
            Now = DateTime.Now;
            if (Now >= End)//倒计时结束
            {
                Execute();
                Enabled = false;//关闭定时器
            }
        }
        public void Start()//启用定时器
        {
            Enabled = true;
            End = DateTime.Now.AddSeconds(Interval / 1000);//记录结束时间

        }
        public void Start(int newInterval)//使用设定的时间间隔
        {
            Enabled = true;
            End = DateTime.Now.AddSeconds(newInterval / 1000);//记录结束时间
        }
        public void Close()//关闭计时器
        {
            Enabled = false;
        }
        public void Exit()//退出计时器
        {
            Process.Update -= Countdown;
        }
    }

}
