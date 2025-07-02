namespace PicPayClone.Services.External
{
    public interface IAuthorizationService
    {
        Task<bool> AuthorizeTransactionAsync();
    }
}