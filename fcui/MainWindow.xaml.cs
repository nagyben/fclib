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
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace fcui {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		/* MEMBERS */
		private volatile ObservableCollection<Rule> RuleList = new ObservableCollection<Rule>();
		private volatile ObservableCollection<RuleFile> FileList = new ObservableCollection<RuleFile>();

		public const string RULE_FILE = "rules.dat";

		public MainWindow() {
			InitializeComponent();

			try {
				RuleList = UserData.LoadRules(RULE_FILE);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}

			//ObservableCollection<RuleFile> herpy = new ObservableCollection<RuleFile>(RuleList[0].Execute());
			//FileList = herpy;

			// Set the topmost listview's DataContext to the RuleList collection
			this.lv_RuleList.DataContext = this.RuleList;

			// Set the bottom listview's DataContext to the FileList collection
			this.lv_FileList.DataContext = this.FileList;
			
			// Set up BackgroundWorker handlers
			Searcher.WorkerReportsProgress = true;
			Searcher.WorkerSupportsCancellation = true;
			
			Searcher.DoWork += Searcher_DoWork;
			Searcher.ProgressChanged += Searcher_ProgressChanged;
			Searcher.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Searcher_RunWorkerCompleted);
		}

		#region RULE_BUTTONS

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

		private void btn_AddRule_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			// Rightclick enables creating a rule based on the current selected existing rule

			if (this.lv_RuleList.SelectedIndex == -1) {
				// If no Rule is selected then run simple rule addition
				btn_AddRule_Click(sender, e);
			} else {
				int SelectedIndex = this.lv_RuleList.SelectedIndex;
				RuleBuilder Builder = new RuleBuilder(this.RuleList[SelectedIndex]);

				Builder.ShowDialog();
				if ((bool)Builder.DialogResult == true) {
					try {
						this.RuleList.Add(Builder.CurrentRule);
					} catch (Exception ex) {
						MessageBox.Show(ex.Message);
					}
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

		#endregion

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			// Save user data
			UserData.SaveRules(this.RuleList, RULE_FILE);
		}

		/* BACKGROUNDWORKERS */
		private readonly BackgroundWorker Searcher = new BackgroundWorker();
		private readonly BackgroundWorker Copier = new BackgroundWorker();

		#region SEARCHER_FUNCTIONS

		private void Searcher_DoWork(object sender, DoWorkEventArgs e) {
			// initialize return variable
			List<RuleFile> result = new List<RuleFile>();

			foreach (Rule rule in RuleList) {
				// check if cancel request has been set
				if (e.Cancel == true) {
					break;
				} else {

					// Calculate progress
					int progress = RuleList.IndexOf(rule) / RuleList.Count * 100;
					Searcher.ReportProgress(progress);

					// Execute rule
					try {
						result.AddRange(rule.Execute());
					} catch (Exception ex) {
						MessageBox.Show(ex.Message);
					}
				}
			}
			e.Result = result;
			return;
		}

		private void Searcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			// dummy variable
			/* 
			 * Has to be implemented this way because only the constructor of ObservableCollection can
			 * convert from List<T> to ObservableCollection<T>
			 * Additionally, e.Result must be cast to a List<T> before this can be done
			 */
			ObservableCollection<RuleFile> dummy = new ObservableCollection<RuleFile>((List<RuleFile>)e.Result);

			// Enable buttons
			UIModeNormal();
			
			// ObservableCollection only implements the OnCollectionChanged event for the Add and Remove methods
			// The best solution to-date is a foreach loop

			foreach (RuleFile file in dummy) {
				// Only add if FileList doesn't already contain the file
				if (!FileList.Contains(file)) {
					this.FileList.Add(file);
				}
			}

			return;
		}

		private void Searcher_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			/* LOGIC EXPLANATION
			 * 
			 * This method controls the visibility / clickability of the
			 * FindFiles and Cancel (i.e. cancel finding files) button.
			 * As long as the progress is less than 100%, btn_FindFiles should be greyed out
			 * and btn_CancelFindFiles should be visible and enabled.
			 */

			if (e.ProgressPercentage < 100) {
				// Grey out / enable buttons
				UIModeFindingFiles();

				// update progressbar
				this.pb_FindFiles.Value = e.ProgressPercentage;
				
			}

		}

		#endregion

		#region COPIER_FUNCTIONS

		private void Copier_DoWork(object sender, DoWorkEventArgs e) {
			foreach (RuleFile file in this.FileList) {

				// Check if cancel request is set
				if (e.Cancel == true) {
					// Cancel requested; break from loop
					break;
				} else {
					try {
						// execute RuleFile operation
						file.Execute();
					} catch (Exception ex) {
						MessageBox.Show(ex.Message);
					}
				}
			}
		}

		private void Copier_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

		}

		private void Copier_ProgressChanged(object sender, ProgressChangedEventArgs e) {

		}

		#endregion

		#region UIMODES

		private void UIModeFindingFiles() {
			this.btn_FindFiles.IsEnabled = false;
			this.btn_CancelFindFiles.IsEnabled = true;
			this.pb_FindFiles.Visibility = System.Windows.Visibility.Visible;
			this.txt_Progress.Visibility = System.Windows.Visibility.Visible;
		}

		private void UIModeNormal() {
			this.btn_FindFiles.IsEnabled = true;
			this.btn_CancelFindFiles.IsEnabled = false;
			this.pb_FindFiles.Visibility = System.Windows.Visibility.Hidden;
			this.txt_Progress.Visibility = System.Windows.Visibility.Hidden;
		}

		#endregion

		private void btn_FindFiles_Click(object sender, RoutedEventArgs e) {
			this.btn_FindFiles.IsEnabled = false;
			Searcher.RunWorkerAsync();
		}

		private void btn_CancelFindFiles_Click(object sender, RoutedEventArgs e) {
			Searcher.CancelAsync();
		}
	}
}
