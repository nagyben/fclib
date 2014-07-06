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
			this.lv_RuleList.DataContext = RuleList;

			// Set up BackgroundWorker handlers
			BWorker.WorkerReportsProgress = true;
			BWorker.WorkerSupportsCancellation = true;
			
			BWorker.DoWork += BWorker_DoWork;
			BWorker.ProgressChanged += BWorker_ProgressChanged;
			BWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
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

		/* BACKGROUNDWORKER */
		private readonly BackgroundWorker BWorker = new BackgroundWorker();

		private void BWorker_DoWork(object sender, DoWorkEventArgs e) {
			// initialize return variable
			List<RuleFile> result = new List<RuleFile>();

			foreach (Rule rule in RuleList) {
				// check if cancel request has been set
				if (e.Cancel == true) {
					break;
				}

				// Calculate progress
				int progress = RuleList.IndexOf(rule) / RuleList.Count * 100;
				BWorker.ReportProgress(progress);

				// Execute rule
				try {
					result.AddRange(rule.Execute());
				} catch (Exception ex) {
					MessageBox.Show(ex.Message);
				}
				
			}
			e.Result = result;
			return;
		}

		private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			// dummy variable
			/* 
			 * Has to be implemented this way because only the constructor of ObservableCollection can
			 * convert from List<T> to ObservableCollection<T>
			 * Additionally, e.Result must be cast to a List<T> before this can be done
			 */
			ObservableCollection<RuleFile> dummy = new ObservableCollection<RuleFile>((List<RuleFile>)e.Result);

			// Enable buttons
			UIModeNormal();
			
			FileList = dummy;

			return;
		}

		private void BWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
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

		private void btn_FindFiles_Click(object sender, RoutedEventArgs e) {
			this.btn_FindFiles.IsEnabled = false;
			BWorker.RunWorkerAsync();
		}

		private void btn_CancelFindFiles_Click(object sender, RoutedEventArgs e) {
			BWorker.CancelAsync();
		}
	}
}
