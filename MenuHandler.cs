using Microsoft.EntityFrameworkCore;

namespace BankingApp
{
    public class Menus
    {
        private readonly BankContext db;

        public Menus(BankContext context)
        {
            db = context;
        }

        #region Welcome Menu

        public void DisplayWelcomeMenu()
        {
            bool continueProgram = true;

            while (continueProgram)
            {
                Console.WriteLine("Welcome to the Bank. Select from the menu below:");
                Console.WriteLine("1. Customer");
                Console.WriteLine("2. Admin");
                Console.WriteLine("3. Exit");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayCustomerLogin();
                        break;

                    case "2":
                        DisplayAdminLogin();
                        break;

                    case "3":
                        continueProgram = false;
                        break;

                    default:
                        Console.WriteLine("Invalid Choice.");
                        break;
                }
            }
        }

        #endregion

        #region Customer Login

        public void DisplayCustomerLogin()
        {
            Console.WriteLine("Please enter your username:");
            string? username = Console.ReadLine();

            Console.WriteLine("Please enter your password:");
            string? password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username and password cannot be empty.");
                return;
            }

            Customer? customer = db.Users
                .OfType<Customer>()
                .Include(c => c.Account)
                .ThenInclude(a => a.Transactions)
                .Include(c => c.Account)
                .ThenInclude(a => a.ChequeBookRequests)
                .FirstOrDefault(c => c.Username == username);

            if (customer == null || !customer.VerifyPassword(password))
            {
                Console.WriteLine("Invalid Credential.");
                return;
            }

            if (customer.MustChangePassword)
            {
                Console.WriteLine("You must change your temporary password.");

                ChangeCustomerPassword(customer);

                if (customer.MustChangePassword)
                {
                    return;
                }
            }

            DisplayCustomerMenu(customer);
        }

        #endregion

        #region Customer Menu

        public void DisplayCustomerMenu(Customer customer)
        {
            bool continueCustomerMenu = true;

            while (continueCustomerMenu)
            {
                Console.WriteLine("1. Check Account Details");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Deposit");
                Console.WriteLine("4. Transfer");
                Console.WriteLine("5. Last 5 Transactions");
                Console.WriteLine("6. Request Cheque Book");
                Console.WriteLine("7. Change Password");
                Console.WriteLine("8. Exit");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (customer.Account == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        Console.WriteLine($"Account Number: {customer.Account.AccountNumber}");
                        Console.WriteLine($"Balance: {customer.Account.AvailableBalance:C}");
                        Console.WriteLine($"Active: {customer.Account.IsAccActive}");
                        break;

                    case "2":
                        if (customer.Account == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        Console.WriteLine("Enter withdrawal amount:");
                        string? withdrawalInput = Console.ReadLine();

                        if (!decimal.TryParse(withdrawalInput, out decimal withdrawalAmount))
                        {
                            Console.WriteLine("Invalid amount.");
                            break;
                        }

                        try
                        {
                            customer.Account.Withdraw(withdrawalAmount);
                            db.SaveChanges();

                            Console.WriteLine("Withdrawal successful.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        break;

                    case "3":
                        if (customer.Account == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        Console.WriteLine("Enter deposit amount:");
                        string? depositInput = Console.ReadLine();

                        if (!decimal.TryParse(depositInput, out decimal depositAmount))
                        {
                            Console.WriteLine("Invalid amount.");
                            break;
                        }

                        try
                        {
                            customer.Account.Deposit(depositAmount);
                            db.SaveChanges();

                            Console.WriteLine("Deposit successful.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        break;

                    case "4":
                        if (customer.Account == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        Console.WriteLine("Enter destination account number:");
                        string? accountInput = Console.ReadLine();

                        if (!int.TryParse(accountInput, out int accountNumber))
                        {
                            Console.WriteLine("Invalid account number.");
                            break;
                        }

                        Console.WriteLine("Enter transfer amount:");
                        string? transferInput = Console.ReadLine();

                        if (!decimal.TryParse(transferInput, out decimal transferAmount))
                        {
                            Console.WriteLine("Invalid amount.");
                            break;
                        }

                        Account? destinationAccount = db.Accounts
                            .FirstOrDefault(a => a.AccountNumber == accountNumber);

                        if (destinationAccount == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        try
                        {
                            customer.Account.Transfer(
                                destinationAccount,
                                transferAmount);

                            db.SaveChanges();

                            Console.WriteLine("Transfer successful.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        break;

                    case "5":
                        if (customer.Account == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        List<Transaction> transactions =
                            customer.Account.RetrieveTransactions();

                        if (transactions.Count == 0)
                        {
                            Console.WriteLine("No transactions found.");
                            break;
                        }

                        foreach (Transaction transaction in transactions)
                        {
                            Console.WriteLine(
                                $"{transaction.TransactionType} " +
                                $"{transaction.TransactionAmount:C} " +
                                $"{transaction.TransactionTime}");
                        }

                        break;

                    case "6":
                        if (customer.Account == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        try
                        {
                            customer.Account.RequestChequeBook();

                            db.SaveChanges();

                            Console.WriteLine("Cheque book requested.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        break;

                    case "7":
                        ChangeCustomerPassword(customer);
                        break;

                    case "8":
                        continueCustomerMenu = false;
                        break;

                    default:
                        Console.WriteLine("Invalid Choice.");
                        break;
                }
            }
        }

        #endregion

        #region Change Customer Password

        private void ChangeCustomerPassword(Customer customer)
        {
            Console.WriteLine("Enter current password:");
            string? currentPassword = Console.ReadLine();

            Console.WriteLine("Enter new password:");
            string? newPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("Password cannot be empty.");
                return;
            }

            try
            {
                bool changed = customer.ChangePassword(
                    currentPassword,
                    newPassword);

                if (!changed)
                {
                    Console.WriteLine("Current password is incorrect.");
                    return;
                }

                db.SaveChanges();

                Console.WriteLine("Password changed successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Admin Login

        public void DisplayAdminLogin()
        {
            Console.WriteLine("Please enter your username:");
            string? username = Console.ReadLine();

            Console.WriteLine("Please enter your password:");
            string? password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username and password cannot be empty.");
                return;
            }

            Admin? admin = db.Users
                .OfType<Admin>()
                .FirstOrDefault(a => a.Username == username);

            if (admin == null || !admin.VerifyPassword(password))
            {
                Console.WriteLine("Invalid Credential.");
                return;
            }

            DisplayAdminMenu(admin);
        }

        #endregion

        #region Admin Menu

        public void DisplayAdminMenu(Admin admin)
        {
            bool continueAdminMenu = true;

            while (continueAdminMenu)
            {
                Console.WriteLine("1. Create New Account");
                Console.WriteLine("2. Delete Account");
                Console.WriteLine("3. Edit Account Details");
                Console.WriteLine("4. Display Summary");
                Console.WriteLine("5. Reset Customer Password");
                Console.WriteLine("6. Approve Cheque book request");
                Console.WriteLine("7. Exit");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                Console.WriteLine("Enter username:");
                string? newUsername = Console.ReadLine();

                Console.WriteLine("Enter first name:");
                string? firstName = Console.ReadLine();

                Console.WriteLine("Enter last name:");
                string? lastName = Console.ReadLine();

                Console.WriteLine("Enter address:");
                string? address = Console.ReadLine();

                Console.WriteLine("Enter email:");
                string? email = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newUsername) ||
                    string.IsNullOrWhiteSpace(firstName) ||
                    string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(address) ||
                    string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("All fields are required.");
                    break;
                }

                Console.WriteLine("Enter opening balance:");
                string? balanceInput = Console.ReadLine();

                if (!decimal.TryParse(balanceInput, out decimal openingBalance))
                {
                    Console.WriteLine("Invalid balance.");
                    break;
                }

                if (openingBalance < 0)
                {
                    Console.WriteLine("Opening balance cannot be negative.");
                    break;
                }

                try
                {
                    string newCustomerPassword = admin.CreateAccount(
                        db,
                        newUsername,
                        firstName,
                        lastName,
                        address,
                        email,
                        openingBalance);

                    Console.WriteLine("Account created successfully.");
                    Console.WriteLine($"Temporary Password: {newCustomerPassword}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating account: {ex.Message}");
                }

                break;

                    case "2":
                        Console.WriteLine("Enter account number:");
                        string? accountInput = Console.ReadLine();

                        if (!int.TryParse(accountInput, out int accountNumber))
                        {
                            Console.WriteLine("Invalid account number.");
                            break;
                        }

                        bool deleted =
                            admin.DeleteAccount(db, accountNumber);

                        if (deleted)
                        {
                            Console.WriteLine("Account deactivated.");
                        }
                        else
                        {
                            Console.WriteLine("Account not found.");
                        }

                        break;

                    case "3":
                        // Call EditAccount when finished
                        break;

                    case "4":
                    Console.WriteLine("Enter customer username:");
                    string? summaryUsername = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(summaryUsername))
                    {
                        Console.WriteLine("Username cannot be empty.");
                        break;
                    }

                    Account? account =
                        admin.GetAccountSummary(
                            db,
                            summaryUsername);

                    if (account == null)
                    {
                        Console.WriteLine("Account not found.");
                        break;
                    }

                    Console.WriteLine($"Account Number: {account.AccountNumber}");
                    Console.WriteLine(
                        $"Customer: {account.Customer.FirstName} " +
                        $"{account.Customer.LastName}");
                    Console.WriteLine($"Username: {account.Customer.Username}");
                    Console.WriteLine($"Balance: {account.AvailableBalance:C}");
                    Console.WriteLine($"Active: {account.IsAccActive}");

                    break;

                    case "5":
                        Console.WriteLine("Enter customer username:");
                        string? customerUsername = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(customerUsername))
                        {
                            Console.WriteLine("Username cannot be empty.");
                            break;
                        }

                        string? temporaryPassword =
                            admin.ResetCustomerPassword(
                                db,
                                customerUsername);

                        if (temporaryPassword == null)
                        {
                            Console.WriteLine("Customer not found.");
                        }
                        else
                        {
                            Console.WriteLine("Password reset.");
                            Console.WriteLine(
                                $"Temporary Password: {temporaryPassword}");
                        }

                        break;

                    case "6":
                        Console.WriteLine("Enter customer username:");
                        string? chequeUsername = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(chequeUsername))
                        {
                            Console.WriteLine("Username cannot be empty.");
                            break;
                        }

                        bool approved =
                            admin.ApproveChequeRequest(
                                db,
                                chequeUsername);

                        if (approved)
                        {
                            Console.WriteLine("Cheque book request approved.");
                        }
                        else
                        {
                            Console.WriteLine(
                                "No pending cheque book request found.");
                        }

                        break;

                    case "7":
                        continueAdminMenu = false;
                        break;

                    default:
                        Console.WriteLine("Invalid Choice.");
                        break;
                }
            }
        }

        #endregion
    }
}