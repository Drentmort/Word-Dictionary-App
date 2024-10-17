using WordNameCounterFacade.DTO;

namespace WordNameCounterFacade.Interfaces
{
	public interface IDataScanner : IDisposable
	{
		void Scan(DataIntervalInfo dataStreamSectionInfo);
	}
}
