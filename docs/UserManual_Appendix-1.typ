#import "common.typ": *
#show: style

#set page(numbering: (..n) => [付1 - #n.at(0)])
#set enum(numbering: "(1)")

#outline()
#pagebreak()

#maketitle(
  title: "添付資料1 「体内残留放射能の計算方法」",
)

= FlexIDの体内残留放射能の計算方法

放射性物質摂取（経口摂取/吸入摂取）時の各臓器/組織の放射性物質の放射能と被ばく線量をICRP OIRシリーズの体内動態モデルに基づき算出する内部被ばく線量評価コードFlexIDの計算方法を示す。

== 残留放射能計算方法の概要

+ 体内動態モデルを構成する臓器/組織をコンパートメントに置き換える形で、FlexIDインプットとしてのコンパートメントモデルを構成する。放射能を蓄積するコンパートメントについては、体内において蓄積を行う場合は蓄積(`acc`)機能を、体外において蓄積を行う場合は排泄(`exc`)機能を設定する。複数のコンパートメントからの流入を混合し割合で再配分する経路が存在する場合は、混合(`mix`)機能を設定したコンパートメントを作成し、これを介した移行経路を設定する。

+ 計算は時間を分割し、時間分割ステップ毎に各コンパートメントへの放射能の流入と流出を計算する。これを収束計算と呼ぶ。コンパートメントモデルは 図@model に示すようにループを構成しうるため、ある時間分割ステップに対する前回の収束計算結果と今回の収束計算結果を各コンパートメントにおいて比較し、その差が一定以下となり安定するまでこれを繰り返す。収束計算の結果が安定した時点で、次の時間分割ステップへ計算を進める。

+ 図@decay に示すように、子孫核種への壊変もコンパートメントの間での移行として取り扱う。

  #figure(
    image("images/Figure_A1-1.png", width: 70%),
    caption: "コンパートメント間の流入と流出",
  ) <model>

  #figure(
    image("images/Figure_A1-2.png", width: 68%),
    caption: "子孫核種への移行",
  ) <decay>


== 基本計算式

以下の方法で体内の時系列放射能分布を計算する。

計算方法にはノード・ジャンクション法を採用し、コンパートメント等の間での接続パス（ジャンクション）に設定された移行係数と移行割合から、各コンパートメント（ノード）の放射能を時間メッシュ毎に計算する。各時間メッシュにおいては、全てのノードの放射能が収束するまで繰り返し計算を実施する。

また、ノードは蓄積機能と混合機能のどちらかを選択でき、混合機能では複数の流入に対して平均値を計算し、その後、流出ジャンクションから移行割合に応じて流出させる。蓄積機能については、以下の基本式によりノードへの流入と流出を考慮した、放射能の蓄積を計算する。

- 蓄積機能における前提条件

  #figure(
    image("images/Figure_A1-3.png", width: 80%),
    caption: "蓄積機能",
  )

  流入放射能（$A$ \[Bq/d\]）は時間メッシュ期間中に一定であると仮定する。

- 放射能$N$の計算

  $ frac(dif N, dif t) &= A - (lambda + beta) dot N \

    N &= N_0 dot          exp{-(lambda + beta) dot t} +
         A   dot frac(1 - exp{-(lambda + beta) dot t}, lambda + beta) $

  ここで、

  #align(center)[
    #table(
      columns: (1.5cm,50%),
      align: (center, left),
      table.header([式], [意味]),
      [$N_0$],    [当該時間メッシュにおける初期放射能 \[Bq\]],
      [$lambda$], [崩壊定数 \[/d\]],
      [$beta$],   [流出放射能 \[Bq/d\]],
    )
  ]

- 時間積分放射能$Q$の計算

  $ Q &= integral_0^T  N_i dif t \
      &= N_0 dot           frac(1 - exp{ -(lambda + beta) dot T }, lambda + beta)
       + A   dot frac( T - frac(1 - exp{ -(lambda + beta) dot T }, lambda + beta), lambda + beta)
  $

//#pagebreak()
- 時間平均放射能$overline(N)$の計算

  $ overline(N) &= Q_i / T \
                &= N_0 dot          frac(1 - exp{ -(lambda + beta) dot T }, (lambda + beta) dot T)
                 + A   dot frac(1 - frac(1 - exp{ -(lambda + beta) dot T }, (lambda + beta) dot T), lambda + beta)
  $

  ここで、

  #align(center)[
    #table(
      columns: (1.5cm,50%),
      align: (center, left),
      table.header([式], [意味]),
      [$T$], [時間メッシュインターバル \[d\]],
    )
  ]

  FlexIDの計算処理フローチャートを 図@flow に示す。

  #figure(
    image("images/Figure_A1-4.png", height: 80%),
    caption: "FlexIDの計算処理フロー",
  ) <flow>

= 集合した臓器・組織の残留放射能の計算

ICRP Electronic Annex OIR Data Viewerで出力される「Whole Body」（全身）、「Alimentaryt Tract」（消化管）、「Lungs」（肺）、「Skeleton」（骨格）、「Liver」（肝臓）、「Thyroid」（甲状腺）の残留放射能データと比較可能な値を算出するための手法について示す。
なおここで示した "Blood fraction" については、OIR Data ViewerのHelpに記載されている。

- 「Whole Body」（全身）

    核種毎に、機能として`acc`が設定された各コンパートメンの残留放射能を合算した数値を出力する。

- 「Blood」（血液、輸送コンパートメント）

    核種毎に、線源領域として`Blood`が設定されたコンパートメントの残留放射能を合算した数値を出力する。

- 「Alimentary Tract」（消化管）

    核種毎に、線源領域として`St-cont`、`St-wall`、`SI-cont`、`SI-wall`、`RC-cont`、`RC-wall`、`LC-cont`、`LC-wall`、`RS-cont`、`RS-wall`
    が設定されたコンパートメントの残留放射能を合算し、これに「Blood」（血液）の残留放射能にBlood fractionとして`0.07`を掛けたものを加算した数値を出力する。また線源領域`Other`からは、その内訳に含まれる上記の線源領域について、`Other`全体に対する質量比を求めたうえで、これを線源領域`Other`の残留放射能に掛けたものを計算し合算する。

- 「Lungs」（肺）

    核種毎に、線源領域として`Bronchi`、`Bronchi-b`、`Bronchi-q`、`Brchiole`、`Brchiole-b`、`Brchiole-q`、`ALV`、`LN-Th`、`Lung-Tis`
    が設定されたコンパートメントの残留放射能を合算し、これに「Blood」（血液）の残留放射能にBlood fractionとして`0.125`を掛けたものを加算した数値を出力する。また線源領域`Other`からは、その内訳に含まれる上記の線源領域について、`Other`全体に対する質量比を求めたうえで、これを線源領域`Other`の残留放射能に掛けたものを計算し合算する。

- 「Skeleton」（骨格）

    核種毎に、線源領域として`C-bone-S`、`C-bone-V`、`T-bone-S`、`T-bone-V`、`C-marrow`、`T-marrow`、`R-marrow`、`Y-marrow`
    が設定されたコンパートメントの残留放射能を合算し、これに「Blood」（血液）の残留放射能にBlood fractionとして`0.07`を掛けたものを加算した数値を出力する。骨格については、OIR Data ViewerのHelpに記載があるように、コンパートメントモデルにおいて明示されている線源領域の残留放射能のみを考慮し、線源領域`Other`からの寄与についてはこれに含めない。

- 「Liver」（肝臓）

    核種毎に、線源領域として`Liver`
    が設定されたコンパートメントの残留放射能を合算し、これに「Blood」（血液）の残留放射能にBlood fractionとして`0.1`を掛けたものを加算した数値を出力する。

- 「Thyroid」（甲状腺）

    核種毎に、線源領域として`Thyroid`
    が設定されたコンパートメントの残留放射能を合算し、これに「Blood」（血液）の残留放射能にBlood fractionとして`0.0006`を掛けたものを加算した数値を出力する。

これらの集合コンパートメントにおける残留放射能は、OIR Data Viewerと同等の出力を得るという意図から、コンパートメントモデルにおいてその構成要素の線源領域が1つ以上使用されている場合にのみ「`*_Retention.out`」ファイルへ計算結果が出力される仕様となっている。