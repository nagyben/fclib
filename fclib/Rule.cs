using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace fclib {

	[Serializable]
	public class Rule : ISerializable {

		/* MEMBER VARIABLES */
		private int _id;
		public int ID {
			get { return _id; }
			set {
				if (value < 0) {
					throw new ArgumentOutOfRangeException("Rule.ID","Rules cannot have a negative ID number");
				} else {
					this._id = value;
				}
			}
		}
		private string _name;
		public string Name {
			get { return _name; }
			set {
				if (value == "") {
					_name = string.Join("|", this.Filters) + "[" + string.Join(",", this.Extensions) + "] from <" + this.ParentDirectory + "> to <" + this.TargetDirectory + ">";
				}
			}
		}
		public List<string> Extensions { get; private set; }
		public List<string> Filters { get; private set; }

		private string _ParentDirectory;
		public string ParentDirectory {
			get { return this._ParentDirectory;  }
			set {
				// TODO: filepath logic
				this._ParentDirectory = value;
			}
		}
		private string _TargetDirectory;
		public string TargetDirectory {
			get { return this._TargetDirectory; }
			set {
				// TODO: filepath logic
				this._TargetDirectory = value;
			}
		}

		public enum Operation {
			Copy = 0,
			Move = 1,
			Delete =2
		}
		public Operation Instruction = Operation.Copy;
		public bool SearchSubdirectories = false;

		/* CONSTRUCTORS */

		// basic constructor
		public Rule(int id,						
					string parentdir,
					string targetdir,
					List<string> extensions,
					List<string> filters,
					string name = "") {
						
			this.ID = id;
			this.ParentDirectory = parentdir;
			this.TargetDirectory = targetdir;
			this.Extensions = extensions;
			this.Filters = filters;
			this.Name = name;
		}

		// serialization constructor
		public Rule(SerializationInfo info, StreamingContext context) {
			this.ID = (int)info.GetValue("ID", typeof(int));
			this.ParentDirectory = (string)info.GetValue("ParentDirectory", typeof(string));
			this.TargetDirectory = (string)info.GetValue("TargetDirectory", typeof(string));
			this.Extensions = (List<string>)info.GetValue("Extensions", typeof(List<string>));
			this.Filters = (List<string>)info.GetValue("Filters", typeof(List<string>));
			this.Name = (string)info.GetValue("Name", typeof(string));
		}

		/* METHODS */
		public void addExtension(string extension) {
			this.Extensions.Add(extension);
		}

		public void addFilter(string filter) {
			this.Filters.Add(filter);
		}
	}
}
