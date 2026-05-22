namespace NLog.Targets
{
    /// <summary>Shim: KafkaTarget stub — 本機不需 Kafka。</summary>
    public class KafkaTarget : NLog.Targets.TargetWithLayout
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;

        protected override void Write(NLog.LogEventInfo logEvent) { /* no-op */ }
    }
}
