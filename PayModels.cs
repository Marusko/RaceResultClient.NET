using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceResultClient;

// Ports of go-model's invoice and pay packages: invoices made for participants, and the records of
// RACE RESULT's online payment service. No go-webapi endpoint returns them (InvoicePaymentTerms, which
// a registration's answer carries, is in Models.cs). Same mapping as the other models.

// ── invoice ───────────────────────────────────────────────────────────────────

/// <summary>An invoice's receiver or issuer (go-model's <c>invoice.Party</c>).</summary>
public record InvoiceParty
{
    public string Company { get; init; } = "";
    public string Name { get; init; } = "";
    public string AddressLine1 { get; init; } = "";
    public string AddressLine2 { get; init; } = "";
    public string City { get; init; } = "";
    [JsonPropertyName("ZIP")]
    public string Zip { get; init; } = "";
    public string State { get; init; } = "";
    public int Country { get; init; }
    [JsonPropertyName("TaxID")]
    public string TaxId { get; init; } = "";
    [JsonPropertyName("VATID")]
    public string VatId { get; init; } = "";
}

/// <summary>go-model's <c>invoice.Invoice</c>.</summary>
public record Invoice
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Number { get; init; } = "";
    public string ReceiverReference { get; init; } = "";
    public string Date { get; init; } = "";
    [JsonPropertyName("PID")]
    public int Pid { get; init; }
    public InvoiceParty Receiver { get; init; } = new();
    public InvoiceParty Issuer { get; init; } = new();
    public string Currency { get; init; } = "";
    public string LegalNotes { get; init; } = "";
    public string PerformanceDate { get; init; } = "";
    public string EInvoice { get; init; } = "";
    public InvoicePaymentTerms PaymentTerms { get; init; } = new();
    public DateTime Sent { get; init; }
}

/// <summary>An invoice with its total (go-model's <c>invoice.WithSum</c>).</summary>
public record InvoiceWithSum : Invoice
{
    public decimal Sum { get; init; }
    [JsonPropertyName("PIDs")]
    public int[]? Pids { get; init; }
}

/// <summary>An entry fee charged on an invoice, as booked (go-model's <c>invoice.SourceItem</c>).</summary>
public record InvoiceSourceItem
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    [JsonPropertyName("InvoiceID")]
    public int InvoiceId { get; init; }
    [JsonPropertyName("PID")]
    public int Pid { get; init; }
    [JsonPropertyName("EntryFeeID")]
    public int EntryFeeId { get; init; }
    public string EntryFeeName { get; init; } = "";
    public decimal Multiplication { get; init; }
    public decimal Amount { get; init; }
    public decimal TaxRate { get; init; }
    public bool Credit { get; init; }
}

/// <summary>A line of an invoice (go-model's <c>invoice.Item</c>).</summary>
public record InvoiceItem
{
    public decimal Count { get; init; }
    [JsonPropertyName("EntryFeeID")]
    public int EntryFeeId { get; init; }
    public string EntryFeeName { get; init; } = "";
    public decimal UnitPrice { get; init; }
    public decimal TaxRate { get; init; }
    public decimal Tax { get; init; }
}

/// <summary>An invoice with its lines and totals (go-model's <c>invoice.WithDetails</c>).</summary>
public record InvoiceWithDetails : Invoice
{
    public decimal Sum { get; init; }
    public InvoiceItem[]? Items { get; init; }
    public InvoiceSourceItem[]? SourceItems { get; init; }
    public Dictionary<string, JsonElement>? Fields { get; init; }
    public Dictionary<decimal, decimal>? TaxesByTaxRate { get; init; }
    public Dictionary<decimal, decimal>? NetByTaxRate { get; init; }
    public Dictionary<decimal, decimal>? GrossByTaxRate { get; init; }
    public decimal TaxSum { get; init; }
    public decimal GrossSum { get; init; }
    public decimal NetSum { get; init; }
}

/// <summary>How an event makes its invoices (go-model's <c>invoice.Settings</c>).</summary>
public record InvoiceSettings
{
    public string NumberScheme { get; init; } = "";
    public string LegalNotes { get; init; } = "";
    public string PerformanceDate { get; init; } = "";
    public string EInvoice { get; init; } = "";
    public string ReceiverFilter { get; init; } = "";
    public string ReceiverMergeField { get; init; } = "";
    public string ReceiverFieldCompany { get; init; } = "";
    public string ReceiverFieldName { get; init; } = "";
    public string ReceiverFieldAddressLine1 { get; init; } = "";
    public string ReceiverFieldAddressLine2 { get; init; } = "";
    public string ReceiverFieldCity { get; init; } = "";
    [JsonPropertyName("ReceiverFieldZIP")]
    public string ReceiverFieldZip { get; init; } = "";
    public string ReceiverFieldState { get; init; } = "";
    public string ReceiverFieldCountry { get; init; } = "";
    public string ReceiverFieldReference { get; init; } = "";
    [JsonPropertyName("ReceiverFieldTaxID")]
    public string ReceiverFieldTaxId { get; init; } = "";
    [JsonPropertyName("ReceiverFieldVATID")]
    public string ReceiverFieldVatId { get; init; } = "";
    public InvoiceParty Issuer { get; init; } = new();
    public InvoicePaymentTerms PaymentTerms { get; init; } = new();
    public int[]? ExcludedEntryFees { get; init; }
}

/// <summary>go-model's <c>invoice.Filter</c>.</summary>
public record InvoiceFilter
{
    [JsonPropertyName("ID")]
    public int[]? Id { get; init; }
    public string[]? Number { get; init; }
    [JsonPropertyName("PID")]
    public int[]? Pid { get; init; }
    public bool[]? Sent { get; init; }
    public string[]? PaymentTermsReference { get; init; }
}

// ── pay ───────────────────────────────────────────────────────────────────────

/// <summary>RACE RESULT's payment methods (go-model's <c>pay.Pm…</c> constants).</summary>
public enum PaymentMethod
{
    NoPayment = 0,
    /// <summary>Credit card, EUR.</summary>
    CcEur = 2,
    /// <summary>Credit card, CHF.</summary>
    CcChf = 3,
    UebD = 4,
    Bar = 5,
    Spf = 6,
    /// <summary>PayPal, EUR.</summary>
    PPalEur = 7,
    UebCh = 8,
    EinzCh = 10,
    UebSof = 12,
    /// <summary>PayPal, GBP.</summary>
    PPalGbp = 14,
    /// <summary>PayPal, USD.</summary>
    PPalUsd = 15,
    Sepa = 16,
    /// <summary>Credit card, GBP.</summary>
    CcGbp = 17,
    SepaData = 19,
    OwnEPay = 20,
    OwnPPal = 21,
    OwnWireT = 22,
    OwnPaytrail = 25,
    OwnOnePay = 26,
    Telr = 27,
    OwnOnePayDom = 28,
    Fatora = 29,
    Twint = 30,
    StripeCard = 31,
    OwnPaytrailV2 = 32,
    TelrSale = 33,
    RedSys = 34,
    MollieBancontact = 35,
    PayTabs = 36,
    AsiaPay = 37,
    MercadoPago = 38,
    Xendit = 41,
    TazaPay = 42,
    Cb = 99
}

/// <summary>A payment's state (go-model's <c>pay.PayState…</c> constants).</summary>
public enum PayState
{
    Undefined = 0,
    Pending = 1,
    Underpaid = 2,
    Paid = 3,
    Overpaid = 4,
    NoPayout = 5
}

/// <summary>A payment method as offered to the one registering (go-model's <c>pay.MethodOption</c>).</summary>
public record PayMethodOption
{
    [JsonPropertyName("ID")]
    public PaymentMethod Id { get; init; }
    public string NameShort { get; init; } = "";
    public string Name { get; init; } = "";
    public decimal EntryFee { get; init; }
    public decimal PaymentFee { get; init; }
    public decimal UserFee { get; init; }
    [JsonPropertyName("KB")]
    public decimal Kickback { get; init; }
    public string Currency { get; init; } = "";
    public double ExchangeRate { get; init; }
    [JsonPropertyName("SEPANotBefore")]
    public string? SepaNotBefore { get; init; }
    public bool NoTestMode { get; init; }
    public string Token { get; init; } = "";
}

/// <summary>A registration paid online (go-model's <c>pay.Registration</c>).</summary>
public record PayRegistration
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public DateTime Timestamp { get; init; }
    [JsonPropertyName("IP")]
    public string Ip { get; init; } = "";
    public int Event { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Sex { get; init; } = "";
    public int Contest { get; init; }
    public string Mail { get; init; } = "";
    public string Lang { get; init; } = "";
    [JsonPropertyName("PaymentID")]
    public int PaymentId { get; init; }
    [JsonPropertyName("PID")]
    public int Pid { get; init; }
}

/// <summary>go-model's <c>pay.Payment</c>.</summary>
public record PayPayment
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public int CustNo { get; init; }
    public int Event { get; init; }
    public PaymentMethod Method { get; init; }
    public string Currency { get; init; } = "";
    public decimal AmountNew { get; init; }
    public decimal Fees { get; init; }
    public decimal UserFees { get; init; }
    public decimal Kickback { get; init; }
    public double ExchangeRate { get; init; }
    public DateTime Created { get; init; }
    public PayState PayState { get; init; }
    public string EventCurrency { get; init; } = "";
    public string Reference { get; init; } = "";
    public string Email { get; init; } = "";
    public int BillNo { get; init; }
    public string Lang { get; init; } = "";
    public bool IgnorePayment { get; init; }
    public string IgnoreReason { get; init; } = "";
    [JsonPropertyName("RequestID")]
    public int RequestId { get; init; }
    [JsonPropertyName("KickbackInvID")]
    public int KickbackInvId { get; init; }
}

/// <summary>go-model's <c>pay.Contract</c>.</summary>
public record PayContract
{
    public int CustNo { get; init; }
    public DateTime ContractDate { get; init; }
    [JsonPropertyName("VATRate")]
    public decimal VatRate { get; init; }
    public string AccountOwner { get; init; } = "";
    [JsonPropertyName("IBAN")]
    public string Iban { get; init; } = "";
    [JsonPropertyName("SWIFT")]
    public string Swift { get; init; } = "";
    public string Type { get; init; } = "";
    public string Language { get; init; } = "";
    public bool HasContract { get; init; }
}

/// <summary>Who receives an event's payments (go-model's <c>pay.EventReceiver</c>).</summary>
public record PayEventReceiver
{
    [JsonPropertyName("EventID")]
    public int EventId { get; init; }
    public int BillCustNo { get; init; }
    public string AccountOwner { get; init; } = "";
    [JsonPropertyName("IBAN")]
    public string Iban { get; init; } = "";
    [JsonPropertyName("SWIFT")]
    public string Swift { get; init; } = "";
    public string Email { get; init; } = "";
    [JsonPropertyName("VATRate")]
    public decimal VatRate { get; init; }
    [JsonPropertyName("SEPANotBefore")]
    public DateTime SepaNotBefore { get; init; }
    [JsonPropertyName("ModJobID")]
    public int ModJobId { get; init; }
}

/// <summary>A payment method and its costs (go-model's <c>pay.Method</c>).</summary>
public record PayMethod
{
    [JsonPropertyName("ID")]
    public PaymentMethod Id { get; init; }
    public string NameShort { get; init; } = "";
    public string Name { get; init; } = "";
    public string Currency { get; init; } = "";
    public decimal TransactionFee { get; init; }
    public decimal Disagio { get; init; }
    public decimal RegFee { get; init; }
    public decimal RefundFee { get; init; }
    public decimal MaxPrice { get; init; }
    public decimal TransactionCosts { get; init; }
    public decimal DisagioCosts { get; init; }
    public int TransferDelay { get; init; }
    public int TransferDelayDecember { get; init; }
    public bool Activated { get; init; }
    public bool NoPayout { get; init; }
    [JsonPropertyName("BankAccountID")]
    public int BankAccountId { get; init; }
    [JsonPropertyName("CaptureAmountAccountID")]
    public int CaptureAmountAccountId { get; init; }
    public bool NoPayoutReceival { get; init; }
    public bool NoTestMode { get; init; }
    public decimal Rounding { get; init; }
}

public record PayBankCheckResult(bool Result, string Error);

/// <summary>go-model's <c>pay.CreateRequest</c>.</summary>
public record PayCreateRequest
{
    public int CustNo { get; init; }
    public int Event { get; init; }
    [JsonPropertyName("IP")]
    public string Ip { get; init; } = "";
    public string RegName { get; init; } = "";
    public string RegKey { get; init; } = "";
    public bool TestMode { get; init; }
    [JsonPropertyName("ChangeID")]
    public int ChangeId { get; init; }
    public decimal Amount { get; init; }
    public decimal Fees { get; init; }
    public decimal UserFees { get; init; }
    public decimal Kickback { get; init; }
    public string Currency { get; init; } = "";
    public double ExchangeRate { get; init; }
    public string EventCurrency { get; init; } = "";
    public Dictionary<string, JsonElement>? PaymentData { get; init; }
    public string Data { get; init; } = "";
    public EntryFeeItem[][]? EntryFeeDetails { get; init; }
    public PaymentMethod Method { get; init; }
    public string Lang { get; init; } = "";
    public string DataToken { get; init; } = "";
    public string PaymentMethodToken { get; init; } = "";
}

/// <summary>go-model's <c>pay.CreateResult</c>.</summary>
public record PayCreateResult
{
    [JsonPropertyName("paymentToken")]
    public string PaymentToken { get; init; } = "";
    [JsonPropertyName("redirect")]
    public string Redirect { get; init; } = "";
    [JsonPropertyName("redirectParams")]
    public Dictionary<string, string>? RedirectParams { get; init; }
    [JsonPropertyName("confirmDetails")]
    public Dictionary<string, string>? ConfirmDetails { get; init; }
}

/// <summary>go-model's <c>pay.FinishResult</c>.</summary>
public record PayFinishResult
{
    public string PaymentToken { get; init; } = "";
    public string RefusalReason { get; init; } = "";
    [JsonPropertyName("PmID")]
    public PaymentMethod PaymentMethod { get; init; }
    public string Data { get; init; } = "";
    public string DataToken { get; init; } = "";
}

/// <summary>go-model's <c>pay.CaptureResponse</c>.</summary>
public record PayCaptureResponse
{
    public Dictionary<string, JsonElement>? AddFields { get; init; }
    [JsonPropertyName("OPJSONs")]
    public string[]? OpJsons { get; init; }
    public InvoicePaymentTerms PaymentTerms { get; init; } = new();
}

/// <summary>go-model's <c>pay.RegistrationFilter</c>.</summary>
public record PayRegistrationFilter
{
    [JsonPropertyName("ID")]
    public int[]? Id { get; init; }
    public int[]? Event { get; init; }
    [JsonPropertyName("PID")]
    public int[]? Pid { get; init; }
    public int[]? Month { get; init; }
    public int[]? Year { get; init; }
    [JsonPropertyName("PaymentID")]
    public int[]? PaymentId { get; init; }
    public string Search { get; init; } = "";
    public DateTime MinTimestamp { get; init; }
    public DateTime MaxTimestamp { get; init; }
}

/// <summary>go-model's <c>pay.PaymentFilter</c>.</summary>
public record PayPaymentFilter
{
    [JsonPropertyName("ID")]
    public int[]? Id { get; init; }
    public int[]? Event { get; init; }
    public int[]? Month { get; init; }
    public int[]? Year { get; init; }
    public PaymentMethod[]? Method { get; init; }
    public string[]? Reference { get; init; }
    public string[]? Email { get; init; }
    public string[]? Search { get; init; }
    public DateTime MinCreated { get; init; }
    public DateTime MaxCreated { get; init; }
    public decimal ToPay { get; init; }
    [JsonPropertyName("RequestID")]
    public int[]? RequestId { get; init; }
    public int[]? BillNo { get; init; }
    public PayState[]? PayState { get; init; }
}
