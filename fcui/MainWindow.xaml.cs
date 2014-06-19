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
using fclib;

namespace fcui {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		/* MEMBERS */
		private List<Rule> RuleList = new List<Rule>();

		public MainWindow() {
			InitializeComponent();

			try {
				RuleList = UserData.LoadRules("rules.dat");
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}
			
		}

		private void btn_AddRule_Click(object sender, RoutedEventArgs e) {

			// RuleBuilder needs to be given the id of the rule, which only the main window knows
			// It is equal to the current count of Rules in the rulelist
			RuleBuilder Builder = new RuleBuilder(RuleList.Count());
			Builder.ShowDialog();
		}
	}
}
