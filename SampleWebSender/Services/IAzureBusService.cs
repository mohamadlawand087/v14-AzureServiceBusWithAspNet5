using System.Threading.Tasks;
using SampleShared.Models;

namespace SampleWebSender.Services
{
    public interface IAzureBusService
    {
         Task SendMessageAsync(Person personMessage, string queueName);
    }
}