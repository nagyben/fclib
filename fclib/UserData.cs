using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;

namespace fclib {
	/// <summary>
	/// Provides helper functions to save user data
	/// </summary>
	public static class UserData {
		public static void SaveRules(ObservableCollection<Rule> rulelist, string filepath) {
			Stream stream = File.Open(filepath, FileMode.Create);
			BinaryFormatter bformatter = new BinaryFormatter();
			
			bformatter.Serialize(stream, rulelist);
			stream.Close();
		}

		public static ObservableCollection<Rule> LoadRules(string filepath) {
			ObservableCollection<Rule> rulelist;
			try {
				Stream stream = File.Open(filepath, FileMode.Open);
				BinaryFormatter bformatter = new BinaryFormatter();

				rulelist = (ObservableCollection<Rule>)bformatter.Deserialize(stream);

				stream.Close();

			} catch (Exception) {
				throw;
			}
			return rulelist;
		}

		public static void DeleteRuleData(string filepath) {
			throw new NotImplementedException();
		}
	}
}
