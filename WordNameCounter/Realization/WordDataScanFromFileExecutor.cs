using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNameCounter.Factory;
using WordNameCounterFacade;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Realization
{
	internal class WordDataScanFromFileExecutor : IDataScanExecutor
	{
		private readonly string _fileName;
		private readonly Encoding _fileEncoding;
		private readonly IEnumerable<string> _separators;
		private readonly int _maxParallelismDegree = Environment.ProcessorCount - 1;

		public WordDataScanFromFileExecutor(string fileName, Encoding fileEncoding, IEnumerable<string> separators)
		{
			_fileName = fileName;
			_fileEncoding = fileEncoding;
			_separators = separators;
		}

		public async Task<IDataScanResult> ExecuteCountingScanAsync()
		{
			using var fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

			var splitter = WordDataComponentsFactory.CreateWordDataStreamSplitter(2 * _maxParallelismDegree - 1, WordDataComponentsFactory.CreateWordRewinder(_separators, _fileEncoding));
			
			var fileScanIntervals = splitter.Split(fileStream);
			
			var dataCounter = WordDataComponentsFactory.CreateWordDataCounter();
			
			await Task.Run(() => Parallel.ForEach(fileScanIntervals, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelismDegree }, interval =>
			{
				using var scanner = WordDataComponentsFactory.CreateWordDataFileScanner(_fileName, WordDataComponentsFactory.CreateWordRewinder(_separators, _fileEncoding), _fileEncoding, dataCounter);
				scanner.Scan(interval);

			})).ConfigureAwait(false);

			return dataCounter as IDataScanResult ?? throw new InvalidDataException();
		} 
	}
}
