using NCoreUtils.Proto;

namespace NCoreUtils.Google.Wallet.Proto;

[ProtoInfo(typeof(IWalletV1Api), Path = "walletobjects/v1")]
[ProtoMethodInfo(nameof(IWalletV1Api.InsertGenericClassAsync),
    Path = "genericClass",
    Input = InputType.Custom,
    HttpMethod = NCoreUtils.Proto.HttpMethod.Post)]
[ProtoMethodInfo(nameof(IWalletV1Api.ListGenericClassesAsync),
    Path = "genericClass",
    Input = InputType.Custom,
    HttpMethod = NCoreUtils.Proto.HttpMethod.Get)]
[ProtoMethodInfo(nameof(IWalletV1Api.LookupGenericClass),
    Path = "genericClass",
    Input = InputType.Custom,
    Output = OutputType.Custom,
    HttpMethod = NCoreUtils.Proto.HttpMethod.Get)]
[ProtoMethodInfo(nameof(IWalletV1Api.InsertGenericObjectAsync),
    Path = "genericObject",
    Input = InputType.Custom,
    HttpMethod = NCoreUtils.Proto.HttpMethod.Post)]
public partial class WalletV1ApiInfo { }