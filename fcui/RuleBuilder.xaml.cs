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
using System.Windows.Shapes;
using fclib;

namespace fcui {
	/// <summary>
	/// Interaction logic for RuleBuilder.xaml
	/// </summary>
	public partial class RuleBuilder : Window {
		public Rule CurrentRule;
		private int id;

		public RuleBuilder(int id) {
			InitializeComponent();
			this.id = id;
		}

		public RuleBuilder(Rule rule) {
			InitializeComponent();

			this.CurrentRule = rule;
			this.id = rule.ID;

			this.txt_Extensions.Text = String.Join(",", rule.Extensions);
			this.txt_Filters.Text = String.Join(",", rule.Filters);
			this.txt_ParentDirectory.Text = rule.ParentDirectory;
			this.txt_TargetDirectory.Text = rule.TargetDirectory;

			switch (rule.Instruction) {
				case Rule.Operation.Copy:
					this.rd_Copy.IsChecked = true;
					break;
				case Rule.Operation.Move:
					this.rd_Move.IsChecked = true;
					break;
				case Rule.Operation.Delete:
					this.rd_Delete.IsChecked = true;
					break;
				default:
					break;
			}
		}

		private Rule.Operation GetOp() {
			if (this.rd_Copy.IsChecked == true) {return Rule.Operation.Copy;}
			if (this.rd_Move.IsChecked == true) {return Rule.Operation.Move;}
			if (this.rd_Delete.IsChecked == true) {return Rule.Operation.Delete;}
				
			return Rule.Operation.None;
		}

		private void btn_OK_Click(object sender, RoutedEventArgs e) {
			this.CurrentRule = new Rule(this.id,
										this.txt_ParentDirectory.Text,
										this.txt_TargetDirectory.Text,
										this.txt_Extensions.Text.Split(',').ToList<string>(),
										this.txt_Filters.Text.Split(',').ToList<string>(),
										GetOp()); // TODO: implement rule name box

			if (this.CurrentRule.IsValid()) {
				this.DialogResult = true;
				this.Close();
			} else {
				MessageBox.Show("There are some errors in the rule parameters. Please check them again", "Rule error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
