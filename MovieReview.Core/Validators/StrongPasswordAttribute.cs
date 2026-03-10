using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieReview.Core.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class StrongPasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Password is required");
        }

        string password = value.ToString()!;

        // Check for spaces
        if (password.Contains(" "))
        {
            return new ValidationResult("Don't use Space.");
        }

        // Check allowed characters
        var allowedPattern = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9!@#$%^&*]*$");
        if (!allowedPattern.IsMatch(password))
        {
            foreach (char c in password)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), @"[a-zA-Z0-9!@#$%^&*]"))
                {
                    return new ValidationResult($"Don't use 👉 {c} 👈");
                }
            }
        }

        // Build error message
        StringBuilder finalError = new StringBuilder();

        // Check minimum length
        if (password.Length < 8)
        {
            int needed = 8 - password.Length;
            string word = needed == 1 ? "character" : "characters";
            finalError.Append($"Password must have {needed} more {word}. ");
        }

        // Check missing requirements
        List<string> missingItems = new List<string>();

        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
        {
            missingItems.Add("lowercase letter");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
        {
            missingItems.Add("uppercase letter");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
        {
            missingItems.Add("digit");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*]"))
        {
            missingItems.Add("symbol");
        }

        if (missingItems.Count > 0)
        {
            StringBuilder useText = new StringBuilder("Add 1 ");

            if (missingItems.Count == 1)
            {
                useText.Append(missingItems[0]);
            }
            else if (missingItems.Count == 2)
            {
                string lastItem = missingItems[1];
                string article = lastItem == "symbol" ? "a" : "an";
                useText.Append($"{missingItems[0]} and {article} {lastItem}");
            }
            else
            {
                for (int i = 0; i < missingItems.Count; i++)
                {
                    if (i == missingItems.Count - 1)
                    {
                        string lastItem = missingItems[i];
                        string article = lastItem == "symbol" ? "a" : "an";
                        useText.Append($"and {article} {lastItem}");
                    }
                    else
                    {
                        useText.Append($"{missingItems[i]}, ");
                    }
                }
            }

            finalError.Append(useText.ToString() + ".");
        }

        if (finalError.Length > 0)
        {
            return new ValidationResult(finalError.ToString().Trim());
        }

        return ValidationResult.Success;
    }
}