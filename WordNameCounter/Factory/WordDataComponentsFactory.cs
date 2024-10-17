using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNameCounter.Realization;
using WordNameCounterFacade;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Factory
{
	public static class WordDataComponentsFactory
	{
		public static IDataRewinder CreateWordRewinder(IEnumerable<string> separators, Encoding separatorEncoding)
		{
			var parsedToBytesSeparatops = separators.Select(s => separatorEncoding.GetBytes(s)).ToList();
			return new WordDataRewinder(parsedToBytesSeparatops);
		}

		public static IDataStreamSplitter CreateWordDataStreamSplitter(int maxSectionCount, IDataRewinder dataRewinder) =>
			new WordDataStreamSplitter(maxSectionCount, dataRewinder);

		public static IDataCounter CreateWordDataCounter() => new WordDataCounter();

		public static IDataScanner CreateWordDataScanner(Stream stream, IDataRewinder dataRewinder, Encoding separatorEncoding, IDataCounter dataCounter)
		{
			return new WordsFromStreamDataScanner(separatorEncoding, stream, dataRewinder, dataCounter, leaveOpen: true);
		}

		public static IDataScanner CreateWordDataFileScanner(string filename, IDataRewinder dataRewinder, Encoding separatorEncoding, IDataCounter dataCounter)
		{
			var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new WordsFromStreamDataScanner(separatorEncoding, fileStream, dataRewinder, dataCounter, leaveOpen: true);
		}

		public static IDataScanExecutor CreateWordDataScanExecutor(string filename, IEnumerable<string> separators, Encoding encoding) => new WordDataScanFromFileExecutor(filename, encoding, separators);

		public static IDataCountScannerManager CreateWordDataCountScanManager(IEnumerable<string> separators, Encoding encoding) => new WordDataCountScannerManager(separators, encoding);
	}
}
