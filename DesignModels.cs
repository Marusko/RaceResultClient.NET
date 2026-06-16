using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceResultClient;

// Ports of the go-model design-object packages (certificate, certificateset,
// label, list, kiosk, emailtemplate, registration, statistic, page).
// datetime.DateTime fields are mapped to string (non-ISO wire format),
// decimal.Decimal to decimal, interface{} to JsonElement?, variant.VariantMap
// to Dictionary<string, JsonElement>.

// ── page (shared layout enums) ────────────────────────────────────────────────

/// <summary>page.Format — serialized numerically.</summary>
public enum PageFormat { Portrait = 0, Landscape = 1 }

/// <summary>page.Size — serialized numerically.</summary>
public enum PageSize
{
    A1 = 0, A2 = 1, A3 = 2, A4 = 3, A5 = 4,
    Letter = 5, Legal = 6, A6 = 7, UserDefined = 8
}

// ── certificate ───────────────────────────────────────────────────────────────

/// <summary>certificate.PageSize — serialized as a string ("A4", "Letter", …).</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CertificatePageSize
{
    A1 = 0, A2 = 1, A3 = 2, A4 = 3, A5 = 4,
    Letter = 5, Legal = 6, A6 = 7, UserDefined = 8
}

/// <summary>certificate.PageFormat — serialized as a string ("Portrait"/"Landscape").</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CertificatePageFormat { Portrait = 0, Landscape = 1 }

public enum CertificateElementType
{
    Field = 0, Text = 1, Picture = 2, PictureName = 3, Barcode = 4,
    PictureExpression = 5, Rect = 6, Line = 7, Circle = 8, ChunkBox = 9,
    ChipBarcode = 11, BibPosition = 12
}

public enum CertificateBarcodeType { NoBarcode = 0, Code39 = 1, CodeEan13 = 2, Code128 = 3 }

public enum CertificatePictureStretchMode { Stretch = 0, NoStretch = 1 }

public record Certificate
{
    [JsonPropertyName("CertificateName")]
    public string Name { get; init; } = "";
    public CertificatePageSize PageSize { get; init; }
    public CertificatePageFormat PageFormat { get; init; }
    public int PageHeight { get; init; }
    public int PageWidth { get; init; }
    public int SheetHeight { get; init; }
    public int SheetWidth { get; init; }
    public int MarginTop { get; init; }
    public int MarginLeft { get; init; }
    public int MarginRight { get; init; }
    public int MarginBottom { get; init; }
    public int CutLeft { get; init; }
    public int CutTop { get; init; }
    public int CutBottom { get; init; }
    public int CutRight { get; init; }
    public int DistanceVertical { get; init; }
    public int DistanceHorizontal { get; init; }
    public int Holes { get; init; }
    public string SpecialHoles { get; init; } = "";
    public string Substrate { get; init; } = "";
    public bool RgbBlackToCmyk { get; init; }
    public string CmykBlackValue { get; init; } = "";
    public string PrintNotes { get; init; } = "";
    public int Copies { get; init; }
    public string PrintMode { get; init; } = "";
    public bool Reverse { get; init; }
    public bool RoundedCorners { get; init; }
    public bool PlotterMarks { get; init; }
    public int ChipType { get; init; }
    public int Machine { get; init; }
    public int BlockSize { get; init; }
    public int Version { get; init; }
    [JsonPropertyName("Fields")]
    public CertificateElement[] Elements { get; init; } = [];
    public CertificateZone[] Zones { get; init; } = [];
}

public record CertificateElement
{
    public CertificateElementType Type { get; init; }
    public string Data { get; init; } = "";
    public decimal Left { get; init; }
    public decimal Top { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public string FontName { get; init; } = "";
    public int FontSize { get; init; }
    public bool FontBold { get; init; }
    public bool FontItalic { get; init; }
    public bool FontUnderlined { get; init; }
    public int FontColor { get; init; }
    public string FontColorCmyk { get; init; } = "";
    public int Alignment { get; init; }
    [JsonPropertyName("vAlignment")]
    public int VAlignment { get; init; }
    public int Page { get; init; }
    public int Rotation { get; init; }
    [JsonPropertyName("DF")]
    public string DynamicFormat { get; init; } = "";
    [JsonPropertyName("Stretch")]
    public CertificatePictureStretchMode PictureStretch { get; init; }
    [JsonPropertyName("Barcode")]
    public CertificateBarcodeType BarcodeType { get; init; }
    public bool Locked { get; init; }
    public decimal TextScaling { get; init; }
    public decimal TextCharSpacing { get; init; }
    public decimal OutlineWidth { get; init; }
    public string OutlineColor { get; init; } = "";
    public decimal Transparency { get; init; }
}

public record CertificateZone(decimal Top, int Page, string Type);

// ── certificateset ────────────────────────────────────────────────────────────

public enum CertificateSetType { SingleCertificate = 0, TeamCertificate = 1, TeamCertificateMulti = 2 }

public record CertificateSet
{
    public string Name { get; init; } = "";
    [JsonPropertyName("CertificateName")]
    public string Certificate { get; init; } = "";
    public CertificateSetType CertificateSetType { get; init; }
    public int TeamScore { get; init; }
    public int FilterRankId { get; init; }
    public string FilterRankOperator { get; init; } = "";
    public int FilterRankCompare { get; init; }
    public int FilterAyn { get; init; }
    public bool FilterOnlyFinishers { get; init; }
    public string FilterGeneral { get; init; } = "";
    public bool OnlyUnshownCertificates { get; init; }
    public string Sort1 { get; init; } = "";
    public string Sort2 { get; init; } = "";
    public string Sort3 { get; init; } = "";
    public string Sort4 { get; init; } = "";
    public bool SortDesc1 { get; init; }
    public bool SortDesc2 { get; init; }
    public bool SortDesc3 { get; init; }
    public bool SortDesc4 { get; init; }
}

// ── label ─────────────────────────────────────────────────────────────────────

public enum LabelDirection { DownThenRight = 0, RightThenDown = 1 }
public enum LabelBarcodeType { NoBarcode = 0, Code39 = 1, CodeEan13 = 2, Code128 = 3 }
public enum LabelAlignment { Left = 1, Center = 2, Right = 3 }

public record Label
{
    [JsonPropertyName("LabelName")]
    public string Name { get; init; } = "";
    public PageFormat PageFormat { get; init; }
    public PageSize PageSize { get; init; }
    public decimal PageHeight { get; init; }
    public decimal PageWidth { get; init; }
    public decimal PageMarginTop { get; init; }
    public decimal PageMarginLeft { get; init; }
    public decimal PageMarginBottom { get; init; }
    public decimal PageMarginRight { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public decimal SpacingVertical { get; init; }
    public decimal SpacingHorizontal { get; init; }
    public LabelDirection Direction { get; init; }
    public string Expression { get; init; } = "";
    public string FontName { get; init; } = "";
    public int FontSize { get; init; }
    public bool FontBold { get; init; }
    public bool FontItalic { get; init; }
    public bool FontUnderline { get; init; }
    public string Design { get; init; } = "";
    public LabelBarcodeType BarcodeType { get; init; }
    public LabelAlignment Alignment { get; init; }
    public string Filter { get; init; } = "";
    public string Sort1 { get; init; } = "";
    public string Sort2 { get; init; } = "";
    public string Sort3 { get; init; } = "";
    public bool SortDesc1 { get; init; }
    public bool SortDesc2 { get; init; }
    public bool SortDesc3 { get; init; }
}

// ── list ──────────────────────────────────────────────────────────────────────

public enum ListShowAt { Never = 0, FirstPage = 1, EveryPage = 2, LastPage = 3 }
public enum ListPageBreak { NoPageBreak = 0, NewPage = 1, KeepTogether = 2, NewColumn = 3, Repeat = 4 }

public record List
{
    [JsonPropertyName("ListName")]
    public string Name { get; init; } = "";
    public string BottomPicture { get; init; } = "";
    public ListShowAt BottomPictureShow { get; init; }
    public string ColumnHeadsFontName { get; init; } = "";
    public int ColumnHeadsFontSize { get; init; }
    public bool ColumnHeadsFontBold { get; init; }
    public bool ColumnHeadsFontItalic { get; init; }
    public bool ColumnHeadsFontUnderlined { get; init; }
    public string ColumnHeadsColor { get; init; } = "";
    public ListShowAt ColumnHeadsShow { get; init; }
    public int Columns { get; init; }
    public decimal ColumnSpacing { get; init; }
    public string CoverSheet { get; init; } = "";
    public string BackSheet { get; init; } = "";
    public string Design { get; init; } = "";
    public ListShowAt DesignShow { get; init; }
    public bool EveryOtherLineGray { get; init; }
    public string FontName { get; init; } = "";
    public int FontSize { get; init; }
    public string FooterFontName { get; init; } = "";
    public int FooterFontSize { get; init; }
    public bool FooterFontBold { get; init; }
    public bool FooterFontItalic { get; init; }
    public bool FooterFontUnderlined { get; init; }
    public string FooterColor { get; init; } = "";
    public bool GrayLine { get; init; }
    public string HeadLine1 { get; init; } = "";
    public string HeadLine1FontName { get; init; } = "";
    public int HeadLine1FontSize { get; init; }
    public bool HeadLine1FontBold { get; init; }
    public bool HeadLine1FontItalic { get; init; }
    public bool HeadLine1FontUnderlined { get; init; }
    public string HeadLine2 { get; init; } = "";
    public string HeadLine1Color { get; init; } = "";
    public ListShowAt HeadLine1Show { get; init; }
    public string HeadLine2FontName { get; init; } = "";
    public int HeadLine2FontSize { get; init; }
    public bool HeadLine2FontBold { get; init; }
    public bool HeadLine2FontItalic { get; init; }
    public bool HeadLine2FontUnderlined { get; init; }
    public string HeadLine2Color { get; init; } = "";
    public ListShowAt HeadLine2Show { get; init; }
    public decimal HeightBottomPicture { get; init; }
    public string LineColor { get; init; } = "";
    public string LineBackColor { get; init; } = "";
    public string LineDynamicFormat { get; init; } = "";
    public decimal LineSpacing { get; init; }
    public int MaxRecords { get; init; }
    public string MultiplierField { get; init; } = "";
    public PageFormat PageFormat { get; init; }
    public decimal PageMarginBottom { get; init; }
    public decimal PageMarginLeft { get; init; }
    public decimal PageMarginRight { get; init; }
    public decimal PageMarginTop { get; init; }
    public PageSize PageSize { get; init; }
    public decimal PageHeight { get; init; }
    public decimal PageWidth { get; init; }
    public bool SepLine { get; init; }
    public string TopRightPicture { get; init; } = "";
    public ListShowAt TopRightPictureShow { get; init; }
    public string ListHeaderText { get; init; } = "";
    public string ListFooterText { get; init; } = "";
    public string ListHeaderFooterFontName { get; init; } = "";
    public int ListHeaderFooterFontSize { get; init; }
    public bool ListHeaderFooterFontBold { get; init; }
    public bool ListHeaderFooterFontItalic { get; init; }
    public bool ListHeaderFooterFontUnderlined { get; init; }
    public int ListHeaderFooterAlignment { get; init; }
    public string Remarks { get; init; } = "";
    public string LastChange { get; init; } = "";
    public string FooterText1 { get; init; } = "";
    public string FooterText2 { get; init; } = "";
    public string FooterText3 { get; init; } = "";
    public ListOrder[] Orders { get; init; } = [];
    public ListFilter[] Filters { get; init; } = [];
    public ListField[] Fields { get; init; } = [];
    public ListSelectorResult[] SelectorResults { get; init; } = [];
}

public record ListOrder
{
    public string Expression { get; init; } = "";
    public bool Descending { get; init; }
    [JsonPropertyName("Grouping")]
    public int Grouping2 { get; init; }
    public int GroupFilterDefault { get; init; }
    public string GroupFilterLabel { get; init; } = "";
    public ListPageBreak PageBreak { get; init; }
    public string FontName { get; init; } = "";
    public int FontSize { get; init; }
    public bool FontBold { get; init; }
    public bool FontItalic { get; init; }
    public bool FontUnderlined { get; init; }
    public string Color { get; init; } = "";
    public string BackgroundColor { get; init; } = "";
    public int Spacing { get; init; }
}

public record ListFilter
{
    public bool OrConjunction { get; init; }
    public string Expression1 { get; init; } = "";
    public string Operator { get; init; } = "";
    public string Expression2 { get; init; } = "";
}

public record ListField
{
    public string Expression { get; init; } = "";
    public string Label { get; init; } = "";
    public string Label2 { get; init; } = "";
    public int Alignment { get; init; }
    public bool FontBold { get; init; }
    public bool FontItalic { get; init; }
    public bool FontUnderlined { get; init; }
    public int Line { get; init; }
    public string Color { get; init; } = "";
    public string Link { get; init; } = "";
    public int ColSpan { get; init; }
    public int ColOffset { get; init; }
    public decimal Position { get; init; }
    public string DynamicFormat { get; init; } = "";
    public bool PreviewOnly { get; init; }
    public int ResponsiveHide { get; init; }
}

public record ListSelectorResult(int ResultId, int ResultId2, string ShowAs);

// ── kiosk ─────────────────────────────────────────────────────────────────────

public record Kiosk
{
    public string Name { get; init; } = "";
    public string Key { get; init; } = "";
    public bool Enabled { get; init; }
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
    public int TransponderMode { get; init; }
    public int AcceptedTransponders { get; init; }
    public bool IgnoreBibRanges { get; init; }
    public bool AutoFinish { get; init; }
    public string Css { get; init; } = "";
    public string Title { get; init; } = "";
    public KioskStep[] Steps { get; init; } = [];
    public KioskAfterSave[] AfterSave { get; init; } = [];
}

public record KioskStep
{
    public string Type { get; init; } = "";
    public string Label { get; init; } = "";
    public string Title { get; init; } = "";
    public string Text { get; init; } = "";
    public string OnlyShowIf { get; init; } = "";
    public KioskSearchField[]? SearchFields { get; init; }
    public KioskDisplayField[]? DisplayFields { get; init; }
    public KioskEditField[]? EditFields { get; init; }
    public Dictionary<string, JsonElement>? Settings { get; init; }
}

public record KioskAfterSave
{
    public string Type { get; init; } = "";
    public string Value { get; init; } = "";
    public string Destination { get; init; } = "";
    public string Filter { get; init; } = "";
    public string Printer { get; init; } = "";
    public string[]? Flags { get; init; }
}

public record KioskDisplayField(string Type, string Value, string Label);

public record KioskEditField
{
    public string Label { get; init; } = "";
    public string Field { get; init; } = "";
    public string Special { get; init; } = "";
    public bool Mandatory { get; init; }
    public string ValidationRule { get; init; } = "";
    public string ValidationMsg { get; init; } = "";
    public string EventTools { get; init; } = "";
}

public record KioskSearchField(string Field, bool Hide, string Function);

// ── emailtemplate ─────────────────────────────────────────────────────────────

public enum EmailTemplateType { Single = 0, Group = 1, Sms = 2, WebService = 3, GroupById = 4 }
public enum EmailAttachmentType { File = 0, Certificate = 1, Url = 2, UnsentInvoice = 3 }
public enum EmailAttachmentSendForType { Last = 0, First = 1, All = 2, Any = 3 }

public record HttpHeader(string Name, string Value);

public record EmailTemplate
{
    public string Name { get; init; } = "";
    public EmailTemplateType Type { get; init; }
    public string Sender { get; init; } = "";
    public string SenderName { get; init; } = "";
    public string ReplyTo { get; init; } = "";
    public string Cc { get; init; } = "";
    public string Bcc { get; init; } = "";
    public string ReceiverField { get; init; } = "";
    public bool Html { get; init; }
    public string Method { get; init; } = "";
    public string Subject { get; init; } = "";
    public string Text { get; init; } = "";
    public string Header { get; init; } = "";
    public string Footer { get; init; } = "";
    public string DefaultFilter { get; init; } = "";
    public string SetCustomFieldAfterSending { get; init; } = "";
    public string SaveResultIn { get; init; } = "";
    public EmailAttachment[]? Attachments { get; init; }
    public HttpHeader[]? HttpHeaders { get; init; }
}

public record EmailAttachment
{
    public EmailAttachmentType Type { get; init; }
    public string Name { get; init; } = "";
    public string Label { get; init; } = "";
    public string Filter { get; init; } = "";
    public EmailAttachmentSendForType SendFor { get; init; }
}

public record EmailPreview
{
    public EmailTemplateType Type { get; init; }
    public int[]? Bibs { get; init; }
    public int[]? Pids { get; init; }
    public string? Sender { get; init; }
    public string? SenderName { get; init; }
    public string? ReplyTo { get; init; }
    public string? Cc { get; init; }
    public string? Bcc { get; init; }
    public string? CellPhone { get; init; }
    public string? Email { get; init; }
    public string? Subject { get; init; }
    public string? Text { get; init; }
    public bool Html { get; init; }
    public string? Url { get; init; }
    public string? Method { get; init; }
    public EmailPreviewAttachment[]? Attachments { get; init; }
    public HttpHeader[]? HttpHeaders { get; init; }
    public string[]? Errors { get; init; }
}

public record EmailPreviewAttachment(EmailAttachmentType Type, string Name, string Label, int Bib, int Pid);

// ── registration ──────────────────────────────────────────────────────────────

public record Registration
{
    public string Name { get; init; } = "";
    public string Key { get; init; } = "";
    public string ChangeKeySalt { get; init; } = "";
    public string Title { get; init; } = "";
    public bool Enabled { get; init; }
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
    public string TestModeKey { get; init; } = "";
    public string Type { get; init; } = "";
    public int GroupMin { get; init; }
    public int GroupMax { get; init; }
    public int GroupDefault { get; init; }
    public int GroupInc { get; init; }
    public int Contest { get; init; }
    public int Limit { get; init; }
    public string ChangeIdentityField { get; init; } = "";
    public string ChangeIdentityFilter { get; init; } = "";
    public RegistrationStep[] Steps { get; init; } = [];
    public RegistrationAdditionalValue[] AdditionalValues { get; init; } = [];
    public bool CheckSex { get; init; }
    public bool CheckDuplicate { get; init; }
    public bool DontProposeGender { get; init; }
    public bool OnlinePayment { get; init; }
    public string OnlinePaymentButtonText { get; init; } = "";
    public RegistrationPaymentMethod[] PaymentMethods { get; init; } = [];
    public bool OnlineRefund { get; init; }
    public RegistrationPaymentMethod[] RefundMethods { get; init; } = [];
    public RegistrationConfirmation Confirmation { get; init; } = new();
    public RegistrationAfterSave[] AfterSave { get; init; } = [];
    public string Css { get; init; } = "";
    public RegistrationErrorMessages ErrorMessages { get; init; } = new();
}

public record RegistrationComponent(string Name, RegistrationElement[] Elements);

public record RegistrationStep
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public bool Enabled { get; init; }
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
    public RegistrationElement[] Elements { get; init; } = [];
    public string ButtonText { get; init; } = "";
}

public record RegistrationElement
{
    public string Type { get; init; } = "";
    public string Label { get; init; } = "";
    public bool Enabled { get; init; }
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
    public RegistrationField? Field { get; init; }
    public string ShowIf { get; init; } = "";
    public int ShowIfMode { get; init; }
    public string ShowIfCurr { get; init; } = "";
    public int ShowIfCurrMode { get; init; }
    public bool ShowIfInitial { get; init; }
    public RegistrationStyle[]? Styles { get; init; }
    public string ClassName { get; init; } = "";
    public int Id { get; init; }
    public int Common { get; init; }
    public RegistrationValidationRule[]? ValidationRules { get; init; }
    public RegistrationElement[]? Children { get; init; }
}

public record RegistrationField
{
    public string Name { get; init; } = "";
    public string ControlType { get; init; } = "";
    public int Mandatory { get; init; }
    public string DefaultValue { get; init; } = "";
    public int DefaultValueType { get; init; }
    public string Placeholder { get; init; } = "";
    public string Unique { get; init; } = "";
    public string Special { get; init; } = "";
    public string SpecialDetails { get; init; } = "";
    public bool ForceUpdate { get; init; }
    public RegistrationValue[]? Values { get; init; }
    public string[]? AdditionalOptions { get; init; }
    public string[]? Flags { get; init; }
}

public record RegistrationStyle(string Attribute, string Value);

public record RegistrationValue
{
    public JsonElement? Value { get; init; }
    public string Label { get; init; } = "";
    public bool Enabled { get; init; }
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
    public int MaxCapacity { get; init; }
    public string ShowIf { get; init; } = "";
}

public record RegistrationAdditionalValue
{
    public string FieldName { get; init; } = "";
    public string Source { get; init; } = "";
    public string Value { get; init; } = "";
    public string Filter { get; init; } = "";
    public string FilterInitial { get; init; } = "";
}

public record RegistrationConfirmation
{
    public string Title { get; init; } = "";
    public string Expression { get; init; } = "";
}

public record RegistrationAfterSave
{
    public string Type { get; init; } = "";
    public string Value { get; init; } = "";
    public string Destination { get; init; } = "";
    public string Filter { get; init; } = "";
    public string[]? Flags { get; init; }
}

public record RegistrationPaymentMethod
{
    public int Id { get; init; }
    public string Label { get; init; } = "";
    public bool Enabled { get; init; }
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
    public string Filter { get; init; } = "";
}

public record RegistrationValidationRule(string Rule, string Msg);

public record RegistrationErrorMessages
{
    public string BeforRegStart { get; init; } = "";
    public string AfterRegEnd { get; init; } = "";
}

// ── statistic ─────────────────────────────────────────────────────────────────

public enum StatisticAggregation { Count = 1, Minimum = 2, Maximum = 3, Mean = 4, Sum = 5 }

public record Statistics
{
    [JsonPropertyName("StatisticName")]
    public string Name { get; init; } = "";
    public string Type { get; init; } = "";
    public string Row { get; init; } = "";
    public string Col { get; init; } = "";
    public string Filter { get; init; } = "";
    public bool OnlyFinishers { get; init; }
    public string Field { get; init; } = "";
    public StatisticAggregation Aggregation { get; init; }
    public bool SortByValue { get; init; }
    public bool SortDesc { get; init; }
    public string Headline1 { get; init; } = "";
    public string Headline2 { get; init; } = "";
    public decimal LineSpacing { get; init; }
    public string FontName { get; init; } = "";
    public int FontSize { get; init; }
    public PageFormat PageFormat { get; init; }
    public decimal PageMarginBottom { get; init; }
    public decimal PageMarginLeft { get; init; }
    public decimal PageMarginRight { get; init; }
    public decimal PageMarginTop { get; init; }
    public PageSize PageSize { get; init; }
    public decimal PageHeight { get; init; }
    public decimal PageWidth { get; init; }
    public string TopLeftHeader { get; init; } = "";
}
