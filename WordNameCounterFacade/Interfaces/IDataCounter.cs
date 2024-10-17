using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordNameCounterFacade
{
	public interface IDataCounter
	{
		void Step(object data);
		IEnumerable<object> GetCountedObjects();
		int GetObjectCount(object data);
	}
}
