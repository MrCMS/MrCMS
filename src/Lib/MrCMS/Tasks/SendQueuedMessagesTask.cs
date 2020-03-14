using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Messaging;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    public class SendQueuedMessagesTask : SchedulableTask
    {
        public const int MAX_TRIES = 5;
        private readonly IGlobalRepository<QueuedMessage> _repository;
        private readonly IEmailSender _emailSender;

        public SendQueuedMessagesTask(IGlobalRepository<QueuedMessage> repository, IEmailSender emailSender)
        {
            _repository = repository;
            _emailSender = emailSender;
        }

        public override int Priority
        {
            get { return 5; }
        }

        protected override async Task OnExecute(CancellationToken token)
        {
            await _repository.Transact(async (repo, ct) =>
            {
                foreach (
                    QueuedMessage queuedMessage in
                    await repo.Query().Where(
                            message => message.SentOn == null && message.Tries < MAX_TRIES)
                        .ToListAsync(ct))
                {
                    if (await _emailSender.CanSend(queuedMessage))
                        await _emailSender.SendMailMessage(queuedMessage);
                    else
                        queuedMessage.SentOn = DateTime.UtcNow;
                    await repo.Update(queuedMessage, ct);
                }
            }, token);
        }
    }
}