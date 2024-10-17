using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordNameCounterFacade.Interfaces
{
	public interface IDataScanExecutor
	{
		Task<IDataScanResult> ExecuteCountingScanAsync();
	}
}
