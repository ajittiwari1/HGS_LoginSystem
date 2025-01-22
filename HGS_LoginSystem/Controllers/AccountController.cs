using System;
using System.Linq;
using System.Web.Mvc;
using BCrypt.Net;
using Dapper;
using HGS_LoginSystem.Models;

public class AccountController : Controller
{
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string userID, string password)
    {
        using (var db = DatabaseHelper.GetConnection())
        {
            var user = db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE UserID = @UserID", new { UserID = userID });

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Session["IsAuthenticated"] = true;
                return RedirectToAction("LandingPage");
            }
        }

        ViewBag.Error = "Invalid User ID or Password!";
        return View();
    }

    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Register(User user, string confirmPassword)
    {
        if (user.PasswordHash != confirmPassword)
        {
            ViewBag.Error = "Passwords do not match!";
            return View();
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        using (var db = DatabaseHelper.GetConnection())
        {
            db.Execute("INSERT INTO Users VALUES (@UserID, @UserName, @DateOfBirth, @Email, @Gender, @Status, @PasswordHash)", user);
        }

        return RedirectToAction("Login");
    }

    public ActionResult LandingPage()
    {
        if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
        {

            return RedirectToAction("Login");
        }
        
        

        using (var db = DatabaseHelper.GetConnection())
        {
            var users = db.Query<User>("SELECT * FROM Users").ToList();

            
            return View(users);
        }
    }
    public ActionResult Logout()
    {
        Session["IsAuthenticated"] = null; 
        return RedirectToAction("Login");
    }

    public ActionResult ResetPassword()
    {
        return View();
    }
    [HttpPost]

    
    public ActionResult ResetPassword(string userID, string email, DateTime dob)
    {
        System.Diagnostics.Debug.WriteLine($"UserID: {userID}, Email: {email}, DOB: {dob}");

        bool isVerified = VerifyUserDetails(userID, email, dob);

        if (isVerified)
        {
            TempData["UserID"] = userID;
            return RedirectToAction("SetNewPassword");
        }
        else
        {
            ViewBag.Error = "User details do not match. Please check and try again.";
            return View();
        }
    }


    // Simulated method to verify the user details
    private bool VerifyUserDetails(string userID, string email, DateTime dob)
    {
        using (var db = DatabaseHelper.GetConnection())
        {
            // Convert the input date to match the database format (yyyy-MM-dd)
            string formattedDOB = dob.ToString("yyyy-MM-dd");

            var user = db.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE UserID = @UserID AND Email = @Email AND DateOfBirth = @DateOfBirth",
                new { UserID = userID, Email = email, DateOfBirth = formattedDOB });

            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine("User details do not match.");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("User details verified successfully.");
            return true;
        }
    }

    public ActionResult SetNewPassword()
    {
        return View();
    }
    [HttpPost]
    public ActionResult SetNewPassword(string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "Passwords do not match. Please try again.";
            return View();
        }

        // Retrieve user ID from TempData
        string userID = TempData["UserID"] as string;

        if (string.IsNullOrEmpty(userID))
        {
            ViewBag.Error = "Session expired. Please restart the reset password process.";
            return RedirectToAction("ResetPassword");
        }

        // Update the password in the database
        using (var db = DatabaseHelper.GetConnection())
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            db.Execute("UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID",
                       new { PasswordHash = hashedPassword, UserID = userID });
        }

        ViewBag.Message = "Your password has been updated successfully.";
        return RedirectToAction("Login");
    }


}