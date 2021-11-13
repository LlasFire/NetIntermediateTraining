// Decompiled with JetBrains decompiler
// Type: MyCalculatorv1.MainWindow
// Assembly: MyCalculatorv1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8E4247A5-25E4-47A6-84F4-A414933F7536
// Assembly location: C:\Users\Vasili_Isakau\source\repos\NetIntermediateTraining\Profiling tools\DumpHomework\MyCalculator.exe

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace MyCalculatorv1
{
    public partial class MainWindow : Window, IComponentConnector
    {
        private readonly Regex _theLastCharIsNumber = new Regex(@"\d$");
        private readonly Regex _theInputCharIsNumber = new Regex(@"^\d$");
        private readonly Regex _rightFormatOfOperation = new Regex(@"^([-]{0,1}\d+)([\/\*\-\+]+)(\d+)$");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var input = ((Button)sender).Content.ToString();

            if (!_theInputCharIsNumber.IsMatch(input) &&
                !_theLastCharIsNumber.IsMatch(tb.Text))
            {
                R_Click(sender, e);
            }

            tb.Text += input;
        }

        private void Result_click(object sender, RoutedEventArgs e) => Result();

        private void Result()
        {
            if (!_rightFormatOfOperation.IsMatch(tb.Text))
            {
                tb.Text = string.Empty;
                return;
            }

            var match = _rightFormatOfOperation.Match(tb.Text);

            var operation = match.Groups[2].Value.Last();

            var firstNumber = Convert.ToDouble(match.Groups[1].Value);
            var secondNumber = Convert.ToDouble(match.Groups[3].Value);

            tb.Text = $"{firstNumber} {operation} {secondNumber} = ";

            if (operation == '+')
            {
                tb.Text += $"{firstNumber + secondNumber}";
            }
            else if (operation == '-')
            {
                tb.Text += $"{firstNumber - secondNumber}";
            }
            else if (operation == '*')
            {
                tb.Text += $"{firstNumber * secondNumber}";
            }
            else
            {
                tb.Text += $"{firstNumber / secondNumber}";
            }
        }

        private void Off_Click_1(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Del_Click(object sender, RoutedEventArgs e) => tb.Text = "";

        private void R_Click(object sender, RoutedEventArgs e)
        {
            if (tb.Text.Length <= 0)
                return;
            tb.Text = tb.Text.Substring(0, tb.Text.Length - 1);
        }

        void Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 2:
                    tb = (TextBox)target;
                    break;
                case 3:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 4:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 5:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 6:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 7:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 8:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 9:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 10:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 11:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 12:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 13:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 14:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 15:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Result_click);
                    break;
                case 16:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Button_Click_1);
                    break;
                case 17:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Off_Click_1);
                    break;
                case 18:
                    ((ButtonBase)target).Click += new RoutedEventHandler(Del_Click);
                    break;
                case 19:
                    ((ButtonBase)target).Click += new RoutedEventHandler(R_Click);
                    break;
                default:
                    _contentLoaded = true;
                    break;
            }
        }
    }
}
