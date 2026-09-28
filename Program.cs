using Microsoft.EntityFrameworkCore;

namespace BankingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using BankContext db = new BankContext();

            Admin? admin = db.Users
                .OfType<Admin>()
                .FirstOrDefault(a => a.Username == "admin");

            if (admin == null)
            {
                admin = new Admin
                {
                    Username = "natalie",
                    MustChangePassword = false
                };

                admin.SetPassword("natalieAdmin@123");

                db.Users.Add(admin);
                db.SaveChanges();

                Console.WriteLine("Initial admin created.");
            }
            else
            //Change this (repetitive)
            {
                admin.SetPassword("natalieAdmin@123");
                admin.MustChangePassword = false;

                db.SaveChanges();
            }

            Menus menus = new Menus(db);
            menus.DisplayWelcomeMenu();
        }
    }
}