using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace BankingApp
{
    public enum RequestStatus
    {
        Submitted,
        InProgress,
        Approved,
        Denied

    }
    public class ChequeBookRequest
    {
        #region Properties
        [Key]
        public int ChequeRequestID {get; set;} //Primary Key
        public int AccountID {get; set;} //Foreign Key
        public Account Account {get; set;}
        public RequestStatus RequestStatus {get; set;}
        #endregion
    }
}
