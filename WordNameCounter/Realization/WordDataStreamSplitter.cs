using WordNameCounterFacade;
using WordNameCounterFacade.DTO;
using WordNameCounterFacade.Interfaces;

namespace WordNameCounter.Realization
{
	internal class WordDataStreamSplitter : IDataStreamSplitter
	{
		private readonly int _maxSectionCount;
		private readonly IDataRewinder _rewinder;

		public WordDataStreamSplitter(int maxSectionCount, IDataRewinder dataRewinder)
		{
			_maxSectionCount = maxSectionCount;
			_rewinder = dataRewinder;
		}

		public IEnumerable<DataIntervalInfo> Split(Stream stream)
		{
			var result = new List<DataIntervalInfo>();
			var sectionStep = stream.Length / _maxSectionCount;

			for(int i = 0; i < _maxSectionCount; i++)
			{
				var currentPosition = stream.Position;
				stream.Position = Math.Min(stream.Length, stream.Position + sectionStep);
				if (stream.Position < stream.Length)
				{
					var rewindInfo = _rewinder.Rewind(stream);
					result.Add(new DataIntervalInfo
					{
						FromPosition = currentPosition,
						ToPosition = rewindInfo.ToPosition
					});
					if(stream.Position == stream.Length)
					{
						break;
					}
				}
				else
				{
					result.Add(new DataIntervalInfo
					{
						FromPosition = currentPosition,
						ToPosition = stream.Position
					});
					break;
				}
				
			}
			return result;
		}
	}
}
