# 体内動態モデル間のコンパートメント対応関係

> ICRP Publ.137 p.369 Para.811
> (811) The progeny of thorium isotopes addressed in the derivation of dose coefficients
> are isotopes of actinium, thorium, protactinium, uranium, radium, radon,
> polonium, lead, bismuth, thallium, francium, and astatine.

全身モデル間で同一と扱うコンパートメント。

|92-U          |91-Pa     |90-Th     |89-Ac    |88-Ra         |87-Fr |86-Rn  |85-At |84-Po    |83-Bi    |82-Pb         |81-Tl    |
|--------------|----------|----------|---------|--------------|------|-------|------|---------|---------|--------------|---------|
|              |Blood     |Blood     |Blood    |Blood         |      |Blood  |      |         |         |              |         |
|              |          |          |         |              |      |       |      |Plasma1  |         |              |         |
|              |          |          |         |              |      |       |      |Plasma2  |         |              |         |
|              |          |          |         |              |      |       |      |Plasma3  |         |              |         |
|Plasma        |          |          |         |              |      |       |      |         |Plasma   |Plasma        |Plasma   |
|RBC           |          |          |         |              |      |       |      |RBC      |RBC      |RBC           |RBC      |
|C-bone-S      |C-bone-S  |C-bone-S  |C-bone-S |C-bone-S      |      |       |      |C-bone-S |C-bone-S |C-bone-S      |C-bone-S |
|T-bone-S      |T-bone-S  |T-bone-S  |T-bone-S |T-bone-S      |      |       |      |T-bone-S |T-bone-S |T-bone-S      |T-bone-S |
|Exch-C-bone-V |          |          |         |Exch-C-bone-V |      |       |      |         |         |Exch-C-bone-V |         |
|Exch-T-bone-V |          |          |         |Exch-T-bone-V |      |       |      |         |         |Exch-T-bone-V |         |
|Noch-C-bone-V |C-bone-V  |C-bone-V  |C-bone-V |Noch-C-bone-V |      |       |      |         |         |Noch-C-bone-V |         |
|Noch-T-bone-V |T-bone-V  |T-bone-V  |T-bone-V |Noch-T-bone-V |      |       |      |         |         |Noch-T-bone-V |         |
|C-marrow      |C-marrow  |C-marrow  |C-marrow |C-marrow      |      |       |      |C-marrow |C-marrow |C-marrow      |C-marrow |
|T-marrow      |T-marrow  |T-marrow  |T-marrow |T-marrow      |      |       |      |T-marrow |T-marrow |T-marrow      |T-marrow |
|Liver1        |Liver1    |Liver1    |Liver1   |Liver1        |      |       |      |Liver1   |Liver1   |Liver1        |Liver    |
|Liver2        |Liver2    |Liver2    |Liver2   |Liver2        |      |       |      |Liver2   |Liver2   |Liver2        |         |
|Kidneys1      |Kidneys1  |Kidneys1  |Kidneys1 |Kidneys1      |      |       |      |Kidneys1 |Kidneys1 |Kidneys1      |         |
|Kidneys2      |Kidneys2  |Kidneys2  |Kidneys2 |Kidneys2      |      |       |      |Kidneys2 |Kidneys2 |Kidneys2      |Kidneys  |
|Testes        |Testes    |Testes    |Testes   |Testes        |      |       |      |Testes   |Testes   |Testes        |Testes   |
|Ovaries       |Ovaries   |Ovaries   |Ovaries  |Ovaries       |      |       |      |Ovaries  |Ovaries  |Ovaries       |Ovaries  |
|Skin          |Skin      |Skin   *1 |Skin     |Skin          |      |       |      |Skin     |Skin     |Skin          |Skin     |
|Spleen        |Spleen    |Spleen *1 |Spleen   |Spleen        |      |       |      |Spleen   |Spleen   |Spleen        |Spleen   |

*1 子孫核種の場合のみ追加される

軟組織は異なる元素の全身モデル間では同一と識別できないものとして扱う。

|92-U          |91-Pa     |90-Th     |89-Ac    |88-Ra         |87-Fr |86-Rn  |85-At |84-Po    |83-Bi    |82-Pb         |81-Tl    |
|--------------|----------|----------|---------|--------------|------|-------|------|---------|---------|--------------|---------|
|ST0           |ST0       |ST0       |ST0      |ST0           |      |       |      |Other    |ST0      |ST0           |Other    |
|ST1           |ST1       |ST1       |ST1      |ST1           |      |       |      |         |ST1      |ST1           |         |
|ST2           |ST2       |ST2       |ST2      |ST2           |      |       |      |         |ST2      |ST2           |         |


# 翻訳メモ

ICRP Publ.137 p.369 Para.811
ICRP Publ.137 p.370 Para.812

```
14.2.3.3. Treatment of radioactive progeny

(811) The progeny of thorium isotopes addressed in the derivation of dose coefficients
are isotopes of actinium, thorium, protactinium, uranium, radium, radon,
polonium, lead, bismuth, thallium, francium, and astatine. The model for uranium
produced by serial decay of members of a uranium chain is a modification of the
model for uranium as a parent radionuclide (see Section 15). Single compartments
representing spleen, trabecular marrow, cortical marrow, testes, ovaries, and skin are
added for consistency with the models for other progeny of thorium. The six added
compartments are taken from the intermediate-term soft tissue compartment ST1 in
the model for uranium as a parent. Deposition of uranium as a progeny in spleen,
trabecular marrow + cortical marrow (i.e. total combined marrow), testes, ovaries,
or skin is calculated as its mass fraction of other soft tissues times the deposition
fraction for ST1 (0.0665 of uranium ‘leaving the circulation’, as defined in Section
15). Deposition in trabecular marrow is assumed to be three times greater than
deposition in cortical marrow. The derived transfer cofficients from blood to
spleen, trabecular marrow, cortical marrow, testes, ovaries, and skin are 0.004 d-¹,
0.075 d-¹, 0.025 d-¹, 0.001 d-¹, 0.0003 d-¹, and 0.09 d-¹, respectively. The removal
half-time from each added compartment is set at 20 d, the removal half-time from
ST1. The deposition fraction for ST1 is reduced by the sum of deposition fractions
for the six added compartments. Uranium produced in a soft tissue compartment
that is not identifiable with a compartment in the characteristic model for uranium is
assumed to transfer to plasma at the rate 8.32 d-¹, the highest rate of loss from any
compartment of other soft tissue in the characteristic model for uranium. Uranium
produced in a bone volume compartment that is ambiguous with regard to the
uranium model (e.g. trabecular or cortical bone volume in the thorium model) is
assumed to be produced in non-exchangeable bone.
(811) 線量係数の導出において考慮されるトリウム同位体の娘核種は、アクチニウム、トリウム、
プロトアクチニウム、ウラン、ラジウム、ラドン、ポロニウム、鉛、ビスマス、タリウム、
フランシウム、およびアスタチンの同位体である。ウラン系列の核種の逐次崩壊によって
生成されるウランのモデルは、親放射性核種としてのウランのモデルを修正したものである
（セクション15を参照）。トリウムの他の娘核種のモデルとの整合性をとるため、脾臓、海綿骨髄、
皮質骨髄、精巣、卵巣、および皮膚を表す単一のコンパートメントが追加されている。
これら6つの追加コンパートメントは、親核種としてのウランのモデルにおける中期的軟部組織
コンパートメントST1から採用されたものである。脾臓、海綿骨髄＋皮質骨髄（すなわち、合わせた全骨髄）、
精巣、卵巣、または皮膚における娘核種としてのウランの沈着量は、ST1の沈着割合（セクション15で
定義される「循環から離脱する」ウランの0.0665）に、他の軟部組織に対する当該組織の質量分率を
乗じることによって算出される。海綿骨髄における沈着量は、皮質骨髄における沈着量の3倍であると
仮定される。血液から脾臓、海綿骨髄、皮質骨髄、精巣、卵巣、および皮膚への移行係数は、それぞれ
0.004 d⁻¹、0.075 d⁻¹、0.025 d⁻¹、0.001 d⁻¹、0.0003 d⁻¹、および0.09 d⁻¹と導出される。
各追加コンパートメントからの除去半減期は、ST1からの除去半減期である20日と設定される。
ST1の沈着割合は、これら6つの追加コンパートメントの沈着割合の合計分だけ低減される。
ウランの特性モデルにおけるコンパートメントと同一視できない軟部組織コンパートメント内で
生成されたウランは、8.32 d⁻¹の速度で血漿へ移行すると仮定される。この速度は、ウランの
特性モデルにおける他の軟部組織のどのコンパートメントからの消失速度よりも高いものである。
ウランのモデルに関してその帰属が不明確な骨体積区画（例：トリウムモデルにおける海綿骨または
皮質骨の体積）で生成されるウランは、非交換性骨で生成されるものと仮定される。

(812) The models for actinium, thorium, radium, radon, polonium, lead, bismuth,
thallium, francium, and astatine produced systemically by serial decay of members of
a thorium chain are the same as the models applied to these elements as progeny of
radium (see Section 13). The model for thorium as a radium progeny is applied to
protactinium as a thorium progeny based on the similar systemic behaviours of
protactinium and thorium in rats (Lanz et al., 1946; Schuppler et al., 1988;
Durbin, 2011). A radionuclide produced in a bone volume compartment that is
ambiguous with regard to the model for that element (e.g. radium produced in trabecular
or cortical bone volume in the thorium model) is assumed to be produced in
non-exchangeable bone.
(812) トリウム系列の核種の逐次崩壊によって体内で生成されるアクチニウム、トリウム、ラジウム、
ラドン、ポロニウム、鉛、ビスマス、タリウム、フランシウム、およびアスタチンのモデルは、
ラジウムの娘核種としてこれらの元素に適用されるモデルと同じである（セクション13を参照）。
ラジウムの娘核種としてのトリウムのモデルは、ラットにおけるプロトアクチニウムとトリウムの
体内挙動が類似していることに基づき（Lanz et al., 1946; Schuppler et al., 1988; Durbin, 2011）、
トリウムの娘核種であるプロトアクチニウムにも適用される。当該元素のモデルに関してその帰属が
不明確な骨体積区画で生成される放射性核種（例：トリウムモデルにおいて海綿骨または皮質骨の
体積で生成されるラジウム）は、非交換性骨で生成されるものと仮定される。
```
