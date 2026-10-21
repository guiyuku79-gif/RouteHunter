# RouteHunterの構成

## 役割

- GameSessionController：初期化と各機能の連携
- GameStateManager：残り時間の管理と時間切れの通知
- DrawingManager：描画操作、手数、Undoの管理
- DrawingView：線・円の表示
- QuestionManager：出題と正解判定の呼び出し
- QuestionView：箱の表示と切り替え
- UIManager：手数、モード、残り時間の表示
- Core：計算、図形データ、問題生成、出題ルール

## 依存関係の方針

- 各機能の連携はGameSessionControllerが担当する。
- 描画とタイマーはイベントで状態を通知する。
- UIの更新はGameSessionControllerを経由する。
- CoreからManagerやViewを参照しない。

## 現在の仕様

- リセットでは描画を消し、手数と問題の進行状態を保持する。
- 新しいステージでは描画を消し、手数を0にする。
- Undoでは、最後に正解した描画より前には戻れない。
- ステージが変わっても残り時間は継続する。
- 時間切れでは未確定の描画を消し、操作を終了する。
- 確定済みの描画と手数は時間切れ後も残る。

## 今後の検討

- 結果画面と再プレイ
- エラー処理
- 出題と進行に関するテストの追加
- 固定コンパス選択中にリセットした場合の半径の扱い