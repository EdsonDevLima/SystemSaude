using SystemSaude.Domain.Interfaces;
using SystemSaude.Entities;
using SystemSaude.Infrastructure.Data;

namespace SystemSaude.Infrastructure.Repositories;

public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(AppDbContext context) : base(context)
    {
    }
}
