#import "common.typ": *
#show: style

//#set enum(numbering: "①")

#outline()
#pagebreak()

#maketitle(
  title: "添付資料3 「S係数の計算方法」",
)

= はじめに

本資料は、ICRP Publ.103の放射線加重係数データ #cite(<icrp103>)、ICRP Publ.107の核崩壊（放出放射線）データ #cite(<icrp107>)、
およびICRP Publ.133のSAFデータ #cite(<icrp133>) を用いて、線源領域に蓄積した残留放射能から標的領域への線量寄与を計算する際に必要となる、
S係数（S-Coefficient）を算出するための計算方法について説明する。

= 概要

== 計算の流れ

+ 各パラメータ（放射線加重係数データ、放出放射線データ、SAFデータ）を読み込む。
+ 各パラメータを用いて、式(1)により線源領域から1つの標的領域へのS係数を計算する。
+ 2.を線源領域から標的領域の全てのパターン（79*43通り）について実施する。

S係数計算処理の概略フローを以下に示す。

#figure(
  image("images/Figure_A3-1.png"),
  caption: "S係数計算処理の概略フロー図",
)


= 線源領域79個×標的領域43個のS-Coefficient計算

ICRP Publ.133において公開された最新の比吸収割合（SAF）データから、以下の計算方法によりS係数を算出する。

== 光子・単一エネルギー電子・α線

光子、単一エネルギー電子、及びα線の$Y_i$、$E_i$、及び$w_("R"_i)$を用いて、次式から核種毎のS係数を計算する。

$ "S–coefficient"("T"←"S") = sum_i ( Y_i dot E_i dot "SAF"("T"←"S")_i dot w_("R"_i) ) "[MeV/kg/nt]" $

ここで、

#figure(
  table(
    columns: (auto,70%),
    align: (center+horizon, left),
    table.header([式], [意味]),
    [$"S–coefficient"("T"←"S")$],[ある核種がある線源領域$"S"$で1壊変したとき、標的領域$"T"$の1kg当たりに吸収されるエネルギー [MeV/kg/nt]],
    [$Y_i$],[壊変あたりの放射線$i$の放出割合 [/nt]],
    [$E_i$],[放射線$i$の平均または単一エネルギー [MeV]],
    [$"SAF"("T"←"S")_i$],[線源領域$"S"$内で放出されるエネルギー$E_i$の放射線から、標的領域$"T"$の単位質量あたりに吸収されるエネルギーの割合（比吸収割合） [/kg]],
    [$w_("R"_i)$],[放射線$i$の放射線加重係数 [-]],
  )
)

== β#super[-]・β#super[+]線スペクトル

β#super[-]、及びβ#super[+]線スペクトルのS係数は、スペクトル全体で積分して、次式(2)から計算される。

$ "S–coefficient"("T"←"S") = w_R integral_0^(E_(max)) Y(E) dot E dot "SAF"("T"←"S", E) dif E "[MeV/kg/nt]" $

ここで、

#figure(
  table(
    columns: 2,
    align: (center, left),
    table.header([式], [意味]),
    [$E$],     [スペクトルデータ点のエネルギー [MeV]],
    [$Y(E)$],  [スペクトルデータ点$E$[MeV]における壊変あたりのβ粒子の放出割合 [/nt]],
    [$"SAF"("T"←"S", E)$], [$E$[MeV]における比吸収割合 [/kg]],
    [$w_"R"$], [放射線$"R"$（電子、陽電）の放射線加重係数 [-]],
  )
)

== 自発核分裂中性子スペクトル

ICRP Publ.133では、自発核分裂中性子放出核種（28核種）については、核種毎に核分裂中性子スペクトルの平均エネルギーで正規化されたスペクトル平均のSAFが記載されている。従って、以下の方法により、核種毎のS係数$("T"←"S")$を計算することができる。

$ "S–coefficient"("T"←"S") = overline("SAF"("T"←"S")) * overline(w_"R") "[MeV/kg/nt]" $

ここで、

#figure(
  table(
    columns: 2,
    align: (center, left),
    table.header([式], [意味]),
    [$overline("SAF"("T"←"S"))$], [スペクトル平均の比吸収割合 [/kg]],
    [$overline(w_"R")$], [対象核種におけるスペクトル平均の中性子の放射線加重係数 [-]]
  )
)


#pagebreak()
= 線源領域Other×標的領域43個のS係数計算

「その他の組織」$"Other"$を線源領域とするS係数の計算は、対象とする体内動態モデル図で明示されていないコンパートメントを決定する必要がある。
体内動態モデルはFlexIDのインプットに対応することから、FlexIDでは1つのインプットの計算処理に先立ってこのS-Cofficientを計算する必要がある。

具体的には、まずICRP Publ.133のSAFデータに含まれる`sregions_2016-08-12.NDX`にてID列が`1`となっている47個の「その他の組織」から、
FlexIDインプットにてコンパートメントに対応付けられている線源領域を除くことで、実際に「コンパートメントとして明示されていない」$"Other"$の内訳を求める。

次に、求めた$"Other"$の内訳と、それらの質量を用いて、次式から43個の標的領域それぞれへのS係数$("T"←"Other")$を計算する。

$ "S–coefficient"("T"←"Other") = frac(1, M_"Other") sum_"S" M_"S" dot "S–coefficient"("T"←"S") $

ここで、

#figure(
  table(
    columns: 2,
    align: (center+horizon, left),
    table.header([式], [意味]),
    [$M_"Other"$], [コンパートメントモデルで明確にされていない組織の \ 合計質量（ICRP Publ.133 Table A3.3 ※1）],
    [$M_"S"$],     [コンパートメントモデル図で明確にされていない \ 個々の線源領域の質量],
  )
)

核種によっては、コンパートメントとして明示されておらずとも$"Other"$の内訳から除外するよう指定される線源領域も存在する。
このためFlexIDでは、$"Other"$の内訳をインプットにて調整するための機能を提供する。
詳細は#link("UserManual_Appendix-4.typ")[添付資料4] の1.1.2を参照。


#pagebreak()
= ICRP Publication 103 放射線加重係数データ

ICRP Publication 103 に示されている放射線加重係数（$w_"R"$）を 表@w_R に示す。

#v(1em)
#figure(
  caption: "放射線加重係数",
  table(
    columns: (4cm, 4cm),
    align: (center+horizon, center),
    table.header([放射タイプ], [$w_"R"$]),
    [光子],       [1],
    [電子],       [1],
    [α粒子],     [20],
    [中性子],     [図@neutron_w_R 参照],
  )
) <w_R>
#v(1em)

S係数計算の為の光子、電子及びα粒子の$w_"R"$は 表@w_R に示す値を用いる。しかし、中性子については、ICRP Publ.133 のSAFデータにおいて、中性子スペクトル平均の$overline(w_"R")$が用いられている為、その値からS係数を計算する。

#v(1em)
#figure(
  image("images/Figure_A3-2.png"),
  caption: "中性子スペクトル平均の放射線加重係数",
) <neutron_w_R>

#pagebreak()
= ICRP Publication 107 核崩壊データファイルの仕様

== `ICRP-07.RAD` ファイル

`ICRP-07.RAD`ファイル（以下RADファイルと呼ぶ）には、放射性核種の壊変で放出された各放射線のエネルギーと収率に関するデータが含まれている。このファイルの放射線にはカットオフエネルギーが適用されていない。RADファイルのデータフィールドを 表@rad_fields に示す。

#figure(
  caption: [`ICRP-07.RAD`ファイルの構造],
  table(
    columns: 3,
    align: (center+horizon, center, left),
    table.header([フィールド], [フォーマット], [説明]),
    table.cell(colspan: 3, align: left)[#underline[核種フィールド] ],
    [核種],       [`A7`],         [核種名（例：`Tc-99m`）],
    [半減期],     [`E11.0`],      [核種の物理的半減期],
    [時間単位],   [`A2`],         [半減期の時間単位：`us`、`ms`、`s`、`m`、`d`、`y`],
    [N],          [`I9`],         [放射線データの数],
    table.cell(colspan: 3, align: left)[#underline[データフィールド]],
    [ICODE],      [`A2`],         [放射線のタイプ（表@icode_jcode 参照）],
    [収率],       [`E12.0`],      [放射線の収率 [/nt]],
    [エネルギー], [`E12.0`],      [放射線のエネルギー [MeV]],
    [JCODE],      [`A3`],         [放射線のタイプ（表@icode_jcode 参照）],
  )
) <rad_fields>

表@rad_fields に示す核種フィールドには核種名、半減期、放射線データの数が記載されている。データフィールドには、放射線タイプを識別する整数コード（ICODE）、壊変あたりの放射線収率、放射線の固有エネルギーまたは平均エネルギー、放射線の種類を示す2文字の簡易標記（JCODE）が記載されている。

#figure(
  caption: [ICODE, JCODE変数の説明],
  table(
    //columns: 3,
    columns: (2cm, 2cm, 3cm) ,
    align: (center+horizon, center, left),
    table.header([ICODE], [JCODE], [説明]),
    table.cell(rowspan: 3)[
      1  ], [ `G`  ], [ γ線 ],
            [ `PG` ], [ 遅発γ線 ],
            [ `DG` ], [ 即発γ線 ],
    [ 2  ], [ `X`  ], [ X線 ],
    [ 3  ], [ `AQ` ], [ 消滅光子 ],
    [ 4  ], [ `B+` ], [ β#super[+]粒子 ],
    table.cell(rowspan: 2)[
      5  ], [ `B`  ], [ β粒子 ],
            [ `DB` ], [ 遅発β粒子 ],
    [ 6  ], [ `IE` ], [ 内部転換電子 ],
    [ 7  ], [ `AE` ], [ オージェ電子 ],
    [ 8  ], [ `A`  ], [ α粒子 ],
    [ 9  ], [ `AR` ], [ α反跳核 ],
    [ 10 ], [ `FF` ], [ 核分裂断片 ],
    [ 11 ], [ `N`  ], [ 中性子 ],
  )
) <icode_jcode>

Tc-99m の核崩壊データの一部を 図@rad_file に示す。データフォーマットは 表@rad_fields に従っており、データはエネルギーの小さい順に並んでいる。

#figure(
  image("images/Figure_A3-3.png", width: 8cm),
  caption: [`ICRP-07.RAD`サンプル],
) <rad_file>


== ICRP-07.BET ファイル

`ICRP-07.BET`（以下、BETファイルと呼ぶ）には、RADファイルに含まれるβ粒子のスペクトルデータが含まれている。スペクトルデータは任意のエネルギー幅毎の積算値で示されている。核種フィールドには、核種毎に核種名とその核種の全放射線データが記録されている。データフィールドには、このエネルギー幅で放出される1壊変当たり、1MeV当たりの電子エネルギーE（MeV）とβ粒子の数が含まれている。BETファイルのデータフィールドを 表@bet_fields に、ファイル内容の一部を 図@bet_file に示す。

#figure(
  caption: [`ICRP-07.BET`ファイルの構造],
  table(
    columns: 3,
    align: (center+horizon, center, left),
    table.header([フィールド], [フォーマット], [説明]),
    table.cell(colspan: 3, align: left)[#underline[核種フィールド] ],
    [核種],       [`A7`],     [核種名（例：`Tc-99m`）],
    [N],          [`I10`],    [データレコード数],
    table.cell(colspan: 3, align: left)[#underline[データフィールド]],
    [エネルギー], [`F7.0`],   [エネルギーグリッドポイント [MeV]],
    [β粒子数],    [`E10.0`],  [エネルギーにおける壊変1MeVあたりのβ粒子数],
  )
) <bet_fields>

#figure(
  image("images/Figure_A3-4.png", width: 34%),
  caption: [`ICRP-07.BET`サンプル],
) <bet_file>


= ICRP Publication 133 比吸収割合（SAF）データファイルの仕様

== α粒子、電子および光子のSAFファイル

α粒子、電子及び光子のSAFファイル名を 表@saf_files に示す。

#figure(
  caption: "α粒子、電子及び光子のSAFファイル名",
  table(
    columns: 3,
    align: (center+horizon, center, left),
    table.header([性別], [粒子], [ファイル名]),
    table.cell(rowspan: 3)[男性],
    [α], [`rcp-am_alpha_2016-08-12.SAF`],
    [β], [`rcp-am_electron_2016-08-12.SAF`],
    [γ], [`rcp-am_photon_2016-08-12.SAF`],
    table.cell(rowspan: 3)[女性],
    [α], [`rcp-af_alpha_2016-08-12.SAF`],
    [β], [`rcp-af_electron_2016-08-12.SAF`],
    [γ], [`rcp-af_photon_2016-08-12.SAF`],
  )
) <saf_files>

ここで、`am`は標準成人男性（Adult Male）、`af`は標準成人女性（Adult Female）を表わす。それぞれのファイルには5つの見出しレコードがあり、6番目以降のレコードが対象となる全ての線源領域と標的領域に対応する全てのSAFレコードとなる。これらのファイルは、79の線源領域から43の標的領域に対するSAFが示されており、レコードの総数は43×79+5の3402である。ファイル内のレコードはすべて同じ長さである。電子と光子のレコードの長さは315で、αのSAFレコードの長さは270である。それに伴うキャリッジリターンとラインフィード（CrLf）は、これらの値に含まれていない。SAFファイルのデータフィールドを 表@saf_fields に示す。

#figure(
  caption: "SAFファイル（α粒子、電子及び光子）の構造",
  table(
    columns: 3,
    align: (left, center, left),
    table.header([フィールド], [フォーマット], [説明]),
    table.cell(colspan: 2)[#underline[1～3行目]], [ファイルの説明],
    table.cell(colspan: 3)[#underline[4行目]],
    [標的の数], [`I4`],       [標的領域の数],
    [線源の数], [`I4`],       [線源領域の数],
    [空白],     [`X14`], [],
    [放射線エネルギー], [`F10.5`], [SAF値に対する放射線エネルギー（1～n 個）],
    table.cell(colspan: 2)[#underline[5行目]], [区切り線],
    table.cell(colspan: 3)[#underline[6行目以降]],
    [標的領域],    [`A10`], [標的頭字語（例：`UB-wall`）],
    [ダミー],      [`A2`], [記号「`<-`」],
    [線源領域],    [`A10`], [線源頭字語（例：`UB-cont`）],
    [SAF(1…`n`)], [`E10.0`], [`n`個のSAF値（kg#super[-1]）],
    [`Ecut`],     [`E10.0`], [ゼロでないSAFの最低エネルギー],
    [`Icut`],     [`I3`], [`Ecut`のエネルギー指数],
    [CrLf],       [―], [改行],
  )
) <saf_fields>

電子と光子のSAFファイルのエネルギー数nは28であり、αのSAFファイルは24のエネルギーについて示されている。前述のとおり、αのSAFファイルの`Ecut`パラメータは長さが5（`F5.0`）である。

各ファイルの4番目のレコードは、SAFの値に対応する放射線エネルギーを表示するものである。エネルギーの単位はMeVである。

`Icut`フィールドには、ゼロでないSAF（T←S）値が示されているエネルギーの低エネルギー側からの順番が示されている。SAFがすべてゼロであれば、`Icut`は`0`に設定される。`Icut`がjであれば、レコードには`E(j)`から`E(n)`までの範囲のエネルギーのゼロでないSAF値が含まれている。ここで、`n`は電子と光子のSAFファイルで28、αのSAFファイルで24である。つまりゼロでないSAF値の数は、`n-j+1`である。

#figure(
  image("images/Figure_A3-5.png", width: 95%),
  caption: [`rcp-af_photon_2016-08-12.SAF`サンプル],
)


== 中性子のSAFファイル

中性子のSAFファイル名を 表@neutron_saf_files に示す。

#figure(
  caption: "中性子のSAFファイル名",
  table(
    columns: 3,
    align: (center+horizon, center, left),
    table.header([性別], [粒子], [ファイル名]),
    [男性], [N], [`rcp-af_neutron_2016-08-12.SAF`],
    [女性], [N], [`rcp-am_neutron_2016-08-12.SAF`],
  )
) <neutron_saf_files>

ここで、`am`は標準成人男性（Adult Male）、`af`は標準成人女性（Adult Female）を表わす。中性子のSAFファイルは、Publ.107（ICRP、2008年）にある自発核分裂崩壊モードの放射性核種に関する、43の標的領域と79の線源領域について、核分裂中性子スペクトルの平均エネルギーで正規化されたスペクトル平均のSAFを表示するものである。中性子のSAFファイルに示されている28の核種は`U-238`、`Pu-236`、`Pu-238`、`Pu-240`、`Pu-242`、`Pu-244`、`Cm-240`、`Cm-242`、`Cm-244`、`Cm-245`、`Cm-246`、`Cm-248`、`Cm-250`、`Cf-246`、`Cf-248`、`Cf-249`、`Cf-250`、`Cf-252`、`Cf-254`、`Es-253`、`Es-254`、`Es-254m`、`Es-255`、`Fm-252`、`Fm-254`、`Fm-255`、`Fm-256`、および`Fm-257`である。

これらのファイルの4番目のレコードは、Publ.103（ICRP、2007年）に示すスペクトル平均の中性子の$W_"R"$を表示するものである。長さ302のSAFレコードのデータフィールドを 表@neutron_saf_fields に示す。

#figure(
  caption: "SAFファイル（中性子）の構造",
  table(
    columns: 3,
    align: (left, center, left),
    table.header([フィールド], [フォーマット], [説明]),
    table.cell(colspan: 2)[#underline[1～2行目]], [ファイルの説明],
    table.cell(colspan: 3)[#underline[3行目]],
    [線種], [`A10`], [線種名（`Neutron`）],
    [空白], [`X14`], [],
    [核種], [`A10`], [核種名（例：`U-238`）],
    table.cell(colspan: 3)[#underline[4行目]],
    [標的の数], [`I4`], [標的領域の数（43）],
    [線源の数], [`I4`], [線源領域の数（79）],
    [$W_"R"$記号], [`A14`], [「`W_R=`」],
    [放射線加重係数], [`F10.5`], [スペクトル平均の放射線加重係数],
    table.cell(colspan: 2)[#underline[5行目]], [区切り線],
    table.cell(colspan: 3)[#underline[6行目以降]],
    [標的領域],  [`A10`],   [標的頭字語（例：`UB-wall`）],
    [ダミー],    [`A2`],    [記号「`<-`」],
    [線源領域],  [`A10`],   [線源頭字語（例：`UB-cont`）],
    [SAF(1…28)], [`E10.0`], [28個のSAF値（kg#super[-1]）],
    [CrLf],      [―],  [改行],
  )
) <neutron_saf_fields>

#figure(
  image("images/Figure_A3-6.png", width: 90%),
  caption: [`rcp-am_neutron_2016-08-12.SAF`サンプル],
)


== 標的領域と線源領域の索引ファイル

ファイル`torgans_2016-08-12.NDX`と`sregions_2016-08-12.NDX`にそれぞれ、SAFデータファイルと同じ順序で線源領域と標的領域が示されている。これらのSAFファイルには79の線源領域によって照射される43の標的領域のSAFデータが示されている。i番目の標的領域とj番目の線源領域のSAFファイルのレコードは、次式のとおりである。

ここで、43はこれらのファイルの中に記録されている標的領域の数の合計量である。例えば、脳は`torgans_2016-08-12.NDX`ファイルの18番目の標的領域であり、胃の内容物は`sregions_2016-08-12.NDX`ファイルの10番目の線源領域である。したがって、SAF（Brain←St-cont）のレコード番号は43×9+23の410番目のレコードである。`sregions_2016-08-12.NDX`ファイルのレコードには、線源領域の実質組織質量とその他の組織の線源領域の一部の領域を識別する「`ID`」フィールドがある（`ID=1`：「その他の組織の線源領域の一部」の対象、`ID=0`：対象外）。`torgans_2016-0812.NDX`ファイルには、様々な標的領域の質量がある（図@sregions_and_torgans 参照）。

#figure(
  image("images/Figure_A3-7.png", width: 90%),
  caption: [`sregions_2016-08-12.NDX`及び`torgans_2016-08-12.NDX`サンプル],
) <sregions_and_torgans>

#figure(
  caption: "標的領域の名称",
  table(
    columns: (3cm,5.5cm,7cm),
    align: (left, left, left),
    table.header([略称], [領域名], [英名]),
    [O-mucosa], [口腔粘膜], [Oral mucosa],
    [Oesophagus], [食道], [Oesophagus],
    [St-stem], [胃], [Stomach],
    [SI-stem], [小腸], [Small intestine],
    [RC-stem], [右結腸], [Right colon],
    [LC-stem], [左結腸], [Left colon],
    [RS-stem], [直腸S], [状結腸 Rectosigmoid colon],
    [ET1-bas], [胸郭外領域1（ET1）基底細胞], [Extrathoracic region1 basal cells],
    [ET2-bas], [胸郭外領域2（ET2）基底細胞], [Extrathoracic region2 basal cells],
    [LN-ET], [胸郭外のリンパ節], [Extrathoracic lymph nodes],
    [Bronch-bas], [気管支基底細胞], [Bronchi basal cells],
    [Bronch-sec], [気管支分泌細胞], [Bronchi secretory cells],
    [Bchiol-sec], [細気管支分泌細胞], [Broncholar secretory cells],
    [AI], [肺胞間質], [Alveolar-interstitial],
    [LN-Th], [胸部リンパ節], [Thoracic lymph nodes],
    [R-marrow], [赤色（活性）骨髄], [Red（active）marrow],
    [Endost-BS], [骨内膜の細胞], [Endosteal cells],
    [Brain], [脳], [Brain],
    [Eye-lens], [水晶体], [Lens of eyes],
    [P-gland], [下垂体], [Pituitary glang],
    [Tongue], [舌], [Tongue],
    [Tonsils], [扁桃腺], [Tonsils],
    [S-glands], [唾液腺], [Salivary glands],
    [Thyroid], [甲状腺], [Thyroid],
    [Breast], [乳房], [Breast],
    [Thymus], [胸腺], [Thymus],
    [Ht-wall], [心臓壁], [Heart wall],
    [Adrenals], [副腎], [Adrenals],
    [Liver], [肝臓], [Liver],
    [Pancreas], [膵臓], [Pancreas],
    [Kidneys], [腎臓], [Kidneys],
    [Spleen], [脾臓], [Spleen],
    [GB-wall], [胆嚢], [Gall bladder],
    [Ureters], [尿管], [Ureters],
    [UB-wall], [膀胱], [Urinary bladder],
    [Ovaries], [卵巣], [Ovaries],
    [Testes], [精巣], [Testes],
    [Prostate], [前立腺], [Prostate],
    [Uterus], [子宮], [Uterus],
    [LN-Sys], [全身リンパ節], [Systemic lymph nodes],
    [Skin], [皮膚], [Skin],
    [Adipose], [脂肪組織], [Adipose tissue],
    [Muscle], [筋肉], [Muscle],
  )
)

#figure(
  caption: "線源領域の名称",
  table(
    columns: (3cm,5.5cm,7cm),
    align: (left, left, left),
    table.header([略称], [領域名], [英名]),
    [O-cavity], [口腔], [Oral cavity],
    [O-mucosa], [口腔粘膜], [Oral mucosa],
    [Teeth-S], [歯表面], [Teeth surface],
    [Teeth-V], [歯体積], [Teeth volume],
    [Tongue], [舌], [Tongue],
    [Tonsils], [扁桃腺], [Tonsils],
    [Oesophag-s], [食道], [slow Oesophagus – slow],
    [Oesophag-f], [食道], [fast Oesophagus – fast],
    [Oesophag-w], [食道], [Oesophagus],
    [St-cont], [胃内容物], [Stomach contents],
    [St-mucosa], [胃粘膜], [Stomach contents],
    [St-wall], [胃壁], [Stomach wall],
    [SI-cont], [小腸内容物], [Small intestine contents],
    [SI-mucosa], [小腸粘膜], [Small intestine mucosa],
    [SI-wall], [小腸壁], [Small intestine wall],
    [SI-villi], [小腸絨毛], [Small intestine villi],
    [RC-cont], [右結腸内容物], [Right colon content],
    [RC-mucosa], [右結腸粘膜], [Right colon mucosa],
    [RC-wall], [右結腸壁], [Right colon wall],
    [LC-cont], [左結腸内容物], [Left colon content],
    [LC-mucosa], [左結腸粘膜], [Left colon mucosa],
    [LC-wall], [左結腸壁], [Left colon wall],
    [RS-cont], [直腸S], [状内容物 Rectosigmoid colon content],
    [RS-mucosa], [直腸S], [状粘膜 Rectosigmoid colon mucosa],
    [RS-wall], [直腸S], [状壁 Rectosigmoid colon wall],
    [ET1-sur], [胸郭外領域1（ET1）表面], [Extrathoracic region1 surface],
    [ET2-sur], [胸郭外領域2（ET2）表面], [Extrathoracic region2 surface],
    [ET2-bnd], [胸郭外領域2（ET2）境界領域], [Extrathoracic region2 bound region],
    [ET2-seq], [胸郭外領域2（ET2）隔離領域], [Extrathoracic region2 sequestered region],
    [LN-ET], [胸郭外のリンパ節], [Extrathoracic lymph nodes],
    [Bronchi], [気管支表面], [Bronchial surface],
    [Bronchi-b], [気管支境界領域], [Bronchial bound region],
    [Bronchi-q], [気管支隔離領域], [Bronchial sequestered region],
    [Brchiole], [細気管支表面], [Bronchiolar surface],
    [Brchiole-b], [細気管支境界領域], [Bronchiolar bound region],
    [Brchiole-q], [細気管支隔離領域], [Bronchiolar sequestered region],
    [ALV], [肺胞間質], [Alveolar-interstitial],
    [LN-Th], [胸部リンパ節], [Thoracic lymph nodes],
    [Lungs], [肺], [Lungs],
    [Adrenals], [副腎], [Adrenals],
    [Blood], [血液], [Blood],
    [C-bone-S], [皮質骨表面], [Cortical bone surface],
    [C-bone-V], [皮質骨], [Cortical bone],
    [T-bone-S], [骨梁表面], [Trabecular bone surface],
    [T-bone-V], [骨梁], [Trabecular bone],
    [C-marrow], [皮質骨髄], [Cortical bone marrow],
    [T-marrow], [骨梁骨髄], [Trabecular bone marrow],
    [R-marrow], [赤色（活性）骨髄], [Red（active）marrow],
    [Y-marrow], [黄色（非活性）骨髄], [Yellow（inactive）marrow],
    [Brain], [脳], [Brain],
    [Breast], [乳房], [Breast],
    [Eye-lens], [水晶体], [Lens of eye],
    [GB-wall], [胆嚢壁], [Gall bladder],
    [GB-cont], [胆嚢内容物], [Gall bladder content],
    [Ht-wall], [心臓壁], [Heart wall],
    [Kidneys], [腎臓], [Kidneys],
    [Liver], [肝臓], [Liver],
    [LN-Sys], [全身リンパ節], [Systemic lymph nodes],
    [Ovaries], [卵巣], [Ovaries],
    [Pancreas], [膵臓], [Pancreas],
    [P-gland], [下垂体], [Pituitary gland],
    [Prostate], [前立腺], [Prostate],
    [S-glands], [唾液腺], [Salivary glands],
    [Skin], [皮膚], [Skin],
    [Spleen], [脾臓], [Spleen],
    [Testes], [精巣], [Testes],
    [Thymus], [胸腺], [Thymus],
    [Thyroid], [甲状腺], [Thyroid],
    [Ureters], [尿管], [Ureters],
    [UB-wall], [膀胱壁], [Urinary bladder],
    [UB-cont], [膀胱内容物], [Urinary bladder content],
    [Uterus], [子宮], [Uterus],
    [Adipose], [脂肪], [Adipose],
    [Cartilage], [軟骨], [Cartilage],
    [Muscle], [筋肉], [Muscle],
    [ET1-wall], [胸郭外領域1（ET1）壁], [Extrathoracic region1 wall],
    [ET2-wall], [胸郭外領域2（ET2）壁], [Extrathoracic region2 wall],
    [Lung-Tis], [肺組織], [Lung tissue],
    [RT-air], [呼吸器の空気], [Respiratory tract air],
  )
)

#bibliography("Bibliography.yaml", style: "ieee")
