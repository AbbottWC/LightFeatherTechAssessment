using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LFTA {
	public class NotificationFormModel : INotifyPropertyChanged {
		public NotificationFormModel( ) => this.GetSupervisors( );
		#region field values		
		private string _firstName;
		private string _lastName;
		private bool _contactByEmail;
		private string _email;
		private bool _contactByPhone;
		private string _phoneNumber;
		private Supervisor _selectedSupervisor;
		private IEnumerable<Supervisor> _supervisors;
		#endregion
		#region properties
		public string FirstName {
			get => this._firstName;
			set {
				this._firstName = value;
				this.OnPropertyChanged("FirstName");
			}
		}
		public string LastName {
			get => this._lastName;
			set {
				this._lastName = value;
				this.OnPropertyChanged("LastName");
			}
		}
		public bool ContactByEmail {
			get => this._contactByEmail;
			set {
				this._contactByEmail = value;
				if( !value )
					this.Email = string.Empty;
				this.OnPropertyChanged("ContactByEmail");
			}
		}
		public string Email {
			get => this._email;
			set {
				this._email = value;
				this.OnPropertyChanged("Email");
			}
		}
		public bool ContactByPhone {
			get => this._contactByPhone;
			set {
				this._contactByPhone = value;
				if( !value )
					this.PhoneNumber = string.Empty;
				this.OnPropertyChanged("ContactByPhone");
			}
		}
		public string PhoneNumber {
			get => this._phoneNumber;
			set {
				this._phoneNumber = value;
				this.OnPropertyChanged("PhoneNumber");
			}
		}
		public IEnumerable<Supervisor> Supervisors {
			get => this._supervisors;
			private set {
				this._supervisors = value;
				this.OnPropertyChanged("Supervisors");
			}
		}
		public Supervisor SelectedSupervisor {
			get => this._selectedSupervisor;
			set {
				this._selectedSupervisor = value;
				this.OnPropertyChanged("SelectedSupervisor");
			}
		}
		#endregion
		#region Methods
		public async void GetSupervisors( ) {
			var endPoint = "https://o3m5qixdng.execute-api.us-east-1.amazonaws.com/api/managers";
			try {
				using( var client = new HttpClient( ) ) {
					using( var response = await client.GetAsync(endPoint) ) {
						using( var content = response.Content ) {
							//Get the data. We now have to parse it into a collection of Supervisor objects.
							var data = await content.ReadAsStringAsync( );
							int outInt;
							this.Supervisors = JsonConvert.DeserializeObject<List<Supervisor>>(data
								).Where(sv => !int.TryParse(sv.jurisdiction, out outInt)
								).OrderBy(sv => sv.jurisdiction
								).ThenBy(sv => string.Format("{0}, {1}", sv.lastName, sv.firstName));

							MessageBox.Show(
								"Superviser list acquired. Please proceed.",
								"Work Completed.", MessageBoxButton.OK,
								MessageBoxImage.Exclamation);
						}
					}
				}
			}
			catch( Exception e ) {
				MessageBox.Show(
					string.Format(
						@"There was a problem attempting to acquire the supervisor's list.
The program will now terminate.
Error Message: {0}", e.Message),
					"Failed to get Supervisor List.", MessageBoxButton.OK, MessageBoxImage.Error);
				Application.Current.Shutdown( );
			}
		}
		public void Submit( ) {
			bool proceed = true;
			var MFV = "Field Value Missing";
			if( string.IsNullOrEmpty(this.FirstName) ) {
				proceed = false;
				MessageBox.Show(
					"Please enter something into the 'First Name' field.", MFV,
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			if( string.IsNullOrEmpty(this.LastName) ) {
				proceed = false;
				MessageBox.Show(
					"Please enter something into the 'Last Name' field.", MFV,
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			if( this.ContactByEmail ) {
				if( !EmailValidationRule.IsValidEmail(this.Email) ) {
					proceed = false;
					MessageBox.Show(
						"Please enter a valid email address, or untick the 'Contact By Email' checkbox.",
						MFV, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			if( this.ContactByPhone) {
				if( !PhoneNumberValidationRule.IsValidPhoneNumber(this.PhoneNumber)){
					
					proceed = false;
					MessageBox.Show(
						"Please enter a valid phone number, or untick the 'Contact By Phone' checkbox.",
						MFV, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			if (this.SelectedSupervisor == null ) {
				proceed = false;
				MessageBox.Show(
					"Please select a Supervisor, then press 'Submit' to continue.", "No Supervisor Selected",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}

			if( !proceed ) {
				MessageBox.Show(
					@"There were one or more errors on the form.
Please resolve these errors, then click 'Submit'.",
					"Errors detected. Please resolve, then try again.",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else {
				var output = string.Format(
					@"First Name: {1}{0}
Last Name: {2}{0}
Email: {3}{0}
Phone: {4}{0}
Supervisor: {5}{0}",
					Environment.NewLine,
					this.FirstName, this.LastName,
					!this.ContactByEmail || string.IsNullOrEmpty(this.Email) ? "N/A" : this.Email,
					!this.ContactByPhone || string.IsNullOrEmpty(this.PhoneNumber) ? "N/A" : new string(
						this.PhoneNumber.Where(char.IsDigit).ToArray( )),
					this.SelectedSupervisor.ToString( ));
				Console.WriteLine(output);
			}
		}
		#endregion
		#region INotifyPropertyChanged Implementation
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName) =>
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		#endregion
	}
	public class NameValidationRule : ValidationRule {
		public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
			if( string.IsNullOrEmpty(value?.ToString( )) )
				return new ValidationResult(false, "Please enter a name.");
			return ValidationResult.ValidResult;
		}
	}
	public class EmailValidationRule : ValidationRule {
		public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
			var email = string.Empty;
			if( value is string ) {
				//Proceed
				email = value.ToString( );
				if( IsValidEmail(email) )
					return ValidationResult.ValidResult;
				else
					return new ValidationResult(false, "Please provide a valid email address.");
			}
			else
				return new ValidationResult(false, @"Value is not a parseable string (Just... How?)");
		}

		public static bool IsValidEmail(string email) {
			if( string.IsNullOrWhiteSpace(email) )
				return true;
			try {
				email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
				string DomainMapper(Match match) {
					var idn = new IdnMapping( );
					string domainName = idn.GetAscii(match.Groups[2].Value);
					return match.Groups[1].Value + domainName;
				}
			}
			catch( RegexMatchTimeoutException ) {
				return false;
			}
			catch( ArgumentException ) {
				return false;
			}

			try {
				return Regex.IsMatch(
					email,
					@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
					RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			}
			catch( RegexMatchTimeoutException ) {
				return false;
			}
		}
	}
	public class PhoneNumberValidationRule : ValidationRule {
		public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
			if (!(value is string))
				return new ValidationResult(false, @"Value is not a parseable string (Just... How?)");
			return IsValidPhoneNumber(value.ToString( ))
				? ValidationResult.ValidResult
				: new ValidationResult(false, "Please enter a valid US Phone Number.");
		}

		public static bool IsValidPhoneNumber(string phoneNumber) {
			if( string.IsNullOrEmpty(phoneNumber) )
				return true;
			if( phoneNumber.Any(char.IsLetter) )
				return false;
			//How many different forms can make up a valid phone number?
			//First, let's strip out the non-digit values.
			phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray( ));
			return phoneNumber.Length == 7 ||
				phoneNumber.Length == 10 ||
				phoneNumber.Length == 11;
		}

	}
}