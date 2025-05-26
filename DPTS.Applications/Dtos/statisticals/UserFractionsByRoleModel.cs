namespace DPTS.Applications.Dtos.statisticals
{
    public class UserFractionsByRoleModel
    {
        public string RoleName { get; set; } = string.Empty;
        public int Quantity {  get; set; }
        public float Percentage { get; set; }
    }
}
