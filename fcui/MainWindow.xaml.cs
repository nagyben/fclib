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

		public MainWindow() {
			DataContext = this;
			InitializeComponent();

			try {
				RuleList = UserData.LoadRules("rules.dat");
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}

			lv_RuleList.ItemsSource = RuleList;
		}

		private void btn_AddRule_Click(object sender, RoutedEventArgs e) {

			// RuleBuilder needs to be given the id of the rule, which only the main window knows
			// It is equal to the current count of Rules in the rulelist
			RuleBuilder Builder = new RuleBuilder(RuleList.Count());
			Builder.ShowDialog();

			// The result is stored in the RuleList
			if ((bool)Builder.DialogResult == true) {
				this.RuleList.Add(Builder.CurrentRule);
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

		private void MoveRuleUp(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			object CP = b.CommandParameter;
		}
	}
}
