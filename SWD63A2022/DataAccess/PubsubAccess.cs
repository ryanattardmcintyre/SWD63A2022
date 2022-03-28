using Common;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PubsubAccess
    {
        private string projectId { get; set; }
        public PubsubAccess(string _projectId)
        {
            projectId = _projectId;
        }
        public async Task<string> Publish(Message msg)
        {

            TopicName topic = new TopicName(projectId, "swd63atopic");

            PublisherClient client = PublisherClient.Create(topic);

            string mail_serialized = JsonConvert.SerializeObject(msg);

            PubsubMessage message = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(mail_serialized)

            };

            return await client.PublishAsync(message);
         
        }

    }
}
