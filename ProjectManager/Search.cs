using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;
using System.IO;

namespace ProjectManager
{
    class Search
    {
        public delegate void dele(Project project);
        public delegate void deleS(string project);
        public delegate void delePs(MLinkedList<Project> projects);
        public event dele Target;//用于编号查找
        public event delePs TargetS;//用于类别查找
        public event dele Posiible;
        public event deleS PosiibleS;

        public static Project? FindNum(string serialNumber, out MLinkedList<Project> possible)//按编号查找
        {
            possible = new MLinkedList<Project>();//可能的项目
            foreach (var project in Program.projects)//寻找
            {
                if (project.projectSerialNumber == serialNumber) return project;//找到后记录该项目
                else //模糊搜索
                {
                    //如果目标编号和当前编号有超过一个字符以上相等则记作可能项目
                    int sameChar = SameNum(serialNumber, project.projectSerialNumber);
                    if ( sameChar>= serialNumber.Length / 2 &&sameChar>0)
                    {
                        possible.Add(project);
                    }
                }
            }
            return null;//如果没有找到则返回空

        }
        public static MLinkedList<Project> FindType(string type, out MLinkedList<string> possibleType)//按类型查找
        {
            possibleType = new MLinkedList<string>();//可能的项目
            var allOfType = new MLinkedList<Project>();//可能的项目
            foreach (var project in Program.projects)//寻找
            {
                if (project.projectType == type) allOfType.Add(project);//找到后记录该项目
                else //模糊搜索
                {
                    int sameChar = SameType(type, project.projectType);
                    if ( sameChar>= type.Length / 2&&sameChar>0)
                    {
                        bool isSame = false;
                        foreach (var t in possibleType)//检测是否已添加了相同类型
                            if (t == project.projectType) isSame = true;
                       if(!isSame) possibleType.Add(project.projectType);
                    }
                }
            }
            return allOfType;//如果没有找到则返回空

        }

        static int SameNum(string target, string current)//比较编号在对应位置有多少个相同数字
        {
            int sumLen = 0;
            int maxNumLen = 0;//相匹配的最大数字字符串长度
            int curNumLen = 0;//相匹配的当前数字字符串长度
            //字符串处理
            MLinkedList<char> tarLetter = new MLinkedList<char>();//字母部分 
            MLinkedList<string> tarNum = new MLinkedList<string>();//数字部分 
            string curLetter = Convert.ToString(current[0]);//处理既定的字符串,字母部分
            string curNum = current.Substring(1);//数字部分
            string serialNum = null;//记录连接在一起的数字
            //处理输入字符串
            for (int i = 0; i < target.Length; i++)//分割
            {
                if ((target[i] >= 65 && target[i] <= 90) || (target[i] >= 97 && target[i] <= 122))
                {
                    tarLetter.Add(target[i]);//添加字母
                   if(serialNum!=null) tarNum.Add(serialNum);
                    serialNum = null;
                }
                if (target[i] <= '9' && target[i] >= '0')//将连在一起的数字统一记录
                {
                    if (serialNum == null||serialNum=="") serialNum = Convert.ToString(target[i]);
                    else serialNum += Convert.ToString(target[i]);
                }
            }
            if (serialNum != null) tarNum.Add(serialNum);
            //匹配
            if (tarLetter.Length > 0) foreach (var cha in tarLetter)//匹配字母
                {
                    if (cha == curLetter[0]) sumLen = 1;
                }
            if (tarNum.Length > 0) foreach (var num in tarNum)//匹配数字
                {
                    curNumLen = 0;
                    if (num != null)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            for (int j = 0; j < num.Length && j < 6 - i; j++)
                            {
                                if (num[j] == curNum[i + j]) curNumLen++;
                            }
                            if (curNumLen > maxNumLen) maxNumLen = curNumLen;//记录最长的字符串长度
                        }
                    }
                }
            sumLen += maxNumLen;
            return sumLen;
        }
        static int SameType(string target, string current)//比较类型在对应位置有多少个相同数字
        {
            int sum = 0;
            int max = 0;//相匹配的最大字符串长度
            int cur = 0;//相匹配的当前字符串长度
            int len = target.Length;
            for (int i = 0; i < current.Length; i++)
            {
                for (int j = 0; j < len && j < current.Length - i; j++)
                {
                    if (target[j] == current[i + j]) cur++;
                }
                if (cur > max) max = cur;//记录最长的字符串长度
            }
            sum += max;
            return sum;
        }

        public void ExecuteNum(string purpose)
        {
            Console.WriteLine($"请输入你要{purpose}的项目编号:");
            string serialNumber = Console.ReadLine();//记录输入的编号
            if (serialNumber.Length < 1)
            {
                Program.Remind("已取消!");
            }
            else
            {
                MLinkedList<Project> possiblyProject = new MLinkedList<Project>();//可能的项目
                Project? target = FindNum(serialNumber, out possiblyProject);
                if (target != null)
                {
                    Project targets = (Project)target;
                    Target(targets);//修改该项目
                    Program.Remind($"项目{targets.projectName}{purpose}成功!");
                }

                else if (possiblyProject.Length > 0)//项目编号不存在
                {
                    OptionGroup op = new OptionGroup($"您输入的项目编号不存在,或者您想{purpose}以下项目的其中之一:");
                    for (int i = 0; i < possiblyProject.Length; i++)
                    {
                        Project current = possiblyProject[i];
                        op.Add($"项目名:{possiblyProject[i].projectName}(编号:{possiblyProject[i].projectSerialNumber})");
                        op[i].OptionEvent += () =>
                        {
                            Posiible(current);
                            Program.Remind($"项目{current.projectName}{purpose}成功!");
                        };
                    }
                    op.Add("取消");
                    op.Show("Не хочу больше писать код!");
                }
                else//寻找不到相关信息
                {
                    Program.Remind("您输入的项目编号不存在!");
                }
            }
        }
        public void ExecuteType(string purpose)
        {
            Console.WriteLine($"请输入你要{purpose}的项目类别:");
            string type = Console.ReadLine();//记录输入的类型
            if (type.Length < 1)
            {
                Program.Remind("已取消!");
            }
            else
            {
                MLinkedList<string> possiblyType = new MLinkedList<string>();//可能的项目
                MLinkedList<Project> targets = FindType(type, out possiblyType);
                if (targets.Length > 0)
                {
                    TargetS(targets);//对该项目进行操作
                    Program.Remind($"类别{type}{purpose}成功!");
                }
                else if (possiblyType.Length > 0)//类型不存在
                {
                    OptionGroup op = new OptionGroup($"您输入的类别不存在任何项目,或者您想{purpose}以下类别的其中之一:");
                    for (int i = 0; i < possiblyType.Length; i++)
                    {
                        string current = possiblyType[i];
                        op.Add($"类型:{possiblyType[i]}");
                        op[op.Count - 1].OptionEvent += () =>
                        {
                            PosiibleS(current);
                            Program.Remind($"类别{current}中所有的项目{purpose}成功!");
                        };
                    }
                    op.Add("取消");
                    op.Show($"你知道吗?明天是{DateTime.Now.AddDays(1).DayOfWeek}哦!");
                }
                else//寻找不到相关信息
                {
                    Program.Remind("您输入的项目类型不存在!");
                }
            }
        }
    }
}
