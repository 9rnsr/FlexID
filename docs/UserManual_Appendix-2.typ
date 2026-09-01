#import "@preview/js:0.1.3": *

#show: js.with(
  lang: "ja",
  sansfont: "Harano Aji Gothic",//sansfont: "Microsoft Sans Serif", // or "Helvetica", "Source Sans Pro", "Arial", ...
  seriffont-cjk: "Harano Aji Mincho",
  sansfont-cjk: "Harano Aji Gothic",
)

// ページの余白を設定
#set page(margin: (x: 20mm, y: 20mm))

// 図の下に空行を挿入する
#show figure: it => {
  if it.kind == image { [ #it #v(1em) ] } else { [ #it #v(1em) ] }
}

// ラベルから「番号 タイトル」のリンクを作成する
#let chapref(label) = context {
  let h = query(label).first()
  let num = numbering(
    h.numbering,
    ..counter(heading).at(h.location())
  )
  link(h.location())[ 「#num #h.body」 ]
}

//#set enum(numbering: "①")

//#show enum.item: it => {
//  if it.body.has("children") { it } else { it }
//}

#maketitle(
  title: "添付資料2 「等価線量・預託実効線量の計算方法」",
  authors: "HARA, Kenji",
  //abstract: [
  //  内部被ばく線量評価コードFlexID (Flexible code for Internal Dosimetry)の
  //  ユーザーマニュアル
  //],
)

#outline()
#pagebreak()

= 預託線量の計算

== 預託等価線量の計算方法

標的領域$T$の預託等価線量（$H_("T")(delta t)$）\[Sv\]は、S–coefficient \[MeV/kg/nt\]を用いて以下のように計算した。

$ H_("T")(Delta t) = sum_"S" U_("S")(Delta t) dot "S–coefficient"("T" ← "S") dot f(r_"T", "T") dot C $

ここで、

  #figure(
    table(
      columns: 2,
      align: (center, left),
      table.header([式], [意味]),
      [$U_("S")(Delta t)$], [摂取した放射性物質が、預託期間$Delta t$の間に線源領域$"S"$で壊変する総数],
      [$f(r_"T", "T")$],    [標的組織の部分的な重量（ICRP Publ.133 Table2.3）],
      [$C$],                [MeV/kgからGy(J/kg)への換算係数（$1.60218×10^(-13)$ \[J/MeV\]）],
    )
  )

ここで使用する$"S–coefficient"("T" ← "S")$について、線源領域の集合$"S"$は、コンパートメントモデル図で明確にされていない「その他の組織」からの寄与を計算するための線源領域$"Other"$を含んでいる。$"S–coefficient"("T" ← "Other")$を含めたS係数の計算方法については#link("UserManual_Appendix-3.md")[添付資料3] を参照。


== 預託実効線量の計算方法

預託実効線量$E(Delta t)$ \[Sv\]は、預託等価線量$H_("T")(Delta t)$を用いて以下のように計算した。

$ E(Delta t) = sum_"T" H_("T")(Delta t) dot w_"T" $

ここで、

  #figure(
    table(
      columns: 2,
      align: (center, left),
      table.header([式], [意味]),
      [$w_"T"$], [組織加重係数 \[-\]（ICRP Publ.103 Table 3 のデータを使用 ※2）]
    ),
  )

    / ※2 : Remainder tissuesについて、内訳は男女いずれも13個の標的組織となっている。そのため与えられた組織加重係数 $w_"T"$＝0.12を
            13等分した0.12/13≒0.00923を、Remainder tissuesに含まれる標的領域毎の実際の組織加重係数として使用する。 \
            ICRP Publ.133 Table2.2に含まれない標的領域については、組織加重係数 $w_"T"$＝0とし、線量計算に影響ないものとして扱った。
