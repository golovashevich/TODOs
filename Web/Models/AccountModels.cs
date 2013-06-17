using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Web.Models {
	public class UsersContext : DbContext {
		public UsersContext()
			: base("DefaultConnection") {
		}

		public DbSet<UserProfile> UserProfiles { get; set; }
	}

	[Table("UserProfile")]
	public class UserProfile {
		[Key]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }
		public string UserName { get; set; }


		[ForeignKey("UserId")]
		public Membership Membership { get; set; }
	}

	[Table("webpages_Membership")]
	public class Membership {
		[Key]
		public int UserId { get; set; }

		public Nullable<DateTime> CreateDate { get; set; }

		public string ConfirmationToken { get; set; }

		public bool IsConfirmed { get; set; }

		public Nullable<DateTime> LastPasswordFailureDate { get; set; }

		public int PasswordFailuresSinceLastSuccess { get; set; }

		public string Password { get; set; }

		public Nullable<DateTime> PasswordChangedDate { get; set; }

		public string PasswordSalt { get; set; }

		public string PasswordVerificationToken { get; set; }

		public Nullable<DateTime> PasswordVerificationTokenExpirationDate { get; set; }
	}

	public class RegisterExternalLoginModel {
		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		public string ExternalLoginData { get; set; }
	}

	public class LocalPasswordModel {
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginModel {
		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}

	public class RegisterModel {
		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class ExternalLogin {
		public string Provider { get; set; }
		public string ProviderDisplayName { get; set; }
		public string ProviderUserId { get; set; }
	}
}
