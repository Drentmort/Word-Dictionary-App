using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordNameCounterFacade.Interfaces
{
	public interface IDataCountScannerManager
	{
		Task ScanFileAsync(string fromFile, string toFile);
		void ScanFile(string fromFile, string toFile);
	}
}
