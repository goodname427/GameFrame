using System;
using System.Collections.Generic;
using System.Text;

namespace GameFrame
{
    public class OptionGroup//选项组类
    {
        int begin;//记录选项开始的行数
        int end;//记录选项结束的行数
        int selector;//选中的选项
        bool isConfirmed = false;//是否确认

        List<Option> Options { get; } = new List<Option>();//选项组内的所有选项
        /// <summary>
        /// 选项组的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 帮助信息
        /// </summary>
        public string Help { get; set; }
        /// <summary>
        /// 选项组长度
        /// </summary>
        public int Count { get => Options.Count; }

        public Option this[int index]
        {
            get
            {
                if (index < Options.Count && index > -1) return Options[index];
                else return null;
            }
            set
            {
                if (value != null && index < Options.Count && index > -1) Options[index] = value;
            }
        }

        public Option this[string optionTitle]
        {
            get
            {
                foreach (var option in Options)
                {
                    if (option.Title == optionTitle) return option;
                }
                return null;
            }
        }

        /// <summary>
        /// 构建一个新的选项组
        /// </summary>
        /// <param name="title">选项组标题</param>
        /// <param name="options">选项标题</param>
        public OptionGroup(string title, params string[] options)
        {
            this.Title = title;
            foreach (var option in options)
                Options.Add(new Option(option));
        }

        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="addOption">选项</param>
        public void Add(Option addOption)
        {
            Options.Add(addOption);
        }
        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="addOption">选项标题</param>
        public void Add(string addOption)
        {
            Options.Add(new Option(addOption));
        }
        /// <summary>
        /// 寻找函数
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public int IndexOf(Option option)
        {
            return Options.IndexOf(option);
        }

        /// <summary>
        /// 显示该选项组
        /// </summary>
        /// <param name="isClearConsole">是否清除屏幕</param>
        /// <param name="help">帮助信息</param>
        public void Show(string help, bool isClearConsole = true)
        {
            if (Options.Count < 1) return;

            isConfirmed = false;
            if (isClearConsole) Console.Clear();//是否清除窗体,默认清除
            Console.WriteLine(Title);//显示标题
            begin = Console.CursorTop;//记录开始的行数
            end = begin + Options.Count - 1;//记录结束的行数
            selector = begin;//选中第一个选项
            for (int i = 0; i < Options.Count; i++)
                Console.WriteLine($"{Options[i].Title}");//显示选项
            Console.CursorTop = Console.WindowHeight - 3;//在最下方显示提示
            Console.Write(help);
            Console.CursorVisible = false;//光标隐藏
            HighLighted();//高亮显示第一个选项
            while (!isConfirmed)
            {
                MoveSelector();//光标移动
            }

        }
        /// <summary>
        /// 显示该选项组
        /// </summary>
        /// <param name="isClearConsole">是否清除屏幕</param>
        public void Show(bool isClearConsole = true)
        {
            Show(Help, isClearConsole);
        }
        /// <summary>
        /// 移动选择器 
        /// </summary>
        void MoveSelector()
        {

            ConsoleKey? button = ISystem.KeyPress();
            if (button == ConsoleKey.UpArrow)//上箭头
            {
                HighLighted(true);//取消高亮
                selector--;//选择项下移
                if (selector < begin) selector += Count;//循环
                HighLighted();//高亮显示
            }
            else if (button == ConsoleKey.DownArrow)//下箭头
            {
                HighLighted(true);
                selector++;
                if (selector > end) selector -= Count;
                HighLighted();
            }
            else if (button == ConsoleKey.Enter)//确认键
            {
                isConfirmed = true;
                int index = selector - begin;//记录当前选项
                Console.CursorVisible = true;
                Console.CursorTop = end + 1;
                Options[index].Execute();//执行该选项
            }
        }
        /// <summary>
        /// 高亮显示或者重置某选项
        /// </summary>
        /// <param name="Reset"></param>
        /// <param name="Color"></param>
        void HighLighted(bool Reset = false, ConsoleColor Color = ConsoleColor.DarkGreen)
        {
            if (Reset)
            {
                Console.ResetColor();//重置颜色
                Console.SetCursorPosition(0, selector);
                Console.Write(Options[Console.CursorTop - begin].Title);//重新输出
                Console.CursorTop = Console.WindowHeight - 2;//移走
            }
            else
            {
                Console.BackgroundColor = Color;//高亮显示选中的选项
                Console.SetCursorPosition(0, selector);
                Console.Write(Options[Console.CursorTop - begin].Title);//重新输出               
                Console.CursorTop = Console.WindowHeight - 2;
                Console.ResetColor();
            }
        }

        public class Option//选项类
        {
            public delegate void Event();//选项委托

            public event Event OptionEvent;//选项事件
            public string Title { get; set; }//选项名称

            /// <summary>
            /// 创建一个选项
            /// </summary>
            /// <param name="title">标题</param>
            public Option(string title) { Title = title; }

            /// <summary>
            /// 执行该选项
            /// </summary>
            public void Execute()
            {
                OptionEvent?.Invoke();
            }
        }
    }
}
