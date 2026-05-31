using System.Threading.Tasks;
using VertrauApi.Models;
using VertrauApi.Enums;

namespace VertrauApi.Services;

public interface IUsuarioService
{
    Task<PagedResult<UsuarioResponseDto>> ListarUsuariosAsync(int page, int pageSize, string? nome, Genero? genero);
    Task<UsuarioResponseDto> ObterUsuarioAsync(long id);
    Task<UsuarioResponseDto> CriarUsuarioAsync(CreateUsuarioDto dto);
    Task<UsuarioResponseDto> AtualizarUsuarioAsync(long id, UpdateUsuarioDto dto);
    Task DeletarUsuarioAsync(long id);
}
