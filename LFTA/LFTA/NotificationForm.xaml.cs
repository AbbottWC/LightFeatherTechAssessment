using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LFTA {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class NotificationForm : Window {
		Key[ ] validKeys = new Key[ ]{
			Key.D1,
			Key.D2,
			Key.D3,
			Key.D4,
			Key.D5,
			Key.D6,
			Key.D7,
			Key.D8,
			Key.D9,
			Key.D0,
			Key.NumPad1,
			Key.NumPad2,
			Key.NumPad3,
			Key.NumPad4,
			Key.NumPad5,
			Key.NumPad6,
			Key.NumPad7,
			Key.NumPad8,
			Key.NumPad9,
			Key.NumPad0,
			Key.Back,
			Key.Delete,
			Key.Left,
			Key.Right,
			Key.End,
			Key.Home
		};

		public NotificationForm( ) {
			InitializeComponent( );
		}

		private void Button_Click(object sender, RoutedEventArgs e) =>
			(this.DataContext as NotificationFormModel)?.Submit( );

		private void FilterNonValidInput(object sender, KeyEventArgs e) =>
			e.Handled = !validKeys.Contains(e.Key);
	}
}
