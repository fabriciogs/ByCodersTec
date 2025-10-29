namespace Domain.Entities
{
    public class Transaction
    {
        public Guid ImportFileId { get; private set; }
        public Guid Id { get; private set; }
        public int Type { get; private set; }
        public DateTime OccurrenceDate { get; private set; }
        public decimal Value { get; private set; }
        public string Cpf { get; private set; }
        public string Card { get; private set; }
        public string StoreOwner { get; private set; }
        public string StoreName { get; private set; }

        public Transaction(Guid importFileId, Guid id, int type, DateTime occurrenceDate, decimal value, string cpf, string card, string storeOwner, string storeName)
        {
            if (!IsValidType(type)) throw new ArgumentException("Invalid transaction type");
            if (value <= 0) throw new ArgumentException("Value must be positive");
            if (string.IsNullOrWhiteSpace(cpf) || !IsValidCpf(cpf)) throw new ArgumentException("Invalid CPF");
            if (string.IsNullOrWhiteSpace(card) || card.Length != 12) throw new ArgumentException("Invalid Card");
            if (string.IsNullOrWhiteSpace(storeOwner)) throw new ArgumentException("Store owner is required");
            if (string.IsNullOrWhiteSpace(storeName)) throw new ArgumentException("Store name is required");

            ImportFileId = importFileId;
            Id = id;
            Type = type;
            OccurrenceDate = occurrenceDate;
            Value = value;
            Cpf = cpf;
            Card = card;
            StoreOwner = storeOwner.Trim();
            StoreName = storeName.Trim();
        }

        public decimal GetSignedValue() => Type switch
        {
            1 or 4 or 5 or 6 or 7 or 8 => Value, // Entrada: +
            2 or 3 or 9 => -Value, // Saída: -
            _ => 0m
        };

        private static bool IsValidType(int type) => type >= 1 && type <= 9;

        private static bool IsValidCpf(string cpf)
        {
            int[] multiplicador1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multiplicador2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            for (int j = 0; j < 10; j++)
                if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
                    return false;

            var tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}