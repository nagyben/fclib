using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Diagnostics;
using fclib;

namespace fcui.Converters {
	class OperationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			// Return value
			string instruction = "";

			Debugger.Break();

			switch ((Rule.Operation)value) {
				case Rule.Operation.None:
					instruction = "None";
					break;
				case Rule.Operation.Copy:
					instruction = "Copy";
					break;
				case Rule.Operation.Move:
					instruction = "Move";
					break;
				case Rule.Operation.Delete:
					instruction = "Delete";
					break;
			}

			return instruction;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
