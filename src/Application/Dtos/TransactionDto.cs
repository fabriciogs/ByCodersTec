using FixedWidthParserWriter;
using FluentValidation;
using System.Globalization;

namespace Application.Dtos
{
    public class TransactionDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [FixedWidthLineField(Start = 1, Length = 1)]
        public int Type { get; set; }

        [FixedWidthLineField(Start = 2, Length = 8)]
        public string? DateOnly { get; set; }

        [FixedWidthLineField(Start = 10, Length = 10)]
        public decimal ValueRaw { get; set; }

        public decimal Value => ValueRaw / 100m; // Normaliza

        [FixedWidthLineField(Start = 20, Length = 11)]
        public string? Cpf { get; set; }

        [FixedWidthLineField(Start = 31, Length = 12)]
        public string? Card { get; set; }

        [FixedWidthLineField(Start = 43, Length = 6)]
        public string? TimeOnly { get; set; }

        [FixedWidthLineField(Start = 49, Length = 14)]
        public string? StoreOwner { get; set; }

        [FixedWidthLineField(Start = 63)]
        public string? StoreName { get; set; }

        public DateTime DateTime => FormatDateTime(DateOnly, TimeOnly);

        private static DateTime FormatDateTime(string? dateOnly, string? timeOnly)
            => DateTime.ParseExact($"{dateOnly} {timeOnly}", "yyyyMMdd HHmmss", new CultureInfo("pt-BR"));
    }

    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator()
        {
            RuleFor(x => x.Type).InclusiveBetween(1, 9).WithMessage("Type must be between 1 and 9");
            RuleFor(x => x.Value).GreaterThan(0).WithMessage("Value must be positive");
            RuleFor(x => x.Cpf).NotEmpty().Length(11).Matches(@"^\d{11}$").WithMessage("Invalid CPF");
            RuleFor(x => x.Card).NotEmpty().Length(12).WithMessage("Invalid Card");
            RuleFor(x => x.StoreOwner).NotEmpty().MaximumLength(14).WithMessage("Invalid Store Owner");
            RuleFor(x => x.StoreName).NotEmpty().MaximumLength(19).WithMessage("Invalid Store Name");
        }
    }
}