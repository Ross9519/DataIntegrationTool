namespace DataIntegrationTool.Config
{
    public class EtlConfiguration
    {
        public string PipelineName { get; set; } = string.Empty;
        public InputSourceConfig Input { get; set; } = new();
        public List<TransformationConfig> Transformations { get; set; } = [];
        public OutputSourceConfig Output { get; set; } = new();
    }
}
