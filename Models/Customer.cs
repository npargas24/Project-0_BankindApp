// Please enter username

// 	Please enter Password 

// 	(for initial skeleton, time being use a hardcoded username and password) - later we will check from database)

// 	if uName and Pwd are correct display below options else display Invalid Credential and display step 5 screen again

// 		1. Check Account Details
// 		2. Withdraw
// 		3. Deposit
// 		4. Transfer
// 		5. Last 5 transactions
// 		6. Request Cheque Book
// 		7. Change Password
// 		8. Exit
//customer inherits from user
namespace BankingApp
{
    
public class Customer : User
{
    #region Properties
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string Address {get; set;}
    public string Email {get; set;}
    public Account? Account {get; set;}
    #endregion

    #region Methods
    // public string changePassword(string newPassword)
    //     {
    //         bool hasDigit = newPassword.Any(char.IsDigit);
    //         bool hasSymbol = newPassword.Any(char.IsSymbol);
    //         if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8 || !hasDigit || !hasSymbol)
    //         {
    //             throw new Exception("New password must be greater than 8 characters, contain a number, and contain a symbol. ");
    //         }
    //         else
    //         {
    //             Password = newPassword;
    //             return Password;
    //         }
    //     }
    public bool ChangePassword(string currentPassword, string newPassword)
    {
        if (!VerifyPassword(currentPassword))
        {
            return false;
        }

        SetPassword(newPassword);
        MustChangePassword = false;

        return true;
    }
    #endregion


}
}
