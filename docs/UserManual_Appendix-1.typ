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

本資料では、放射性物質摂取（経口摂取/吸入摂取）時の各臓器/組織の放射性物質の放射能と被ばく線量をICRP OIRシリーズの体内動態モデルに基づいて算出する方法を示す。

== 計算方法の概要

FlexIDでは、体内の時系列放射能分布の計算方法としてノード・ジャンクション法を採用する。コンパートメント等の間での接続パス（ジャンクション）に設定された移行係数と移行割合から、各コンパートメント（ノード）の放射能を時間メッシュ毎に計算する。各時間メッシュにおいては、全てのノードの放射能が収束するまで繰り返し計算を実施する。

  #figure(caption: "コンパートメント間の流入と流出")[
    #image("images/Figure_A1-1.png", width: 70%)
  ] <model>

- 体内動態モデルを構成する臓器/組織をコンパートメントに置き換える形で、FlexIDインプットとしてのコンパートメントモデルを構成する。
- 放射能を蓄積するコンパートメントについては、体内の臓器/組織に対応する場合は`acc`機能を、体外に排泄された放射能を受け止める箇所には`exc`機能を設定する。`exc`コンパートメントは体外に位置するため同じ核種のまま移行する経路を設定できないが、しかし壊変によって子孫核種へ移行する経路は設定することができる。
- 複数のコンパートメントからの流入を混合し割合で再配分する経路では、`mix`機能を設定したコンパートメントを作成し、これを介した移行経路を設定する。
#columns(2)[
- 動態モデル内を同じ核種のまま移行する場合は、インプットによって設定した移行速度がその経路に使用される。
- 子孫核種への壊変は、図@decay に示すように親核種の崩壊定数と子孫核種への分岐比によってその速度が決まる、子孫核種を保持するコンパートメントへの移行として取り扱う。
#colbreak()
#figure(caption: "子孫核種への移行")[
  #image("images/Figure_A1-2.png", width: 95%)
] <decay>
]

- 計算は評価期間を任意の時間幅で分割し、それぞれの時間分割ステップにおいて各コンパートメントへの放射能の流入と流出を計算する。残留放射能の計算を実施する分割の設定を計算時間メッシュと呼び、これに対して計算結果を出力するための分割設定を出力時間メッシュと呼ぶ。任意の出力時間メッシュの時間分割ステップについて、その開始と終了が一致する連続した1つ以上の計算時間メッシュの時間分割ステップが存在する必要がある。
- 計算処理は計算時間メッシュの時間分割ステップ毎に行われ、これを収束計算と呼ぶ。コンパートメントモデルは 図@model に示すようにループを構成しうるため、ある時間分割ステップに対する前回の収束計算結果と今回の収束計算結果を各コンパートメントにおいて比較し、その差が一定以下となり安定するまでこれを繰り返す。収束計算の結果が安定した時点で、次の時間分割ステップへ計算を進める。

FlexIDの計算処理フローチャートを 図@flow に示す。
#figure(caption: "FlexIDの計算処理フロー")[
  #image("images/Figure_A1-4.png", width: 85%)
] <flow>

== 基本計算式

蓄積機能（コンパートメント機能: `acc`, `exc`）を設定するコンパートメントでは、以下の基本式によりノードへの流入と流出を考慮した放射能の蓄積を計算する。

#[
#show figure.where(kind: table, caption: none): it => { block(above: 0em)[#it #v(1em)] }

- 蓄積機能における前提条件

  流入放射能（$A$ [Bq/d]）は時間メッシュ期間中に一定であると仮定する。
  #figure(caption: "蓄積機能")[
    #image("images/Figure_A1-3.png", width: 80%)
  ]

- 放射能$N$の計算
  $ frac(dif N, dif t) &= A - (lambda + beta) dot N \

    N &= N_0 dot          exp{-(lambda + beta) dot t} +
          A   dot frac(1 - exp{-(lambda + beta) dot t}, lambda + beta) $
  ここで、
  #figure[
    #table(
      columns: (1.5cm,50%),
      align: (center, left),
      table.header([式], [意味]),
      [$N_0$],    [当該時間メッシュにおける初期放射能 [Bq]],
      [$lambda$], [崩壊定数 [/d]],
      [$beta$],   [流出放射能 [Bq/d]],
    )
  ]

- 時間積分放射能$Q$の計算
  $ Q &= integral_0^T  N_i dif t \
      &= N_0 dot           frac(1 - exp{ -(lambda + beta) dot T }, lambda + beta)
      +   A  dot frac( T - frac(1 - exp{ -(lambda + beta) dot T }, lambda + beta), lambda + beta) $

- 時間平均放射能$overline(N)$の計算
  $ overline(N) &= Q_i / T \
                &= N_0 dot          frac(1 - exp{ -(lambda + beta) dot T }, (lambda + beta) dot T)
                +   A  dot frac(1 - frac(1 - exp{ -(lambda + beta) dot T }, (lambda + beta) dot T), lambda + beta) $
  ここで、
  #figure[
    #table(
      columns: (1.5cm,50%),
      align: (center, left),
      table.header([式], [意味]),
      [$T$], [時間メッシュインターバル [d]],
    )
  ]
]

混合機能（コンパートメント機能: `mix`）を設定したコンパートメントでは、複数の流入に対して平均値を計算し、その後、流出ジャンクションから移行割合に応じて流出させる。

#pagebreak()
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
