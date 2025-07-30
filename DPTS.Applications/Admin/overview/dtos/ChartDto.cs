namespace DPTS.Applications.Admin.overview.dtos
{
    public class ChartDto
    {
        public List<NodeDto> Nodes { get; set; } = new List<NodeDto>();
    }
    public class NodeDto
    {
        public decimal Value { get; set; }
        public string Name { get; set; }
    }
}
