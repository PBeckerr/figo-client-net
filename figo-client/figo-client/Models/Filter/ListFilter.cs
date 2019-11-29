using System;
using Figo.Client.Core.Model;

namespace Figo.Client.Models.Filter
{
    public class ListFilter
    {
        public int? Count { get; set; }
        public string Filter { get; set; }
        public int? Offset { get; set; }
        public DateTime? Since { get; set; }
        public string SinceType { get; set; }
        public string Sort { get; set; }
        public TransactionTypes? Types { get; set; }
        public DateTime? Until { get; set; }
    }

    public class AccountListFilter : ListFilter
    {
        public bool? Cents { get; set; }
        public bool? IncludePending { get; set; }
        public bool? IncludeStatistics { get; set; }
    }

    public class SecurityListFilter
    {
        public int? Count { get; set; }
        public int? Offset { get; set; }
        public DateTime? Since { get; set; }
        public string SinceType { get; set; }
    }
}