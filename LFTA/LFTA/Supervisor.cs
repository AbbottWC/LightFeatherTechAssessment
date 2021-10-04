using System;
using System.Collections.Generic;
using System.Text;

namespace LFTA {
	public class Supervisor {
		public string jurisdiction { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }

		public override string ToString( ) => string.Format(
			"Jurisdiction: {0} Name: {1} {2}",
			this.jurisdiction, this.lastName, this.firstName);
	}
}