using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gambling.Models;

namespace Gambling.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            TestViewModel model = new TestViewModel();
            model.data1 = "test";
            int Num = 30;

            //Note data1
            for (int i = 1; i <= Num; i++)
                model.data1 += (Fib(i).ToString() + ",");

            //Note: data2
            model.data2 = FibFor(Num);

            //Note: 氣泡排序
            int[] array = {1,4,0,5,3};
            model.data3 = string.Join(",", array);
            
            //用linq ＋浪打 搜尋
            string[] words = { "believe", "relief", "receipt", "field" };
            words = words.Where(x => x.IndexOf("ld") > -1).ToArray();
            model.data4 = string.Join(",", words);

            //abc -> cba 字串反轉法
            string s = "abcdefgh";
            char[] c = s.ToCharArray();
            Array.Reverse(c);
            s = new string(c);
            model.data5 = s;

            return View(model);
        }

        //費氏數列(Fibonacci) 遞迴
        private static int Fib(int num)
        {
            if (num <= 2)
                return 1;
            else
                return Fib(num - 1) + Fib(num - 2);
        }

        //費氏數列 For迴圈版本
        private static string FibFor(int num)
        {
            string result = "";
            int p1 = 1; // 第一個數 
            int p2 = 1; // 第二個數 
            int res = 0; ;
            result += string.Format("{0}-{1}", p1, p2); // 印出第一個和第二個數

            for (int i = 3; i <= num; i++)
            {
                res = p1 + p2;
                p1 = p2;
                p2 = res;
                result += (result == "" ? "" : "-") + string.Format("{0}", res);
            }
            return result;
        }

        private static int[] foo(int[] array)
        {
            int temp = 0;
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[j] < array[i])
                    {
                        temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
            return array;
        }



    }
}