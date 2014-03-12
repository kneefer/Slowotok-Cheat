using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
