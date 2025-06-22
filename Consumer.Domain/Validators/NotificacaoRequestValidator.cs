using Consumer.Domain.ViewModels.Localizacao;
using FluentValidation;

namespace Consumer.Domain.Validators;

public class NotificacaoRequestValidator : AbstractValidator<NotificacaoRequest>
{
    public NotificacaoRequestValidator()
    {
        RuleFor(x => x.Data).NotEmpty();
        RuleFor(x => x.TipoNotificacao).NotEmpty();
    }
}
