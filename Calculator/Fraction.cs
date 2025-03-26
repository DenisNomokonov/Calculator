namespace Calculator
{
    public class Fraction
    {
        public int Numerator { get; private set; }

        public int Denominator { get; private set; }

        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
                throw new ArgumentException("Denominator is zero.");

            int gcd = GCD(Math.Abs(numerator), Math.Abs(denominator));
            Numerator = numerator / gcd;
            Denominator = denominator / gcd;

            if (Denominator < 0)
            {
                Numerator = -Numerator;
                Denominator = -Denominator;
            }
        }

        public static Fraction Parse(string input)
        {
            if (int.TryParse(input, out int wholeNumber))
                return new Fraction(wholeNumber, 1);

            string[] parts = input.Split('/');

            if (parts.Length == 2 && int.TryParse(parts[0], out int num) && int.TryParse(parts[1], out int denom))
                return new Fraction(num, denom);

            throw new FormatException("Invalid fraction format.");
        }

        public static Fraction operator +(Fraction a, Fraction b) => 
            new Fraction(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);

        public static Fraction operator -(Fraction a, Fraction b) => 
            new Fraction(a.Numerator * b.Denominator - b.Numerator * a.Denominator, a.Denominator * b.Denominator);

        public static Fraction operator *(Fraction a, Fraction b) => 
            new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Fraction operator /(Fraction a, Fraction b) => 
            new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);

        public override string ToString() => Denominator == 1 ? Numerator.ToString() : $"{Numerator} / {Denominator}";

        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }
}
