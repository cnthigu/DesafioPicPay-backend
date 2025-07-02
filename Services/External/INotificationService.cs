using PicPayClone.Models;

namespace PicPayClone.Services.External
{
    public interface INotificationService
    {
        Task SendNotificationAsync(User recipient, User sender, decimal amount);
    }
}