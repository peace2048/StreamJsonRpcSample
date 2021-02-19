# StreamJsonRpcSample

VisualStudio Code 等でも使用されている [StreamJsonRpc](https://github.com/microsoft/vs-streamjsonrpc) を使用した RPC のサンプルです。

サンプルとして、1対1 の接続だけ対応しています。
ネットワーク層として NamedPipe を使用しています。

## サーバー

まず、インターフェースを定義します。サンプルでは ISampleService にあたります。
次に、ISampleService を実装したクラスを作成します。
最後に、NamedPipeSingleRpcServer<ISampleService> をインスタンス化します。

## クライアント

NamedPipeRpcClientBase を継承し、ISampleService を実装したクラス (SampleClient) を作成します。

## サンプルの実行方法

`StreamJsonRpcSample server` でサーバーとして起動します。終了は、Ctrl+C です。
`StreamJsonRpcSample server` でクライアントとして起動します。
