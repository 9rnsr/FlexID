#import "common.typ": *
#show: style.with(
  appendix: 4,
  title: "インプットファイル等の作成方法",
)

= インプットファイルの作成
ここでは、FlexIDが読み込むインプットファイル、時間メッシュファイル、および臓器名称統一ファイルの作成方法について示す。
インプットファイルは、ICRP OIR モデルの体内動態・移行係数・線量計算に必要な情報を
柔軟に記述できるよう構造化されている。

== 入力フォーマット
インプットファイルはUTF-8で記述され、以下の基本的な文法規則が適用される。
- 空行、および行頭・行末の空白文字は無視される。
- 行内に`#`文字が出現した場合、それ以降はコメントとして扱われる。
- インプットは複数の「セクション」から構成され、セクションは `[` _section-name_ `]` の行で開始する。

使用可能なセクションを 表@section-list に示す。

#v(1em)
#figure(caption: "使用可能なセクション名とその設定内容")[
  #table(
    columns: 2,
    align: (left, left),
    table.header([ セクション名 ], [ 内容 ]),
    [ `title` ], [ インプットのタイトルテキストを設定する。 ],
    [ `nuclide` ], [ 計算対象とする核種を設定する。 ],
    [ `parameter` ], [ 計算処理に関するパラメータや変数を設定する。 ],
    [ _nuc_`:compartment` ], [ 核種_nuc_の体内動態モデルで使用するコンパートメントを設定する。 ],
    [ _nuc_`:transfer` ], [ 核種_nuc_の体内動態モデルにあるコンパートメントへの移行を設定する。 ],
    [ `intake` ], [ 摂取経路を設定する。 ],
    [ `include:`_path_ ], [ 別ファイルを読み込む。 ],
  )
] <section-list>

== `title`セクション
インプットのタイトルテキストを1行以上で設定する。

== `nuclide`セクション
計算対象とする核種を設定する。図@simple-nuclides に示す簡易形式と、図@detail-nuclides に示す詳細形式のいずれかを使用可能。

#v(1em)
#columns(2)[
  #figure(caption: "簡易記述の例", kind: image)[
    #block(width: 80%)[
      ```
      [nuclide]
        Sr-90  Y-90


      ```
    ]
  ] <simple-nuclides>
  #colbreak()
  #figure(caption: "詳細記述の例", kind: image)[
    #block(width: 90%)[
      ```
      [nuclide]
        Sr-90  6.596156E-05  Y-90/1.0
        Y-90   2.595247E-01
      ```
    ]
  ] <detail-nuclides>
]
#v(1em)
簡易形式では核種名のみを空白文字や改行で区切って列挙する。`ICRP-07.NDX`に記載がある核種はここから崩壊定数や分岐比の情報が自動取得され、記載がない核種は崩壊定数$λ=0$の安定核種として取り扱われる。

詳細形式では崩壊系列を構成する1つの核種の情報を1行に記述し、これを繰り返す。
親核種の情報は1行目に記述する。各行は 表@detail-nuclide-record で示す3つ以上の列で構成される。

#v(1em)
#figure(caption: [`nuclide`セクションの詳細形式における各行のフィールド])[
  #table(
    columns: 3,
    align: (center, left, left),
    table.header([ 列番号 ], [ 内容 ], [ 備考 ]),
    [ 1 ], [ 核種名 ], [ 例：`Cs-137` ],
    [ 2 ], [ 崩壊定数λ [/day] ], [$=ln(2)\/"半減期"$ [day]、安定核種の場合は0を設定する。],
    [ 3…n ], [ 娘核種とその分岐比 [-] ], [ *核種名*`/`*分岐比*を分岐先の数だけ空白区切りで設定する。 ],
  )
] <detail-nuclide-record>


== `parameter`セクション(TODO)
計算処理に関する各種パラメータを設定する。
また、ユーザー定義の定数や数式に名前を付け、これをインプット記述で使用する変数機能も使用可能。

セクション名に核種名を含める場合は当該核種を対象として、
そうでない場合はインプット全体を対象としてパラメータ設定を行う。

#v(1em)
#figure(caption: [`parmeter`セクションで指定可能なパラメータ])[
  #table(
    columns: 3,
    align: left,
    table.header([ パラメータ名 ], [ 対象 ], [ 内容 ]),
    [ `ExcludeOtherSourceRegions` ], [ 全体, 核種 ], [ 線源領域`Other`の内訳から除外する線源領域を空白区切りで並べる。  ],
    [ `IncludeOtherSourceRegions` ], [ 全体, 核種 ], [ 線源領域`Other`の内訳として含める線源領域を空白区切りで並べる。  ],
  )
]


== _nuc_`:compartment`セクション
核種_nuc_の体内動態モデルを構成するコンパートメントを設定する。

1行につき1個のコンパートメントを設定でき、各行は 表@define-compartment で示す3つの列で構成される。

#v(1em)
#figure(caption: [`compartment`セクションの定義行])[
  #table(
    columns: 3,
    align: (center, left, left),
    table.header([ 列番号 ], [ 内容 ], [ 備考 ]),
    [ 1 ], [ コンパートメント機能 ], [ 蓄積：`acc`、 混合：`mix`、 排泄：`exc` ],
    [ 2 ], [ コンパートメント名称 ], [ ],
    [ 3 ], [ 線源領域の名称 ], [ `acc`でのみ指定可能。指定しない場合は「`---`」。 ],
  )
] <define-compartment>
#v(1em)

コンパートメント機能の詳細は以下の通り。
- `acc`：流入した放射能の蓄積と、時間経過による流出および壊変による減衰を計算する。
- `mix`：流入した放射能の割合による再配分を行う。分配は瞬時に行われるものとして計算される。
- `exc`：体外への排泄量を計算する。

コンパートメントと対応する線源領域の名称は、当該コンパートメントの残留放射能から線量を計算する際に使用されるため、`lib`フォルダ配下に置かれたICRP Publ.133のSAFデータに付随する線源領域定義ファイル`sregions_2016-08-12.NDX`に記載されている名称、あるいはOIRで「その他の組織」として線量を計算する場合の名称「Other」を設定する。コンパートメントが線源領域に対応しない場合は「`---`」を入力する。


== _nuc_`:transfer`セクション
核種_nuc_の体内動態モデルに入る移行経路を設定する。

セクション内の各行が移行経路を定義し、1行は表@define-transer で示す3つの列で構成される。

#v(1em)
#figure(caption: [`transfer`セクションの定義行])[
  #table(
    columns: 3,
    align: (center+horizon, left+horizon, left),
    table.header([ 列番号 ], [ 内容 ], [ 備考 ]),
    [ 1 ], [ 移行元コンパートメント ],
           [ *祖先核種名\/*を先頭に付加することで、祖先核種に定義 \
             されたコンパートメントからの壊変経路を設定する。 ],
    [ 2 ], [ 移行先コンパートメント ], [ 核種_nuc_に定義されたコンパートメントの名称。 ],
    [ 3 ], [ #stack(dir: ltr, 
      text[ 移行係数 #h(0.3em) ],
      pad(x: 0.1em)[ #math.equation(numbering: none, math.lr()[ \{ #block(height: 4.5em)[] ]) ],
      [ 移行速度[/d] \ 移行割合[%] \ `---` ])
    ], [
      速度付きの移行経路の場合に使用可能。 \
      `mix`からの流出経路でのみ使用可能。 \
      速度なしの壊変経路でのみ使用可能。 ]
  )
] <define-transer>
#v(1em)

移行速度[/d]は、時間経過を伴う核種の移行が起きる経路に対して設定が可能。具体的には動態モデル内で同じ核種のまま移行する経路と、生成核種が任意の速度で子孫の動態モデルに入る壊変経路であり、これらは必ず`acc`からの流出経路になる。

移行割合[%]は、`mix`からの流出経路においてのみ設定が可能。数値は移行元からの流出量全体を1とした時の割合であり、百分率(例: `60%`)と割合値(例: `0.6`、`3/5`)のどちらでも記述可能。`mix`からの移行は瞬時に行われ、時間経過を伴わない。

「`---`」は、壊変によって生じた生成核種_nuc_が、体内の同一領域にて移動せずにその動態モデルに入る壊変経路に対してのみ設定が可能。こちらも移行は瞬時に行われ、時間経過を伴わない。

#v(1em)
=== 壊変経路における中間コンパートメントの自動設定

崩壊系列として親核種`P`が娘核種`D`へ崩壊することを考える。核種`P`と核種`D`に体内で同一部位に対応するコンパートメント`C1`が定義されているとき、核種の壊変は`P/C1`から`D/C1`への速度なし壊変経路として表現される。

#figure(caption: "親から娘への速度なし壊変経路", kind: image)[
  #block(width: 80%)[
    ```
    [D:transfer]          # 核種Dの動態モデルに入る移行経路の定義セクション
      P/C1    C1    ---   # P/C1からD/C1へ入る速度なし壊変経路
    ```
  ]
]
#v(1em)

次に`C1`とは体内で異なる部位にあるコンパートメント`C2`と、`P/C1`から`D/C2`へ移行速度付き壊変経路を設定する場合を考える。FlexIDはこのような経路を、`P/C1`において生成された核種`D`を速度なし壊変経路で受け止めて一時的に保持する中間コンパートメント`D/<decay>`を自動設定し、さらにそこから核種Dを設定された速度で`D/C2`へ移行させるため経路を設定する、という形で表現する。またこの時、中間コンパートメントには移行元コンパートメント`P/C1`と同じ線源領域が設定される。

#figure(caption: "親から娘への速度付き壊変経路", kind: image)[
  #block(width: 80%)[
    ```
    [D:transfer]          # 核種Dの動態モデルに入る移行経路の定義セクション
      P/C1    C2    100   # P/C1からD/C2へ入る速度付き壊変経路

    # P/C1
    #   ↓
    # D/<decay>  → 100[/d] → D/C2
    ```
  ]
]
#v(1em)

崩壊系列の祖先にある核種から壊変経路を定義することもできる。親核種`P`から娘核種`D`が生成され、そこから孫核種`G`が生成される場合に、核種`P`から核種`G`への移行経路を次のように定義すると、途中の核種`D`の位置には壊変経路を構成するための中間コンパートメントが自動設定される。

#figure(caption: "親から孫への速度なし壊変経路", kind: image)[
  #block(width: 80%)[
    ```
    [G:transfer]          # 核種Gの動態モデルに入る移行経路の定義セクション
      P/C1    C1    ---   # P/C1からG/C1へ入る速度なし壊変経路

    # P/C1
    #   ↓
    # D/<decay>
    #   ↓
    # G/C1
    ```
  ]
]
#v(1em)

ただしここで`P/C1`から`D/C1`への壊変経路が別途定義されていると、これらの経路定義は互いに合成されることで1つの壊変経路を構成する。

#figure(caption: "親から娘＋親から孫への速度なし壊変経路", kind: image)[
  #block(width: 80%)[
    ```
    [D:transfer]          # 核種Dの動態モデルに入る移行経路の定義セクション
      P/C1    C1    ---   # (1) P/C1からD/C1へ入る速度なし壊変経路

    [G:transfer]          # 核種Gの動態モデルに入る移行経路の定義セクション
      P/C1    C1    ---   # (2) P/C1からG/C1へ入る速度なし壊変経路

    # (1) + (2) =
    #  o     o     P/C1
    #  ↓     |       ↓
    #  o     |     D/C1
    #        ↓       ↓
    #        o     G/C1
    ```
  ]
]
#v(1em)

この壊変経路の合成処理は、速度付きの壊変経路を織り交ぜた場合でも同様に適用される。

#figure(caption: "親から娘＋親から孫への速度付き壊変経路", kind: image)[
  #block(width: 80%)[
    ```
    [D:transfer]          # 核種Dの動態モデルに入る移行経路の定義セクション
      P/C1    C1    ---   # (1) P/C1からD/C1へ入る速度なし壊変経路

    [G:transfer]          # 核種Gの動態モデルに入る移行経路の定義セクション
      P/C1    C2    100   # (2) P/C1からG/C1へ入る速度あり壊変経路
      D/C1    C2    100   # (3) D/C1からG/C1へ入る速度あり壊変経路

    # (1) + (2) + (3) =
    #  o     o           P/C1
    #  ↓     |             ↓
    #  o     |     o     D/C1
    #        ↓     ↓       ↓
    #        o     o     G/<decay> → 100[/d] → G/C2
    ```
  ]
]
#v(1em)
#pagebreak()

注意すべき点として、速度付き壊変経路で自動設定される中間コンパートメントはこれを直接指定できないことから、経路定義が不足していると意図しない経路が構成されることがある点に注意。

#figure(caption: "親から娘＋親から孫への速度付き壊変経路(不足あり)", kind: image)[
  #block(width: 80%)[
    ```
    [D:transfer]          # 核種Dの動態モデルに入る移行経路の定義セクション
    # P/C1    C1    ---   # (1) P/C1からD/C1へ入る速度なし壊変経路

    [G:transfer]          # 核種Gの動態モデルに入る移行経路の定義セクション
      P/C1    C2    100   # (2) P/C1からG/C1へ入る速度あり壊変経路
      D/C1    C2    100   # (3) D/C1からG/C1へ入る速度あり壊変経路

    # (1)が不足すると、互いに無関係の2つの壊変経路がG/C2を終端とする形になる
    #    (2) + (3) =
    #     o           P/C1
    #     |             ↓
    #     |     o     D/<decay1>                              D/C1
    #     ↓     ↓       ↓                                       ↓
    #     o     o     G/<decay1> → 100[/d] → G/C2 ← 100[/d] ← G/<decay2>
    ```
  ]
]

#v(1em)
=== 不正な移行経路の設定

移行元と移行先の両方が一致する同一経路が2つ以上存在する場合や、移行元と移行先が同一の経路などは、不正な経路定義としてエラーとなる。

#v(1em)
=== 核種パターンの使用(TODO)


#pagebreak()
== `intake`セクション
体内動態モデルの計算開始時刻 $t = 0$ において、摂取された親核種がどのコンパートメントに、どの割合で体内動態モデルへ入るかの初期配分を設定する。このセクションは、インプットにおいて必ず1度だけ記述する必要がある。

摂取経路は1行につき1つ記述し、各行は 表@define-intake で示す3列で構成される。初期配分割合の合計は100%となる必要がある。

#figure(caption: [`intake`セクションの定義行])[ 
  #table(
    columns: 3,
    align: (center, left, left),
    table.header([ 列番号 ], [ 内容 ], [ 備考 ]),
    [ 1 ], [ 初期配分先コンパートメント ], [ 親核種の`compartment`セクションで定義された名称。 ],
    [ 2 ], [ 初期配分割合[%] ], [ 摂取量全体に対する割合。 ],
  )
] <define-intake>
#v(1em)

`intake`セクションの定義例を以下に示す。

#figure(caption: [経口摂取における`intake`セクションの定義例], kind: image)[
  #block(width: 80%)[
    ```
    [intake]
    # ICRP Publ.130 p.75 Fig.3.6
      Oralcavity              100.0%
    ```
  ]
]
#v(1em)

#figure(caption: [吸入摂取における`intake`セクションの定義例], kind: image)[
  #block(width: 80%)[
    ```
    [intake]
    # ICRP Publ.130 p.62 Table 3.1
    # ICRP Publ.130 p.64 Para.98 & p.65 Fig.3.4 footnote
      ET1-F                 $(      fr  *                 47.94% )
      ET2-F                 $(      fr  * (100% - 0.2%) * 25.82% )
      ETseq-F               $(      fr  *         0.2%  * 25.82% )
      BB-F                  $(      fr  * (100% - 0.2%) *  1.78% )
      BBseq-F               $(      fr  *         0.2%  *  1.78% )
      bb-F                  $(      fr  * (100% - 0.2%) *  1.10% )
      bbseq-F               $(      fr  *         0.2%  *  1.10% )
      ALV-F                 $(      fr  *                  5.32% )

      ET1-S                 $( (1 - fr) *                 47.94% )
      ET2-S                 $( (1 - fr) * (100% - 0.2%) * 25.82% )
      ETseq-S               $( (1 - fr) *         0.2%  * 25.82% )
      BB-S                  $( (1 - fr) * (100% - 0.2%) *  1.78% )
      BBseq-S               $( (1 - fr) *         0.2%  *  1.78% )
      bb-S                  $( (1 - fr) * (100% - 0.2%) *  1.10% )
      bbseq-S               $( (1 - fr) *         0.2%  *  1.10% )
      ALV-S                 $( (1 - fr) *                  5.32% )

      Environment           $(                     100% - 81.96% )
    ```
  ]
]
#pagebreak()

== `include`セクション
インプットの記述を複数のファイルに分割し、再利用性や可読性を高めるために外部ファイルを読み込むことができる。
指定されたファイルパスは、現在読み込み中のファイルが置かれたディレクトリを基準とした相対パスとして解釈される。
また、`include`は入れ子構造（多段階のinclude）になることができる。

`include`セクションは、指定ファイルをその位置に挿入したようにインプットの読み取り処理を進める。ただし`include`位置は同時にセクションの境界にもなるため、あるセクション定義の一部のみを別ファイルに切り出し`include`によって取り込むことは原理的に許可されない。言い換えると、1つのセクションの定義は必ず1つのファイル内で完結する仕様となっている。

`title`と`nuclide`セクションについては、現状では`include`セクションによる別ファイルからの取り込みが動作しない。そのため、これらのセクションについてはFlexIDによって最初に読み取られるインプットファイルに直接記述する必要がある。

`include`セクションの使用例を以下に示す。

#figure(caption: [`include`セクションの使用例], kind: image)[
  #block(width: 80%)[
    ```
    [title]
    Sr-90 Ingestion

    [nuclide]
      Sr-90  Y-90

    [intake]
      Oralcavity   100%

    [include:Sr-90_compartment.txt]   # compartmentセクションの定義を取り込む
    [include:Sr-90_transfer.txt]      # transferセクションの定義を取り込む
    ```
  ]
]

#v(1em)
== インプットのサンプル

```
[title]
Sr-90 Ingestion:Other

[nuclide]
# Nuclide | λ=ln(2)/t½[/d]   | Branching Fraction
#---------+-------------------+---------------------
  Sr-90     6.596156E-05        0.0
  Y-90      2.595247E-01        1.0


[Sr-90:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  inp   input                 ---
  acc   Oralcavity            O-cavity
  acc   Oesophagus-F          Oesophag-f
  acc   Oesophagus-S          Oesophag-s
  acc   St-con                St-cont
  acc   SI-con                SI-cont
  acc   RC-con                RC-cont
  acc   LC-con                LC-cont
  acc   RS-con                RS-cont
  exc   Faeces                ---
  acc   Blood1                Blood
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other
  acc   C-bone-S              C-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   T-bone-S              T-bone-S
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   UB-con                UB-cont
  exc   Urine                 ---

[Sr-90:transfer]
#-----------------------+---------------------+--------------
# From                  | To                  | Coefficient[/d] or [%]
#-----------------------+---------------------+--------------

  input                   Oralcavity              100.0%

# ICRP Publ.130 p.76 Table 3.4 & footnote
  Oralcavity              Oesophagus-F           6480
  Oralcavity              Oesophagus-S            720
  Oesophagus-F            St-con                12343
  Oesophagus-S            St-con                 2160
  St-con                  SI-con                   20.57
  SI-con                  RC-con                    6
  RC-con                  LC-con                    2
  LC-con                  RS-con                    2
  RS-con                  Faeces                    2

# ICRP Publ.134 p.215 Table 10.2
#   fA = 0.25   (Ingested material, All other chemical forms)
#   λ(SI->Blood) = fA*λ(SI->RC)/(1-fA) = 0.25 * 6 / (1 - 0.25) = 2
  SI-con                  Blood1                    2

# ICRP Publ.134 p.220 Table 10.3
  Blood1                  UB-con                    1.73
  Blood1                  RC-con                    0.525
  Blood1                  T-bone-S                  2.08
  Blood1                  C-bone-S                  1.67
  Blood1                  ST0                       7.5
  Blood1                  ST1                       1.5
  Blood1                  ST2                       0.003
  T-bone-S                Blood1                    0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Blood1                    0.578
  C-bone-S                Exch-C-bone-V             0.116
  ST0                     Blood1                    2.50
  ST1                     Blood1                    0.116
  ST2                     Blood1                    0.00038
  Exch-T-bone-V           T-bone-S                  0.0043
  Exch-T-bone-V           Noch-T-bone-V             0.0043
  Exch-C-bone-V           C-bone-S                  0.0043
  Exch-C-bone-V           Noch-C-bone-V             0.0043
  Noch-C-bone-V           Blood1                    0.0000821
  Noch-T-bone-V           Blood1                    0.000493

# ICRP Publ.130 p.85 Para.172
  UB-con                  Urine                    12


[Y-90:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Oralcavity            O-cavity
  acc   Oesophagus-F          Oesophag-f
  acc   Oesophagus-S          Oesophag-s
  acc   St-con                St-cont
  acc   SI-con                SI-cont
  acc   RC-con                RC-cont
  acc   LC-con                LC-cont
  acc   RS-con                RS-cont
  exc   Faeces                ---
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   ST0                   Other
  acc   ST1                   Other
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Kidneys               Kidneys
  acc   C-bone-S              C-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-S              T-bone-S
  acc   T-bone-V              T-bone-V
  acc   UB-con                UB-cont
  exc   Urine                 ---

[Y-90:transfer]
#-----------------------+---------------------+--------------
# From                  | To                  | Coefficient[/d or %]
#-----------------------+---------------------+--------------

# from parent to progeny
  Sr-90/Oralcavity        Oralcavity                ---
  Sr-90/Oesophagus-F      Oesophagus-F              ---
  Sr-90/Oesophagus-S      Oesophagus-S              ---
  Sr-90/St-con            St-con                    ---
  Sr-90/SI-con            SI-con                    ---
  Sr-90/RC-con            RC-con                    ---
  Sr-90/LC-con            LC-con                    ---
  Sr-90/RS-con            RS-con                    ---
  Sr-90/Faeces            Faeces                    ---
  Sr-90/Blood1            Blood1                    ---
  Sr-90/ST0               ST0                       ---
  Sr-90/ST1               ST0                       ---
  Sr-90/ST2               ST0                       ---
  Sr-90/C-bone-S          C-bone-S                  ---
  Sr-90/Exch-C-bone-V     C-bone-V                  ---
  Sr-90/Noch-C-bone-V     C-bone-V                  ---
  Sr-90/T-bone-S          T-bone-S                  ---
  Sr-90/Exch-T-bone-V     T-bone-V                  ---
  Sr-90/Noch-T-bone-V     T-bone-V                  ---
  Sr-90/UB-con            UB-con                    ---
  Sr-90/Urine             Urine                     ---

# ICRP Publ.130 p.76 Table 3.4 & footnote
  Oralcavity              Oesophagus-F           6480
  Oralcavity              Oesophagus-S            720
  Oesophagus-F            St-con                12343
  Oesophagus-S            St-con                 2160
  St-con                  SI-con                   20.57
  SI-con                  RC-con                    6
  RC-con                  LC-con                    2
  LC-con                  RS-con                    2
  RS-con                  Faeces                    2

# ICRP Publ.134 p.242 Table 11.2
#   fA = 1E-4   (Ingested material, All chemical forms)
#   λ(SI->Blood) = fA*λ(SI->RC)/(1-fA) = 1E-4 * 6 / (1 - 1E-4) = 6.000600060006001E-4
  SI-con                  Blood1                    6.000600060006001E-4

# ICRP Publ.134 p.252 Table 11.3
  Blood1                  Blood2                    0.498
  Blood1                  Liver0                    1.66
  Blood1                  Kidneys                   0.166
  Blood1                  ST0                       3.652
  Blood1                  ST1                       1.328
  Blood1                  UB-con                    2.49
  Blood1                  SI-con                    0.166
  Blood1                  T-bone-S                  3.32
  Blood1                  C-bone-S                  3.32
  Blood2                  Blood1                    0.462
  Liver0                  SI-con                    0.0231
  Liver0                  Blood1                    0.0924
  Liver0                  Liver1                    0.116
  Liver1                  Blood1                    0.0019
  Kidneys                 Blood1                    0.0019
  ST0                     Blood1                    0.231
  ST1                     Blood1                    0.0019
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

# ICRP Publ.130 p.85 Para.172
  UB-con                  Urine                    12
```

== 移行係数の設定方法

吸入摂取における呼吸器への初期配分割合と、これをインプットとして設定する例を示す。

#figure[
  #table(
    columns: 2,
    align: left,
    table.header([ Region ], [ Deposition (%) ]),
    [ ET#sub[1] ], [ 47.94 ],
    [ ET#sub[2] ], [ 25.82 ],
    [ BB        ], [  1.78 ],
    [ bb        ], [  1.10 ],
    [ AI        ], [  5.32 ],
    [ Total     ], [ 81.96 ],
  )
]

```
[intake]
# ICRP Publ.130 p.62 Table 3.1
# ICRP Publ.130 p.64 Para.98
# ICRP Publ.130 p.65 Fig.3.4 footnote
# ICRP Publ.134 p.215 Table 10.2
# f_r = 1 (100%)
  ET1-F            47.94%       # =          47.94%
  ET2-F            25.76836%    # = 99.8% of 25.82%
  ETseq-F           0.05164%    # =  0.2% of 25.82%
  BB-F              1.77644%    # = 99.8% of  1.78%
  BBseq-F           0.00356%    # =  0.2% of  1.78%
  bb-F              1.0978%     # = 99.8% of  1.10%
  bbseq-F           0.0022%     # =  0.2% of  1.10%
  ALV-F             5.32%       # =           5.32%
  Environment      18.04%       # = 100% - 81.96%
```

コンパートメント間の移行係数の例と、これをインプットとして設定する例を示す。

#figure[
  #table(
    columns: 3,
    align: left,
    table.header([ From ], [ To ], [ 移行速度[/d] ]),
    [  Oralcavity  ], [ Oesophagus-F ], [  6480 ],
    [  Oralcavity  ], [ Oesophagus-S ], [   720 ],
    [ Oesophagus-F ], [ Stomach-con  ], [ 12343 ],
    [ Oesophagus-S ], [ Stomach-con  ], [  2160 ],
  )
]

```
[Sr-90:transfer]
...
Oralcavity      Oesophagus-f     6480
Oralcavity      Oesophagus-s      720
Oesophagus-f    Stomach-con     12343
Oesophagus-s    Stomach-con      2160
```

消化管から血液への吸収を伴う場合の生物学的半減期及び移行割合を導出する場合は、消化管から血液への吸収割合を示す$f_"A"$値を考慮する必要がある。

表@abosorption に消化管から血液への吸収割合を示す$f_"A"$値の例を示す。

#figure(caption: "吸入摂取と経口摂取の吸収パラメータ")[
  #table(
    columns: (auto, 10%, 10%, 10%, 20%),
    align: center+horizon,
    table.cell(rowspan: 2, align: left+bottom)[Inhaled particulate materials],
    table.cell(colspan: 3)[Absorption parameter values], [Absorption from \ the alimentary],
    [$f_"r"$], [$s_"r"$ (/d)], [$s_"s"$ (/d)], [tract ($f_"A"$)],
    [F], [1   ], [100], [  ―   ], [1   ],
    [M], [0.2 ], [  3], [0.005 ], [0.2 ],
    [S], [0.01], [  3], [0.0001], [0.01],
    table.cell(colspan: 5, align: left)[Ingested materials],
    [All forms], [ ― ], [  ―   ], [ ―  ], [0.1],
  )
 ] <abosorption>

- $f_"A"$値（SI からBlood への吸収値）を伴う場合の生物学的半減期及び移行割合の計算方法

  #figure(caption: [])[
    #image("images/Figure_A4-1.png")
  ]

// `SI-con`から`Blood`への移行割合は、$f_"A"=0.1$ より10%となり、「SI-con」から「Blood」への移行係数は、下記の方法で導出する。

// - SI-con の生物学的半減期
//    ＝ Blood を除くSI-con からの全ての流出 + （f<sub>A</sub> 値 × SI-con → R-colon への流出）／（1－f<sub>A</sub> 値）
//    ＝ 6 +（0.1×6）／（1-0.1）
//    ＝ 6 + 0.6／0.9
//    ＝ 6.6667 /d

// - SI-con からBlood への移行割合
//    ＝ Blood への流出 ／ SI-con からの全ての流出
//    ＝ 0.6667 ／ 6.6667
//    ＝ 0.1 ＝ 10％


// = S-Coefficientファイルの作成
// 
// + `FlexUI.exe`をダブルクリックして起動。
// + `OIR S-Coeff`タブを選択する。
// + 計算結果の出力フォルダを設定する。既定では`FlexID\out\`が設定されている。
// + 計算対象とする性別にチェックを入れる。
// + SAF値の補間方法を選択する。
// + 計算対象とする核種名にチェックを入れる。なお、複数核種の計算を一度に実施することも可能（下図参照）。
// 
//   #figure(caption: "")[
//     #image("images/Figure_A4-2.png")
//   ]
// 
// + `Run`ボタンを押すと、出力フォルダに `{核種名}_{AM|AF}.txt` というファイル名で計算結果が出力される。
// 
//   #figure(caption: "")[
//     #image("images/Figure_A4-3.png")
//   ]
// 
// + 出力されたファイルを`FlexID\lib\OIR\Scoeff`フォルダに移動してS-Coefficientファイルの作成は完了となる。

= 時間メッシュファイルの作成

時間メッシュファイルのフォーマットを以下に示す。

- 空行は意味を持たず、単に無視される。
- 行内に`#`文字が出現する場合、それ以降の文字列はコメントとして扱われ、無視される。
- 最初の行はヘッダー行として無視される。
- 2行目以降は、カンマ区切りの2列で構成され、1列目は時間メッシュ分割区間の終端時刻を、2列目は区間を実際の時間メッシュに分割する時間幅を示す。
- 時刻及び時間幅は、数値 + 単位で入力する。単位の大文字小文字は無視される。

  #figure(caption: "時間メッシュファイルで使用可能な単位")[
    #table(
      columns: 2,
      align: left,
      table.header([ 単位 ], [ 意味 ]),
      [ `s`, `sec`, `secs`, `second`, `seconds` ], [ 秒                          ],
      [ `m`, `min`, `mins`, `minute`, `minutes` ], [ 分 (1分 = 60秒)             ],
      [ `h`, `hour`, `hours`                    ], [ 時間 (1時間 = 60分)         ],
      [ `d`, `day`, `days`                      ], [ 日 (1日 = 24時間 = 1440分)  ],
      [ `y`, `year`, `year`                     ], [ 年 (1年 = 365日 = 8760時間) ],
    )
  ]

時間メッシュファイルのサンプルを以下に示す。

#figure[
  #text(
    ```
    #----------------------------------------------------------------
    # 計算用のタイムメッシュファイル。
    # 評価の開始直後は細かく、時間の経過に応じて徐々に粗くなるよう
    # 時間メッシュを定義している。
    #----------------------------------------------------------------
    until     , step
       30 mins,    1 min
       50 days,    3 mins
      100 days,    5 mins
      250 days,   10 mins
      365 days,   30 mins      # 1 year
     1000 days,    1 hour
     2000 days,    4 hours
     3650 days,   12 hours     # 10 year
    27375 days,   24 hours     # 75 year
    ```
  )
]

#figure[
  #text(
    ```
    #----------------------------------------------------------------
    # 出力用のタイムメッシュファイル。
    # 預託期間75年まで、1日刻みの時間メッシュを定義している。
    #----------------------------------------------------------------
    until   , step
    75 years, 1 day
    ```
  )
]

出力時間メッシュを作成する際は、出力時間メッシュが計算時間メッシュに含まれていることを確認する。（含まれていない場合は計算エラーとなる。）

= 臓器名称統一ファイルの編集

臓器名称統一ファイルである`lib/FixList.txt`のフォーマットを以下に示す。

このファイルを参照し、「結果表示画面(Model)」に結果が出力される。

赤枠内に記載された名称が統一後の名称、青枠内に記載された名称が統一前の名称であり、これらの積算値が赤枠内の統一名称の結果として出力される。

#figure[
  #image("images/Figure_A4-4.png")
]
