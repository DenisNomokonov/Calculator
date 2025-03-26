using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private string currentInput = "";
        private string operation = "";
        private string memory = "";
        private bool memoryActive = false;
        private List<string> history = new List<string>();
        private bool resultDisplayed = false;
        private bool showAsFraction = true;
        private bool IsOperator(char c) => "+-*/".Contains(c);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (resultDisplayed)
            {
                currentInput = "";
                resultDisplayed = false;
            }

            currentInput += (sender as Button).Content.ToString();
            Display.Text = currentInput;
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput) && !IsOperator(currentInput[^1]))
            {
                currentInput += " " + (sender as Button).Content.ToString() + " ";
                Display.Text = currentInput;
            }
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = EvaluateExpression(currentInput);
                history.Add(currentInput + " = " + result);
                Display.Text = result;
                currentInput = result;
                resultDisplayed = true;
            }
            catch
            {
                Display.Text = "Ошибка";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "";
            operation = "";
            Display.Text = "0";
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                Display.Text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
            }
        }

        private void CE_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "";
            Display.Text = "0";
        }

        private void FunctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentInput))
                return;

            Button button = sender as Button;
            string func = button.Content.ToString();
            string result = "";

            try
            {
                switch (func)
                {
                    case "Sqr":
                        result = EvaluateExpression(currentInput + " * " + currentInput);
                        break;
                    case "1/x":
                        result = EvaluateExpression("1 / " + currentInput);
                        break;
                }

                history.Add(func + "(" + currentInput + ") = " + result);
                Display.Text = result;
                currentInput = result;
            }
            catch (Exception)
            {
                Display.Text = "Ошибка";
            }
        }

        private void ShowHistory_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(history);
            historyWindow.Show();
        }

        private void MemorySave_Click(object sender, RoutedEventArgs e)
        {
            memory = currentInput;
            memoryActive = true;
        }

        private void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            if (memoryActive)
            {
                currentInput = memory;
                Display.Text = memory;
            }
        }

        private void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            memory = "";
            memoryActive = false;
        }

        private void MemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            if (memoryActive)
            {
                memory = EvaluateExpression(memory + " + " + currentInput);
                Display.Text = memory;
            }
            else
            {
                memory = currentInput;
                memoryActive = true;
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Display.Text);
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            string clipboardText = Clipboard.GetText();

            if (IsValidFraction(clipboardText))
            {
                currentInput = clipboardText;
                Display.Text = clipboardText;
            }
        }

        private bool IsValidFraction(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^-?\d+(/\d+)?$");
        }

        private void SetFractionFormat(object sender, RoutedEventArgs e)
        {
            showAsFraction = true;
        }

        private void SetNumberFormat(object sender, RoutedEventArgs e)
        {
            showAsFraction = false;
        }

        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Калькулятор простых дробей\nРазработчик: Номоконов Д.И. ИП-112", "О программе");
        }

        private string EvaluateExpression(string expression)
        {
            string[] tokens = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length < 3) 
                return expression;

            Fraction result = Fraction.Parse(tokens[0]);
            for (int i = 1; i < tokens.Length - 1; i += 2)
            {
                string op = tokens[i];
                Fraction next = Fraction.Parse(tokens[i + 1]);

                result = op switch
                {
                    "+" => result + next,
                    "-" => result - next,
                    "*" => result * next,
                    "/" => result / next,
                    _ => throw new InvalidOperationException("Unknown operator")
                };
            }

            return result.ToString();
        }

        private void ParseFraction(string fraction, out int numerator, out int denominator)
        {
            string trimmedFraction = fraction.Trim();
            string[] parts = trimmedFraction.Split(new[] { '/', '|' }, StringSplitOptions.RemoveEmptyEntries);

            numerator = int.Parse(parts[0]);
            denominator = parts.Length > 1 ? int.Parse(parts[1]) : 1;
        }

        private string SimplifyFraction(int numerator, int denominator)
        {
            int gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;

            if (!showAsFraction && denominator == 1)
                return numerator.ToString();

            return $"{numerator} / {denominator}";
        }

        private int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return Math.Abs(a);
        }
    }
}