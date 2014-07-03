using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace fclib {

	[Serializable]
	public class Rule : ISerializable {

		/* INTRODUCTION
		 * The Rule class provides the instances which describe each file copying / moving / deleting rule
		 */
		
		/* MEMBER VARIABLES */
		private int _id;
		public int ID {
			get { return _id; }
			set {
				if (value < 0) {
					throw new ArgumentOutOfRangeException("Rule.ID", "Rules cannot have a negative ID number");
				} else {
					this._id = value;
				}
			}
		}

		private string _name;
		public string Name {
			get { return _name; }
			set {
				if (value == "" || value == null) {
					_name = string.Join("|", this.Filters) + "[" + string.Join(",", this.Extensions) + "] from <" + this.ParentDirectory + "> to <" + this.TargetDirectory + ">";
				}
			}
		}

		public List<string> Extensions { get; private set; }	// List of extenstions in the format ".mkv", ".avi" etc.
		public List<string> Filters { get; private set; }		/* List of string filters, eg. "family guy" or "simpsons"
																 * these are not case-sensitive as all working filenames
																 * are converted to lowercase */

		private string _ParentDirectory;						// The directory which files will be moved / copied / deleted from
		public string ParentDirectory {
			get { return this._ParentDirectory; }
			set {
				if (!CheckFilePath(value)) {
					throw new FormatException("File path may not contain the following characters: " + Path.GetInvalidPathChars().ToString());
				} else {
					this._ParentDirectory = value;
				}
			}
		}

		private string _TargetDirectory;						// The directory which files will be moved / copied to
		public string TargetDirectory {							// note that this does not apply when the rule is a
			get { return this._TargetDirectory; }				// delete operation
			set {
				if (!CheckFilePath(value)) {
					throw new FormatException("File path may not contain the following characters: " + Path.GetInvalidPathChars().ToString());
				} else {
					this._TargetDirectory = value;
				}
			}
		}

		public enum Operation {
			None = 0,
			Copy = 1,
			Move = 2,
			Delete = 3
		}
		public Operation Instruction = Operation.Copy;			// move, copy or delete

		// Whether to check the subfolders of the parent directory.
		// Does not apply to the target directory because this is ambiguous.
		private SearchOption _SearchSubdirectories;
		public bool SearchSubdirectories {
			get {
				if (this._SearchSubdirectories == SearchOption.AllDirectories) { return true; } else { return false; }
			}
			set {
				if (value == true) {
					this._SearchSubdirectories = SearchOption.AllDirectories;
				} else {
					this._SearchSubdirectories=SearchOption.TopDirectoryOnly;
				}
			}
		}

		public bool Enabled = true;											

		/* CONSTRUCTORS */

		// basic constructor
		public Rule(int id,
					string parentdir,
					string targetdir,
					List<string> extensions,
					List<string> filters,
					Operation instruction,
					bool searchsubdir = false,
					string name = "") {

			// The checks should be done by the set methods as defined by the properties
			this.ID = id;
			this.ParentDirectory = parentdir;
			this.TargetDirectory = targetdir;
			this.Extensions = extensions;
			this.Filters = filters;
			this.Instruction = instruction;
			this.SearchSubdirectories = searchsubdir;
			this.Name = name;
		}

		/* SERIALIZATION */
		public Rule(SerializationInfo info, StreamingContext context) {
			this.ID = (int)info.GetValue("ID", typeof(int));
			this.ParentDirectory = (string)info.GetValue("ParentDirectory", typeof(string));
			this.TargetDirectory = (string)info.GetValue("TargetDirectory", typeof(string));
			this.Extensions = (List<string>)info.GetValue("Extensions", typeof(List<string>));
			this.Filters = (List<string>)info.GetValue("Filters", typeof(List<string>));
			this.Name = (string)info.GetValue("Name", typeof(string));
			this.Instruction = (Operation)info.GetValue("Instruction", typeof(Operation));
			this.SearchSubdirectories = (bool)info.GetValue("Subdirectories", typeof(bool));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("ID", this.ID);
			info.AddValue("Name", this.Name);
			info.AddValue("ParentDirectory", this.ParentDirectory);
			info.AddValue("TargetDirectory", this.TargetDirectory);
			info.AddValue("Extensions", this.Extensions);
			info.AddValue("Filters", this.Filters);
			info.AddValue("Instruction", this.Instruction);
			info.AddValue("Subdirectories", this.SearchSubdirectories);
		}

		/* METHODS */
		public override string ToString() {
			string buffer = "ID: " + this.ID + "\n" +
							"Name: " + this.Name + "\n" +
							"ParentDirectory: " + this.ParentDirectory + "\n" +
							"TargetDirectory: " + this.TargetDirectory + "\n" +
							"Extensions: " + string.Join(",", this.Extensions) + "\n" +
							"Filters: " + string.Join(",", this.Filters) + "\n";

			return buffer;
		}

		public void addExtension(string extension) {
			this.Extensions.Add(extension);
		}

		public void addFilter(string filter) {
			this.Filters.Add(filter);
		}

		public void Modify(int id,
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

		public List<RuleFile> Execute() {
			// Returns a list of file descriptors which match the rule
			/* Several steps take place here:
			 * 1. First, every file in the directory is returned
			 * 2. Each filename is checked against the filters (these are by definiton OR filters)
			 * 3. For every filename which passes the filter, it is added to the return list as a new RuleFile object
			 */

			// Initialize return variable for this function
			List<RuleFile> FilteredRuleFileList = new List<RuleFile>();

			// Get all files with the corresponding extensions
			List<FileInfo> AllFiles = GetFilesByMultipleExtensions(this.Extensions, this.ParentDirectory);

			// Pass the filelist through the filters
			List<FileInfo> FilteredFileInfoList = ApplyFilters(AllFiles);

			// Convert the filelist to a RuleFile list
			foreach (FileInfo file in FilteredFileInfoList) {
				FilteredRuleFileList.Add(new RuleFile(this, file));
			}
			return FilteredRuleFileList;
		}

		private List<FileInfo> ApplyFilters(List<FileInfo> files) {
			// Initialize return variable for this function
			List<FileInfo> FileInfoList = new List<FileInfo>();

			foreach (string filter in this.Filters) {
				FileInfoList.AddRange(files.Where(s => s.FullName.ToLower() == filter.ToLower()));		// TODO: Remember to explain this lambda expression
			}

			return FileInfoList;
		}

		private List<FileInfo> GetFilesByExtension(string extension, string directory) {
			// Initialize return variable for this function
			List<FileInfo> FileInfoList = new List<FileInfo>();

			// Create DirectoryInfo object
			/* We can't use the static Directory class because it returns
			 * an array of filenames which is not enough information
			 * for our purposes
			 */
			DirectoryInfo DirectoryInfo = new DirectoryInfo(directory);

			// Get all files with extension
			FileInfo[] FileInfoArray = DirectoryInfo.GetFiles(extension, this._SearchSubdirectories);

			// convert from array into list
			foreach (FileInfo FI in FileInfoArray) {
				FileInfoList.Add(FI);
			}

			return FileInfoList;
		}

		private List<FileInfo> GetFilesByMultipleExtensions(List<string> extensions, string directory) {
			// Initialize return variable
			List<FileInfo> FileInfoList = new List<FileInfo>();

			// Get the files for each extensions
			foreach (string ext in extensions) {
				FileInfoList.AddRange(GetFilesByExtension(ext, directory)) ;
			}

			return FileInfoList;
		}

		public bool IsValid() {
			// Checks if the rule is valid

			// Various checking logic to be implemented here
			if (this.ParentDirectory == null || this.ParentDirectory == "") { return false; }
			if (this.TargetDirectory == null || this.TargetDirectory == "") { return false; }
			if (this.Extensions.Count() == 0) { return false; }
			if (this.Instruction == Operation.None) { return false; }

			// If above checks pass then rule is valid
			return true;
		}

		private bool CheckFilePath(string filepath) {
			//TODO: fix this RegEx
			Regex r = new Regex(@"^(?:[\w]\:|\\)(\\[a-z_\-\s0-9\.]+)+$");
			return r.IsMatch(filepath);
		}
	}
}
