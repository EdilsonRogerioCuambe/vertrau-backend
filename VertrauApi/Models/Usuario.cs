using System;
using VertrauApi.Enums;

namespace VertrauApi.Models;

public class Usuario
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Genero Genero { get; set; }
    public DateTime? DataNascimento { get; set; }
}
