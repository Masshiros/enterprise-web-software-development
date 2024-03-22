using AutoMapper;
using Server.Application.Common.Interfaces.Persistence;
using Server.Infrastructure.Persistence.Repositories;

namespace Server.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;
  // private Dictionary<Type, object> _repositories;

  public UnitOfWork(AppDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public IFacultyRepository FacultyRepository => new FalcutyRepository(_context, _mapper);

  public async Task<int> CompleteAsync()
  => await _context.SaveChangesAsync();

  public void Dispose()
      => _context.Dispose();


  // public IRepository<T, Key> GetRepository<T, Key>() where T : class
  // {
  //   _repositories ??= new Dictionary<Type, object>();

  //   var type = typeof(T);

  //   if (!_repositories.ContainsKey(type))
  //   {
  //     _repositories[type] = new RepositoryBase<T, Key>(_context);
  //   }

  //   return (IRepository<T, Key>)_repositories[type];
  // }
}