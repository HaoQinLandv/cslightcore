﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CLScriptExt
{
    class Student
    {
        public class StudentAss
        {
            public int size = 10;

        }
        public string name
        {
            get;
            set;
        }
        public static int page;
        public int age;
        public class S1
        {
            public int v;
        }
        public List<int> vs = new List<int>();
        public int[] vs2 = new int[] { 1, 2, 3, 4 };

        public void ToString2()
        {

        }
        public void ToString2<T>(T obj)
        {
            Console.WriteLine(obj.ToString());
        }



    }

}
