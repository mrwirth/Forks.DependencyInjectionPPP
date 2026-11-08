using System;
using System.Diagnostics;

namespace Ploeh.Samples.Commerce.Domain
{
    [DebuggerDisplay("{Currency.Code,nq} {Amount}")]
    // ---- Start code Listing 4.6 ----
    public class Money
    {
        public readonly decimal Amount;
        public readonly Currency Currency;

        public Money(decimal amount, Currency currency)
        {
            ArgumentNullException.ThrowIfNull(currency);

            this.Amount = amount;
            this.Currency = currency;
        }
        // ---- End code Listing 4.6 ----

        public override bool Equals(object obj)
        {
            return obj is Money other && (this.Amount.Equals(other.Amount) && this.Currency.Equals(other.Currency));
        }

        public override int GetHashCode() => HashCode.Combine(this.Amount, this.Currency);

        public override string ToString() => $"{this.Currency} {this.Amount:f5}";
    }
}