#import "common.typ": *
#show: style

//#set enum(numbering: "①")

#outline()
#pagebreak()

#maketitle(
  title: "添付資料2 「等価線量・預託実効線量の計算方法」",
)

= 預託線量の計算

== 預託等価線量の計算方法

標的領域$"T"$の預託等価線量（$H_("T")(delta t)$）[Sv]は、$"S–coefficient"$ [MeV/kg/nt]を用いて以下のように計算する。

$ H_("T")(Delta t) = sum_"S" U_("S")(Delta t) dot "S–coefficient"("T" ← "S") dot f(r_"T", "T") dot C $

ここで、

  #figure(
    table(
      columns: 2,
      align: (center, left),
      table.header([式], [意味]),
      [$U_("S")(Delta t)$], [摂取した放射性物質が、預託期間$Delta t$の間に線源領域$"S"$で壊変する総数],
      [$f(r_"T", "T")$],    [標的組織の部分的な重量（ICRP Publ.133 Table2.3）],
      [$C$],                [MeV/kgからGy(J/kg)への換算係数（$1.60218×10^(-13)$ [J/MeV]）],
    )
  )

ここで使用する$"S–coefficient"("T" ← "S")$について、線源領域の集合$"S"$は、コンパートメントモデル図で明確にされていない「その他の組織」からの寄与を計算するための線源領域$"Other"$を含んでいる。$"S–coefficient"("T" ← "Other")$を含めたS係数の計算方法については#link("UserManual_Appendix-3.typ")[添付資料3] を参照。


== 預託実効線量の計算方法

預託実効線量$E(Delta t)$ [Sv]は、預託等価線量$H_("T")(Delta t)$を用いて以下のように計算する。

$ E(Delta t) = sum_"T" H_("T")(Delta t) dot w_"T" $

ここで、

  #figure(
    table(
      columns: 2,
      align: (center, left),
      table.header([式], [意味]),
      [$w_"T"$], [組織加重係数 [-]（ICRP Publ.103 Table 3 のデータを使用 ※2）]
    ),
  )

    / ※2 : Remainder tissuesについて、内訳は男女いずれも13個の標的領域となっている。そのため与えられた組織加重係数 $w_"T"$＝0.12を
            13等分した0.12/13≒0.00923を、Remainder tissuesに含まれる標的領域毎の実際の組織加重係数として使用する。 \
            ICRP Publ.133 Table 2.2に含まれない標的領域については、組織加重係数 $w_"T"$＝0とし、線量計算に影響ないものとして扱った。
