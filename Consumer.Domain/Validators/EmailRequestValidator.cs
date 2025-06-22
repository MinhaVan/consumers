using Consumer.Domain.ViewModels.Localizacao;
using FluentValidation;

namespace Consumer.Domain.Validators;

public class EmailRequestValidator : AbstractValidator<EmailRequest>
{
    public EmailRequestValidator()
    {
        RuleFor(x => x.Data).NotEmpty();
        RuleFor(x => x.TipoEmail).NotEmpty();
    }
}
