using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fclib {
	class RuleFile {

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
	}
}
