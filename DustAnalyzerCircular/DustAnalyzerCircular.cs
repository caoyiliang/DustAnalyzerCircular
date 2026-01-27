using Communication;
using Communication.Bus.PhysicalPort;
using Communication.Exceptions;
using LogInterface;
using Modbus;
using Modbus.Parameter;
using TopPortLib.Interfaces;
using Utils;

namespace DustAnalyzerCircular
{
    public class DustAnalyzerCircular : IDustAnalyzerCircular
    {
        private static readonly ILogger _logger = Logs.LogFactory.GetLogger<DustAnalyzerCircular>();
        private readonly IModBusMaster _modBusMaster;
        private bool _isConnect = false;
        public bool IsConnect => _isConnect;

        /// <inheritdoc/>
        public event DisconnectEventHandler? OnDisconnect { add => _modBusMaster.OnDisconnect += value; remove => _modBusMaster.OnDisconnect -= value; }
        /// <inheritdoc/>
        public event ConnectEventHandler? OnConnect { add => _modBusMaster.OnConnect += value; remove => _modBusMaster.OnConnect -= value; }

        public DustAnalyzerCircular(SerialPort serialPort, int defaultTimeout = 5000)
        {
            _modBusMaster = new ModBusMaster(serialPort, ModbusType.RTU, defaultTimeout)
            {
                IsHighByteBefore_Req = true,
                IsHighByteBefore_Rsp = true
            };
            _modBusMaster.OnSentData += CrowPort_OnSentData;
            _modBusMaster.OnReceivedData += CrowPort_OnReceivedData;
            _modBusMaster.OnConnect += CrowPort_OnConnect;
            _modBusMaster.OnDisconnect += CrowPort_OnDisconnect;
        }

        public DustAnalyzerCircular(ICrowPort crowPort)
        {
            _modBusMaster = new ModBusMaster(crowPort)
            {
                IsHighByteBefore_Req = true,
                IsHighByteBefore_Rsp = true
            };
            _modBusMaster.OnConnect += CrowPort_OnConnect;
            _modBusMaster.OnDisconnect += CrowPort_OnDisconnect;
        }

        private async Task CrowPort_OnDisconnect()
        {
            _isConnect = false;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnConnect()
        {
            _isConnect = true;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnReceivedData(byte[] data)
        {
            _logger.Trace($"DustAnalyzerCircular Rec:<-- {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnSentData(byte[] data)
        {
            _logger.Trace($"DustAnalyzerCircular Send:--> {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task OpenAsync()
        {
            await _modBusMaster.OpenAsync();
            _isConnect = _modBusMaster.IsConnect;
        }

        /// <inheritdoc/>
        public async Task CloseAsync(bool closePhysicalPort)
        {
            if (closePhysicalPort) await _modBusMaster.CloseAsync(closePhysicalPort);
        }

        public async Task<Dictionary<string, string>?> Read(string addr, int tryCount = 0, CancellationToken cancelToken = default)
        {
            if (!_isConnect) throw new NotConnectedException();
            var b = new BlockList();
            b.Add(new PDustAnalyzerCircular());
            Func<Task<PDustAnalyzerCircular>> func = () => _modBusMaster.GetAsync<PDustAnalyzerCircular>(addr, b);
            var rs = await func.ReTry(tryCount, cancelToken);
            return rs == null ? null : new Dictionary<string, string>()
            {
                { "k", rs.k.ToString() },
                { "b", rs.b.ToString() },
                { "28", rs.浓度.ToString() },
                { "状态", rs.状态 switch
                {
                    1 => "N",
                    2 => "C",
                    3 => "D",
                    4 => "M",
                    7 => "P",
                    _ => "N"
                }},
                { "01-FullRange", rs.量程.ToString() }
            };
        }
    }
}
