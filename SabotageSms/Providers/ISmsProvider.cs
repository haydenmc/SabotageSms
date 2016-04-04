using System.Threading.Tasks;

namespace SabotageSms.Providers
{
    /// <summary>
    /// Interface for sending a message to a particular phone number
    /// </summary>
    public interface ISmsProvider
    {
        Task SendSms(string phoneNumber, string body);
    }
}