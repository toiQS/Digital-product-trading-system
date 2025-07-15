namespace DPTS.Applications.Buyer.Dtos.wallet
{
    public class PaymentMethodDto
    {
        public string PaymentMethodId { get; set; } = string.Empty;
        public string PaymentMethodName { get; set;} = string.Empty;
        public string MaskedAccountNumber {  get; set; } = string.Empty;
    }
}
