using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
namespace GameFrame
{
    public class MLinkedList<T> : IEnumerable<T>
    {
        public class Node//节点
        {
            internal Node Next { get; set; }//指针
            internal T Data { get; set; }//元素
            internal Node(T data)//构造函数
            {
                Data = data;
                Next = null;
            }
            internal Node() { }  //空构造函数，用于链表创建时的头节点  
        }

        public Node Head { get;private set; }//头节点
        public int Length { get;private set; }//长度
        public T this[int index]
        {
            get
            {
                int indexs = Length - 1;
                foreach (var data in this)
                {
                    if (indexs == index) return data;
                    else indexs--;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                int indexs = Length - 1;
                if (index < Length && index >= 0)
                {
                    foreach (var node in AccessNode)
                    {
                        if (indexs == index) { node.Data = value; break; }
                        else indexs--;
                    }
                }
                else throw new IndexOutOfRangeException();
            }
        }

        public MLinkedList(params T[] datas)
        {
            Head = null;//最初的头节点不储存数据
            Length = 0;
            //Head.index = -1;//下标为-1
            Add(datas);//定义变量时直接添加数据
        }

        public void Add(params T[] datas)//添加数据
        {
            if (datas == null)
            {
                throw new Exception("请输入要添加的数据!");
            }
            else foreach (var data in datas)
                {
                    Node newNode = new Node(data);//构造一个新节点用于储存数据
                    newNode.Next = Head;//将该节点指向头节点
                                        //newNode.index = Head.index + 1;
                    Head = newNode;//让头节点引用该节点
                    Length++;
                }
        }
        public void Remove(params T[] datas)//删除数据
        {
            if (datas == null)
            {
                Head = Head.Next;
            }
            else foreach (var data in datas)
                {
                    if (Head != null && Head.Data.Equals(data)) { Head = Head.Next; Length--; }
                    foreach (var node in AccessNode)//遍历所有节点
                    {
                        if (node.Next != null && node.Next.Data.Equals(data))
                        {
                            node.Next = node.Next.Next;//将该节点的上一个节点指向该节点的下一个节点
                            Length--;
                            break;
                        }
                    }
                }
        }
        public void Sort(IComparer<T> comparer)//排序
        {
            Node next, dnext, tnext, head, current;//用于交换的中间节点
            for (int i = 0; i < Length; i++)
            {
                if (Head.Next != null && comparer.Compare(Head.Data, Head.Next.Data) == 1)
                {
                    head = Head;//第一节点
                    next = Head.Next;//第二节点                                    
                    tnext = next.Next;//第三节点
                    head.Next = tnext;//将第一节点指向第三节点
                    next.Next = head;//将第二节点指向第一节点
                    Head = next;//设置新的头节点
                }
                current = Head;
                while (current.Next != null && current.Next.Next != null)
                {
                    next = current.Next;//第一节点
                    dnext = next.Next;//第二节点
                    tnext = dnext.Next;//第三节点
                    if (comparer.Compare(next.Data, dnext.Data) == 1)
                    {
                        current.Next = dnext;//先将当前节点指向第二节点
                        dnext.Next = next;//将第二节点指向第一节点
                        next.Next = tnext;//将第一节点指向第三节点      
                    }
                    current = current.Next;//移动到下一个节点
                }
            }

        }
        public void Display(string format = "{0}\n")//展示链表中所有数据
        {
            foreach (var data in this)
                Console.Write(format, data);
        }

        public IEnumerator<T> GetEnumerator()//直接访问链表中节点的数据
        {
            Node Current = Head;//定义Current引用为头节点
            while (Current != null)//直到最后一个节点
            {
                yield return Current.Data;//返回节点
                Current = Current.Next;//指向下一个节点
            }
        }
        IEnumerable<Node> AccessNode//访问链表中的节点
        {
            get
            {
                Node Current = Head;
                while (Current != null)
                {
                    yield return Current;
                    Current = Current.Next;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()//可枚举类型
        {
            return GetEnumerator();
        }


    }
}
