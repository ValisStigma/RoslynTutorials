using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxVisualizerExample1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime oneDate = new DateTime(2015, 8, 19);

            Console.WriteLine($"Today is {oneDate.ToShortDateString()}");
            Console.ReadLine();
        }
    }
}
