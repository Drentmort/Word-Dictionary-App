using System.IO;
using WordNameCounterFacade.DTO;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Realization
{
	internal class WordDataRewinder : IDataRewinder
	{
		private readonly List<byte[]> _separators;
		private readonly int _searchWindow;
		private readonly byte[] _buffer;
		public WordDataRewinder(IEnumerable<byte[]> separators)
		{
			_separators = separators.ToList();
			_searchWindow = _separators?.OrderByDescending(x=>x.Length)?.FirstOrDefault()?.Length ?? 1;
			_buffer = new byte[_searchWindow];
		}

		public DataIntervalInfo Rewind(Stream stream) => RewindInternal(stream, doWriteRewindData: false).Info;

		public DataIntervalInfo Rewind(Stream stream, out IEnumerable<byte> rewindedData)
		{
			var result = RewindInternal(stream, doWriteRewindData: true);
			rewindedData = result.RewindPayloadData;
			return result.Info;
		}

		public (IEnumerable<byte> RewindPayloadData, DataIntervalInfo Info) RewindInternal(Stream stream, bool doWriteRewindData)
		{
			RewindSeparators(stream);
			var startPosition = stream.Position;
			var rewindedDataList = RewindPayload(stream, doWriteRewindData);
			var endPosition = stream.Position;
			RewindSeparators(stream);

			return (rewindedDataList, new DataIntervalInfo
			{
				FromPosition = startPosition,
				ToPosition = endPosition
			});
		}

		private IEnumerable<byte> RewindPayload(Stream stream, bool doWriteRewindData)
		{
			var readData = new List<byte>();
			while (stream.Position < stream.Length) 
			{
				var windowSize = (int)Math.Min(_searchWindow, stream.Length - stream.Position);
				if (!TryRewindPayload(stream, windowSize))
				{
					break;
				}
				else
				{
					if (doWriteRewindData)
					{
						readData.Add(_buffer[0]);
					}
				}
			}
			return readData;
		}

		private bool TryRewindPayload(Stream stream, int windowSize)
		{
			ReadToBuffer(stream, windowSize);
			if(!_separators.Any(s => DataIsSeparator(_buffer, s)))
			{
				stream.Position++;
				return true;
			}
			return false;
		}

		private void RewindSeparators(Stream stream)
		{
			while (stream.Position < stream.Length)
			{
				var windowSize = (int)Math.Min(_searchWindow, stream.Length - stream.Position);
				if(!TryRewindSeparators(stream, windowSize))
				{
					break;
				}
			}
		}

		private bool TryRewindSeparators(Stream stream, int windowSize)
		{
			ReadToBuffer(stream, windowSize);
			var matchSeparator = _separators?.FirstOrDefault(s => DataIsSeparator(_buffer, s));
			if(matchSeparator != null)
			{
				stream.Position += matchSeparator.Length;
				return true;
			}
			return false;
		}

		private void ReadToBuffer(Stream stream, int windowSize)
		{
			stream.Read(_buffer, 0, windowSize);
			stream.Position -= windowSize;
		}

		private bool DataIsSeparator(byte[] data, byte[] separator) 
		{
			for (int i = 0; i < separator.Length; i++)
			{
				if (data[i] != separator[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
