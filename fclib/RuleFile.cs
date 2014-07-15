using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fclib {
	public class RuleFile : IEquatable<RuleFile> {

		public string RuleName { get; private set; }
		public FileInfo FileInfo { get; private set; }
		public string ParentDirectory { get; private set; }
		public string TargetDirectory { get; private set; }
		public Rule.Operation Instruction { get; private set; }
		public bool Overwrite { get; private set; }

		/* CONSTRUCTOR */
		public RuleFile(Rule rule, FileInfo fileinfo) {
			this.RuleName = rule.Name;
			this.FileInfo = fileinfo;
			this.ParentDirectory = rule.ParentDirectory;
			this.TargetDirectory = rule.TargetDirectory;
			this.Instruction = rule.Instruction;
		}

		/* METHODS */
		public void Execute() {
			switch (this.Instruction) {
				case Rule.Operation.Copy:
					File.Copy(this.FileInfo.FullName, this.TargetDirectory, this.Overwrite);
					break;
				case Rule.Operation.Move:
					File.Move(this.FileInfo.FullName, this.TargetDirectory);
					break;
				case Rule.Operation.Delete:
					File.Delete(this.FileInfo.FullName);
					break;
				default:
					break;
			}
		}
		
		public bool Equals(RuleFile other) {
			/* WHAT DEFINES WHETHER TWO FILES ARE THE SAME?
			 * 
			 * Some logic needs to be established to mimic what criteria the user would consider
			 * when comparing two files.
			 * 
			 * CRITERIA FOR DIFFERENTIATION (list may change over time):
			 * - Filename (NOT fullpath)
			 * - Size
			 * - Time of last modification
			 */
			if (this.FileInfo.Name != other.FileInfo.Name) { return false; }
			if (this.FileInfo.Length != other.FileInfo.Length) { return false; }
			if (this.FileInfo.LastAccessTime != other.FileInfo.LastAccessTime) { return false; }

			return true;
		}
	}
}
