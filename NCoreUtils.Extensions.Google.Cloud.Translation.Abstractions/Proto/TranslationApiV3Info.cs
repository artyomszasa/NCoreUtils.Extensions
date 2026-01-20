using NCoreUtils.Proto;

namespace NCoreUtils.Google.Proto;

[ProtoInfo(typeof(ITranslationApiV3), Path = "v3")]
[ProtoMethodInfo(nameof(ITranslationApiV3.DetectLanguageAsync), Path = "projects", Input = InputType.Custom)]
[ProtoMethodInfo(nameof(ITranslationApiV3.GetSupportedLanguagesAsync), Path = "projects", Input = InputType.Custom)]
[ProtoMethodInfo(nameof(ITranslationApiV3.TranslateTextAsync), Path = "projects", Input = InputType.Custom)]
public partial class TranslationApiV3Info { }