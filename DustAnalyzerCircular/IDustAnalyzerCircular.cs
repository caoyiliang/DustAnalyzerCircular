using ProtocolInterface;

namespace DustAnalyzerCircular
{
    public interface IDustAnalyzerCircular : IProtocol
    {
        Task<Dictionary<string, string>?> Read(string addr, int tryCount = 0, CancellationToken cancelToken = default);
    }
}
