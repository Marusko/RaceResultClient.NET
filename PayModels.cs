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
    public RaceResultDate Date { get; init; }
    [JsonPropertyName("PID")]
    public int Pid { get; init; }
    public InvoiceParty Receiver { get; init; } = new();
    public InvoiceParty Issuer { get; init; } = new();
    public string Currency { get; init; } = "";
    public string LegalNotes { get; init; } = "";
    public RaceResultDate PerformanceDate { get; init; }
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

/// <summary>
/// How an event makes its invoices (go-model's <c>invoice.Settings</c>; RaceResult's invoice settings).
/// The event stores them as separate settings named <c>Invoice…</c>; <see cref="FromSettings"/> and
/// <see cref="ToSettings"/> convert, and <see cref="SettingsEndpoints.GetInvoiceSettingsAsync"/> /
/// <see cref="SettingsEndpoints.SaveInvoiceSettingsAsync"/> read and save them.
/// </summary>
public record InvoiceSettings
{
    /// <summary>"First Invoice Number"; <c>[yyyy]</c> is the current year, e.g. <c>[yyyy]0001</c>.</summary>
    public string NumberScheme { get; init; } = "";
    /// <summary>"Legal Notes" (company registration ID, CEO, …).</summary>
    public string LegalNotes { get; init; } = "";
    /// <summary>"Performance Date", when it is not the event date.</summary>
    public RaceResultDate PerformanceDate { get; init; }
    /// <summary>"E-Invoice Format"; empty for none.</summary>
    public string EInvoice { get; init; } = "";
    /// <summary>"Participants grouping: Filter": invoices only for participants who meet it.</summary>
    public string ReceiverFilter { get; init; } = "";
    /// <summary>
    /// "Participants grouping: Grouping Field": other participants with the same value in this field are
    /// added to the invoice (e.g. a corporate run with an extra participant for the company registration).
    /// </summary>
    public string ReceiverMergeField { get; init; } = "";
    /// <summary>"Invoice Receiver": the participant fields the receiver's data is taken from (e.g. <c>CLUB</c>).</summary>
    public string ReceiverFieldCompany { get; init; } = "";
    public string ReceiverFieldName { get; init; } = "";
    public string ReceiverFieldAddressLine1 { get; init; } = "";
    public string ReceiverFieldAddressLine2 { get; init; } = "";
    public string ReceiverFieldCity { get; init; } = "";
    [JsonPropertyName("ReceiverFieldZIP")]
    public string ReceiverFieldZip { get; init; } = "";
    public string ReceiverFieldState { get; init; } = "";
    public string ReceiverFieldCountry { get; init; } = "";
    /// <summary>The field for "Customer's Order Reference".</summary>
    public string ReceiverFieldReference { get; init; } = "";
    [JsonPropertyName("ReceiverFieldTaxID")]
    public string ReceiverFieldTaxId { get; init; } = "";
    [JsonPropertyName("ReceiverFieldVATID")]
    public string ReceiverFieldVatId { get; init; } = "";
    /// <summary>"Invoice Issuer": fixed values.</summary>
    public InvoiceParty Issuer { get; init; } = new();
    /// <summary>"Payment Terms" for invoices made by hand.</summary>
    public InvoicePaymentTerms PaymentTerms { get; init; } = new();
    /// <summary>"Exclude Entry Fees": the IDs of entry fees left off invoices.</summary>
    public int[]? ExcludedEntryFees { get; init; }

    // ── the event's Invoice… settings ────────────────────────────────────────

    /// <summary>The names of the event settings that hold these settings.</summary>
    public static readonly string[] SettingNames =
    [
        "InvoiceNumberScheme", "InvoiceEInvoice", "InvoicePerformanceDate", "InvoiceLegalNotes",
        "InvoiceReceiverFilter", "InvoiceReceiverMergeField",
        "InvoiceIssuerCompany", "InvoiceIssuerName", "InvoiceIssuerAddressLine1", "InvoiceIssuerAddressLine2",
        "InvoiceIssuerCity", "InvoiceIssuerZIP", "InvoiceIssuerState", "InvoiceIssuerCountry",
        "InvoiceIssuerTaxID", "InvoiceIssuerVATID",
        "InvoiceReceiverFieldCompany", "InvoiceReceiverFieldName", "InvoiceReceiverFieldAddressLine1",
        "InvoiceReceiverFieldAddressLine2", "InvoiceReceiverFieldCity", "InvoiceReceiverFieldZIP",
        "InvoiceReceiverFieldState", "InvoiceReceiverFieldCountry", "InvoiceReceiverFieldTaxID",
        "InvoiceReceiverFieldVATID", "InvoiceReceiverFieldReference",
        "InvoicePaymentTermsAccountOwner", "InvoicePaymentTermsIBAN", "InvoicePaymentTermsBIC",
        "InvoicePaymentTermsAccountNo", "InvoicePaymentTermsBranchNo", "InvoicePaymentTermsBankName",
        "InvoicePaymentTermsDueDays", "InvoicePaymentTerms", "InvoiceExcludedEntryFees"
    ];

    /// <summary>
    /// Reads the event's <c>Invoice…</c> settings (as <c>settings/getsettings</c> returns them for
    /// <see cref="SettingNames"/>). A missing setting reads as empty; <c>InvoiceExcludedEntryFees</c> is
    /// the IDs as text (<c>"1,2"</c>).
    /// </summary>
    public static InvoiceSettings FromSettings(IReadOnlyDictionary<string, JsonElement> settings)
    {
        string Str(string name) => settings.TryGetValue(name, out var v)
            ? v.ValueKind switch
            {
                JsonValueKind.String => v.GetString() ?? "",
                JsonValueKind.Number => v.GetRawText(),
                _ => ""
            }
            : "";
        int Int(string name) => int.TryParse(Str(name), System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out var n) ? n : 0;

        return new InvoiceSettings
        {
            NumberScheme = Str("InvoiceNumberScheme"),
            EInvoice = Str("InvoiceEInvoice"),
            PerformanceDate = RaceResultDate.TryParse(Str("InvoicePerformanceDate"), out var d) ? d : default,
            LegalNotes = Str("InvoiceLegalNotes"),
            ReceiverFilter = Str("InvoiceReceiverFilter"),
            ReceiverMergeField = Str("InvoiceReceiverMergeField"),
            Issuer = new InvoiceParty
            {
                Company = Str("InvoiceIssuerCompany"),
                Name = Str("InvoiceIssuerName"),
                AddressLine1 = Str("InvoiceIssuerAddressLine1"),
                AddressLine2 = Str("InvoiceIssuerAddressLine2"),
                City = Str("InvoiceIssuerCity"),
                Zip = Str("InvoiceIssuerZIP"),
                State = Str("InvoiceIssuerState"),
                Country = Int("InvoiceIssuerCountry"),
                TaxId = Str("InvoiceIssuerTaxID"),
                VatId = Str("InvoiceIssuerVATID"),
            },
            ReceiverFieldCompany = Str("InvoiceReceiverFieldCompany"),
            ReceiverFieldName = Str("InvoiceReceiverFieldName"),
            ReceiverFieldAddressLine1 = Str("InvoiceReceiverFieldAddressLine1"),
            ReceiverFieldAddressLine2 = Str("InvoiceReceiverFieldAddressLine2"),
            ReceiverFieldCity = Str("InvoiceReceiverFieldCity"),
            ReceiverFieldZip = Str("InvoiceReceiverFieldZIP"),
            ReceiverFieldState = Str("InvoiceReceiverFieldState"),
            ReceiverFieldCountry = Str("InvoiceReceiverFieldCountry"),
            ReceiverFieldTaxId = Str("InvoiceReceiverFieldTaxID"),
            ReceiverFieldVatId = Str("InvoiceReceiverFieldVATID"),
            ReceiverFieldReference = Str("InvoiceReceiverFieldReference"),
            PaymentTerms = new InvoicePaymentTerms
            {
                AccountOwner = Str("InvoicePaymentTermsAccountOwner"),
                Iban = Str("InvoicePaymentTermsIBAN"),
                Bic = Str("InvoicePaymentTermsBIC"),
                AccountNo = Str("InvoicePaymentTermsAccountNo"),
                BranchNo = Str("InvoicePaymentTermsBranchNo"),
                BankName = Str("InvoicePaymentTermsBankName"),
                DueDays = Int("InvoicePaymentTermsDueDays"),
                Terms = Str("InvoicePaymentTerms"),
            },
            ExcludedEntryFees = Str("InvoiceExcludedEntryFees")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => int.TryParse(x, out var id) ? id : (int?)null)
                .OfType<int>()
                .ToArray(),
        };
    }

    /// <summary>
    /// These settings as the event's <c>Invoice…</c> settings, typed as RaceResult stores them (the issuer's
    /// country and the due days as numbers, the rest as text). <see cref="PaymentTerms"/>' <c>Method</c>,
    /// <c>MandateId</c> and <c>Reference</c> have no event setting and are left out.
    /// </summary>
    public Setting[] ToSettings()
    {
        Setting S(string name, object value) => new() { Name = name, Value = value };
        return
        [
            S("InvoiceNumberScheme", NumberScheme),
            S("InvoiceEInvoice", EInvoice),
            S("InvoicePerformanceDate", PerformanceDate.ToString()),
            S("InvoiceLegalNotes", LegalNotes),
            S("InvoiceReceiverFilter", ReceiverFilter),
            S("InvoiceReceiverMergeField", ReceiverMergeField),
            S("InvoiceIssuerCompany", Issuer.Company),
            S("InvoiceIssuerName", Issuer.Name),
            S("InvoiceIssuerAddressLine1", Issuer.AddressLine1),
            S("InvoiceIssuerAddressLine2", Issuer.AddressLine2),
            S("InvoiceIssuerCity", Issuer.City),
            S("InvoiceIssuerZIP", Issuer.Zip),
            S("InvoiceIssuerState", Issuer.State),
            S("InvoiceIssuerCountry", Issuer.Country),
            S("InvoiceIssuerTaxID", Issuer.TaxId),
            S("InvoiceIssuerVATID", Issuer.VatId),
            S("InvoiceReceiverFieldCompany", ReceiverFieldCompany),
            S("InvoiceReceiverFieldName", ReceiverFieldName),
            S("InvoiceReceiverFieldAddressLine1", ReceiverFieldAddressLine1),
            S("InvoiceReceiverFieldAddressLine2", ReceiverFieldAddressLine2),
            S("InvoiceReceiverFieldCity", ReceiverFieldCity),
            S("InvoiceReceiverFieldZIP", ReceiverFieldZip),
            S("InvoiceReceiverFieldState", ReceiverFieldState),
            S("InvoiceReceiverFieldCountry", ReceiverFieldCountry),
            S("InvoiceReceiverFieldTaxID", ReceiverFieldTaxId),
            S("InvoiceReceiverFieldVATID", ReceiverFieldVatId),
            S("InvoiceReceiverFieldReference", ReceiverFieldReference),
            S("InvoicePaymentTermsAccountOwner", PaymentTerms.AccountOwner),
            S("InvoicePaymentTermsIBAN", PaymentTerms.Iban),
            S("InvoicePaymentTermsBIC", PaymentTerms.Bic),
            S("InvoicePaymentTermsAccountNo", PaymentTerms.AccountNo),
            S("InvoicePaymentTermsBranchNo", PaymentTerms.BranchNo),
            S("InvoicePaymentTermsBankName", PaymentTerms.BankName),
            S("InvoicePaymentTermsDueDays", PaymentTerms.DueDays),
            S("InvoicePaymentTerms", PaymentTerms.Terms),
            S("InvoiceExcludedEntryFees", string.Join(",", ExcludedEntryFees ?? [])),
        ];
    }
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

/// <summary>
/// RACE RESULT's payment methods (go-model's <c>pay.Pm…</c> constants), named as RaceResult's
/// registration settings list them ("Payment Method"). Used by <see cref="RegistrationPaymentMethod.Id"/>,
/// <see cref="InvoicePaymentTerms.Method"/> and the pay records. "Your Own …" is the organizer's own
/// account with that provider.
/// </summary>
public enum PaymentMethod
{
    /// <summary>No payment (go-model's <c>PmNoPayment</c>).</summary>
    NoPayment = 0,
    /// <summary>Credit Card EUR (Visa/Master) (<c>PmCCEUR</c>).</summary>
    CreditCardEur = 2,
    /// <summary>Credit Card CHF (Visa/Master) (<c>PmCCCHF</c>).</summary>
    CreditCardChf = 3,
    /// <summary>Money Transfer (to European bank account) (<c>PmUEBD</c>).</summary>
    MoneyTransferEuropean = 4,
    /// <summary>Pay in cash at the event (<c>PmBAR</c>).</summary>
    CashAtEvent = 5,
    /// <summary>Swiss PostFinance (<c>PmSPF</c>).</summary>
    SwissPostFinance = 6,
    /// <summary>PayPal (EUR) (<c>PmPPalEUR</c>).</summary>
    PayPalEur = 7,
    /// <summary>Money Transfer (to Swiss bank account) (<c>PmUEBCH</c>).</summary>
    MoneyTransferSwiss = 8,
    /// <summary>Swiss Payment Order (<c>PmEINZCH</c>).</summary>
    SwissPaymentOrder = 10,
    /// <summary>go-model's <c>PmUEBSOF</c>; not offered in RaceResult's list any more.</summary>
    UebSof = 12,
    /// <summary>PayPal (GBP) (<c>PmPPalGBP</c>).</summary>
    PayPalGbp = 14,
    /// <summary>PayPal (USD) (<c>PmPPalUSD</c>); not offered in RaceResult's list any more.</summary>
    PayPalUsd = 15,
    /// <summary>SEPA Direct Debit (EUR) (<c>PmSEPA</c>).</summary>
    SepaDirectDebit = 16,
    /// <summary>Credit Card GBP (Visa/Master) (<c>PmCCGBP</c>).</summary>
    CreditCardGbp = 17,
    /// <summary>SEPA Direct Debit (collect data only) (<c>PmSEPADATA</c>).</summary>
    SepaDirectDebitDataOnly = 19,
    /// <summary>Your Own EPay Account (<c>PmOwnEPAY</c>).</summary>
    OwnEPay = 20,
    /// <summary>Your Own PayPal Account (<c>PmOwnPPal</c>).</summary>
    OwnPayPal = 21,
    /// <summary>Wire Transfer to Race Director Bank Account (<c>PmOwnWireT</c>).</summary>
    OwnWireTransfer = 22,
    /// <summary>Your Own PaysBuy Account (in RaceResult's list, not in go-model).</summary>
    OwnPaysBuy = 24,
    /// <summary>go-model's <c>PmOwnPaytrail</c>; RaceResult's list offers only <see cref="OwnPaytrailV2"/>.</summary>
    OwnPaytrail = 25,
    /// <summary>Your Own OnePAY Account (<c>PmOwnOnePAY</c>).</summary>
    OwnOnePay = 26,
    /// <summary>Your Own Telr Account (Auth+Capture) (<c>PmTelr</c>).</summary>
    OwnTelr = 27,
    /// <summary>Your Own OnePAY Domestic Account (<c>PmOwnOnePAYDom</c>).</summary>
    OwnOnePayDomestic = 28,
    /// <summary>Your Own Fatora Account (<c>PmFatora</c>).</summary>
    OwnFatora = 29,
    /// <summary>TWINT (CHF) (<c>PmTwint</c>).</summary>
    Twint = 30,
    /// <summary>Your Own Stripe Account (<c>PmStripeCard</c>).</summary>
    OwnStripe = 31,
    /// <summary>Your Own Paytrail Account V2 (<c>PmOwnPaytrailV2</c>).</summary>
    OwnPaytrailV2 = 32,
    /// <summary>Your Own Telr Account (Sale) (<c>PmTelrSale</c>).</summary>
    OwnTelrSale = 33,
    /// <summary>Your Own RedSys Account (<c>PmRedSys</c>).</summary>
    OwnRedSys = 34,
    /// <summary>Your Own Mollie Account for Bancontact (<c>PmMollieBancontact</c>).</summary>
    OwnMollieBancontact = 35,
    /// <summary>Your Own PayTabs Account (<c>PmPayTabs</c>).</summary>
    OwnPayTabs = 36,
    /// <summary>Your Own AsiaPay Account (<c>PmAsiaPay</c>).</summary>
    OwnAsiaPay = 37,
    /// <summary>Your Own MercadoPago Account (<c>PmMercadoPago</c>).</summary>
    OwnMercadoPago = 38,
    /// <summary>Your Own Peach Payments Account (in RaceResult's list, not in go-model).</summary>
    OwnPeachPayments = 39,
    /// <summary>Your Own Przelewy24 Account (in RaceResult's list, not in go-model).</summary>
    OwnPrzelewy24 = 40,
    /// <summary>Your Own Xendit Account (<c>PmXendit</c>).</summary>
    OwnXendit = 41,
    /// <summary>Your Own Tazapay Account (<c>PmTazaPay</c>).</summary>
    OwnTazaPay = 42,
    /// <summary>go-model's <c>PmCB</c>; not in RaceResult's list.</summary>
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
