using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Linq;

namespace LFTA {
	public class Program {
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int AllocConsole( );
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int FreeConsole( );

		[STAThread]
		public static void Main( ) {
			//For later.
			AllocConsole( );
			var app = new App( );
			app.InitializeComponent( );
			app.Run( );
			FreeConsole( );
		}		
	}
}