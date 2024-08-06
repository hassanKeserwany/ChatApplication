using API.DTOs;
using API.Entities;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IUnitOfWork> _logger;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public async Task MarkMessagesAsDeletedForUserAsync(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
        .Where(m => (m.SenderUserName == currentUserName && m.RecipientUserName == recipientUserName) ||
                    (m.SenderUserName == recipientUserName && m.RecipientUserName == currentUserName))
        .ToListAsync();

            foreach (var message in messages)
            {
                message.SenderDeleted = true;  // or your specific delete logic
                message.RecipientDeleted = true;
                _context.Entry(message).State = EntityState.Modified;
            }


        }
    

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUserName == messageParams.Username
                    && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUserName == messageParams.Username
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUserName == messageParams.Username
                    && u.RecipientDeleted == false && u.DateRead == null)
            };


            return await PagedList<MessageDto>
                .CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    m => m.RecipientUserName == currentUserName && m.RecipientDeleted == false &&
                    m.SenderUserName == recipientUserName ||
                    m.RecipientUserName == recipientUserName && m.SenderDeleted == false &&
                    m.SenderUserName == currentUserName
                )
                .OrderBy(m => m.MessageSent).AsQueryable();
                


            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return await messages.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
    }
}