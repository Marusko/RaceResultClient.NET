using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceResultClient;

// Ports of go-model's website and presenter packages: the event site (my.raceresult.com) with its
// tabs, the participant view's elements, the site's registration overview, and the presenter screens.
// No go-webapi endpoint returns them; they come from the event's settings or RaceResult's other services.
// Same mapping as the other models: datetime.DateTime → string, date.Date → string,
// time.Time → DateTime, json.RawMessage / interface{} → JsonElement?, variant.VariantMap →
// Dictionary<string, JsonElement>. Text fields whose values are a known set keep named string
// constants (WebsiteTabTypes, WebsiteElementTypes, …), since a value outside the set is still valid text.

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
    /// <summary><see cref="WebsiteApplyContestColor.Yes"/> or empty.</summary>
    public string ApplyContestColor { get; init; } = "";
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
public static class WebsiteApplyContestColor
{
    public const string Yes = "yes";
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
    /// <summary>One of <see cref="WebsiteAdditionalFeatureTypes"/>.</summary>
    public string Type { get; init; } = "";
    [JsonPropertyName("ID")]
    public string Id { get; init; } = "";
    public bool Active { get; init; }
}

/// <summary><see cref="WebsiteAdditionalFeature.Type"/>'s values.</summary>
public static class WebsiteAdditionalFeatureTypes
{
    public const string None = "";
    public const string GoogleAnalytics = "GoogleAnalytics";
    public const string MetaPixel = "MetaPixel";
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
    /// <summary>One of <see cref="WebsiteShowInMenu"/>.</summary>
    public string ShowInMenu2 { get; init; } = "";
    public string ActiveFrom { get; init; } = "";
    public string ActiveUntil { get; init; } = "";
    /// <summary>One of <see cref="WebsiteTabTypes"/>.</summary>
    public string Type { get; init; } = "";
    public JsonElement? Config { get; init; }

    /// <summary>
    /// False when <paramref name="t"/> is before <see cref="ActiveFrom"/> or after <see cref="ActiveUntil"/>
    /// (an empty bound does not limit). As in go-model, a bound without a zone is read in
    /// <paramref name="t"/>'s zone, and a <paramref name="t"/> of <see cref="DateTimeKind.Unspecified"/>
    /// in the bound's.
    /// </summary>
    public bool IsWithinActiveTimeFrame(DateTime t)
    {
        if (GoDateTime.TryParse(ActiveFrom, out var from) && GoDateTime.Compare(t, from) < 0)
            return false;
        if (GoDateTime.TryParse(ActiveUntil, out var until) && GoDateTime.Compare(t, until) > 0)
            return false;
        return true;
    }

    /// <summary>Whether the tab is shown in the menu at <paramref name="t"/>.</summary>
    public bool IsVisibleInMenu(DateTime t) => ShowInMenu2 switch
    {
        WebsiteShowInMenu.Always => true,
        WebsiteShowInMenu.Never => false,
        WebsiteShowInMenu.WhenActive => IsWithinActiveTimeFrame(t),
        _ => ShowInMenu
    };
}

/// <summary><see cref="WebsiteTab.Type"/>'s values.</summary>
public static class WebsiteTabTypes
{
    public const string Text = "text";
    public const string ExternalContent = "externalcontent";
    public const string Registration = "registration";
    public const string Lists = "lists";
    public const string Favorites = "favorites";
    public const string ParticipantView = "participantview";
    /// <summary>The home tab built by my.raceresult.com.</summary>
    public const string Home = "home";
    /// <summary>Legacy.</summary>
    public const string Contact = "contact";
    /// <summary>Legacy.</summary>
    public const string Details = "details";
    /// <summary>Legacy.</summary>
    public const string Reviews = "reviews";
}

/// <summary><see cref="WebsiteTab.ShowInMenu2"/>'s values.</summary>
public static class WebsiteShowInMenu
{
    /// <summary>Follow <see cref="WebsiteTab.ShowInMenu"/>.</summary>
    public const string Default = "";
    public const string Always = "always";
    public const string Never = "never";
    /// <summary>Only between <see cref="WebsiteTab.ActiveFrom"/> and <see cref="WebsiteTab.ActiveUntil"/>.</summary>
    public const string WhenActive = "whenactive";
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
    /// <summary>One of <see cref="WebsiteExternalContentSubTypes"/>.</summary>
    public string SubType { get; init; } = "";
    public JsonElement? Config { get; init; }
}

/// <summary><see cref="WebsiteTabExternalContentConfig.SubType"/>'s values.</summary>
public static class WebsiteExternalContentSubTypes
{
    public const string IFrame = "";
    public const string Youtube = "youtube";
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
    public string RegTestModeUntil { get; init; } = "";
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
    /// <summary>One of <see cref="WebsiteListsSubtypes"/>.</summary>
    public string Subtype { get; init; } = "";
    public string StandardDetails { get; init; } = "";
    public WebsiteConfigList[]? Lists { get; init; }
    public bool LeaderboardHideCount { get; init; }
}

/// <summary><see cref="WebsiteTabListsConfig.Subtype"/>'s values.</summary>
public static class WebsiteListsSubtypes
{
    public const string Boxes = "boxes";
    public const string Dropdown = "dropdown";
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
    public string CertificatesShowFrom { get; init; } = "";
    public string CertificatesShowUntil { get; init; } = "";
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
    [JsonPropertyName("Participantview")]
    public string ParticipantView { get; init; } = "";
    public WebsiteSearchConfig Search { get; init; } = new();
    public WebsiteElement[]? SearchResultElements { get; init; }
    public WebsiteElement[]? FavoriteElements { get; init; }
    public string ListSortedField { get; init; } = "";
    public bool ListSortAsc { get; init; }
}

public record WebsiteSearchConfig
{
    public string Filter { get; init; } = "";
    public string[]? Fields { get; init; }
    public string SortedField { get; init; } = "";
    public bool SortAsc { get; init; }
}

// ── website: registration overview ───────────────────────────────────────────

public record WebsiteRegistrationContestEntryFee
{
    public string DateStart { get; init; } = "";
    public string DateEnd { get; init; } = "";
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
    public string AgeStart { get; init; } = "";
    public string AgeEnd { get; init; } = "";
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
    /// <summary>One of <see cref="WebsiteElementTypes"/>.</summary>
    public string Type { get; init; } = "";
    /// <summary>One of <see cref="WebsiteElementActive"/>.</summary>
    public string Active { get; init; } = "";
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
    public string ShowIf { get; init; } = "";
    public WebsiteStyle[]? Styles { get; init; }
    public string DynamicFormat { get; init; } = "";
    public WebsiteElement[]? Children { get; init; }
    public JsonElement? Config { get; init; }
}

/// <summary><see cref="WebsiteElement.Type"/>'s values.</summary>
public static class WebsiteElementTypes
{
    public const string Text = "text";
    public const string Html = "html";
    public const string Picture = "picture";
    public const string Columns = "columns";
    public const string Flexbox = "flexbox";
    public const string Field = "field";
    public const string Box = "box";
    public const string Tabs = "tabs";
    public const string Splits = "splits";
    public const string Legs = "legs";
    public const string Links = "links";
    public const string List = "list";
    public const string Photos = "photos";
    public const string Certificates = "certificates";
    public const string Chart = "chart";
    public const string InlineBlock = "inlineblock";
    public const string Map = "map";
    /// <summary>Legacy.</summary>
    public const string Favorite = "favorite";
    /// <summary>Legacy.</summary>
    public const string Comments = "comments";
}

/// <summary><see cref="WebsiteElement.Active"/>'s values.</summary>
public static class WebsiteElementActive
{
    public const string No = "no";
    public const string OnlyPopup = "onlypopup";
    public const string OnlyStandalone = "onlystandalone";
    public const string Yes = "yes";
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

public record WebsiteElementTextConfig
{
    public string Text { get; init; } = "";
    public int Alignment { get; init; }
}

public record WebsiteElementHtmlConfig
{
    public string Text { get; init; } = "";
}

public record WebsiteElementPictureConfig
{
    public string Src { get; init; } = "";
    public int Alignment { get; init; }
}

public record WebsiteElementFieldConfig
{
    public string Field { get; init; } = "";
    /// <summary>One of <see cref="WebsiteFieldDisplayModes"/>.</summary>
    public string DisplayMode { get; init; } = "";
    public string Title { get; init; } = "";
    /// <summary>For <see cref="WebsiteFieldDisplayModes.TitleAbove"/>.</summary>
    public WebsiteStyle[]? TitleStyles { get; init; }
    public string TitleDynamicFormat { get; init; } = "";
    public int Alignment { get; init; }
}

/// <summary><see cref="WebsiteElementFieldConfig.DisplayMode"/>'s values.</summary>
public static class WebsiteFieldDisplayModes
{
    public const string FieldOnly = "";
    public const string Badge = "badge";
    public const string TitleAbove = "titleabove";
    public const string TitleBelow = "titlebelow";
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
    public int Alignment { get; init; }
}

public record WebsiteElementChartConfig
{
    public bool EmbedInBox { get; init; }
    /// <summary>One of <see cref="WebsiteChartTypes"/>.</summary>
    public string ChartType { get; init; } = "";
    /// <summary>One of <see cref="WebsiteRankScopes"/>; left out when null.</summary>
    public string? RankScope { get; init; }
}

/// <summary><see cref="WebsiteElementChartConfig.ChartType"/>'s values.</summary>
public static class WebsiteChartTypes
{
    public const string SplitPace = "splitpace";
    public const string SplitRank = "splitrank";
    public const string Elevation = "elevation";
}

/// <summary><see cref="WebsiteElementChartConfig.RankScope"/>'s values.</summary>
public static class WebsiteRankScopes
{
    public const string Overall = "";
    public const string Gender = "gender";
    public const string AgeGroup = "agegroup";
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
    public string EnabledFrom { get; init; } = "";
    public string EnabledTo { get; init; } = "";
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
    public int ScrollBar { get; init; }
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

// ── go-model's datetime.DateTime, read from its text ─────────────────────────

internal static class GoDateTime
{
    /// <summary>A datetime.DateTime: a wall clock without a zone, or an instant with one.</summary>
    public readonly record struct Value(DateTime WallClock, DateTimeOffset Instant, bool HasZone);

    /// <summary>
    /// Reads "2006-01-02", "2006-01-02 15:04:05" (no zone) or RFC 3339 (with zone). False for an
    /// empty value, go-model's zero dates (0001-01-01, 1899-12-30) and text go-model would not read.
    /// </summary>
    public static bool TryParse(string? s, out Value value)
    {
        value = default;
        if (string.IsNullOrWhiteSpace(s)) return false;

        if (s.Length is 10 or 19)
        {
            if (!DateTime.TryParseExact(s, s.Length == 10 ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var wallClock))
                return false;
            value = new Value(wallClock, default, HasZone: false);
        }
        else
        {
            if (!DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var instant))
                return false;
            value = new Value(instant.DateTime, instant, HasZone: true);
        }
        return value.WallClock != DateTime.MinValue && value.WallClock != new DateTime(1899, 12, 30);
    }

    /// <summary>
    /// Compares <paramref name="t"/> with <paramref name="v"/> as go-model's DateTime.Before/After do:
    /// as instants when both have a zone, else as wall clocks in the zone of the one that has it.
    /// A <paramref name="t"/> of <see cref="DateTimeKind.Unspecified"/> has no zone.
    /// </summary>
    public static int Compare(DateTime t, Value v)
    {
        if (v.HasZone && t.Kind != DateTimeKind.Unspecified)
            return t.ToUniversalTime().CompareTo(v.Instant.UtcDateTime);
        return DateTime.SpecifyKind(t, DateTimeKind.Unspecified)
            .CompareTo(DateTime.SpecifyKind(v.WallClock, DateTimeKind.Unspecified));
    }
}
