using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridOfTextBoxesControl
{
    public class GridOfTextBoxesControl : Control
    {
        public static readonly DependencyProperty ArrayOfCharsProperty =
            DependencyProperty.Register("ArrayOfChars", typeof(char[][]), typeof(GridOfTextBoxesControl),
            new FrameworkPropertyMetadata(new char[4][]{new char[4], new char[4], new char[4], new char[4]}));

        public char[][] ArrayOfChars
        {
            get { return (char[][])GetValue(ArrayOfCharsProperty); }
            set { SetValue(ArrayOfCharsProperty, value); }
        }

        public static readonly RoutedEvent EnterPressedEvent = EventManager.RegisterRoutedEvent(
            "EnterPressed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GridOfTextBoxesControl));

        public event RoutedEventHandler EnterPressed
        {
            add { AddHandler(EnterPressedEvent, value); }
            remove { RemoveHandler(EnterPressedEvent, value); }
        }

        static GridOfTextBoxesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridOfTextBoxesControl), new FrameworkPropertyMetadata(typeof(GridOfTextBoxesControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IEnumerable<TextBox> textBoxes = this.FindVisualChilds<TextBox>();

            foreach (var item in textBoxes)
            {
                item.Text = "";
                item.KeyDown += item_KeyDown;
                item.KeyUp += item_KeyUp;
            }
        }

        void item_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            txtBox.Select(0, 0);
        }

        void item_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightAlt ||
                e.Key == Key.LeftAlt ||
                e.Key == Key.System)
            {
                e.Handled = true;
                return;
            }

            TextBox txtBox = sender as TextBox;
            char mem ='0';

            if(txtBox.Text.Length > 0)
                mem = txtBox.Text.ToUpper()[0];

            txtBox.Text = "";

            if (Regex.IsMatch(mem.ToString(), @"^[A-ZzżźćńółęąśŻŹĆŃĄŚĘŁÓ]$"))
            {
                txtBox.Text = mem.ToString();

                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                TextBox keyboardFocus = Keyboard.FocusedElement as TextBox;

                if (keyboardFocus != null)
                    keyboardFocus.MoveFocus(tRequest);
            }

            e.Handled = true;
        }
    }
}
