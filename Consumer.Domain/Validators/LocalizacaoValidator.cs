using Consumer.Domain.ViewModels.Localizacao;
using FluentValidation;

namespace Consumer.Domain.Validators;

public class EnviarLocalizacaoWebSocketRequestValidator : AbstractValidator<EnviarLocalizacaoWebSocketRequest>
{
    public EnviarLocalizacaoWebSocketRequestValidator()
    {
        RuleFor(x => x.Latitude).NotEmpty();
        RuleFor(x => x.Longitude).NotEmpty();
        RuleFor(x => x.RotaId).NotEmpty();
    }
}