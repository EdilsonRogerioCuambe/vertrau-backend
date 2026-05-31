using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VertrauApi.Data;
using VertrauApi.Enums;
using VertrauApi.Models;
using Xunit;

namespace VertrauApi.Tests.Controllers;

public class UsuariosControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UsuariosControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=TestDB;Mode=Memory;Cache=Shared");
        connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Post_Usuarios_ComDadosValidos_DeveRetornar201Created()
    {
        // Arrange
        var dto = new CreateUsuarioDto
        {
            Nome = "Teste",
            Sobrenome = "Silva",
            Email = "integration1@teste.com",
            Genero = Genero.Masculino
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/usuarios", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<UsuarioResponseDto>();
        created.Should().NotBeNull();
        created!.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task Post_Usuarios_ComEmailDuplicado_DeveRetornar409Conflict()
    {
        // Arrange
        var dto = new CreateUsuarioDto
        {
            Nome = "Teste",
            Sobrenome = "Silva",
            Email = "duplicado@teste.com",
            Genero = Genero.Masculino
        };
        await _client.PostAsJsonAsync("/api/v1/usuarios", dto);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/usuarios", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Post_Usuarios_ComCamposInvalidos_DeveRetornar400BadRequest()
    {
        // Arrange
        var dto = new CreateUsuarioDto(); // campos vazios

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/usuarios", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("errors");
    }

    [Fact]
    public async Task Get_Usuarios_IdInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/usuarios/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_Usuarios_ComPaginacao_DeveRetornarEstruturaCorreta()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/usuarios?page=1&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<UsuarioResponseDto>>();
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task Put_Usuarios_IdExistente_DeveRetornar200ComDadosAtualizados()
    {
        // Arrange
        var createDto = new CreateUsuarioDto
        {
            Nome = "Teste Put",
            Sobrenome = "Silva",
            Email = "put@teste.com",
            Genero = Genero.Feminino
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/usuarios", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<UsuarioResponseDto>();

        var updateDto = new UpdateUsuarioDto
        {
            Nome = "Teste Atualizado",
            Sobrenome = "Silva",
            Email = "put@teste.com",
            Genero = Genero.Feminino
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/usuarios/{created!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<UsuarioResponseDto>();
        updated!.Nome.Should().Be("Teste Atualizado");
    }

    [Fact]
    public async Task Delete_Usuarios_IdExistente_DeveRetornar204NoContent()
    {
        // Arrange
        var createDto = new CreateUsuarioDto
        {
            Nome = "Teste Delete",
            Sobrenome = "Silva",
            Email = "delete@teste.com",
            Genero = Genero.Outro
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/usuarios", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<UsuarioResponseDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/usuarios/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
