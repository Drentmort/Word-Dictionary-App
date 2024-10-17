using System.Text;
using WordNameCounterFacade;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Realization
{
	internal class WordDataCounter : IDataCounter, IDataScanResult
	{
		private readonly Dictionary<object, int> _counts = new();
		private readonly object _lock = new object();

		public IEnumerable<object> GetCountedObjects()
		{
			lock(_lock)
			{
				return _counts.Keys.ToArray();
			}
		}

		public int GetObjectCount(object data)
		{
			lock( _lock)
			{
				if( _counts.ContainsKey(data))
				{
					return _counts[data];
				}
				else
				{
					return 0;
				}
			}
		}

		public void Step(object data)
		{
			lock(_lock)
			{
				if (_counts.ContainsKey(data))
				{
					_counts[data]++;
				}
				else
				{
					_counts.Add(data, 1);
				}
			}
		}

		public void WriteTo(Stream stream, Encoding encoding)
		{
			lock(_lock)
			{
				foreach(var count in _counts.OrderByDescending(x=>x.Value))
				{
					stream.Write(encoding.GetBytes($"{count.Key},{count.Value}\n"));
				}
			}
		}
	}
}
