using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceResultClient;

// Ports of go-model's website and presenter packages: the event site (my.raceresult.com) with its
// tabs, the participant view's elements, the site's registration overview, and the presenter screens.
// No go-webapi endpoint returns them; they come from the event's settings or RaceResult's other services.
// Same mapping as the other models (see Models.cs). Text fields whose values are a known set are
// string-backed value types (WebsiteTabType, WebsiteElementType, …): typed, with the known values as
// static members, and a value RaceResult adds later still round-trips.

// ── website ───────────────────────────────────────────────────────────────────

/// <summary>An event site (go-model's <c>website.Website</c>).</summary>
public record Website
{
    /// <summary>The name of the default site (go-model's <c>WebsiteDefault</c>).</summary>
    public const string Default = "default";

    public string Label { get; init; } = "";
    public bool Publish { get; init; }
    /// <summary>URL of the cover picture.</summary>
    public string CoverPicture { get; init; } = "";
    /// <summary>URL of the cover picture for mobile devices.</summary>
    public string CoverPictureMobile { get; init; } = "";
    public bool HideEventLogo { get; init; }
    /// <summary>Additional HTML or JavaScript included in the site.</summary>
    public string AdditionalCode { get; init; } = "";
    public WebsiteAdditionalFeature[]? AdditionalFeatures { get; init; }
    public string BrandColorDark { get; init; } = "";
    /// <summary></summary>
    public WebsiteApplyContestColor ApplyContestColor { get; init; }
    public string PortalTestKey { get; init; } = "";
    /// <summary>The minimum age (0–21) for comments on the participant view.</summary>
    public int CommentsMinimumAge { get; init; }
    public WebsiteCompany Organizer { get; init; } = new();
    /// <summary>The timekeeper of the event.</summary>
    public WebsiteCompany Timer { get; init; } = new();
    /// <summary>The payment processor of the event.</summary>
    public WebsiteCompany PayProc { get; init; } = new();
    public WebsiteTab[]? Tabs { get; init; }
    public WebsiteHomeTab? HomeTab { get; init; }
}

/// <summary><see cref="Website.ApplyContestColor"/>'s value (go-model's <c>ApplyContestColorYes</c>).</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteApplyContestColor>))]
public readonly struct WebsiteApplyContestColor(string value) : IStringValue<WebsiteApplyContestColor>, IEquatable<WebsiteApplyContestColor>
{
    public static readonly WebsiteApplyContestColor No = new("");
    public static readonly WebsiteApplyContestColor Yes = new("yes");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteApplyContestColor From(string value) => new(value);
    public static implicit operator WebsiteApplyContestColor(string value) => new(value);
    public bool Equals(WebsiteApplyContestColor other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteApplyContestColor o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteApplyContestColor a, WebsiteApplyContestColor b) => a.Equals(b);
    public static bool operator !=(WebsiteApplyContestColor a, WebsiteApplyContestColor b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary>The home tab of an event site (go-model's <c>website.HomeTab</c>).</summary>
public record WebsiteHomeTab
{
    /// <summary>Info tab: HTML (plain text is already HTML here).</summary>
    public string InfoText { get; init; } = "";
    public bool ShowEventGroups { get; init; }
    public string ExternalWebsiteLink { get; init; } = "";
    public bool ForumDeactivated { get; init; }
    public bool ComplaintsDeactivated { get; init; }
    public string ComplaintsEmail { get; init; } = "";
}

public record WebsiteAdditionalFeature
{
    public WebsiteAdditionalFeatureType Type { get; init; }
    [JsonPropertyName("ID")]
    public string Id { get; init; } = "";
    public bool Active { get; init; }
}

/// <summary><see cref="WebsiteAdditionalFeature.Type"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteAdditionalFeatureType>))]
public readonly struct WebsiteAdditionalFeatureType(string value) : IStringValue<WebsiteAdditionalFeatureType>, IEquatable<WebsiteAdditionalFeatureType>
{
    public static readonly WebsiteAdditionalFeatureType None = new("");
    public static readonly WebsiteAdditionalFeatureType GoogleAnalytics = new("GoogleAnalytics");
    public static readonly WebsiteAdditionalFeatureType MetaPixel = new("MetaPixel");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteAdditionalFeatureType From(string value) => new(value);
    public static implicit operator WebsiteAdditionalFeatureType(string value) => new(value);
    public bool Equals(WebsiteAdditionalFeatureType other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteAdditionalFeatureType o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteAdditionalFeatureType a, WebsiteAdditionalFeatureType b) => a.Equals(b);
    public static bool operator !=(WebsiteAdditionalFeatureType a, WebsiteAdditionalFeatureType b) => !a.Equals(b);
    public override string ToString() => Value;
}

public record WebsiteCompany
{
    public string Name { get; init; } = "";
    public string Mail { get; init; } = "";
    public string Web { get; init; } = "";
    public string Picture { get; init; } = "";
    public string DataProtection { get; init; } = "";
}

/// <summary>
/// A tab of an event site (go-model's <c>website.Tab</c>). <see cref="Config"/> holds the tab's
/// settings, whose shape follows <see cref="Type"/>: <see cref="WebsiteTabTextConfig"/>,
/// <see cref="WebsiteTabExternalContentConfig"/>, <see cref="WebsiteTabRegistrationConfig"/>,
/// <see cref="WebsiteTabListsConfig"/>, <see cref="WebsiteTabFavoritesConfig"/>,
/// <see cref="WebsiteTabParticipantViewConfig"/>, …
/// </summary>
public record WebsiteTab
{
    public string Label { get; init; } = "";
    [JsonPropertyName("URLName")]
    public string UrlName { get; init; } = "";
    public bool Enabled { get; init; }
    /// <summary>Whether the tab is in the menu when <see cref="ShowInMenu2"/> is <see cref="WebsiteShowInMenu.Default"/>.</summary>
    public bool ShowInMenu { get; init; }
    public WebsiteShowInMenu ShowInMenu2 { get; init; }
    public RaceResultDateTime ActiveFrom { get; init; }
    public RaceResultDateTime ActiveUntil { get; init; }
    public WebsiteTabType Type { get; init; }
    public JsonElement? Config { get; init; }

    /// <summary>
    /// False when <paramref name="t"/> is before <see cref="ActiveFrom"/> or after <see cref="ActiveUntil"/>
    /// (a bound that is not set does not limit). Compared as go-model does: see
    /// <see cref="RaceResultDateTime.CompareTo"/>. A <see cref="DateTime"/> converts implicitly.
    /// </summary>
    public bool IsWithinActiveTimeFrame(RaceResultDateTime t)
    {
        if (ActiveFrom.IsSet && t.Before(ActiveFrom))
            return false;
        if (ActiveUntil.IsSet && t.After(ActiveUntil))
            return false;
        return true;
    }

    /// <summary>Whether the tab is shown in the menu at <paramref name="t"/>.</summary>
    public bool IsVisibleInMenu(RaceResultDateTime t)
    {
        if (ShowInMenu2 == WebsiteShowInMenu.Always) return true;
        if (ShowInMenu2 == WebsiteShowInMenu.Never) return false;
        if (ShowInMenu2 == WebsiteShowInMenu.WhenActive) return IsWithinActiveTimeFrame(t);
        return ShowInMenu;
    }
}

/// <summary><see cref="WebsiteTab.Type"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteTabType>))]
public readonly struct WebsiteTabType(string value) : IStringValue<WebsiteTabType>, IEquatable<WebsiteTabType>
{
    public static readonly WebsiteTabType Text = new("text");
    public static readonly WebsiteTabType ExternalContent = new("externalcontent");
    public static readonly WebsiteTabType Registration = new("registration");
    public static readonly WebsiteTabType Lists = new("lists");
    public static readonly WebsiteTabType Favorites = new("favorites");
    public static readonly WebsiteTabType ParticipantView = new("participantview");
    /// <summary>The home tab built by my.raceresult.com.</summary>
    public static readonly WebsiteTabType Home = new("home");
    /// <summary>Legacy.</summary>
    public static readonly WebsiteTabType Contact = new("contact");
    /// <summary>Legacy.</summary>
    public static readonly WebsiteTabType Details = new("details");
    /// <summary>Legacy.</summary>
    public static readonly WebsiteTabType Reviews = new("reviews");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteTabType From(string value) => new(value);
    public static implicit operator WebsiteTabType(string value) => new(value);
    public bool Equals(WebsiteTabType other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteTabType o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteTabType a, WebsiteTabType b) => a.Equals(b);
    public static bool operator !=(WebsiteTabType a, WebsiteTabType b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary><see cref="WebsiteTab.ShowInMenu2"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteShowInMenu>))]
public readonly struct WebsiteShowInMenu(string value) : IStringValue<WebsiteShowInMenu>, IEquatable<WebsiteShowInMenu>
{
    /// <summary>Follow <see cref="WebsiteTab.ShowInMenu"/>.</summary>
    public static readonly WebsiteShowInMenu Default = new("");
    public static readonly WebsiteShowInMenu Always = new("always");
    public static readonly WebsiteShowInMenu Never = new("never");
    /// <summary>Only between <see cref="WebsiteTab.ActiveFrom"/> and <see cref="WebsiteTab.ActiveUntil"/>.</summary>
    public static readonly WebsiteShowInMenu WhenActive = new("whenactive");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteShowInMenu From(string value) => new(value);
    public static implicit operator WebsiteShowInMenu(string value) => new(value);
    public bool Equals(WebsiteShowInMenu other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteShowInMenu o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteShowInMenu a, WebsiteShowInMenu b) => a.Equals(b);
    public static bool operator !=(WebsiteShowInMenu a, WebsiteShowInMenu b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary>A text tab's <see cref="WebsiteTab.Config"/>.</summary>
public record WebsiteTabTextConfig
{
    /// <summary>HTML (plain text is already HTML here).</summary>
    public string InfoText { get; init; } = "";
    public bool ShowEventGroups { get; init; }
    public bool ShowEventNews { get; init; }
    public bool ShowExternalWebsiteLink { get; init; }
    public string ExternalWebsiteLink { get; init; } = "";
}

/// <summary>An external-content tab's <see cref="WebsiteTab.Config"/>.</summary>
public record WebsiteTabExternalContentConfig
{
    /// <summary>HTML (plain text is already HTML here).</summary>
    public string Data { get; init; } = "";
    public bool ShowEventGroups { get; init; }
    public bool ShowEventNews { get; init; }
    public bool ShowExternalWebsiteLink { get; init; }
    public string ExternalWebsiteLink { get; init; } = "";
    public WebsiteExternalContentSubType SubType { get; init; }
    public JsonElement? Config { get; init; }
}

/// <summary><see cref="WebsiteTabExternalContentConfig.SubType"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteExternalContentSubType>))]
public readonly struct WebsiteExternalContentSubType(string value) : IStringValue<WebsiteExternalContentSubType>, IEquatable<WebsiteExternalContentSubType>
{
    public static readonly WebsiteExternalContentSubType IFrame = new("");
    public static readonly WebsiteExternalContentSubType Youtube = new("youtube");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteExternalContentSubType From(string value) => new(value);
    public static implicit operator WebsiteExternalContentSubType(string value) => new(value);
    public bool Equals(WebsiteExternalContentSubType other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteExternalContentSubType o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteExternalContentSubType a, WebsiteExternalContentSubType b) => a.Equals(b);
    public static bool operator !=(WebsiteExternalContentSubType a, WebsiteExternalContentSubType b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary>A contact tab's <see cref="WebsiteTab.Config"/> (legacy).</summary>
public record WebsiteTabContactConfig
{
    public bool ComplaintsDeactivated { get; init; }
    public string ComplaintsEmail { get; init; } = "";
}

/// <summary>A registration tab's <see cref="WebsiteTab.Config"/>.</summary>
public record WebsiteTabRegistrationConfig
{
    /// <summary>HTML (plain text is already HTML here).</summary>
    public string InfoText { get; init; } = "";
    public RaceResultDateTime RegTestModeUntil { get; init; }
    public WebsiteRegistrationLogin RegistrationLogin { get; init; } = new();
    public WebsiteRegConfig[]? RegistrationForms { get; init; }
}

/// <summary>When a registration form is offered on the site (go-model's <c>IfLoggedIn</c>: 0, 1, 2).</summary>
public enum WebsiteRegIfLoggedIn
{
    Always = 0,
    IfLoggedIn = 1,
    IfNotLoggedIn = 2
}

/// <summary>A registration form on a registration tab (go-model's <c>website.RegConfig</c>).</summary>
public record WebsiteRegConfig
{
    public string Form { get; init; } = "";
    public string InfoText { get; init; } = "";
    public bool ShowContests { get; init; }
    public bool HideEntryFees { get; init; }
    public string ButtonText { get; init; } = "";
    public WebsiteRegIfLoggedIn IfLoggedIn { get; init; }
}

/// <summary>A lists tab's <see cref="WebsiteTab.Config"/>.</summary>
public record WebsiteTabListsConfig
{
    /// <summary>HTML (plain text is already HTML here).</summary>
    public string InfoText { get; init; } = "";
    public WebsiteListsSubtype Subtype { get; init; }
    public string StandardDetails { get; init; } = "";
    public WebsiteConfigList[]? Lists { get; init; }
    public bool LeaderboardHideCount { get; init; }
}

/// <summary><see cref="WebsiteTabListsConfig.Subtype"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteListsSubtype>))]
public readonly struct WebsiteListsSubtype(string value) : IStringValue<WebsiteListsSubtype>, IEquatable<WebsiteListsSubtype>
{
    public static readonly WebsiteListsSubtype Boxes = new("boxes");
    public static readonly WebsiteListsSubtype Dropdown = new("dropdown");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteListsSubtype From(string value) => new(value);
    public static implicit operator WebsiteListsSubtype(string value) => new(value);
    public bool Equals(WebsiteListsSubtype other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteListsSubtype o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteListsSubtype a, WebsiteListsSubtype b) => a.Equals(b);
    public static bool operator !=(WebsiteListsSubtype a, WebsiteListsSubtype b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary>A list on a lists tab (go-model's <c>website.ConfigList</c>).</summary>
public record WebsiteConfigList
{
    public string Name { get; init; } = "";
    public string Mode { get; init; } = "";
    public JsonElement? Contest { get; init; }
    public string ShowAs { get; init; } = "";
    public string Format { get; init; } = "";
    public JsonElement? Live { get; init; }
    public JsonElement? Sortable { get; init; }
    public JsonElement? Leader { get; init; }
    public string? Details { get; init; }
    [JsonPropertyName("ID")]
    public string? Id { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool HideProfilePictures { get; init; }
}

/// <summary>A details tab's <see cref="WebsiteTab.Config"/> (legacy).</summary>
public record WebsiteTabDetailsConfig
{
    public string Head { get; init; } = "";
    public string HeadText { get; init; } = "";
    public string SplitConfig { get; init; } = "";
    public string PortalPhotographer { get; init; } = "";
    [JsonPropertyName("PhotographerEventID")]
    public string PhotographerEventId { get; init; } = "";
    public string PortalPhotographerBib { get; init; } = "";
    public WebsitePublishedCertificateSet[]? CertificateSets { get; init; }
    public WebsiteConfigLink[]? Links { get; init; }
    public RaceResultDateTime CertificatesShowFrom { get; init; }
    public RaceResultDateTime CertificatesShowUntil { get; init; }
    public bool EnableComments { get; init; }
    public string List { get; init; } = "";
}

/// <summary>A participant-view tab's <see cref="WebsiteTab.Config"/>.</summary>
public record WebsiteTabParticipantViewConfig
{
    public bool LiveUpdate { get; init; }
    public WebsiteElement[]? Elements { get; init; }
}

public record WebsitePublishedCertificateSet
{
    public string Mode { get; init; } = "";
    public string Name { get; init; } = "";
    public string ShowAs { get; init; } = "";
    public string Picture { get; init; } = "";
    public JsonElement? Contest { get; init; }
}

public record WebsiteConfigLink
{
    public bool Disabled { get; init; }
    [JsonPropertyName("URL")]
    public string Url { get; init; } = "";
    public string Filter { get; init; } = "";
    public string Picture { get; init; } = "";
    public string Label { get; init; } = "";
}

/// <summary>A favorites tab's <see cref="WebsiteTab.Config"/>.</summary>
public record WebsiteTabFavoritesConfig
{
    /// <summary>"Linked Participantview": the <see cref="WebsiteTab.UrlName"/> of a participant-view tab.</summary>
    [JsonPropertyName("Participantview")]
    public string ParticipantView { get; init; } = "";
    public WebsiteSearchConfig Search { get; init; } = new();
    /// <summary>"Search Result Visualisation": the elements of each search result.</summary>
    public WebsiteElement[]? SearchResultElements { get; init; }
    /// <summary>"Favorite List Visualisation": the elements of each favorite.</summary>
    public WebsiteElement[]? FavoriteElements { get; init; }
    /// <summary>The favorite list's "Order by Field".</summary>
    public string ListSortedField { get; init; } = "";
    public bool ListSortAsc { get; init; }
    /// <summary>Written by RaceResult's site editor; not in go-model.</summary>
    public string ParticipantColorRule { get; init; } = "";
}

public record WebsiteSearchConfig
{
    public string Filter { get; init; } = "";
    public string[]? Fields { get; init; }
    /// <summary>The search results' "Order by Field".</summary>
    public string SortedField { get; init; } = "";
    public bool SortAsc { get; init; }
}

// ── website: registration overview ───────────────────────────────────────────

public record WebsiteRegistrationContestEntryFee
{
    public RaceResultDate DateStart { get; init; }
    public RaceResultDate DateEnd { get; init; }
    public DateTime RegStart { get; init; }
    public DateTime RegEnd { get; init; }
    public decimal Fee { get; init; }
    public string Currency { get; init; } = "";
}

public record WebsiteRegistrationContest
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public DateTime EnabledFrom { get; init; }
    public DateTime EnabledTo { get; init; }
    public DateTime Start { get; init; }
    public string Name { get; init; } = "";
    public string Sex { get; init; } = "";
    public RaceResultDate AgeStart { get; init; }
    public RaceResultDate AgeEnd { get; init; }
    public WebsiteRegistrationContestEntryFee[]? EntryFees { get; init; }
    public int SlotsLeft { get; init; }
}

/// <summary>A registration form as the event site shows it (go-model's <c>website.Registration</c>).</summary>
public record WebsiteRegistration
{
    public string Name { get; init; } = "";
    public string Title { get; init; } = "";
    public string Key { get; init; } = "";
    public string Type { get; init; } = "";
    public string TestModeKey { get; init; } = "";
    public DateTime EnabledFrom { get; init; }
    public DateTime EnabledTo { get; init; }
    public string InfoText { get; init; } = "";
    public bool ShowContests { get; init; }
    public bool HideEntryFees { get; init; }
    public string ButtonText { get; init; } = "";
    public int SlotsLeft { get; init; }
    public WebsiteRegistrationContest[]? Contests { get; init; }
    public WebsiteRegIfLoggedIn IfLoggedIn { get; init; }
}

public record WebsiteRegistrationConfig
{
    public WebsiteRegistration[]? Registrations { get; init; }
    public WebsiteRegistrationLogin Login { get; init; } = new();
}

public record WebsiteRegLoginField(string Name, string Label);

public record WebsiteRegistrationLogin
{
    public bool Enabled { get; init; }
    public string InfoText { get; init; } = "";
    public string LoginNameField { get; init; } = "";
    public WebsiteRegLoginField[]? Fields { get; init; }
}

public record WebsiteRegistrationLoginResponse
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public Dictionary<string, string>? ChangeKeys { get; init; }
    public string LoginName { get; init; } = "";
}

// ── website: participant view ─────────────────────────────────────────────────

/// <summary>
/// An element of the participant view (go-model's <c>website.Element</c>). <see cref="Config"/>
/// follows <see cref="Type"/>: <see cref="WebsiteElementBoxConfig"/>, <see cref="WebsiteElementTextConfig"/>,
/// <see cref="WebsiteElementSplitsConfig"/>, <see cref="WebsiteElementMapConfig"/>, …
/// </summary>
public record WebsiteElement
{
    [JsonPropertyName("ID")]
    public string Id { get; init; } = "";
    public WebsiteElementType Type { get; init; }
    public WebsiteElementActive Active { get; init; }
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
    public string ShowIf { get; init; } = "";
    public WebsiteStyle[]? Styles { get; init; }
    public string DynamicFormat { get; init; } = "";
    public WebsiteElement[]? Children { get; init; }
    public JsonElement? Config { get; init; }
}

/// <summary><see cref="WebsiteElement.Type"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteElementType>))]
public readonly struct WebsiteElementType(string value) : IStringValue<WebsiteElementType>, IEquatable<WebsiteElementType>
{
    public static readonly WebsiteElementType Text = new("text");
    public static readonly WebsiteElementType Html = new("html");
    public static readonly WebsiteElementType Picture = new("picture");
    /// <summary>"Fixed Columns" in the site editor.</summary>
    public static readonly WebsiteElementType Columns = new("columns");
    public static readonly WebsiteElementType Flexbox = new("flexbox");
    /// <summary>"Field/Expression" in the site editor.</summary>
    public static readonly WebsiteElementType Field = new("field");
    public static readonly WebsiteElementType Box = new("box");
    public static readonly WebsiteElementType Tabs = new("tabs");
    public static readonly WebsiteElementType Splits = new("splits");
    public static readonly WebsiteElementType Legs = new("legs");
    public static readonly WebsiteElementType Links = new("links");
    public static readonly WebsiteElementType List = new("list");
    public static readonly WebsiteElementType Photos = new("photos");
    public static readonly WebsiteElementType Certificates = new("certificates");
    public static readonly WebsiteElementType Chart = new("chart");
    /// <summary>"Compact Row" in the site editor.</summary>
    public static readonly WebsiteElementType InlineBlock = new("inlineblock");
    public static readonly WebsiteElementType Map = new("map");
    /// <summary>Legacy.</summary>
    public static readonly WebsiteElementType Favorite = new("favorite");
    /// <summary>Legacy.</summary>
    public static readonly WebsiteElementType Comments = new("comments");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteElementType From(string value) => new(value);
    public static implicit operator WebsiteElementType(string value) => new(value);
    public bool Equals(WebsiteElementType other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteElementType o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteElementType a, WebsiteElementType b) => a.Equals(b);
    public static bool operator !=(WebsiteElementType a, WebsiteElementType b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary><see cref="WebsiteElement.Active"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteElementActive>))]
public readonly struct WebsiteElementActive(string value) : IStringValue<WebsiteElementActive>, IEquatable<WebsiteElementActive>
{
    public static readonly WebsiteElementActive No = new("no");
    public static readonly WebsiteElementActive OnlyPopup = new("onlypopup");
    public static readonly WebsiteElementActive OnlyStandalone = new("onlystandalone");
    public static readonly WebsiteElementActive Yes = new("yes");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteElementActive From(string value) => new(value);
    public static implicit operator WebsiteElementActive(string value) => new(value);
    public bool Equals(WebsiteElementActive other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteElementActive o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteElementActive a, WebsiteElementActive b) => a.Equals(b);
    public static bool operator !=(WebsiteElementActive a, WebsiteElementActive b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary>A CSS attribute/value pair.</summary>
public record WebsiteStyle(string Attribute, string Value);

public record WebsiteElementBoxConfig
{
    public string Label { get; init; } = "";
    public string LabelPreset { get; init; } = "";
    public WebsiteStyle[]? TitleStyles { get; init; }
    public string TitleDynamicFormat { get; init; } = "";
}

/// <summary>
/// An element's horizontal alignment (the site editor's "Alignment" buttons), on text, picture,
/// field and compact-row elements.
/// </summary>
public enum WebsiteAlignment
{
    /// <summary>Not set (the element's config has no alignment).</summary>
    None = 0,
    Left = 1,
    Center = 2,
    Right = 3
}

public record WebsiteElementTextConfig
{
    public string Text { get; init; } = "";
    public WebsiteAlignment Alignment { get; init; }
}

public record WebsiteElementHtmlConfig
{
    public string Text { get; init; } = "";
}

public record WebsiteElementPictureConfig
{
    public string Src { get; init; } = "";
    public WebsiteAlignment Alignment { get; init; }
}

public record WebsiteElementFieldConfig
{
    public string Field { get; init; } = "";
    public WebsiteFieldDisplayMode DisplayMode { get; init; }
    public string Title { get; init; } = "";
    /// <summary>For <see cref="WebsiteFieldDisplayMode.TitleAbove"/>.</summary>
    public WebsiteStyle[]? TitleStyles { get; init; }
    public string TitleDynamicFormat { get; init; } = "";
    public WebsiteAlignment Alignment { get; init; }
}

/// <summary><see cref="WebsiteElementFieldConfig.DisplayMode"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteFieldDisplayMode>))]
public readonly struct WebsiteFieldDisplayMode(string value) : IStringValue<WebsiteFieldDisplayMode>, IEquatable<WebsiteFieldDisplayMode>
{
    public static readonly WebsiteFieldDisplayMode FieldOnly = new("");
    public static readonly WebsiteFieldDisplayMode Badge = new("badge");
    public static readonly WebsiteFieldDisplayMode TitleAbove = new("titleabove");
    public static readonly WebsiteFieldDisplayMode TitleBelow = new("titlebelow");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteFieldDisplayMode From(string value) => new(value);
    public static implicit operator WebsiteFieldDisplayMode(string value) => new(value);
    public bool Equals(WebsiteFieldDisplayMode other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteFieldDisplayMode o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteFieldDisplayMode a, WebsiteFieldDisplayMode b) => a.Equals(b);
    public static bool operator !=(WebsiteFieldDisplayMode a, WebsiteFieldDisplayMode b) => !a.Equals(b);
    public override string ToString() => Value;
}

public record WebsiteElementSplitsConfig
{
    public bool EmbedInBox { get; init; }
    [JsonPropertyName("ShowTOD")]
    public bool ShowTimeOfDay { get; init; }
    public bool ShowGunTime { get; init; }
    public bool ShowChipTime { get; init; }
    public bool ShowSectorTime { get; init; }
    public bool ShowPace { get; init; }
    public bool ShowOverallRank { get; init; }
    public bool ShowGenderRank { get; init; }
    public bool ShowAgeGroupRank { get; init; }
    public bool ShowMaxRank { get; init; }
    public bool HidePrediction { get; init; }
}

public record WebsiteElementLegsConfig
{
    public bool EmbedInBox { get; init; }
    public bool ShowOverallRank { get; init; }
    public bool ShowGenderRank { get; init; }
    public bool ShowAgeGroupRank { get; init; }
    public bool ShowMaxRank { get; init; }
    public bool ShowPace { get; init; }
    public bool HidePrediction { get; init; }
}

public record WebsiteElementLinksConfig
{
    public bool EmbedInBox { get; init; }
    public WebsiteConfigLink[]? Links { get; init; }
}

public record WebsiteElementCertificatesConfig
{
    public bool EmbedInBox { get; init; }
    public WebsitePublishedCertificateSet[]? CertificateSets { get; init; }
}

public record WebsiteElementPhotosConfig
{
    public bool EmbedInBox { get; init; }
    public string PortalPhotographer { get; init; } = "";
    [JsonPropertyName("PhotographerEventID")]
    public string PhotographerEventId { get; init; } = "";
    public string PortalPhotographerBib { get; init; } = "";
}

public record WebsiteElementListConfig
{
    public bool EmbedInBox { get; init; }
    public string List { get; init; } = "";
}

public record WebsiteElementInlineBlockConfig
{
    public WebsiteAlignment Alignment { get; init; }
}

public record WebsiteElementChartConfig
{
    public bool EmbedInBox { get; init; }
    public WebsiteChartType ChartType { get; init; }
    /// <summary>Left out when <see cref="WebsiteRankScope.Overall"/> (empty), as in go-model.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public WebsiteRankScope RankScope { get; init; }
}

/// <summary><see cref="WebsiteElementChartConfig.ChartType"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteChartType>))]
public readonly struct WebsiteChartType(string value) : IStringValue<WebsiteChartType>, IEquatable<WebsiteChartType>
{
    public static readonly WebsiteChartType SplitPace = new("splitpace");
    public static readonly WebsiteChartType SplitRank = new("splitrank");
    public static readonly WebsiteChartType Elevation = new("elevation");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteChartType From(string value) => new(value);
    public static implicit operator WebsiteChartType(string value) => new(value);
    public bool Equals(WebsiteChartType other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteChartType o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteChartType a, WebsiteChartType b) => a.Equals(b);
    public static bool operator !=(WebsiteChartType a, WebsiteChartType b) => !a.Equals(b);
    public override string ToString() => Value;
}

/// <summary><see cref="WebsiteElementChartConfig.RankScope"/>'s values.</summary>
[JsonConverter(typeof(StringValueConverter<WebsiteRankScope>))]
public readonly struct WebsiteRankScope(string value) : IStringValue<WebsiteRankScope>, IEquatable<WebsiteRankScope>
{
    public static readonly WebsiteRankScope Overall = new("");
    public static readonly WebsiteRankScope Gender = new("gender");
    public static readonly WebsiteRankScope AgeGroup = new("agegroup");

    private readonly string? _value = value;
    public string Value => _value ?? "";
    public static WebsiteRankScope From(string value) => new(value);
    public static implicit operator WebsiteRankScope(string value) => new(value);
    public bool Equals(WebsiteRankScope other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is WebsiteRankScope o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(WebsiteRankScope a, WebsiteRankScope b) => a.Equals(b);
    public static bool operator !=(WebsiteRankScope a, WebsiteRankScope b) => !a.Equals(b);
    public override string ToString() => Value;
}

public record WebsiteElementMapConfig
{
    [JsonPropertyName("MapShowFavorites")]
    public bool ShowFavorites { get; init; }
    [JsonPropertyName("MapMarkerLabels")]
    public string MarkerLabels { get; init; } = "";
    [JsonPropertyName("MapMarkerTooltips")]
    public string MarkerTooltips { get; init; } = "";
    [JsonPropertyName("MapMarkerFilter")]
    public string MarkerFilter { get; init; } = "";
    [JsonPropertyName("MapMarkerWait")]
    public bool MarkerWait { get; init; }
    [JsonPropertyName("MapMarkerHideIcon")]
    public bool MarkerHideIcon { get; init; }
    public bool EmbedInBox { get; init; }
}

/// <summary>The tracking map's data, by contest ID (go-model's <c>website.TrackingData</c>).</summary>
public sealed class WebsiteTrackingData : Dictionary<int, WebsiteTrackingContest>;

public record WebsiteTrackingContest
{
    public string? Color { get; init; }
    public WebsiteTrackingContestSplit[]? Splits { get; init; }
    public WebsiteTrackingParticipants Participants { get; init; } = new();
}

public record WebsiteTrackingContestSplit
{
    public string Name { get; init; } = "";
    /// <summary>Always in metres.</summary>
    public int Distance { get; init; }
    public string? Position { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Internal { get; init; }
    /// <summary>Not serialized.</summary>
    [JsonIgnore]
    public int Id { get; init; }
    /// <summary>Not serialized.</summary>
    [JsonIgnore]
    public int OrderPos { get; init; }
}

public record WebsiteTrackingParticipants
{
    public string[]? Columns { get; init; }
    /// <summary>One row of <see cref="Columns"/> per participant, by PID.</summary>
    public Dictionary<int, JsonElement[]>? Data { get; init; }
}

// ── presenter ─────────────────────────────────────────────────────────────────

/// <summary>A presenter: the screens shown on a display at the venue (go-model's <c>presenter.Presenter</c>).</summary>
public record Presenter
{
    public string Name { get; init; } = "";
    public string Key { get; init; } = "";
    public string Title { get; init; } = "";
    public bool Enabled { get; init; }
    public RaceResultDateTime EnabledFrom { get; init; }
    public RaceResultDateTime EnabledTo { get; init; }
    public string SwitchMode { get; init; } = "";
    public bool AutoHideTabs { get; init; }
    public PresenterScreen[]? Screens { get; init; }
    [JsonPropertyName("CSS")]
    public string Css { get; init; } = "";
    public PresenterDisplayDimensions DisplayDimensions { get; init; } = new(0, 0);
}

public record PresenterScreen
{
    public bool Disabled { get; init; }
    public string Title { get; init; } = "";
    public string BackgroundColor { get; init; } = "";
    public string BackgroundImage { get; init; } = "";
    [JsonPropertyName("CSS")]
    public string Css { get; init; } = "";
    public PresenterWindow[]? Windows { get; init; }
    public string LayoutMode { get; init; } = "";
}

/// <summary><see cref="PresenterWindow.ScrollBar"/>, as RaceResult's presenter settings list it.</summary>
public enum PresenterScrollBar { Normal = 0, Hide = 1, Thin = 2 }

public record PresenterWindow
{
    public string ItemType { get; init; } = "";
    public string ItemName { get; init; } = "";
    public int Left { get; init; }
    public int Top { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int[]? Contests { get; init; }
    public int[]? Results { get; init; }
    public string[]? TimingPoints { get; init; }
    public string[]? Splits { get; init; }
    public string ScrollMode { get; init; } = "";
    public int PageTime { get; init; }
    public int FontSize { get; init; }
    public PresenterScrollBar ScrollBar { get; init; }
    public string Filter { get; init; } = "";
    public bool ShowFilter { get; init; }
    public bool IgnoreManualPassings { get; init; }
    public PresenterStyle[]? Styles { get; init; }
    public int Rotation { get; init; }
    public Dictionary<string, JsonElement>? Details { get; init; }
}

/// <summary>A CSS attribute/value pair.</summary>
public record PresenterStyle(string Attribute, string Value);

public record PresenterDisplayDimensions(int Width, int Height);
