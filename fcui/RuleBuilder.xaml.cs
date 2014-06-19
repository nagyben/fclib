﻿using System;
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

		public RuleBuilder(Rule rule, int id) {
			InitializeComponent();

			this.CurrentRule = rule;
			this.id = id;

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

		private void btn_OK_Click(object sender, RoutedEventArgs e) {
			this.CurrentRule = new Rule(this.id,
										this.txt_ParentDirectory.Text,
										this.txt_TargetDirectory.Text,
										this.txt_Extensions.Text.Split(',').ToList<string>(),
										this.txt_Filters.Text.Split(',').ToList<string>()); // TODO: implement rule name box

			this.DialogResult = true;
			this.Close();
		}
	}
}
