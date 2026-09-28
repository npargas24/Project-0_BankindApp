/* Account class with different types -- Like admin is a child class or customer child class shared functionality in the parent class w
while specific admin/user actions in their respectove classes 
things in together could be like login/logout function
Also have unauthenticated class
unAuthenticated can onloy read from DB         Composition design pattern with account manager

// 		1. Check Account Details
// 		2. Withdraw
// 		3. Deposit
// 		4. Transfer
// 		5. Last 5 transactions
// 		6. Request Cheque Book
// 		7. Change Password
// 		8. Exit

Needs to be in a library and handle db connection*/

//Customer HAS A Account
//Account MUST link to customer -- how ensure?

using System.ComponentModel.DataAnnotations;

namespace BankingApp
{
    public enum AccountType
    {
        Checking,
        Savings
    }
    public class Account
    {
        #region Properties
        [Key]
        public int AccountNumber {get; set;} //Primary Key
        public int CustomerId {get; set;} // FK that links to the customer
        public Customer Customer {get; set;} // Navigation property
        //public AccountType AccountType {get; set;}
        public decimal AvailableBalance {get; set;}
        public bool IsAccActive {get; set;}
        //Hold records of the transactions
        public ICollection<Transaction> Transactions {get; set;} = new List<Transaction>();
        public ICollection<ChequeBookRequest> ChequeBookRequests {get; set;} = new List<ChequeBookRequest>();

        #endregion

        #region Methods
        public decimal Withdraw(decimal amount)
        {
            if (!IsAccActive)
            {
                throw new InvalidOperationException(
                    "Cannot withdraw from an inactive account.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Withdrawal amount must be greater than zero.");
            }

            if (amount > AvailableBalance)
            {
                throw new InvalidOperationException(
                    "Insufficient funds.");
            }

            AvailableBalance -= amount;

            Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Withdrawal,
                TransactionAmount = amount,
                TransactionTime = DateTime.Now
            });

            return AvailableBalance;
        }

        public decimal Deposit(decimal amount)
        {
            if (!IsAccActive)
            {
                throw new InvalidOperationException(
                    "Cannot deposit into an inactive account.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Deposit amount must be greater than zero.");
            }

            AvailableBalance += amount;

            Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Deposit,
                TransactionAmount = amount,
                TransactionTime = DateTime.Now
            });

            return AvailableBalance;
        }
        public void Transfer(Account destinationAccount, decimal amount)
        {
            if (!IsAccActive || !destinationAccount.IsAccActive)
            {
                throw new InvalidOperationException(
                    "Both accounts must be active.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Transfer amount must be greater than zero.");
            }

            if (amount > AvailableBalance)
            {
                throw new InvalidOperationException(
                    "Insufficient funds.");
            }

            if (destinationAccount.AccountNumber == AccountNumber)
            {
                throw new InvalidOperationException(
                    "Cannot transfer to the same account.");
            }

            AvailableBalance -= amount;
            destinationAccount.AvailableBalance += amount;

            Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Transfer,
                TransactionAmount = amount,
                TransactionTime = DateTime.Now
            });

            destinationAccount.Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Transfer,
                TransactionAmount = amount,
                TransactionTime = DateTime.Now
            });
        }
       public List<Transaction> RetrieveTransactions()
        {
            return Transactions
                .OrderByDescending(t => t.TransactionTime)
                //Get first 5
                .Take(5)
                .ToList();
        }
        public ChequeBookRequest RequestChequeBook()
        {
            if (!IsAccActive)
            {
                throw new InvalidOperationException(
                    "Cannot request a cheque book for an inactive account.");
            }
                                    //Do ANY items in the collection meet the conditions
            bool alreadyRequested = ChequeBookRequests.Any(
                r => r.RequestStatus == RequestStatus.Submitted ||
                    r.RequestStatus == RequestStatus.InProgress);

            if (alreadyRequested)
            {
                throw new InvalidOperationException(
                    "A cheque book request is already pending.");
            }

            ChequeBookRequest request = new ChequeBookRequest
            {
                RequestStatus = RequestStatus.Submitted
            };

            ChequeBookRequests.Add(request);

            return request;
        }
        #endregion
    }
}