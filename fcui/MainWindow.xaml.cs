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
using System.Collections.ObjectModel;

namespace fcui {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		/* MEMBERS */
		private ObservableCollection<Rule> RuleList = new ObservableCollection<Rule>();
		private ObservableCollection<RuleFile> FileList = new ObservableCollection<RuleFile>();

		public const string RULE_FILE = "rules.dat";

		public MainWindow() {
			InitializeComponent();

			try {
				RuleList = UserData.LoadRules(RULE_FILE);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}

			// Set the topmost listview's DataContext to the RuleList collection
			lv_RuleList.DataContext = RuleList;
		}

		private void btn_AddRule_Click(object sender, RoutedEventArgs e) {

			// RuleBuilder needs to be given the id of the rule, which only the main window knows
			// It is equal to the current count of Rules in the rulelist
			RuleBuilder Builder = new RuleBuilder(RuleList.Count());
			Builder.ShowDialog();

			// The result is stored in the RuleList
			if ((bool)Builder.DialogResult == true) {
				try {
					this.RuleList.Add(Builder.CurrentRule);
				} catch (Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void btn_DeleteRule_Click(object sender, RoutedEventArgs e) {
			this.RuleList.RemoveAt(lv_RuleList.SelectedIndex);
		}

		private void btn_ModifyRule_Click(object sender, RoutedEventArgs e) {
			int SelectedIndex = this.lv_RuleList.SelectedIndex;
			RuleBuilder Builder = new RuleBuilder(this.RuleList[SelectedIndex]);

			Builder.ShowDialog();
			if ((bool)Builder.DialogResult == true) {
				RuleList[SelectedIndex] = Builder.CurrentRule;
			}
		}

		[Obsolete]
		private void MoveRuleUp(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			object CP = b.CommandParameter;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			// Save user data
			UserData.SaveRules(this.RuleList, RULE_FILE);
		}
	}
}
