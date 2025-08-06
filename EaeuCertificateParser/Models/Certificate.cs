namespace EaeuCertificateParser.Models
{
    public class Certificate
    {
        public string DocId { get; set; }
        public string CountryCode { get; set; }
        public string StatusCode { get; set; }
        public string Applicant { get; set; }
        public string Manufacturer { get; set; }
        public string DocKind { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ValidityDate { get; set; }
    }
}
