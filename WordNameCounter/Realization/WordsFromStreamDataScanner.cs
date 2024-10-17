using System.Text;
using WordNameCounter.Factory;
using WordNameCounterFacade;
using WordNameCounterFacade.DTO;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Realization
{
	internal class WordsFromStreamDataScanner : IDataScanner
	{
		private readonly Encoding _wordEncoding;
		private readonly IDataRewinder _rewinder;
		private readonly IDataCounter _counter;
		private readonly Stream _stream;
		private readonly bool _leaveOpen;

		public WordsFromStreamDataScanner(Encoding wordEncoding, Stream stream, IDataRewinder dataRewinder, IDataCounter dataCounter)
		{
			_wordEncoding = wordEncoding;
			_stream = stream;
			_rewinder = dataRewinder;
			_counter = dataCounter;
		}

		public WordsFromStreamDataScanner(Encoding wordEncoding, Stream stream, IDataRewinder dataRewinder, IDataCounter dataCounter, bool leaveOpen) 
			: this(wordEncoding, stream, dataRewinder, dataCounter)
		{
			_leaveOpen = leaveOpen;
		}

		public void Scan(DataIntervalInfo dataStreamSectionInfo)
		{
			_stream.Position = dataStreamSectionInfo.FromPosition;
			while (_stream.Position < dataStreamSectionInfo.ToPosition)
			{
				_rewinder.Rewind(_stream, out var readBytes);

				var word = _wordEncoding.GetString(readBytes.ToArray());
				_counter.Step(word);
			}
		}

		public void Dispose()
		{
			if (!_leaveOpen)
			{
				_stream.Close();
				_stream.Dispose();
			}
		}
	}
}
