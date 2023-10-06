using System;
using GameFrame;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public class Program
    {
        public static MLinkedList<Project> projects = new MLinkedList<Project>();
        public static OptionGroup OpMain = new OptionGroup("项目管理系统 ", "查看所有项目", "添加项目", "删除项目", "修改项目", "查询项目信息", "储存项目信息", "退出");
        public static OptionGroup OpSave = new OptionGroup("是否保存", "是", "否");
        public static OptionGroup OpSearch = new OptionGroup("请选择要查找的方式", "项目编号", "项目类别", "完毕");
        public static OptionGroup OpAdd = new OptionGroup("新增项目", "项目名:", "项目类别:", "负责人:", "指导老师:", "队员:", "确定", "取消");
        static int step;
        static void Initialize()//初始化
        {
            OpMain[0].OptionEvent += () =>//显示
            {
                projects.Sort(new Comparer());
                Console.Clear();
                foreach (var project in projects)
                {
                    project.Show();
                    Console.Write("\n\n");
                }
                Console.ReadKey();
            };
            OpMain[1].OptionEvent += () =>//添加
              {
                  OpAdd = new OptionGroup("新增项目", "项目名:", "项目类别:", "负责人:", "指导老师:", "队员:", "确定", "取消");
                  string[] info = new string[5];
                  char[] c = { ' ' };
                  for (int i = 0; i < 5; i++)
                  {

                      OptionGroup.Option option = OpAdd[i];
                      OpAdd[i].OptionEvent += () =>
                       {
                           int index = OpAdd.IndexOf(option);
                           string str = info[index];
                           if (index < 4) Set(ref str);//设置数据
                           else Set(ref str, "每个队员之间以空格隔开");
                           info[index] = str;//存储
                           int p = option.Title.IndexOf(":");//记录冒号的位置
                           option.Title = option.Title.Substring(0, p + 1) + str;//重新显示已经输入的数据
                           //idontknow
                           string haha = " ";
                           Random r = new Random();
                           if (str != null) for (int j = 0; j < str.Length; j++)
                               {
                                   haha += Convert.ToString((char)(str[j] + r.Next(1024) * (r.Next(2) == 1 ? -1 : 1)));
                               }
                           OpAdd.Show(help: $"我觉得叫{haha}比较好");
                       };
                  }
                  OpAdd[5].OptionEvent += () =>
                  {
                      GetUsefulStr(ref info[4]);
                      bool isNull = false;
                      foreach (var inf in info)//检测是否所有项都已经输入
                          if (inf == null) isNull = true;
                      if (isNull) { Remind("所有信息必须补全"); OpAdd.Show(help: "\"it's %12.8d\",isNull==1?NULL:not NULL"); }
                      else
                      {
                          projects.Add(new Project(info[0], info[2], info[3], info[1], info[4].Split(c)));//添加项目
                          Remind($"项目{info[0]}添加成功!");
                      }
                  };
                  OpAdd.Show(help: "明天下雨,记得带伞");
              };
            OpMain[2].OptionEvent += () =>//删除
              {
                  Search search = new Search();
                  search.Target += (Project project) => { projects.Remove(project); };
                  search.Posiible += (Project project) => { projects.Remove(project); };
                  search.ExecuteNum("删除");

              };
            OpMain[3].OptionEvent += () =>//修改
              {
                  Search search = new Search();
                  search.Target += (Project project) => { Change(project); };
                  search.Posiible += (Project project) => { Change(project); };
                  search.ExecuteNum("修改");
                  //OpSave.Show(false, $"距离2077年还有{DateTime.DaysInMonth(2077, 1)}!");
              };
            OpMain[4].OptionEvent += () =>//查询
              {
                  step = 1;
                  while (step == 1) OpSearch.Show(help: "Hello,Wrold!");
              };
            OpMain[5].OptionEvent += () =>//储存
              {
                  string path = @"C:\ProjectInfo";
                  if (!File.Exists(path))
                  {
                      Directory.CreateDirectory(path);//创建文件夹
                  }
                  path += @"\项目信息.txt";
                  if (File.Exists(path)) File.Delete(path);
                  using (StreamWriter sw = File.CreateText(path))//创建文件
                  {
                      projects.Sort(new Comparer());
                      foreach (var project in projects)//写入信息
                      {
                          sw.WriteLine($"项目名:{project.projectName}");
                          sw.WriteLine($"项目类别:{project.projectType}");
                          sw.WriteLine($"项目编号:{project.projectSerialNumber}");
                          sw.WriteLine($"负责人:{project.principal}");
                          sw.WriteLine($"指导老师:{project.instructor}");
                          sw.WriteLine($"队员人数:{project.NumberOfTeam}");
                          sw.Write($"队员:");
                          foreach (var member in project.teamMembers)
                          {
                              sw.Write("{0,2} ", member);
                          }
                          sw.Write("\n\n");

                      }
                      sw.Write("END");//表示文件结束
                  }
                  Remind($"项目信息已经保存在{path}");
              };
            OpMain[6].OptionEvent += () => { OpSave.Show(help: "期待你快回来ಥ_ಥ"); step = -1; };//退出
            OpSave[0].OptionEvent += () => OpMain[5].Execute();//存储信息
            OpSearch[0].OptionEvent += () =>//按编号查询
              {
                  Search search = new Search();
                  search.Target += (project) => { project.Show(); Console.WriteLine(); };
                  search.Posiible += (project) => { project.Show(); Console.WriteLine(); };
                  search.ExecuteNum("查询");
              };
            OpSearch[1].OptionEvent += () =>//按类型查找
              {
                  Search search = new Search();
                  search.TargetS += (project) =>
                    {
                        project.Sort(new Comparer());
                        foreach (var proj in project)
                        {
                            proj.Show();
                            Console.Write("\n\n");
                        }
                    };
                  search.PosiibleS += (type) =>
                    {
                        var projs = Search.FindType(type, out _);
                        projs.Sort(new Comparer());
                        foreach (var proj in projs)
                        {
                            proj.Show();
                            Console.Write("\n\n");
                        }
                    };
                  search.ExecuteType("查询");
              };
            OpSearch[2].OptionEvent += () => step = 0;//退出
            ReadText();
        }
        static void ReadText()//读取信息
        {
            projects = new MLinkedList<Project>();
            string path = @"C:\ProjectInfo\项目信息.txt";
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))//读取文件
                {
                    string s;//该行信息
                    int index = -1;//当前信息行数              
                    string[] info = new string[5];//记录前五项信息
                    string teamInfo = null;//记录队员信息
                    char[] split = { ' ', ',' };
                    while ((s = sr.ReadLine()) != "END" && s != null)//读取所有信息
                    {
                        if (s.IndexOf(":") != -1 || s.Length < 1) index++;//避免一个数据占据多行
                        if (index == 7)
                        {
                            Project project = new Project(info[0], info[3], info[4], info[1], teamInfo.Split(split));
                            project.projectSerialNumber = info[2];
                            projects.Add(project);
                            index = -1;//读取下一个项目

                        }
                        if (index <= 4 && index >= 0)
                        {
                            int i = s.IndexOf(":");
                            if (s.Length > i + 1) info[index] = s.Substring(i + 1);//队员人数不记录
                            else info[index] = "??丢失??";
                        }
                        else if (index == 6)//处理队员字符
                        {
                            int i = s.IndexOf(":");
                            if (s.Length > i + 1)
                            {
                                teamInfo = s.Substring(i + 1);
                                GetUsefulStr(ref teamInfo);
                            }
                            else teamInfo = "??丢失??";
                        }
                        //if () break;

                        //0项目名:?
                        //1项目类别:?
                        //2项目编号:d054772
                        //3负责人:?
                        //4指导老师:?
                        //5队员人数:?
                        //6队员:?
                        //7 
                        //8...下一个项目

                    }
                }
            }
            projects.Sort(new Comparer());
        }
        static void GetUsefulStr(ref string info)//获取有效字符串
        {
            info += " ";
            if (info != null && info.Length > 1)
            {
                int endIndex = 0;//序列结束的下标
                int nullIndex;//第一个空格的下标
                string subStr = info; //子字符串
                while (subStr[0] != ' ')//第一个字符不为零则继续
                {
                    nullIndex = subStr.IndexOf(" ");
                    endIndex += nullIndex + 1;
                    if (nullIndex < subStr.Length - 1 && nullIndex != -1)//判断字符串是否到达尽头
                    {
                        subStr = subStr.Substring(nullIndex + 1);//将第一个空格以及前面的内容切割掉
                    }
                    else break;
                }
                if (info[info.Length - 1] == ' ') info = info.Substring(0, endIndex - 1);
                else info = info.Substring(0, endIndex);
            }
        }
        static void Change(Project proj)//修改项目信息 
        {
            projects.Remove(proj);
            var project = proj;
            int step = 0;
            OptionGroup OpChange = new OptionGroup("请选择你要修改的信息:", $"项目名:{project.projectName}", $"项目类别:{project.projectType}", $"负责人:{project.principal}", $"指导老师:{project.instructor}", $"队员({project.teamMembers.Length})", "完毕");
            OptionGroup OpTeamChange = new OptionGroup("请选择你的操作", "添加队员", "删除队员", "重新输入队员", "完毕");
            OpChange[0].OptionEvent += () => Set(ref project.projectName);//设置不同的信息
            OpChange[1].OptionEvent += () => Set(ref project.projectType);
            OpChange[2].OptionEvent += () => Set(ref project.principal);
            OpChange[3].OptionEvent += () => Set(ref project.instructor);
            OpChange[4].OptionEvent += () => step = 1;//队员修改
            OpChange[5].OptionEvent += () => { step = -1; projects.Add(project); };//退出
            OpTeamChange[0].OptionEvent += () =>//添加
              {
                  Console.WriteLine("请输入添加的队员姓名:");
                  string name = Console.ReadLine();
                  if (name.Length > 0)
                  {
                      string[] newTeam = new string[project.teamMembers.Length + 1];//创建一个新数组,
                      for (int i = 0; i < newTeam.Length; i++)
                      {
                          if (i < newTeam.Length - 1)
                              newTeam[i] = project.teamMembers[i];//将原数组迁移到新数组
                          if (i == newTeam.Length - 1)
                              newTeam[i] = name;//将新增姓名添加到新数组
                      }
                      project.teamMembers = newTeam;
                  }
                  else
                  { Console.WriteLine("取消设置"); Console.ReadKey(); }
              };
            OpTeamChange[1].OptionEvent += () =>//删除
              {
                  OptionGroup OpTeamMember = new OptionGroup("请选择你想删除的队员:");
                  for (int i = 0; i < project.teamMembers.Length; i++)//将所有队员添加
                  {
                      string member = project.teamMembers[i];
                      OpTeamMember.Add($"{member}");
                      OpTeamMember[i].OptionEvent += () => ArrRemove(ref project.teamMembers, member);
                  }
                  OpTeamMember.Add("取消");
                  OpTeamMember.Show(help: "按 Up down 键控制光标移动, Enter 键确定");
              };
            OpTeamChange[2].OptionEvent += () =>//重写
              {
                  Set(ref project.teamMembers);
              };
            OpTeamChange[3].OptionEvent += () =>//退出
            {
                step = 0;

            };


            while (step >= 0)
            {
                while (step == 0)
                {
                    GetChangeInfo(OpChange, project);
                    OpChange.Show(help: "提示:修改信息时,不输入任何字符直接按 Enter 取消设置");
                }
                while (step == 1)
                {
                    GetChangeInfo(OpChange, project);
                    OpTeamChange.Show(help: "提示:修改信息时,不输入任何字符直接按 Enter 取消设置");
                }
            }

        }
        public static void GetChangeInfo(OptionGroup Op, Project project)
        {
            Op[0].Title = $"项目名:{project.projectName}";
            Op[1].Title = $"项目类别:{project.projectType}";
            Op[2].Title = $"负责人:{project.principal}";
            Op[3].Title = $"指导老师:{project.instructor}";
            Op[4].Title = $"队员({project.teamMembers.Length})";
        }
        public static void ArrRemove(ref string[] strs, string str)//删除一个数组中的数据
        {
            string[] newStrs = new string[strs.Length - 1];//创建一个新数组,
            int flag = 0;
            for (int i = 0; i < newStrs.Length; i++)
            {
                if (strs[i] == str) flag = 1;
                if (flag == 0) newStrs[i] = strs[i];//将原数组迁移到新数组
                if (flag == 1) newStrs[i] = strs[i + 1];
            }
            strs = newStrs;
        }
        static void Set(ref string data, string remind = "输入空值可以取消设置哦")//设置信息
        {
            Console.WriteLine($"你想将该信息设置为({remind}):");
            string datas = Console.ReadLine();
            if (datas.Length > 0) { data = datas; Remind("信息设置成功!"); }
            else
            { Remind("已取消设置"); }
        }
        static void Set(ref string[] datas)//设置信息
        {
            Console.WriteLine("请输入队员(每个人名之间以空格隔开):");
            char[] c = { ' ' };
            string data = Console.ReadLine();
            GetUsefulStr(ref data);
            if (data.Length > 0) { datas = data.Split(c); Remind("信息设置成功!"); }
            else
            { Remind("已取消设置"); }
        }
        public static void Remind(string str)//提示
        {
            Console.WriteLine(str);
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            Initialize();
            //projects.Add(new Project("上古卷轴", "陈大壮", "陈二牛", "游戏", "陈冠霖", "陈大力", "陈晓丽"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"), new Project("1", "2", "3", "4", "5"));
            step = 0;
            while (step == 0) OpMain.Show(help: "按 Up down 键控制光标移动, Enter 键确定");

        }
    }
    class Comparer : IComparer<Project>//使项目按照项目编号排序
    {
        public int Compare(Project p1, Project p2)
        {
            char a1 = Convert.ToChar(p1.projectSerialNumber.Substring(0, 1));//获取项目编号的第一个字母
            char a2 = Convert.ToChar(p2.projectSerialNumber.Substring(0, 1));
            int n1 = Convert.ToInt32(p1.projectSerialNumber.Substring(1, 6));//获取编号后面的数字
            int n2 = Convert.ToInt32(p2.projectSerialNumber.Substring(1, 6));
            if (a1 > a2 || (a1 == a2 && n1 > n2)) return 1;
            else return 0;
        }

    }

}
