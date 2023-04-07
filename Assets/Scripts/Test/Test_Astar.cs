using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Astar : Test_Base
{
    private void Start()
    {
        // Operator(연산자, 명령어)
        // +, -, *, /   : 산술 연산자
        // &&, ||       : 논리 연산자
        // &, |         : 비트 연산자
        // >, <, >=, <= : 비교 연산자
        // =            : 대입 연산자

        /*List<int> list = new List<int>();
        list.Add(1);
        list.Add(10);
        list.Add(5);
        list.Add(7);
        list.Add(8);
        foreach(int num in list)
        {
            Debug.Log(num);
        }

        list.Sort();

        foreach (int num in list)
        {
            Debug.Log(num);
        }*/


        Node a = new Node();
        a.G = 10;
        Node b = new Node();
        b.G = 30;
        Node c = new Node();
        c.G = 15;
        List<Node> list = new List<Node>();
        list.Add(a);
        list.Add(b);
        list.Add(c);
        foreach(Node node in list)
        {
            Debug.Log(node.F);
        }

        list.Sort();

        foreach (Node node in list)
        {
            Debug.Log(node.F);
        }
    }
}
