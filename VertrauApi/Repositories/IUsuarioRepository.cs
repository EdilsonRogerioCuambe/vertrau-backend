using System.Collections.Generic;
using System.Threading.Tasks;
using VertrauApi.Models;
using VertrauApi.Enums;

namespace VertrauApi.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(long id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? nome, Genero? genero);
    Task<Usuario> CreateAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DeleteAsync(Usuario usuario);
    Task<bool> ExistsByEmailAsync(string email, long? excludeId = null);
}
