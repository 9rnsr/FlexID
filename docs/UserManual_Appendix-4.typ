#import "common.typ": *
#show: style

//#set enum(numbering: "①")

#outline()
#pagebreak()

#maketitle(
  title: "添付資料4 「インプットファイル等の作成方法」",
)

= インプットファイルの作成

インプットファイルは作成対象の核種が記載されているICRP OIR の$f_"A"$ 値および移行係数を参考とする。

作成したインプットは以下のように保存する。

- （例：Sr-90 の場合）

    `FlexID\inp\OIR\Sr-90`フォルダ内にインプットファイルを置く。

== 入力フォーマット

以下に入力フォーマットを示す。

インプットにおいて空行は意味を持たず、単に無視される。

行内に`#`文字が出現する場合、それ以降の文字列はコメントとして扱われ、無視される。

インプットはセクションによる区分けが行われ、#underline([`[` *セクション名* `]`])という行がその始まりとなる。

#figure(
  caption: "使用可能なセクション名とその設定内容",
  table(
    columns: 2,
    align: (left, left),
    table.header([ セクション名 ], [ 内容 ]),
    [ `title` ], [ インプットのタイトルテキストを設定する。 ],
    [ `nuclide` ], [ インプットで計算対象とする核種を設定する。 ],
    [ `parameter` ], [ インプット全体に対するパラメータを設定する。 ],
    [ `intake` ], [ 摂取経路を設定する。 ],
    [ 核種 + `:compartment` ], [ 当該核種の体内動態モデルにおいて使用するコンパートメントを設定する。 ],
    [ 核種 + `:transfer` ], [ 当該核種の体内動態モデルにおけるコンパートメント間の移行を設定する。 ],
  )
)

=== `nuclide`セクション

インプットで計算対象とする核種を設定する。核種の情報を`ICRP-07.NDX`から自動取得する簡易形式と、
崩壊定数や分岐比を明示する詳細形式の2種類が使用可能。

簡易形式では、下図のように核種名のみを空白または改行文字で区切って列挙する。

#figure(
  [
    ```
    [nuclide]
      Sr-90  Y-90
    ```
  ],
  caption: "簡易記述の例",
)

詳細形式では、1行に崩壊系列を構成する1つの核種情報を記述し、これを繰り返す。このとき親核種の情報は1行目に記述する必要がある。
各行は次表で示す3つ以上の列で構成される。

#figure(
  caption: [`[nuclide]`セクションの詳細形式における各行のフィールド],
  table(
    columns: 3,
    align: left,
    table.header([ 列番号 ], [ 内容 ], [ 備考 ]),
    [ 1 ], [ 核種名 ], [ 例：`Cs-137` ],
    [ 2 ], [ 崩壊定数λ [/day] ], [$=ln(2)\/"半減期"$ [day]、安定核種の場合は0を設定可能。],
    [ 3…n ], [ 娘核種とその分岐比 [-] ], [ `<核種名>/<分岐比>`を空白区切りで娘核種の数だけ入力する。\ 娘核種が存在しない場合は空とする。 ],
  )
)


=== `parameter`セクション

計算処理に関する各種のパラメータを指定する。セクション名に核種名を含める場合は当該核種を対象として、
そうでない場合はインプット全体を対象としてパラメータ設定を行う。

#figure(
  caption: [`[parmeter]`セクションの～～～],
  table(
    columns: 3,
    align: left,
    table.header([ パラメータ名 ], [ 対象 ], [ 内容 ]),
    [ `ExcludeOtherSourceRegions` ], [ 全体, 核種 ], [ 線源領域`Other`の内訳から除外する線源領域を空白区切りで並べる。  ],
    [ `IncludeOtherSourceRegions` ], [ 全体, 核種 ], [ 線源領域`Other`の内訳として含める線源領域を空白区切りで並べる。  ],
  )
)

=== `compartment`セクション

当該核種の体内動態モデルにおいて使用するコンパートメントを設定する。

1行につき1個のコンパートメントを設定でき、各行は次表で示す3つの列で構成される。

#figure(
  caption: [`[compartment]`セクションの定義行],
  table(
    columns: 3,
    align: left,
    table.header([ 列番号 ], [ 内容 ], [ 備考 ]),
    [ 1 ], [ コンパートメント機能 ], [ `acc`：蓄積、 `mix`：混合、 `exc`：排泄 ],
    [ 2 ], [ コンパートメント名 ], [ ],
    [ 3 ], [ 対応する線源領域の名称 ], [ `acc`のみ指定可能。指定しない場合は「-」を入力する。 ],
  )
)

コンパートメント機能の詳細は以下の通り。
- `acc`：流入した放射能の蓄積と、時間経過による流出および壊変による減衰を計算する。
- `mix`：流入した放射能の割合による再配分を行う。分配は瞬時に行われるものとして計算される。
- `exc`：体外への排泄、即ち、尿や糞などの排泄量を計算する。

対応する線源領域の名称は、当該コンパートメントの残留放射能から線量を計算する際に使用されるため、
`lib`フォルダ配下に置かれたICRP Publ.133のSAFデータに付随する線源領域定義ファイル`sregions_2016-08-12.NDX`に記載されている名称であるか、あるいはOIRで「その他の組織」として線量を計算する場合の名称「Other」を設定する。
コンパートメントが線源領域に対応しない場合は「`---`」を入力する。


=== `transfer`セクション

当該核種の体内動態モデルにおけるコンパートメント間の移行を設定する。

1行につき1個の移行経路を設定でき、各行は次表で示す3つの列で構成される。

#figure(
  caption: [`[transfer]`セクションの定義行],
  table(
    columns: 3,
    align: (center+horizon, left+horizon, left),
    table.header([ 列番号 ], [ 内容 ], [ 備考 ]),
    [ 1 ], [ 移行元のコンパートメント名 ], [ 親核種にあるコンパートメントは、先頭に「核種名 + `/`」を付加する。 ],
    [ 2 ], [ 移行先のコンパートメント名 ], [],
    [ 3 ], [ 移行速度[/d] (`acc`の場合) または \ 移行割合[%] (`mix`の場合) ], [ 速度がない壊変経路の場合は「`---`」を入力する。 ],
  )
)
時間経過を伴う移行には移行速度[/d]を設定し、これは一般的に、OIRに定義された数値をそのまま入力することができる。
蓄積計算を行う`acc`からの流出には、移行速度での設定が必要となる。

時間経過を伴わない瞬時に行われる移行には、移行元から流出する放射能に対する移行割合[%]を設定する。この時、数値の末尾には百分率であることを示す`%`を付加する。合算と再配分を行う`mix`からの流出には、移行割合での設定が必要となる。

親核種からの壊変によって生じた核種が、体内の同一領域にて移動せずに娘核種のモデルに入る場合は、移行係数として「`---`」を設定する。

親核種からの壊変によって生じた核種が、移行速度を伴う経路で娘核種のモデルに入る（OIRでは概ね中央血液コンパートメントへ移動する）場合は、生成核種を一時的に保持しておくための中間コンパートメントが娘核種モデル内に自動生成され、そこから指定された移行速度で核種が移動するよう経路が構成される。またこのとき、中間コンパートメントには親核種があったコンパートメントと同じ線源領域が設定される。

移行元と移行先の両方が一致する複数の同一経路、移行先が移行元と同じ経路などの、不正な経路の定義はエラーとなる。

=== インプットのサンプル

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

#figure(
  table(
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
)

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

#figure(
  table(
    columns: 3,
    align: left,
    table.header([ From ], [ To ], [ 移行速度[/d] ]),
    [  Oralcavity  ], [ Oesophagus-F ], [  6480 ],
    [  Oralcavity  ], [ Oesophagus-S ], [   720 ],
    [ Oesophagus-F ], [ Stomach-con  ], [ 12343 ],
    [ Oesophagus-S ], [ Stomach-con  ], [  2160 ],
  )
)

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

#figure(
  caption: "吸入摂取と経口摂取の吸収パラメータ",
  table(
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
) <abosorption>

- $f_"A"$値（SI からBlood への吸収値）を伴う場合の生物学的半減期及び移行割合の計算方法

  #figure(
    image("images/Figure_A4-1.png"),
    caption: [],
  )

`SI-con`から`Blood`への移行割合は、$f_"A"=0.1$ より10%となり、「SI-con」から「Blood」への移行係数は、下記の方法で導出する。
