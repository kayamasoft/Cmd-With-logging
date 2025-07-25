# Cmd with logging

## 概要

Cmd with logging は、Windows向けのカスタムコマンド実行ツールです。  
標準の cmd.exe に似たインターフェースを提供しながら、すべてのコマンド出力をログファイルに自動保存することができます。

## 特徴

- コマンド実行結果を自動でログファイルに記録
- バッチファイル実行履歴の管理と再実行機能
- su による管理者権限でのコマンド再実行
- ログファイルの切り替え・削除機能
- カスタムコマンドサポート（log help, log ver など）

## インストール方法

- [最新のexeをダウンロード](https://github.com/kayamasoft/Cmd-With-logging/releases/latest)
- またはソースコードからビルドしてください

## 使用方法

- 起動時にログファイルの保存先を選択
- 通常のコマンドを入力・実行するだけで出力がログに記録されます
- log help と入力すると、すべてのカスタムコマンドを確認できます

## カスタムコマンド一覧

| コマンド             | 内容                                                             |
|----------------------|------------------------------------------------------------------|
| log change           | ログファイルの保存先を変更                                      |
| log clear            | ログファイルを削除し、初期化                                     |
| log bat history      | 過去に実行したバッチファイルの一覧を表示（bat_tmpフォルダ）     |
| log bat run          | 過去のバッチファイルを再実行                                     |
| log help             | ヘルプを表示                                                    |
| log ver              | バージョン情報を表示                                            |
| log logo             | ASCIIアートのロゴを表示                                          |
| su [command]         | 管理者権限でコマンドを実行|

## バッチファイル連携

.bat ファイルを .cwl のような拡張子に変更し、このツールと関連付けて実行することで、  
バッチファイル内のコマンドもログ付きで記録できます。

## 既知の制限

- pause, title, diskpart などの対話型/特殊コマンドには未対応
- cd ../../.. のような複数の .. による移動は非サポート
- echo コマンドは ON 固定

## プライバシーポリシー

https://www.kayamasoft.org/privacy.html

## お問い合わせ先

- Webサイト: https://www.kayamasoft.org
- メール: hello@kayamasoft.org

© 2025 KayamaSoft. All rights reserved.
