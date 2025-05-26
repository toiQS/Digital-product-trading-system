namespace DPTS.Domains
{
    public enum StatusEntity 
    {
        // defaut
        None,

        // for products are selled
        Newest,
        BestSeller,

        // for manage products
        Pending,
        Available,
        Block,


        // for manage orders
        Done,
        Comfirmed,
        Cancel,

        // for complaint management
        Resolved,
        Complaint,
        
        // for paied
        FundsInEscrow,

        // for wallet
        Resolving   
    }
}
