using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace GameFrame
{
    public class Process//进程类
    {
        public delegate void dele();
        public event dele Update;//高频执行
        event dele Control;//控制进程

        Task timerTask;//记录当前正在执行的计时器任务
        Task processTask;//记录当前正在执行的进程任务

        /// <summary>
        /// 计时器列表
        /// </summary>
        List<Timer> Timers { get; } = new List<Timer>();
        /// <summary>
        /// 进程已经启动的时间
        /// </summary>
        public int Elapsed { get; private set; }
        /// <summary>
        /// 记录当前进程是否执行完所有任务
        /// </summary>
        public bool IsCompleted { get => timerTask.IsCompleted && processTask.IsCompleted; }

        public Process(params Timer[] Timers) { foreach (var timer in Timers) this.Timers.Add(timer); }//构造函数

        /// <summary>
        /// 添加计时器
        /// </summary>
        /// <param name="timers">计时器</param>
        public void Add(params Timer[] timers)
        {
            if (timers != null)
            {
                foreach (var timer in timers)
                    Timers.Add(timer);
            }
        }
        /// <summary>
        /// 去除计时器
        /// </summary>
        /// <param name="timers">计时器</param>
        public void Remove(params Timer[] timers)
        {
            if (timers != null)
            {
                foreach (var timer in timers)
                    Timers.Remove(timer);
            }
        }
        /// <summary>
        /// 重置进程事件
        /// </summary>
        public void ResetUpdate()
        {
            Update = null;
        }
        /// <summary>
        /// 重置所有计时器事件
        /// </summary>
        public void ResetTimers()
        {
            foreach (var timer in Timers)
                timer.ResetUpdate();
        }
        /// <summary>
        /// 重置进程
        /// </summary>
        public void Reset()
        {
            ResetTimers();
            ResetUpdate();
        }
        /// <summary>
        /// 阻塞进程指定毫秒
        /// </summary>
        /// <param name="interval"></param>
        public void Stop(int interval)
        {
            void StopEvent() { Task.Delay(interval); Control -= StopEvent; }//建立一个暂停函数，并放入控制事件中
            Control += StopEvent;
        }
        /// <summary>
        /// 进程进行
        /// </summary>
        /// <param name="begin">计时器</param>
        public void Proceed(Stopwatch begin)
        {
            Control?.Invoke();
            Elapsed = (int)begin.Elapsed.TotalMilliseconds;
            if (Timers != null)
            {
                foreach (var timer in Timers)
                {
                    if (timer != null && Elapsed % timer.Interval == 0 && timer.LastTime != Elapsed/*储存器*/)//检测是否经过了指定间隔时间
                    {
                        timerTask = Task.Run(() => timer.Execute());
                        timer.LastTime = Elapsed;//储存已经执行的时间,避免同一时间多次执行
                    }
                }
            }
            if (Elapsed % 10 == 0 && (processTask == null || processTask.IsCompleted)) processTask = Task.Run(() => Update?.Invoke());
        }
    }
}
