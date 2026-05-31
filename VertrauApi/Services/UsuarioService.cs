using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VertrauApi.Exceptions;
using VertrauApi.Models;
using VertrauApi.Repositories;
using VertrauApi.Enums;

namespace VertrauApi.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IMapper _mapper;

    public UsuarioService(IUsuarioRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UsuarioResponseDto>> ListarUsuariosAsync(int page, int pageSize, string? nome, Genero? genero)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize, nome, genero);

        return new PagedResult<UsuarioResponseDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items.Select(u => _mapper.Map<UsuarioResponseDto>(u))
        };
    }

    public async Task<UsuarioResponseDto> ObterUsuarioAsync(long id)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null)
            throw new NotFoundException($"Usuário com ID {id} não encontrado.");

        return _mapper.Map<UsuarioResponseDto>(usuario);
    }

    public async Task<UsuarioResponseDto> CriarUsuarioAsync(CreateUsuarioDto dto)
    {
        await ValidarRegrasDeNegocioAsync(dto.Email, dto.DataNascimento);

        var usuario = _mapper.Map<Usuario>(dto);
        var createdUsuario = await _repository.CreateAsync(usuario);

        return _mapper.Map<UsuarioResponseDto>(createdUsuario);
    }

    public async Task<UsuarioResponseDto> AtualizarUsuarioAsync(long id, UpdateUsuarioDto dto)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null)
            throw new NotFoundException($"Usuário com ID {id} não encontrado.");

        await ValidarRegrasDeNegocioAsync(dto.Email, dto.DataNascimento, id);

        _mapper.Map(dto, usuario);
        await _repository.UpdateAsync(usuario);

        return _mapper.Map<UsuarioResponseDto>(usuario);
    }

    public async Task DeletarUsuarioAsync(long id)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null)
            throw new NotFoundException($"Usuário com ID {id} não encontrado.");

        await _repository.DeleteAsync(usuario);
    }

    private async Task ValidarRegrasDeNegocioAsync(string email, DateTime? dataNascimento, long? excludeId = null)
    {
        if (dataNascimento.HasValue && dataNascimento.Value > DateTime.UtcNow)
        {
            throw new BusinessException("A data de nascimento não pode ser no futuro.");
        }

        if (await _repository.ExistsByEmailAsync(email, excludeId))
        {
            throw new BusinessException($"O email {email} já está em uso.");
        }
    }
}
