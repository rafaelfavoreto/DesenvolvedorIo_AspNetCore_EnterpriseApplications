using FluentValidation;
using NerdStore.Core.Messagens;

namespace NerdStore.Vendas.Application.Commands
{
    public class AplicarVoucherPedidoCommand : Command
    {      
        public Guid ClienteId { get;private set; }
        public string CodigoVoucher { get;private set; }

        public AplicarVoucherPedidoCommand(Guid clienteId, string codigoVoucher)
        {           
            ClienteId = clienteId;
            CodigoVoucher = codigoVoucher;
        }

        public override bool EhValido()
        {
            ValidationResult = new AplicarVoucherPedidoValidation().Validate(this);
            return ValidationResult.IsValid;
        }

    }
    public class AplicarVoucherPedidoValidation : AbstractValidator<AplicarVoucherPedidoCommand>
    {
        public AplicarVoucherPedidoValidation()
        {          
            RuleFor(p => p.ClienteId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do cliente inválido");

            RuleFor(p => p.CodigoVoucher)
                .NotEmpty()
                .WithMessage("Valor do voucher inválido");
        }

    }

}
