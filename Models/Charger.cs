using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMCharger.Models
{
    class Charger
    {
   		public async static Task<string> ChargeMobileAsyncTask(string mobileNum)
		{
			Debug.WriteLine( "Starting download for " + mobileNum );

            //WebClient client = new WebClient();
            //var download = Task.Run<string>( () => { try { return client.DownloadString( mobileNum ); } catch { return ""; } } );
            //await download;

            //Debug.WriteLine( "Finished download of " + mobileNum );

            return "aaa";//download.Result;
		}

    }
}
