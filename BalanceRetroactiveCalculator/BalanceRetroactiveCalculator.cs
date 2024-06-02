using System.Reflection.Metadata;

namespace BalanceRetroactiveCalculator.Calculator;


class Transaction
{
  public DateTime Timestamp { get; set; }
  public decimal Amount { get; set; }
}

class Balance
{
  public decimal InitialBalance { get; set; }
  public decimal TotalCashin { get; set; }
  public decimal TotalCashout { get; set; }
}

class BalanceByDate : Balance
{
  public required string DateRef { get; set; }
}

interface IBalanceRetroactiveCalculator
{
  Balance RetroactiveCalculate(decimal currentBalance, List<Transaction> transactions);
  List<BalanceByDate> RetroactiveCalculateGroupedByDates(decimal currentBalance, List<Transaction> transactions);
}

class BalanceRetroactiveCalculatorService : IBalanceRetroactiveCalculator
{
  public Balance RetroactiveCalculate(decimal currentBalance, List<Transaction> transactions)
  {
    var seed = new Balance
    {
      InitialBalance = currentBalance,
      TotalCashout = 0,
      TotalCashin = 0
    };

    var retroactiveBalance = transactions.Aggregate(
    seed, (acc, transaction) =>
    {
      var initialBalance = acc.InitialBalance - transaction.Amount;
      var totalCashin = transaction.Amount >= 0 ? acc.TotalCashin + transaction.Amount : acc.TotalCashin;
      var totalCashout = transaction.Amount < 0 ? acc.TotalCashout - transaction.Amount : acc.TotalCashout;

      return new Balance
      {
        InitialBalance = initialBalance,
        TotalCashin = totalCashin,
        TotalCashout = totalCashout,
      };

    });
    return retroactiveBalance;
  }

  public List<BalanceByDate> RetroactiveCalculateGroupedByDates(decimal currentBalance, List<Transaction> transactions)
  {
    var groupedTransactionsByDate = transactions.GroupBy((x) => x.Timestamp.ToString().Substring(0, 10));
    var balances = new Dictionary<string, BalanceByDate>();
    var dates = groupedTransactionsByDate.Select(x => x.Key);
    var initialBalance = currentBalance;
    foreach (var dateRef in dates)
    {
      var transactionsInGroup = groupedTransactionsByDate.First(x => x.Key == dateRef).ToList();
      var calculatedBalance = RetroactiveCalculate(initialBalance, transactionsInGroup);
      initialBalance = calculatedBalance.InitialBalance;
      balances[dateRef] = new BalanceByDate
      {
        DateRef = dateRef,
        InitialBalance = calculatedBalance.InitialBalance,
        TotalCashin = calculatedBalance.TotalCashin,
        TotalCashout = calculatedBalance.TotalCashout,
      };
    }
    return balances.Values.ToList();
  }
}
