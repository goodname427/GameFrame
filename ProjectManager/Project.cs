using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

//1.用一个结构体实现：包括项目的项目名，负责人，队员，指导老师，队员人数（不包含指导老师），项目类别，项目编号。
//2.向链表中插入一个项目。（项目编号为1个随机大小写字母和随机的6位数字组成的7位字符串，且不能与已有项目编号重复，其他的变量在命令行输入）
//项目编号例子：T123456
//3.按项目编号降序输出链表中所有的项目信息。
//4.删除一个项目，按项目编号删除（扩展需求：如果项目编号不存在，支持按输入模糊搜索后删除）。
//5.修改项目信息，按项目编号修改（扩展需求：如果项目编号不存在，支持按输入模糊搜索后删除）。
//6.查询项目信息，按项目类别查找该类别所有项目信息。
//7.按项目编号查找项目详细信息，要求支持模糊搜索（比如编号为T123456，输入T或输入1，也可显示该项目）。
//8.将所有项目信息按一个变量一行，到下一个项目换行的格式保存在文本文件中，要求每次重复此操作能在文本文件中显示更新后的所有项目信息。


namespace ProjectManager
{
   public struct Project
    {
        public string projectName;//项目名
        public string principal;//负责人
        public string[] teamMembers;//队员
        public string instructor;//指导老师
        public int NumberOfTeam { get => teamMembers.Length; }//队员人数
        public string projectType;//项目类别
        public string projectSerialNumber;//项目编号

        public Project(string projectName,string principal,string instructor,string projectType,params string[] teamMembers)
        {
            Random r = new Random();
            this.projectName = projectName;
            this.principal = principal;
            this.instructor = instructor;
            this.projectType = projectType;
            this.teamMembers = teamMembers;
            while (true)
            {
                projectSerialNumber = "";//生成随机编号
                projectSerialNumber += (char)(r.Next(26) + (r.Next(2) == 1 ? 65 : 97));//字母
                for (int i = 0; i < 6; i++)//数字
                    projectSerialNumber += r.Next(10);
                foreach (var project in Program.projects)//检测编号是否冲突
                    if (project.projectSerialNumber == projectSerialNumber) continue;
                break;
            }
        }
        public void Show()//显示项目信息
        {
            Console.WriteLine($"项目名:{projectName}");
            Console.WriteLine($"项目类别:{projectType}");
            Console.WriteLine($"项目编号:{projectSerialNumber}");
            Console.WriteLine($"负责人:{principal}");
            Console.WriteLine($"指导老师:{instructor}");
            Console.WriteLine($"队员人数:{NumberOfTeam}");
            Console.Write($"队员:");
            foreach (var member in teamMembers)
            {
                if (!member.Equals(teamMembers[teamMembers.Length-1])) Console.Write("{0,2},",member);
                else Console.Write("{0,2}", member);
            }
            
        }
        
    }
}
