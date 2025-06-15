using Consumer.Domain.ViewModels.Localizacao;
using FluentValidation;

namespace Consumer.Domain.Validators;

public class EnviarLocalizacaoWebSocketRequestValidator : AbstractValidator<EnviarLocalizacaoWebSocketRequest>
{
    public EnviarLocalizacaoWebSocketRequestValidator()
    {
        RuleFor(x => x.RotaId).NotEmpty();
        RuleFor(x => x.Latitude)
            .NotEmpty()
            .When(x => x.TipoMensagem != "finalizar_corrida");

        RuleFor(x => x.Longitude)
            .NotEmpty()
            .When(x => x.TipoMensagem != "finalizar_corrida");
    }
}
