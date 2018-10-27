using System;
using System.Collections.Generic;
using System.Linq;
using static Utility.Core.Attributes.InvokerAttributes;
using Utility.Tools;

namespace Utility.Invocables
{
    [Invoker("SumAsString")]
    public class Sumf
    {
        [InvokerCaller]
        public string ToSum(string[] args)
        {
            return args.Select(n => Convert.ToInt32(n)).Sum().ToString();
        }
    }

    [Invoker("Range")]
    public class RangerFall
    {
        [InvokerCaller]
        public IEnumerable<Int32> Range(string[] args)
        {
            var inicial = Convert.ToInt32(args.First());
            var final = Convert.ToInt32(args.Skip(1).First());

            return Enumerable.Range(inicial, final);
        }
    }

    [Invoker("Sum")]
    public class Sumer
    {
        [InvokerCaller]
        public int Sumar(IEnumerable<int> nums)
        {
            return nums.Sum();
        }
    }

    [Invoker("Multiply")]
    public class Multiply
    {
        [InvokerCaller]
        public int Mult(string[] args)
        {
            var nums = args.Select(s => Convert.ToInt32(s));

            var result = nums.Aggregate((acum, item) => acum * item);

            return result;
        }
    }

    [Invoker("Primes")]
    public class Primes
    {
        [InvokerCaller]
        public void Calculate(string[] arg)
        {
            var until = arg.First().Project(Convert.ToInt32);
            var acum = 0;
            var numAcum = 0;
            var nums = new List<int>();

            while(until > acum)
            {
                if (IsPrime(numAcum))
                {
                    acum++;
                    nums.Add(numAcum);
                }

                numAcum++;
            }

            Console.WriteLine(string.Join(", ", nums));
        }

        public bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;
        }
    }
}
