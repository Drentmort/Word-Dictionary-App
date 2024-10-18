using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNameCounter.Factory;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Realization
{
	internal class WordDataCountScannerManager : IDataCountScannerManager
	{
		private readonly IEnumerable<string> _wordSeparators;
		private readonly Encoding _encoding;

		public WordDataCountScannerManager(IEnumerable<string> wordSeparators, Encoding encoding)
		{
			_wordSeparators = wordSeparators;
			_encoding = encoding;
		}

		public void ScanFile(string fromFile, string toFile)
		{
			throw new NotImplementedException();
		}

		public async Task ScanFileAsync(string fromFile, string toFile)
		{
			var executor = WordDataComponentsFactory.CreateWordDataScanExecutor(fromFile, _wordSeparators, _encoding);
			var result = await executor.ExecuteCountingScanAsync().ConfigureAwait(false);
			using var toFileStream = new FileStream(toFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
			await Task.Run(() => result.WriteTo(toFileStream, _encoding)).ConfigureAwait(false);
		}
	}
}
