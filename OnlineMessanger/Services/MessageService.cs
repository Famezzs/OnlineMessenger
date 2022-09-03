﻿using OnlineMessanger.Helpers;
using OnlineMessanger.Models;
using OnlineMessanger.Services.Interfaces;

namespace OnlineMessanger.Services
{
    public class MessageService : IMessageService
    {
        public async Task SaveMessage(Message message)
        {
            await context.Messages.AddAsync(message);

            await context.SaveChangesAsync();
        }

        public async Task DeleteMessage(string userId, string messageId)
        {
            if (!await IsUserOwnerOfMessage(userId, messageId))
            {
                return;
            }

            var message = await context.Messages.FindAsync(messageId);

            if (message == null)
            {
                return;
            }

            context.Messages.Remove(message);

            await context.SaveChangesAsync();
        }

        public async Task DeleteMessageForSelf(string userId, string messageId)
        {
            if (!await IsUserOwnerOfMessage(userId, messageId))
            {
                return;
            }

            var message = await context.Messages.FindAsync(messageId);

            if (message == null)
            {
                return;
            }

            message.IsDeletedForSelf = true;

            context.Update(message);

            await context.SaveChangesAsync();
        }

        public async Task EditMessage(string userId, string messageId, string contents)
        {
            if (!await IsUserOwnerOfMessage(userId, messageId))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(contents))
            {
                return;
            }

            var message = await context.Messages.FindAsync(messageId);

            if (message == null ||
                message.Contents == contents)
            {
                return;
            }

            var cleanContents = TagCleaner.CleanUp(contents);

            if (String.IsNullOrWhiteSpace(cleanContents))
            {
                return;
            }

            message.Contents = cleanContents;

            message.IsEdited = true;

            context.Update(message);

            await context.SaveChangesAsync();
        }

        public async Task<List<MessageRepresentation>> GetMessagesByChannelId(string channelId, int messageLimit, int messageOffset)
        {
            var messages = new List<MessageRepresentation>();

            var fields = $"Id, OwnerId, ChannelId, Contents, Created, IsEdited, IsDeletedForSelf";

            var source = "dbo.Messages";

            var condition = $"ChannelId='{channelId}'";

            var additionalOptions = $"ORDER BY Created DESC OFFSET {messageOffset} ROWS FETCH FIRST {messageLimit} ROWS ONLY";

            using var queryService = new QueryService();

            var sqlReader = await queryService.Select(fields, source, condition, additionalOptions);

            while (await sqlReader.ReadAsync())
            {
                var id = (string)sqlReader["Id"];

                var ownerId = (string)sqlReader["OwnerId"];

                var messageChannelId = (string)sqlReader["ChannelId"];

                var contents = (string)sqlReader["Contents"];

                var createdOn = (DateTime)sqlReader["Created"];

                var isEdited = (bool)sqlReader["IsEdited"];

                var isDeletedForSelf = (bool)sqlReader["IsDeletedForSelf"];

                var message = new Message(id, ownerId, messageChannelId, contents, createdOn, isEdited, isDeletedForSelf);

                var author = await context.Users.FindAsync(ownerId);

                messages.Add(new MessageRepresentation(message, author!.Email));
            }

            messages.Reverse();

            return messages;
        }

        public async Task<bool> IsUserOwnerOfMessage(string userId, string messageId)
        {
            var user = await context.Users.FindAsync(userId);

            var message = await context.Messages.FindAsync(messageId);

            if (user == null || message == null)
            {
                return false;
            }

            if (user.Id != message.OwnerId)
            {
                return false;
            }

            return true;
        }

        public MessageService(MessangerDataContext context)
        {
            this.context = context;
        }

        private readonly MessangerDataContext context;
    }
}