using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VertrauApi.Models;
using VertrauApi.Services;
using VertrauApi.Enums;

namespace VertrauApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Lista de usuários com paginação e filtros
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="nome">Filtro opcional por nome</param>
    /// <param name="genero">Filtro opcional por gênero</param>
    /// <returns>Lista paginada de usuários</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UsuarioResponseDto>>> Listar(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? nome = null, 
        [FromQuery] Genero? genero = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var result = await _usuarioService.ListarUsuariosAsync(page, pageSize, nome, genero);
        return Ok(result);
    }

    /// <summary>
    /// Detalhes de um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Dados do usuário</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioResponseDto>> Obter(long id)
    {
        var usuario = await _usuarioService.ObterUsuarioAsync(id);
        return Ok(usuario);
    }

    /// <summary>
    /// Cadastra um novo usuário
    /// </summary>
    /// <param name="dto">Dados do usuário para cadastro</param>
    /// <returns>Usuário criado</returns>
    [HttpPost]
    public async Task<ActionResult<UsuarioResponseDto>> Criar([FromBody] CreateUsuarioDto dto)
    {
        var usuario = await _usuarioService.CriarUsuarioAsync(dto);
        return CreatedAtAction(nameof(Obter), new { id = usuario.Id }, usuario);
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados para atualização</param>
    /// <returns>Usuário atualizado</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<UsuarioResponseDto>> Atualizar(long id, [FromBody] UpdateUsuarioDto dto)
    {
        var usuario = await _usuarioService.AtualizarUsuarioAsync(id, dto);
        return Ok(usuario);
    }

    /// <summary>
    /// Remove um usuário
    /// </summary>
    /// <param name="id">ID do usuário a ser removido</param>
    /// <returns>No Content se sucesso</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Deletar(long id)
    {
        await _usuarioService.DeletarUsuarioAsync(id);
        return NoContent();
    }
}
