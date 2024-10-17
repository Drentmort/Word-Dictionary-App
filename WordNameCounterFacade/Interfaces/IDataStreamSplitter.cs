using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNameCounterFacade.DTO;

namespace WordNameCounterFacade
{
	public interface IDataStreamSplitter
	{
		IEnumerable<DataIntervalInfo> Split(Stream stream);
	}
}
