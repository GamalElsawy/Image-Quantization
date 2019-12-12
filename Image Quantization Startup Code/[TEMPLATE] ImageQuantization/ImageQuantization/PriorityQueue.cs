using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class PriorityQueue<T>
    {
        Node<T>[] Items;

        int NoOfItems;

        public PriorityQueue(int n)
        {
            Items = new Node<T>[n];

            NoOfItems = 0;
        }

        private int GetLeftChild(int index)
        {
            return (index * 2) + 1;
        }

        private Node<T> GetLeftChildValue(int index)
        {
            return Items[GetLeftChild(index)];
        }

        private int GetRightChild(int index)
        {
            return (index * 2) + 2;
        }

        private Node<T> GetRightChildValue(int index)
        {
            return Items[GetRightChild(index)];
        }

        private int GetParent(int index)
        {
            return ((index - 2) + 1) / 2;
        }

        private Node<T> GetParentValue(int index)
        {
            return Items[GetParent(index)];
        }

        private bool HasLeftChild(int index)
        {
            return GetLeftChild(index) < NoOfItems;
        }

        private bool HasRightChild(int index)
        {
            return GetRightChild(index) < NoOfItems;
        }

        public void Enqueue(T item, double priority)
        {
            Items[NoOfItems] = new Node<T>(item, priority);
            NoOfItems++;
            BubbleUp();
        }

        public T Dequeue()
        {
            Node<T> res = Items[0];
            NoOfItems--;
            Items[0] = Items[NoOfItems];
            Items[NoOfItems] = null;
            BubbleDown();

            return res.Data;
        }

        private void BubbleDown()
        {
            int index = 0;
            while (HasLeftChild(index))
            {
                int maxIndex = GetLeftChild(index);
                if (HasRightChild(index) && GetRightChildValue(index).Priority > Items[maxIndex].Priority)
                {
                    maxIndex = GetRightChild(index);
                }

                if (Items[index].Priority >= Items[maxIndex].Priority)
                    break;

                Node<T> temp = Items[index];
                Items[index] = Items[maxIndex];
                Items[maxIndex] = temp;
                index = maxIndex;
            }
        }

        private void BubbleUp()
        {
            int index = NoOfItems - 1;

            while (GetParent(index) >= 0 && GetParentValue(index).Priority < Items[index].Priority)
            {
                int parentIdx = GetParent(index);
                Node<T> temp = Items[index];
                Items[index] = Items[parentIdx];
                Items[parentIdx] = temp;
                index = parentIdx;
            }
        }
        public int Size()
        {
            return NoOfItems;
        }
        public T Get()
        {
            return Items[0].Data;
        }
    }

    public class Node<T>
    {
        public T Data;

        public double Priority;

        public Node(T data, double priority)
        {
            Data = data;
            Priority = priority;

        }
    }
}
