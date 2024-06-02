using BalanceRetroactiveCalculator.Calculator;


namespace BalanceRetroactiveCalculator;

class Program
{
    static void Main(string[] args)
    {
        var transactions = new List<Transaction>
        {
            new() { Amount = 100, Timestamp = DateTime.Parse("2024-06-02T00:00:02-03:00") },
            new() { Amount = -100, Timestamp = DateTime.Parse("2024-06-02T00:00:01-03:00") },
            new() { Amount = 100, Timestamp = DateTime.Parse("2024-06-02T00:00:00-03:00") },

            new() { Amount = 100, Timestamp = DateTime.Parse("2024-06-01T00:00:02-03:00") },
            new() { Amount = 100, Timestamp = DateTime.Parse("2024-06-01T00:00:01-03:00") },
            new() { Amount = 100, Timestamp = DateTime.Parse("2024-06-01T00:00:00-03:00") },

            new() { Amount = 100, Timestamp = DateTime.Parse("2024-05-31T00:00:02-03:00") },
            new() { Amount = -100, Timestamp = DateTime.Parse("2024-05-31T00:00:01-03:00") },
            new() { Amount = 100, Timestamp = DateTime.Parse("2024-05-31T00:00:00-03:00") },
        };
        var currentBalance = 1000M;

        var calculator = new BalanceRetroactiveCalculatorService();
        var balance = calculator.RetroactiveCalculate(currentBalance, transactions);

        Console.WriteLine("----- RETROACTIVE BALANCE START -----");
        Console.WriteLine($"InitialBalance: {balance.InitialBalance}");
        Console.WriteLine($"TotalCashin: {balance.TotalCashin}");
        Console.WriteLine($"TotalCashout: {balance.TotalCashout}");
        Console.WriteLine("----- RETROACTIVE BALANCE END -----");

        var balances = calculator.RetroactiveCalculateGroupedByDates(currentBalance, transactions);
        foreach (var balanceByDate in balances)
        {
            Console.WriteLine("----- GROUPED BALANCE START -----");
            Console.WriteLine($"DateRef: {balanceByDate.DateRef}");
            Console.WriteLine($"InitialBalance: {balanceByDate.InitialBalance}");
            Console.WriteLine($"TotalCashin: {balanceByDate.TotalCashin}");
            Console.WriteLine($"TotalCashout: {balanceByDate.TotalCashout}");
            Console.WriteLine("----- GROUPED BALANCE END -----");

        }
        Console.WriteLine("Hello, World!");
    }
}
