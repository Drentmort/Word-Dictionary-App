using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Realization
{
	internal class SimpleWordDataCountScannerManager : IDataCountScannerManager
	{
		private readonly Encoding _encoding;

		public SimpleWordDataCountScannerManager(Encoding encoding)
		{
			_encoding = encoding;
		}

		public void ScanFile(string fromFile, string toFile)
		{
			var result = new ConcurrentDictionary<string, int>();
			Parallel.ForEach(File.ReadLines(fromFile, _encoding), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1}, line =>
			{
				var splitedWords = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var word in splitedWords)
				{
					result.AddOrUpdate(word, 1, (_, x) => x + 1);
				}
			});
			using var resultFileStream = new FileStream(toFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
			foreach(var count in result)
			{
				resultFileStream.Write(_encoding.GetBytes($"{count.Key},{count.Value}\n"));
			}
		}

		public Task ScanFileAsync(string fromFile, string toFile)
		{
			throw new NotImplementedException();
		}
	}
}
