namespace BankingApp
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }
    public class Transaction
    {
        #region Properties
        public int TransactionID {get; set;} //Primary Key
        public int AccountID{get; set;} //Foreign Key
        public TransactionType TransactionType {get; set;}
        public decimal TransactionAmount {get; set;}
        public DateTime TransactionTime {get; set;}
        
        #endregion
        
        // Withdraw
        // Deposit
        // Transfer
    }
}