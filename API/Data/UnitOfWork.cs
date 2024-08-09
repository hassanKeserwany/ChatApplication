using API.Controllers;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(DataContext context, IMapper mapper, ILogger<UnitOfWork> logger)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);

        public async Task<bool> Complete()
        {
            var result= await _context.SaveChangesAsync() > 0;

            return result;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}