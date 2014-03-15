using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.Utilities
{
    public static class Extensions
    {
        public static void AddSorted<T>(this IList<T> list, T item, Func<T, T, int> comparer)
        {
            int i = 0;
            while (i < list.Count && comparer(list[i], item) < 0)
                i++;

            list.Insert(i, item);
        }

        public static char[,] ConvertTo2DArray(this char[][] jaggedArray, int numOfColumns, int numOfRows)
        {
            char[,] temp2DArray = new char[numOfColumns, numOfRows];

            for (int c = 0; c < numOfColumns; c++)
            {
                for (int r = 0; r < numOfRows; r++)
                {
                    temp2DArray[c, r] = Char.ToLower(jaggedArray[c][r]);
                }
            }

            return temp2DArray;
        }

        public static char[][] ConvertToJaggedArray(this char[] multiArray, int numOfColumns, int numOfRows)
        {
            char[][] jaggedArray = new char[numOfColumns][];

            for (int c = 0; c < numOfColumns; c++)
            {
                jaggedArray[c] = new char[numOfRows];
                for (int r = 0; r < numOfRows; r++)
                {
                    jaggedArray[c][r] = multiArray[c*numOfRows+r];
                }
            }

            return jaggedArray;
        }

        public static string CalculateMD5(this string input, string format = "x2")
        {
            input = input.ToUpper();
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString(format));
            }
            return sb.ToString();
        }
    }
}
