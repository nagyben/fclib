using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fclib;

namespace fclib_Console {
	class Program {

		private static string filepath = AppDomain.CurrentDomain.BaseDirectory + "rules.dat";

		static void Main(string[] args) {
			TestSave();
			TestLoad();
			Console.ReadLine();
		}

		static void TestLoad() {
			Console.WriteLine(String.Format("Loading rules from {0} ...", filepath));
			List<Rule> rulelist = UserData.LoadRules(filepath);
			Console.WriteLine("Loaded rules:\n");
			foreach (Rule rule in rulelist) {
				Console.WriteLine(rule.ToString());
			}
		}

		static void TestSave() {
			string name = "Herp";
			int id = 1;
			string parentdir = "derpie";
			string targetdir = "herbie";
			List<string> extensions = new List<string>();
			extensions.Add(".mkv");

			List<string> filters = new List<string>();
			filters.Add("anyad");

			Console.WriteLine("Creating new Rule class instance");
			Rule rule1 = new Rule(id, parentdir, targetdir, extensions, filters, name);
			List<Rule> rulelist = new List<Rule>();
			rulelist.Add(rule1);

			Console.WriteLine(String.Format("Rule class created. Serializing to file {0} ...", filepath));
			
			UserData.SaveRules(rulelist, filepath);
			
			Console.ReadLine();
		}
	}
}
