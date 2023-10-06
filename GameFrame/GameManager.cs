using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GameFrame
{
    public enum Language { English, Chinese };
    public static class GameManager
    {
        public static event Action GameOver;

        static OptionGroup op;
        public static OptionGroup SettingUI { get; private set; } = new OptionGroup("Setting", "Exit");//设置界面
        public static OptionGroup MainUI { get; private set; } = new OptionGroup("", "Start", "Setting", "Help", "Exit");//主界面选项
        public static Language Language { get; set; } = Language.English;

        /// <summary>
        /// 添加设置选项
        /// </summary>
        /// <param name="title"></param>
        public static void AddSettingOp(params string[] optionTitles)
        {
            op = SettingUI;
            foreach (var optionTitle in optionTitles)
            {
                OptionGroup.Option addOption = new OptionGroup.Option(optionTitle);
                int count = op.Count;
                op.Add(op[count - 1]);//将退出键放在最后
                op[count - 1] = addOption;//将添加的选项插入
            }
        }
        /// <summary>
        /// 改变游戏标题
        /// </summary>
        /// <param name="title"></param>
        public static void SetGameTitle(string title)
        {
            MainUI.Title = title;
        }
        /// <summary>
        /// 改变选项“Help”的内容
        /// </summary>
        /// <param name="introduce"></param>
        public static void SetHelp(string introduce)
        {
            MainUI[2].OptionEvent += () => { Console.WriteLine(introduce); Console.ReadKey(); };
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="system"></param>
        static void Init(ISystem system)
        {
            switch (Language)//根据语言显示
            {
                case Language.English:
                    MainUI = new OptionGroup("", "Start", "Setting", "Help", "Exit");//主界面选项
                    SettingUI = new OptionGroup("Setting", "Exit");//设置界面
                    break;
                case Language.Chinese:
                    MainUI = new OptionGroup("", "开始", "设置", "帮助", "退出");
                    SettingUI = new OptionGroup("设置", "退出");
                    break;
            }

            #region 选项类一 主界面
            MainUI[0].OptionEvent += () =>
                {
                    system.Step = 2;
                    Console.Clear();
                };
            MainUI[1].OptionEvent += () =>
            {
                system.Step = 1;
            };
            MainUI[3].OptionEvent += () =>
            {
                system.Step = -1;
                Destruct();
            };
            #endregion
            #region 选项类二 设置       
            SettingUI[SettingUI.Count - 1].OptionEvent += () =>
              {
                  system.Step = 0;
              };
            #endregion
        }
        /// <summary>
        /// 析构,在退出游戏时使用
        /// </summary>
        static void Destruct()
        {
            Language = default;
        }
       
        /// <summary>
        /// 游戏主流程
        /// </summary>
        /// <param name="system"></param>
        /// <param name="language"></param>
        public static void GameStart(ISystem system, Language language = Language.English)
        {
            Language = language;
            string optionRemind = language == Language.Chinese ? "按 \"↑，↓\" 键来移动光标，\"Enter\"键确定\nMade By CGL" : "Press \"↑，↓\" to select option,\"Entry\" to ensure \nMade By CGL";

            Init(system);//初始化默认选项组
            system.Map = new Map();
            system.Screen = new Screen();
            system.Camera = new Camera(null, FllowMode.NoFllow, false, system.Map, system.Screen);
            system.Step = 0;
            system.Process = new Process();
            Move<GameObject>.MoveInit();
            system.GlobalInit();//初始化游戏
            Console.Title = MainUI.Title;
            while (system.Step >= 0)//游戏运行中
            {
                while (system.Step == 0)//主界面
                {
                    MainUI.Show(help: optionRemind);
                }
                while (system.Step == 1)//设置界面
                {
                    SettingUI.Show(help: optionRemind);
                }
                if (system.Step == 2)//游戏开始时初始化游戏内数据
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    system.LocalInit();
                    while (system.Step == 2)//游戏进行
                    {
                        system.Process.Proceed(sw/*system.Begin*/); //进程
                    }
                    if (system.Step >= 0)
                    {
                        GameOver?.Invoke();//游戏结束事件
                        while (!system.Process.IsCompleted) ;//等待进程完全结束

                        Console.SetCursorPosition(0, 0);//重置控制台
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
