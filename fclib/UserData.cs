using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace fclib {
	/// <summary>
	/// Provides helper functions to save user data
	/// </summary>
	public static class UserData {
		public static void SaveRules(List<Rule> rulelist, string filepath) {
			Stream stream = File.Open(filepath, FileMode.Create);
			BinaryFormatter bformatter = new BinaryFormatter();
			
			bformatter.Serialize(stream, rulelist);
			stream.Close();
		}

		public static List<Rule> LoadRules(string filepath) {
			List<Rule> rulelist;
			try {
				Stream stream = File.Open(filepath, FileMode.Open);
				BinaryFormatter bformatter = new BinaryFormatter();

				rulelist = (List<Rule>)bformatter.Deserialize(stream);
			} catch (Exception) {
				throw;
			}
			return rulelist;
		}
	}
}
