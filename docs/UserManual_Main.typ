#import "common.typ": *
#show: style

#set enum(numbering: "①")

= はじめに
本書は、ICRP2007年勧告に基づく内部被ばく線量評価コードFlexID（Flexible code for Internal Dosimetry）のユーザーマニュアルである。FlexIDは、ICRP2007年勧告に基づく体内動態モデルを臓器・組織ごとに組合せ、放射性核種の人間の体内での移行を計算し、子孫核種も含めた体内動態、及び線量評価を行う。入力データの変更だけで体内動態モデルの組合せや移行係数等を変更でき、ICRPの体内動態モデルの変更に迅速かつ容易に対応可能な汎用コードである。


= 動作環境
プログラムの動作環境を以下に示す。
- Windows 11以降


= 導入方法
FlexIDフォルダを任意の場所にコピーすることでFlexIDプログラムを導入できる。以下にFlexIDフォルダの構成を示す。

```
FlexID
  ┣ FlexID.exe（残留放射能・線量計算プログラム：CLI用）
  ┣ FlexUI.exe（残留放射能・線量計算プログラム：GUI用）
  ┣ inp
  ┃  ┣ OIR
  ┃  ┃  ┗ 元素ごとの職業人（OIR）に対する作成済みインプットファイル
  ┃  ┗ EIR
  ┃     ┗ 元素ごとの公衆の構成員（EIR）に対する作成済みインプットファイル
  ┣ expect 
  ┃  ┗ OIRに対する残留放射能と預託線量の期待値データ
  ┣ lib
  ┃  ┣ OIR
  ┃  ┃  ┗ 元素ごとのOIRに対する組織加重係数データ
  ┃  ┣ EIR
  ┃  ┃  ┗ 元素ごとのEIRに対する組織加重係数データやSEEファイル
  ┃  ┗ TimeMesh
  ┃     ┗ 計算時間と出力時間を定義するタイムメッシュファイル
  ┗ license
        ┗ 添付データの再配布に関する許諾など
```


= 主な機能
FlexID の主な機能を以下に示す。
- 臓器／組織ごとの残留・積算放射能量の計算
  （#link("UserManual_Appendix-1.typ")[添付資料1] 参照）

- 臓器／組織ごとの等価線量、積算線量、及び線量率の計算
  （#link("UserManual_Appendix-2.md")[添付資料2] 参照）

- 預託実効線量、積算線量、及び線量率の計算
  （#link("UserManual_Appendix-2.md")[添付資料2] 参照）

- 核種ごとのS-Coefficientの計算
  （#link("UserManual_Appendix-3.md")[添付資料3] 参照）


= 計算実行画面

本画面は、`FlexUI.exe`をダブルクリックすることによって開くことができる。

#figure(
  image("images/Figure_1.png", width: 80%),
  caption: "入力画面",
)


+ 被ばく対象のタブを選択する。
   - OIR：職業人の内部被ばく（Occupational Intakes of Radionuclides）
   - EIR：公衆の構成員の内部被ばく（Environmental Intakes of Radionuclides）


== OIR計算実行画面 <oir-calc>

#figure(
    markrect(
      image("images/Figure_1.png"), factor: 75%,
      (x:24.2%, y:69.70%, w: 29.6%, h: 4.3%),
      (x:57.5%, y:69.70%, w: 24.0%, h: 4.3%),
      (x:24.2%, y:75.10%, w: 73.0%, h: 4.3%),
      (x:24.2%, y:80.55%, w: 73.0%, h: 4.3%),
      (x:24.2%, y:86.00%, w: 73.0%, h: 4.3%),
    )
  ,
  caption: "OIR計算実行画面",
)

新しい核種を追加するためのインプットファイル作成方法は、#link("UserManual_Appendix-4.md")[添付資料4]を参照。

+ 預託期間を`Commitment Period`欄に整数で入力し、ドロップダウンから預託期間の単位を選択する <a>

+ 計算結果をOIR Data Viewerの値と比較する場合は、`Compare with OIR data`にチェックを入れる。

+ 計算タイムメッシュファイルと出力タイムメッシュファイルを、それぞれ#linebreak()
  `Computational Time Mesh`欄と`Output Time Mesh`欄で選択する。

    タイムメッシュファイルの作成方法については、#link("UserManual_Appendix-4.md")[添付資料4]を参照。
  #v(0.5em)

+ 出力フォルダのパスを`Output Directory`欄で指定する。
  #linebreak()
  #linebreak()

+ 計算対象とするインプットを一覧からチェックボックスで選択する。

  #figure(
    markrect(
      image("images/OIR/Select_Sr-90.png"),
      (x:4.0%, y:36%, w: 5%, h: 64%)
    ),
    caption: "チェックボックスによる計算対象インプットの選択",
  )

  - 上部左側のドロップダウン`Elements ...`では、元素による一覧のフィルタリングが可能。

    #figure(
      markrect(
        image("images/OIR/Elements.png"),
        (x: 2.5%, y: 15.2%, w: 15.4%, h: 5.8%)
      ),
      caption: "元素記号によるフィルタリング",
    )

  - 上部中央のドロップダウン`Intake Route`では、摂取形態によるフィルタリングが可能。

    #figure(
      markrect(
        image("images/OIR/IntakeRoute.png"),
        factor: 60%,
        (x: 1%, y: 1%, w: 60%, h: 18%)
      ),
      caption: "摂取形態によるフィルタリング",
    )

  - 上部右側のテキストボックス`Search Filter`では、インプットのタイトルテキストに対する正規表現でのフィルタリングが可能。

    #figure(
      markrect(
        image("images/OIR/SearchFilter.png"),
        (x: 37.8%, y: 24.2%, w: 59.4%, h: 9.5%)
      ),
      caption: "タイトルテキストに対する正規表現でのフィルタリング",
    )

  - 左上のチェックボックスで、一覧に表示されている全てのインプットの選択状態を切り替えることが可能。

    #figure(
      markrect(
        image("images/OIR/Select_All.png"),
        (x: 4%, y: 0%, w: 5%, h: 16%)
      ),
      caption: "全インプットの一括選択",
    )

//+ 子孫核種の考慮の有無を選択する。


== EIR計算実行画面

#figure(
  image("images/Figure_2.png", width: 90%),
  caption: "EIR計算実行画面",
)

+ 一覧から計算対象のインプットをチェックボックスで選択する。

  インプットの選択方法については #chapref(<oir-calc>) と同様。
  #v(0.5em)

//+ 子孫核種の考慮の有無を選択する。

+ 預託期間を`Commitment Period`欄に整数で入力し、ドロップダウンから預託期間の単位を選択する

+ 被ばく時の年齢（摂取時年齢）を`Intake Age`ドロップダウンから選択する。

+ 計算タイムメッシュファイルと出力タイムメッシュファイルを、それぞれ#linebreak()
  `Computational Time Mesh`欄と`Output Time Mesh`欄で選択する。

    タイムメッシュファイルの作成方法については、#link("UserManual_Appendix-4.md")[添付資料4]を参照。
  #v(0.5em)

+ 出力フォルダのパスを`Output Directory`欄で指定する。


== S係数作成画面

#figure(
  image("images/Figure_3.png", width: 90%),
  caption: "S係数作成画面",
)

+ 出力対象の核種にチェックを入れて選択する。

+ 出力対象の性別として、成人男性、成人女性を`Sex`欄のチェックボックスで選択する。

+ SAF値の補間方法を`SAF Information`欄のラジオボタンで選択する。
  - PCHIP（区分的3次エルミート内挿多項式）による補間
  - 線形補間
  #v(0.5em)

+ 出力ファイルの形式を`Output Format`欄のラジオボタンで、FlexID独自形式(`*.txt`)と、IDAC-Dose 2.1互換の出力形式(`*.csv`)から選択する。

+ 出力フォルダのパスを`Output Directory`欄で指定する。


#pagebreak()
= 結果表示画面

本画面は、`FlexUI.exe`に計算結果ファイル(`*.out`や`*.log`)をドラッグ＆ドロップすることによって開くことができる。

== 結果表示画面(共通部分)

#figure(
  markrect(
    image("images/Viewer/Viewer_Common.png"),
    (x:  1%, y: 19%, w: 75.5%, h: 80%),
    (x: 82%, y: 19%, w: 17.0%, h: 30%),
  ),
  caption: " 結果表示画面（共通部分）",
)

画面右上には、計算結果データの臓器/組織ごとの数値を表示する`Model`画面と、時系列データをグラフ表示する`Graph`画面を切り替えるタブが配置されている。

画面左上には、表示対象となる計算結果データを選択するための入力欄が配置されている。
ここでは以下に示す4つの操作を行うことができる。

+ 結果表示のために読み込むファイルを選択する。

  以下のいずれかの操作が使用可能。
  - `Open`ボタンで開くファイル選択ダイアログを使用する
  - ファイルパスを入力欄に入れてEnterキーを押す
  - 結果表示画面に対して対象ファイルをドラッグ＆ドロップする
  #v(0.5em)

+ 表示中データのインプットタイトルが`Title`欄に表示される。

  この欄は表示専用とだが、テキストを選択してコピーすることが可能。
  #v(0.5em)

+ 表示するデータを`Output Type`ドロップダウンで選択する。
  #figure(
    caption: "表示対象データの選択",
    table(
      columns: 2,
      align: left,
      table.header([項目],[説明]),
      table.hline(),
      [`RetentionActivity`],[臓器/組織毎の残留放射能量 [－]],
      [`CumulativeActivity`],[臓器/組織毎の積算残留放射能量 [Bq]],
      [`Dose`],[預託実効線量(`WholeBody`)と預託等価線量 [Sv/Bq]],
      [`DoseRate`],[等価線量率 [Sv/Bq/h]],
    ),
  )

  これらの項目は現在表示中のデータと同じフォルダにあるファイルが存在する場合に選択可能となる。データが存在しない項目は表示されない。
  #v(0.5em)

+ ドロップダウン`Nuclides`から表示対象とするデータブロックを選択する。

  `RetentionActivity`と`CumulativeActivity`の場合は、崩壊系列上の核種を選択可能。

  `Dose`と`DoseRate`の場合は、男女別に計算された数値データを選択可能。
  #v(0.5em)


== 結果表示画面(Model)

#figure(
  image("images/Figure_4.png", width: 100%),
  caption: " 結果表示画面(Model)",
)

+ 対象時刻の計算結果のデジタル値を表示する。

+ ボタン操作により体内を模擬したコンター図の動画が表示される。

+ 臓器/組織ごとのデータをコンター表示する。

+ コンターの閾値を設定する。


== 結果表示画面(Graph)

#figure(
  image("images/Figure_5.png", width: 100%),
  caption: " 結果表示画面(Graph)",
)

+ 表示したい臓器/組織にチェックを入れる。

+ 選択された臓器の計算結果の摂取後の時系列データが表示される。

+ 必要に応じて、横軸と縦軸について対数表示と線形表示を、チェックボックス(`Log(Horizontal)`と`Log(Vertical)`)で切り替える。

+ グラフ画面において、マウスによるドラッグやスクロールにより、グラフの移動、拡大縮小、軸レンジの変更が行える。

  #table(
    columns: 2,
    [マウス操作],[動作],
    [軸位置でホイール回転],[軸レンジの拡大縮小],
    [グラフエリア内でのホイール回転],[グラフ全体の拡大縮小],
    //[プロット上で左クリック],[対象臓器名称とデジタル値の表示],
    [左ドラッグ],[グラフデータ全体の移動],
    [ホイールボタンによる範囲選択],[選択範囲内の拡大表示],
    [ホイールボタンのダブルクリック],[全てのプロットデータの収まる最小範囲の軸レンジによる表示],
  )
