namespace DustAnalyzerCircular
{
    internal class PDustAnalyzerCircular
    {
        [CHProtocol(6)]
        public float b { get; set; }

        [CHProtocol(17)]
        public int 量程 { get; set; }

        [CHProtocol(19)]
        public double 不要 { get; set; }

        [CHProtocol(23)]
        public double k { get; set; }

        [CHProtocol(28)]
        public float 浓度 { get; set; }

        [CHProtocol(55)]
        public ushort 状态 { get; set; }
    }
}
