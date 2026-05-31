using FluentValidation;
using VertrauApi.Models;

namespace VertrauApi.Validations;

public class UpdateUsuarioDtoValidator : AbstractValidator<UpdateUsuarioDto>
{
    public UpdateUsuarioDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");

        RuleFor(x => x.Sobrenome)
            .NotEmpty().WithMessage("O sobrenome é obrigatório.")
            .MaximumLength(100).WithMessage("O sobrenome não pode ter mais de 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("O formato do email é inválido.");

        RuleFor(x => x.Genero)
            .IsInEnum().WithMessage("Gênero inválido.");
    }
}
