# 添付資料2 「等価線量・預託実効線量の計算方法」

# 1. S–coefficientの計算

## 1.1 計算方法

ICRP Publ.133において公開された最新の比吸収割合（SAF）データから、以下の計算方法によりS–coefficientを算出した。

$$ S–coefficient(T ← S) = \sum_i ( Y_i * E_i * SAF(T ← S)\_i * w\_{R_i} )\ \mathrm{[MeV/kg/nt]} $$

ここで、

|式|意味|
|:--:|---|
|$`S–coefficient(T ← S)`$|ある核種がある線源臓器で1壊変したときの標的臓器1kg当たりに吸収されるエネルギー \[MeV/kg/nt]|
|$`𝑌_i`$|壊変あたりの放射線$`i`$の放出割合 \[/nt] （ICRP Publ.107データを使用）|
|$`𝐸_i`$|放射線$`i`$の平均または単一エネルギー \[MeV] （ICRP Publ.107データを使用）|
|$`𝑆AF(𝑇 ← 𝑆)_i`$|線源領域$`S`$𝑆内で放出されるエネルギー$`E_i`$の放射線から、標的領域$`𝑇`$`の単位質量あたりに吸収されるエネルギーの割合（比吸収割合） \[/kg] （ICRP Publ.133データを使用）|
|$`w_{R_i}`$|放射線iの放射線加重係数 \[-]（ICRP Publ.103 Table2データを使用）|

なお、FlexIDで使用するS–coefficientデータの作成方法は、[添付資料3](UserManual_Appendix-3.md)を参照。

# 2. 預託線量の計算

## 2.1 預託等価線量の計算方法

標的領域$`T`$の預託等価線量（$`𝐻_\mathrm{T}(\Delta t)`$）\[Sv]は、S–coefficient \[MeV/kg/nt]を用いて以下のように計算した。

$$H_\mathrm{T}(\Delta 𝑡) = \sum_S U_S(\Delta 𝑡) \cdot S–coefficient(𝑇 ← 𝑆) \cdot f(r_\mathrm{T},T) \cdot C$$

ここで、

|式|意味|
|:--:|---|
|$`U_S(\Delta t)`$|摂取した放射性物質が、預託期間$`\Delta t`$の間に線源領域$`S`$で壊変する総数|
|$`f(r_\mathrm{T},T)`$|標的組織の部分的な重量（ICRP Publ.133 Table2.3）|
|$`C`$|MeV/kgからGy（J/kg）への換算係数（1.60218×10<sup>-13</sup> \[J/MeV]）|

この時、コンパートメントモデル図で明確にされていない線源領域（その他線源領域）のうち、骨を除く（※1）軟組織を合計した$`S–coefficient(T ← Other)`$も計算対象とする。

$$ S–coefficient(T ← Other) = \frac{1}{M_{Other}} \cdot \sum_S M_\mathrm{S} \cdot S–coefficient(T ← S) $$

ここで、

|式|意味|
|:--:|---|
|$`M_{Other}`$|コンパートメントモデルで明確にされていない軟組織の合計質量（ICRP Publ.133 TableA3.3 ※1）|
|$`M_\mathrm{S}`$|コンパートメントモデル図で明確にされていない骨を除く個々の線源領域の質量|

※1 $`M_\mathrm{S}`$は「骨を除く」コンパートメントなので、以下のコンパートメントは$`M_\mathrm{S}`$から常に除外する。

- C-bone-S
- C-bone-V
- T-bone-S
- T-bone-V

## 2.2 預託実効線量の計算方法

預託実効線量$`E(\Delta t)`$ \[Sv]は、預託等価線量$`H_\mathrm{T}(\Delta t)`$を用いて以下のように計算した。

$$ E(\Delta t) = \sum_T H_\mathrm{T}(\Delta t) \cdot w_\mathrm{T} $$

ここで、

|式|意味|
|:--:|---|
|$`w_\mathrm{T}`$|組織加重係数 \[-]（ICRP Publ.103 Table3データを使用 ※2）|

※2 Remainder tissuesについて、内訳は男女いずれも13個の標的組織となっている。そのため与えられた組織加重係数 $`w_\mathrm{T}`$＝0.12を13等分した0.12/13≒0.00923を、Remainder tissuesに含まれる標的領域毎の実際の組織加重係数として使用する。

ICRP Publ.133 Table2.2に含まれない標的領域については、組織加重係数＝0とし、線量計算に影響ないものとして扱った。

