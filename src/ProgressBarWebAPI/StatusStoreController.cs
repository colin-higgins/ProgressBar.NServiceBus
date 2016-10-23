using System;
using System.Collections.Concurrent;
using System.Web.Http;

namespace ProgressBarWebAPI
{
    public class StatusStoreController : ApiController
    {
        static readonly ConcurrentDictionary<Guid, ConcurrentBag<Guid>> _completedItems = new ConcurrentDictionary<Guid, ConcurrentBag<Guid>>();

        [HttpPost]
        [Route("CompleteMessage")]
        public void CompleteMessage(CompletedMessage message)
        {
            if (!_completedItems.ContainsKey(message.BatchId))
            {
                _completedItems.TryAdd(message.BatchId, new ConcurrentBag<Guid>());
            }

            _completedItems[message.BatchId].Add(message.MessageId);

            Console.WriteLine($"Message {message.MessageId} marked as completed.");
        }

        [HttpGet]
        [Route("Batch")]
        public BatchStatus Status(string id)
        {
            Console.WriteLine($"Query for status of {id}.");

            var batchId = Guid.Parse(id);

            if (!_completedItems.ContainsKey(batchId))
            {
                return null;
            }

            return new BatchStatus()
            {
                Id = batchId,
                NumberOfItemsCompleted = _completedItems[batchId].Count
            };
        }
    }
}