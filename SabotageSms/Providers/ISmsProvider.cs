using System.Threading.Tasks;

namespace SabotageSms.Providers
{
    public interface ISmsProvider
    {
        Task SendSms(string phoneNumber, string body);
    }
}