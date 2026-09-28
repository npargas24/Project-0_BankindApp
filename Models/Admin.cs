// When user chooses Admin, , display login screen

// 	Please enter username

// 	Please enter Password

// 	(for initial skeleton, time being use a hardcoded username and password) - later we will check from database)

// 	if uName and Pwd are correct display below options else display Invalid Credential and display step 5 screen again

// 		1. Create New Account
// 		2. Delete Account
// 		3. Edit Account Details
// 		4. Display Summary
// 		5. Reset Customer Password
// 		6. Approve Cheque book request
// 		7. Exit

//admin inherits from user
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingApp
{
    public class Admin : User
    {
       #region Methods
       /*Create Account
       Edit Account
       Delete Account
       GetAccountSummary
       ChangeCustomerPassword
       ApproveChequeRequest*/
       #endregion
       public string CreateAccount(
            BankContext db,
            string username,
            string firstName,
            string lastName,
            string address,
            string email,
            decimal openingBalance)
        {
            Customer customer = new Customer
            {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                Email = email,
                MustChangePassword = true
            };

            string temporaryPassword =
                User.GenerateTemporaryPassword();

            customer.SetPassword(temporaryPassword);

            Account account = new Account
            {
                AvailableBalance = openingBalance,
                IsAccActive = true,
                Customer = customer
            };

            customer.Account = account;

            db.Users.Add(customer);
            db.Accounts.Add(account);

            db.SaveChanges();

            return temporaryPassword;
        }

        public bool DeleteAccount(BankContext db, int accountNumber)
        {
            Account? account = db.Accounts
            //Finds first account matching account number or the default (null)
                .FirstOrDefault(account => account.AccountNumber == accountNumber);

            if (account == null)
            {
                return false;
            }

            account.IsAccActive = false;

            db.SaveChanges();

            return true;
        }

        public bool ApproveChequeRequest(
            BankContext db,
            string username)
        {
            ChequeBookRequest? request = db.ChequeBookRequests
                .Include(r => r.Account)
                .ThenInclude(a => a.Customer)
                .FirstOrDefault(r =>
                    r.Account.Customer.Username == username &&
                    r.RequestStatus == RequestStatus.Submitted);

            if (request == null)
            {
                return false;
            }

            request.RequestStatus = RequestStatus.Approved;

            db.SaveChanges();

            return true;
        }

        public Account? GetAccountSummary(BankContext db, string username)
        {
            Account? account = db.Accounts
                .Include(a => a.Customer)
                .FirstOrDefault(a => a.Customer.Username == username);

            return account;
        }

        public string? ResetCustomerPassword(BankContext db, string username)
        {
            Customer? customer = db.Users
            //use OfType because Table Per Hierarchy (TPH) discriminator can be Customer or Admin
                .OfType<Customer>()
                .FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return null;
            }

            string temporaryPassword =
                User.GenerateTemporaryPassword();

            customer.SetPassword(temporaryPassword);

            customer.MustChangePassword = true;

            db.SaveChanges();

            return temporaryPassword;
        }
        
    }
}