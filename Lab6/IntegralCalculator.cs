
namespace Lab6
{
    public class IntegralCalculator
    {
        private double a;
        private double b;
        private int n;

        public IntegralCalculator(double a, double b, int n)
        {
            this.a = a;
            this.b = b;
            this.n = n;
        }

        public async IAsyncEnumerable<(double x, double S, double progress)> CalculateAsync()
        {
            double h = (b - a) / n;
            double sum = 0.0;


            for (int i = 0; i <= n; i++)
            {
                double x = a + h * i;

                if (i < n)
                {
                    sum += Function(x);
                }

                double currentIntegral = sum * h;
                double progress = (double)i / n;

                // Небольшая задержка для наглядности
                if (i % 10 == 0) // Обновляем каждые 10 итераций
                {
                    await Task.Delay(10);
                }

                yield return (x, currentIntegral, progress);
            }
        }

        private double Function(double x)
        {
            return x*x*x;
        }
    }
}
