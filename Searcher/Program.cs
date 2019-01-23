using System;

namespace SearchString
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please, enter main string (search source):");
            string str = Console.ReadLine();
            Console.WriteLine();
            string pattern = string.Empty;
            bool accepted = false;
            while (!accepted)
            {
                Console.WriteLine("Enter search pattern to search in main string:");
                pattern = Console.ReadLine();
                accepted = pattern.Length <= str.Length;
                Console.WriteLine(accepted ? "Thanks!" : "Main string is shorter search pattern, please, re-enter search template.");
            }
            int[] result = str.SearchString(pattern);
            string msg = string.Empty;
            if (result[0] == -1)
                msg = $"Sorry, string \"{pattern}\" not found.";
            else
                msg = $"Pattern \"{pattern}\" was found at positions: {string.Join(", ", result)}";
            Console.WriteLine(msg);
            Console.ReadLine();
        }
    }
}
