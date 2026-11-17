using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceResultClient;

// Ports of the go-model design-object packages (certificate, certificateset,
// label, list, kiosk, emailtemplate, registration, statistic, page).
// datetime.DateTime fields are mapped to RaceResultDateTime,
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

public static class PageSizeExtensions
{
    /// <summary>
    /// Width and height in mm, swapped for landscape (go-model's <c>page.Size.ToMM</c>);
    /// <see cref="PageSize.UserDefined"/> has no size and gives (0, 0).
    /// </summary>
    public static (decimal Width, decimal Height) ToMm(this PageSize size, bool landscape)
    {
        var (w, h) = size switch
        {
            PageSize.A1 => (594m, 840m),
            PageSize.A2 => (420m, 594m),
            PageSize.A3 => (297m, 420m),
            PageSize.A4 => (210m, 297m),
            PageSize.A5 => (148m, 210m),
            PageSize.A6 => (105m, 148m),
            PageSize.Letter => (216m, 279m),
            PageSize.Legal => (216m, 356m),
            _ => (0m, 0m)
        };
        return landscape ? (h, w) : (w, h);
    }
}

// ── certificate ───────────────────────────────────────────────────────────────

/// <summary>certificate.PageSize — serialized as a string ("A4", "Letter", …).</summary>
[JsonConverter(typeof(CertificatePageSizeConverter))]
public enum CertificatePageSize
{
    A1 = 0, A2 = 1, A3 = 2, A4 = 3, A5 = 4,
    Letter = 5, Legal = 6, A6 = 7, UserDefined = 8
}

/// <summary>certificate.PageFormat — serialized as a string ("Portrait"/"Landscape").</summary>
[JsonConverter(typeof(CertificatePageFormatConverter))]
public enum CertificatePageFormat { Portrait = 0, Landscape = 1 }

/// <summary>
/// As go-model's <c>certificate.PageSize</c>: reads the name in any case, and any other text as
/// <see cref="CertificatePageSize.UserDefined"/>; writes "A1"…"A6", "Letter", "Legal" or "UserDefined".
/// </summary>
public sealed class CertificatePageSizeConverter : JsonConverter<CertificatePageSize>
{
    public override CertificatePageSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number) return (CertificatePageSize)reader.GetInt32();
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"A certificate page size is text, not {reader.TokenType}.");
        return reader.GetString()?.ToUpperInvariant() switch
        {
            "A1" => CertificatePageSize.A1,
            "A2" => CertificatePageSize.A2,
            "A3" => CertificatePageSize.A3,
            "A4" => CertificatePageSize.A4,
            "A5" => CertificatePageSize.A5,
            "A6" => CertificatePageSize.A6,
            "LEGAL" => CertificatePageSize.Legal,
            "LETTER" => CertificatePageSize.Letter,
            _ => CertificatePageSize.UserDefined
        };
    }

    public override void Write(Utf8JsonWriter writer, CertificatePageSize value, JsonSerializerOptions options)
        => writer.WriteStringValue(value switch
        {
            CertificatePageSize.A1 => "A1",
            CertificatePageSize.A2 => "A2",
            CertificatePageSize.A3 => "A3",
            CertificatePageSize.A4 => "A4",
            CertificatePageSize.A5 => "A5",
            CertificatePageSize.A6 => "A6",
            CertificatePageSize.Letter => "Letter",
            CertificatePageSize.Legal => "Legal",
            _ => "UserDefined"
        });
}

/// <summary>
/// As go-model's <c>certificate.PageFormat</c>: reads "portrait"/"landscape" in any case, and any other
/// text as <see cref="CertificatePageFormat.Portrait"/> (go-model leaves the zero value).
/// </summary>
public sealed class CertificatePageFormatConverter : JsonConverter<CertificatePageFormat>
{
    public override CertificatePageFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number) return (CertificatePageFormat)reader.GetInt32();
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"A certificate page format is text, not {reader.TokenType}.");
        return string.Equals(reader.GetString(), "landscape", StringComparison.OrdinalIgnoreCase)
            ? CertificatePageFormat.Landscape
            : CertificatePageFormat.Portrait;
    }

    public override void Write(Utf8JsonWriter writer, CertificatePageFormat value, JsonSerializerOptions options)
        => writer.WriteStringValue(value switch
        {
            CertificatePageFormat.Portrait => "Portrait",
            CertificatePageFormat.Landscape => "Landscape",
            _ => ""
        });
}

public static class CertificatePageSizeExtensions
{
    /// <summary>The portrait height in mm (go-model's <c>certificate.PageSize.Height</c>); 0 for <see cref="CertificatePageSize.UserDefined"/>.</summary>
    public static decimal Height(this CertificatePageSize size) => size switch
    {
        CertificatePageSize.A1 => 841m,
        CertificatePageSize.A2 => 594m,
        CertificatePageSize.A3 => 420m,
        CertificatePageSize.A4 => 297m,
        CertificatePageSize.A5 => 210m,
        CertificatePageSize.A6 => 148m,
        CertificatePageSize.Letter => 11 * 25.4m,
        CertificatePageSize.Legal => 14 * 25.4m,
        _ => 0m
    };

    /// <summary>The portrait width in mm (go-model's <c>certificate.PageSize.Width</c>); 0 for <see cref="CertificatePageSize.UserDefined"/>.</summary>
    public static decimal Width(this CertificatePageSize size) => size switch
    {
        CertificatePageSize.A1 => 594m,
        CertificatePageSize.A2 => 420m,
        CertificatePageSize.A3 => 297m,
        CertificatePageSize.A4 => 210m,
        CertificatePageSize.A5 => 148m,
        CertificatePageSize.A6 => 105m,
        CertificatePageSize.Letter => 8.5m * 25.4m,
        CertificatePageSize.Legal => 8.5m * 25.4m,
        _ => 0m
    };
}

public enum CertificateElementType
{
    Field = 0, Text = 1, Picture = 2, PictureName = 3, Barcode = 4,
    PictureExpression = 5, Rect = 6, Line = 7, Circle = 8, ChunkBox = 9,
    ChipBarcode = 11, BibPosition = 12
}

public enum CertificateBarcodeType { NoBarcode = 0, Code39 = 1, CodeEan13 = 2, Code128 = 3 }

public enum CertificatePictureStretchMode { Stretch = 0, NoStretch = 1 }

/// <summary><see cref="CertificateElement.Alignment"/> (go-model's <c>ALeft</c>/<c>ACenter</c>/<c>ARight</c>).</summary>
public enum CertificateAlignment { Left = 1, Center = 2, Right = 3 }

/// <summary><see cref="CertificateElement.VAlignment"/> (go-model's <c>ATop</c>/<c>AMiddle</c>/<c>ABottom</c>).</summary>
public enum CertificateVAlignment { Top = 0, Middle = 1, Bottom = 2 }

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
    [JsonPropertyName("RGBBlackToCMYK")]
    public bool RgbBlackToCmyk { get; init; }
    [JsonPropertyName("CMYKBlackValue")]
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

    /// <summary>The number of pages: the highest <see cref="CertificateElement.Page"/>, at least 1.</summary>
    public int PageCount() => Elements.Aggregate(1, (c, e) => Math.Max(c, e.Page));

    /// <summary>The width in mm, as printed (landscape swaps the page's sides).</summary>
    public decimal GetWidth() => PageFormat == CertificatePageFormat.Landscape
        ? (PageSize == CertificatePageSize.UserDefined ? PageHeight : PageSize.Height())
        : (PageSize == CertificatePageSize.UserDefined ? PageWidth : PageSize.Width());

    /// <summary>The height in mm, as printed (landscape swaps the page's sides).</summary>
    public decimal GetHeight() => PageFormat == CertificatePageFormat.Landscape
        ? (PageSize == CertificatePageSize.UserDefined ? PageWidth : PageSize.Width())
        : (PageSize == CertificatePageSize.UserDefined ? PageHeight : PageSize.Height());

    /// <summary>
    /// Where the holes are punched, (x, y) in mm from the top left, for <see cref="Holes"/>:
    /// 1 normal, 2 MTB, 3 duo (two chips), 4 single, 99 <see cref="SpecialHoles"/> ("x:y;x:y;…").
    /// Empty for any other value (go-model's <c>Certificate.GetHoles</c>).
    /// </summary>
    public (decimal X, decimal Y)[] GetHoles()
    {
        var dest = new List<(decimal X, decimal Y)>();
        void Add(decimal x, decimal y) => dest.Add((x, y));

        var w = GetWidth();
        var h = GetHeight();
        decimal cl = CutLeft, ct = CutTop, cb = CutBottom, cr = CutRight;

        switch (Holes)
        {
            case 1 or 2:
                Add(cl + 9, ct + 9);
                Add(w - cr - 9, ct + 9);
                break;
            case 3 or 4:
                // lower chip
                Add(cl + 30, h - cb - 9.5m);
                Add(cl + 30, h - cb - 22);
                Add(w - cr - 30, h - cb - 9.5m);
                Add(w - cr - 30, h - cb - 22);

                // upper chip
                if (Holes == 3) // duo
                {
                    Add(cl + 30, h - cb - 41.5m);
                    Add(cl + 30, h - cb - 54);
                    Add(w - cr - 30, h - cb - 41.5m);
                    Add(w - cr - 30, h - cb - 54);
                }
                break;
        }

        switch (Holes)
        {
            case 1: // normal
                Add(cl + 9, h - cb - 9);
                Add(w - cr - 9, h - cb - 9);
                break;

            case 2: // mtb
                // go-model's decimal has 4 places, so its halving truncates there
                var middle = cl + Math.Truncate((w - cl - cr) / 2 * 10000) / 10000;
                Add(middle - 10, h - cb - 9);
                Add(middle + 10, h - cb - 9);
                break;

            case 3: // duo: bib top holes, then bib bottom holes
                Add(cl + 9, ct + 83 + 9);
                Add(w - cr - 9, ct + 83 + 9);
                Add(cl + 9, ct + 233 - 9);
                Add(w - cr - 9, ct + 233 - 9);
                break;

            case 4: // single: bib top holes, then bib bottom holes
                Add(cl + 9, ct + 85 + 9);
                Add(w - cr - 9, ct + 85 + 9);
                Add(cl + 9, ct + 265 - 9);
                Add(w - cr - 9, ct + 265 - 9);
                break;

            case 99: // special
                foreach (var s in SpecialHoles.Split(';'))
                {
                    var c = s.Split(':');
                    if (c.Length < 2) continue;
                    var x = ParseMm(c[0]);
                    var y = ParseMm(c[1]);
                    if (x <= 0 || y <= 0) continue;
                    Add(cl + x, ct + y);
                }
                break;
        }
        return dest.ToArray();
    }

    // As go-model: an unreadable number is 0, a number keeps 4 decimal places.
    private static decimal ParseMm(string s)
        => double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float,
               System.Globalization.CultureInfo.InvariantCulture, out var f)
           && double.IsFinite(f) && Math.Abs(f) < 1e15
            ? Math.Round((decimal)f, 4, MidpointRounding.ToEven)
            : 0m;
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
    [JsonPropertyName("FontColorCMYK")]
    public string FontColorCmyk { get; init; } = "";
    public CertificateAlignment Alignment { get; init; }
    [JsonPropertyName("vAlignment")]
    public CertificateVAlignment VAlignment { get; init; }
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
    [JsonPropertyName("FilterRankID")]
    public int FilterRankId { get; init; }
    public string FilterRankOperator { get; init; } = "";
    public int FilterRankCompare { get; init; }
    [JsonPropertyName("FilterAYN")]
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
    public RaceResultDateTime LastChange { get; init; }
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

public record ListSelectorResult([property: JsonPropertyName("ResultID")] int ResultId, [property: JsonPropertyName("ResultID2")] int ResultId2, string ShowAs);

// ── kiosk ─────────────────────────────────────────────────────────────────────

public record Kiosk
{
    public string Name { get; init; } = "";
    public string Key { get; init; } = "";
    public bool Enabled { get; init; }
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
    public int TransponderMode { get; init; }
    public int AcceptedTransponders { get; init; }
    public bool IgnoreBibRanges { get; init; }
    public bool AutoFinish { get; init; }
    [JsonPropertyName("CSS")]
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
    [JsonPropertyName("CC")]
    public string Cc { get; init; } = "";
    [JsonPropertyName("BCC")]
    public string Bcc { get; init; } = "";
    public string ReceiverField { get; init; } = "";
    [JsonPropertyName("HTML")]
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
    [JsonPropertyName("HTTPHeaders")]
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
    [JsonPropertyName("PIDs")]
    public int[]? Pids { get; init; }
    public string? Sender { get; init; }
    public string? SenderName { get; init; }
    public string? ReplyTo { get; init; }
    [JsonPropertyName("CC")]
    public string? Cc { get; init; }
    [JsonPropertyName("BCC")]
    public string? Bcc { get; init; }
    public string? CellPhone { get; init; }
    public string? Email { get; init; }
    public string? Subject { get; init; }
    public string? Text { get; init; }
    [JsonPropertyName("HTML")]
    public bool Html { get; init; }
    [JsonPropertyName("URL")]
    public string? Url { get; init; }
    public string? Method { get; init; }
    public EmailPreviewAttachment[]? Attachments { get; init; }
    [JsonPropertyName("HTTPHeaders")]
    public HttpHeader[]? HttpHeaders { get; init; }
    public string[]? Errors { get; init; }
}

public record EmailPreviewAttachment(EmailAttachmentType Type, string Name, string Label, int Bib, [property: JsonPropertyName("PID")] int Pid);

// ── registration ──────────────────────────────────────────────────────────────

public record Registration
{
    public string Name { get; init; } = "";
    public string Key { get; init; } = "";
    public string ChangeKeySalt { get; init; } = "";
    public string Title { get; init; } = "";
    public bool Enabled { get; init; }
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
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
    [JsonPropertyName("CSS")]
    public string Css { get; init; } = "";
    public RegistrationErrorMessages ErrorMessages { get; init; } = new();
}

public record RegistrationComponent(string Name, RegistrationElement[] Elements);

public record RegistrationStep
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public bool Enabled { get; init; }
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
    public RegistrationElement[] Elements { get; init; } = [];
    public string ButtonText { get; init; } = "";
}

public record RegistrationElement
{
    public string Type { get; init; } = "";
    public string Label { get; init; } = "";
    public bool Enabled { get; init; }
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
    public RegistrationField? Field { get; init; }
    public string ShowIf { get; init; } = "";
    public int ShowIfMode { get; init; }
    public string ShowIfCurr { get; init; } = "";
    public int ShowIfCurrMode { get; init; }
    public bool ShowIfInitial { get; init; }
    public RegistrationStyle[]? Styles { get; init; }
    public string ClassName { get; init; } = "";
    [JsonPropertyName("ID")]
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
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
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
    [JsonPropertyName("ID")]
    public PaymentMethod Id { get; init; }
    public string Label { get; init; } = "";
    public bool Enabled { get; init; }
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
    public string Filter { get; init; } = "";
}

public record RegistrationValidationRule(string Rule, string Msg);

public record RegistrationErrorMessages
{
    public string BeforRegStart { get; init; } = "";
    public string AfterRegEnd { get; init; } = "";
    public string RegFull { get; init; } = "";
    public string QueueFull { get; init; } = "";
    public string ChangeNotAllowed { get; init; } = "";
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
