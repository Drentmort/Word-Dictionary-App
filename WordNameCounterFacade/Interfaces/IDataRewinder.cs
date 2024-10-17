using WordNameCounterFacade.DTO;

namespace WordNameCounterFacade.Interfaces
{
	public interface IDataRewinder
	{
		DataIntervalInfo Rewind(Stream data);
		DataIntervalInfo Rewind(Stream data, out IEnumerable<byte> rewindedData);
	}
}
