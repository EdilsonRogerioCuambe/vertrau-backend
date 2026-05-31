using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using VertrauApi.Enums;
using VertrauApi.Exceptions;
using VertrauApi.Mappings;
using VertrauApi.Models;
using VertrauApi.Repositories;
using VertrauApi.Services;
using Xunit;

namespace VertrauApi.Tests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _repositoryMock = new Mock<IUsuarioRepository>();
        
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<Usuario>(It.IsAny<CreateUsuarioDto>()))
            .Returns((CreateUsuarioDto dto) => new Usuario { Nome = dto.Nome, Email = dto.Email, Genero = dto.Genero });
        mapperMock.Setup(m => m.Map<UsuarioResponseDto>(It.IsAny<Usuario>()))
            .Returns((Usuario u) => new UsuarioResponseDto { Id = u.Id, Nome = u.Nome, Email = u.Email, Genero = u.Genero });
        mapperMock.Setup(m => m.Map(It.IsAny<UpdateUsuarioDto>(), It.IsAny<Usuario>()))
            .Returns((UpdateUsuarioDto dto, Usuario u) => 
            {
                u.Nome = dto.Nome;
                u.Email = dto.Email;
                u.Genero = dto.Genero;
                return u;
            });
        
        _mapper = mapperMock.Object;

        _service = new UsuarioService(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task CriarUsuario_EmailDuplicado_DeveRetornarBusinessException()
    {
        // Arrange
        var dto = new CreateUsuarioDto { Email = "teste@teste.com" };
        _repositoryMock.Setup(r => r.ExistsByEmailAsync(dto.Email, null)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.CriarUsuarioAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
                 .WithMessage("*já está em uso*");
    }

    [Fact]
    public async Task CriarUsuario_DataNascimentoFutura_DeveRetornarBusinessException()
    {
        // Arrange
        var dto = new CreateUsuarioDto 
        { 
            Email = "novo@teste.com",
            DataNascimento = DateTime.UtcNow.AddDays(1)
        };
        _repositoryMock.Setup(r => r.ExistsByEmailAsync(dto.Email, null)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.CriarUsuarioAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
                 .WithMessage("*futuro*");
    }

    [Fact]
    public async Task CriarUsuario_DadosValidos_DeveRetornarUsuarioCriado()
    {
        // Arrange
        var dto = new CreateUsuarioDto 
        { 
            Nome = "Teste",
            Sobrenome = "Sobrenome",
            Email = "teste@teste.com",
            Genero = Genero.Outro
        };
        
        _repositoryMock.Setup(r => r.ExistsByEmailAsync(dto.Email, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Usuario>()))
            .ReturnsAsync((Usuario u) => 
            {
                u.Id = 1;
                return u;
            });

        // Act
        var result = await _service.CriarUsuarioAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Nome.Should().Be("Teste");
        result.Email.Should().Be("teste@teste.com");
    }

    [Fact]
    public async Task AtualizarUsuario_IdInexistente_DeveRetornarNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Usuario?)null);
        var dto = new UpdateUsuarioDto();

        // Act
        Func<Task> act = async () => await _service.AtualizarUsuarioAsync(99, dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ListarUsuarios_ComFiltroNome_DeveRetornarFiltrado()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Nome = "João da Silva" },
            new Usuario { Id = 2, Nome = "Maria Souza" }
        };

        _repositoryMock.Setup(r => r.GetPagedAsync(1, 10, "João", null))
            .ReturnsAsync((usuarios.Where(u => u.Nome.Contains("João")), 1));

        // Act
        var result = await _service.ListarUsuariosAsync(1, 10, "João", null);

        // Assert
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().Nome.Should().Be("João da Silva");
    }

    [Fact]
    public async Task DeletarUsuario_IdExistente_DeveRemoverComSucesso()
    {
        // Arrange
        var usuario = new Usuario { Id = 1 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);
        _repositoryMock.Setup(r => r.DeleteAsync(usuario)).Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _service.DeletarUsuarioAsync(1);

        // Assert
        await act.Should().NotThrowAsync();
        _repositoryMock.Verify(r => r.DeleteAsync(usuario), Times.Once);
    }
}
