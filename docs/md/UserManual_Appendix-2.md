# 添付資料2 「等価線量・預託実効線量の計算方法」

# 1. 預託線量の計算

## 1.1 預託等価線量の計算方法

標的領域$`T`$の預託等価線量（$`𝐻_\mathrm{T}(\Delta t)`$）\[Sv]は、S–coefficient \[MeV/kg/nt]を用いて以下のように計算した。

$$H_\mathrm{T}(\Delta 𝑡) = \sum_S U_S(\Delta 𝑡) \cdot S–coefficient(𝑇 ← 𝑆) \cdot f(r_\mathrm{T},T) \cdot C$$

ここで、

|式|意味|
|:--:|---|
|$`U_S(\Delta t)`$|摂取した放射性物質が、預託期間$`\Delta t`$の間に線源領域$`S`$で壊変する総数|
|$`f(r_\mathrm{T},T)`$|標的組織の部分的な重量（ICRP Publ.133 Table2.3）|
|$`C`$|MeV/kgからGy（J/kg）への換算係数（1.60218×10<sup>-13</sup> \[J/MeV]）|

ここで使用する$`S–coefficient(𝑇 ← 𝑆)`$について、線源領域の集合$`S`$は、コンパートメントモデル図で明確にされていない「その他の組織」からの寄与を計算するための線源領域`Other`を含んでいる。$`S–coefficient(T ← Other)`$を含めたＳ係数の計算方法については[添付資料3](UserManual_Appendix-3.md) を参照。

## 1.2 預託実効線量の計算方法

預託実効線量$`E(\Delta t)`$ \[Sv]は、預託等価線量$`H_\mathrm{T}(\Delta t)`$を用いて以下のように計算した。

$$ E(\Delta t) = \sum_T H_\mathrm{T}(\Delta t) \cdot w_\mathrm{T} $$

ここで、

|式|意味|
|:--:|---|
|$`w_\mathrm{T}`$|組織加重係数 \[-]（ICRP Publ.103 Table3データを使用 ※2）|

※2 Remainder tissuesについて、内訳は男女いずれも13個の標的組織となっている。そのため与えられた組織加重係数 $`w_\mathrm{T}`$＝0.12を13等分した0.12/13≒0.00923を、Remainder tissuesに含まれる標的領域毎の実際の組織加重係数として使用する。

ICRP Publ.133 Table2.2に含まれない標的領域については、組織加重係数＝0とし、線量計算に影響ないものとして扱った。

