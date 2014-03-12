using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GridOfTextBoxesControl
{
    public static class Extensions
    {
        public static IEnumerable<T> FindVisualChilds<T>(this DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                    yield return (T)child;
                else
                {
                    IEnumerable<T> childsOfChild = FindVisualChilds<T>(child);
                    if (childsOfChild != null)
                    {
                        foreach (var item in childsOfChild)
                            yield return item;
                    }
                }
            }
            yield break;
        }
    }
}
