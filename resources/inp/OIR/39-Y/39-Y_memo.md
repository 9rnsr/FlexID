# 体内動態モデル間のコンパートメント対応関係

ICRP Publ.134 p.254 Para.529
> (529) Chain members addressed in the derivation of dose coefficients for internally
> deposited yttrium isotopes include isotopes of yttrium, strontium, zirconium, and
> niobium. 
> (529) 体内に取り込まれたイットリウム同位体の線量係数の導出において対象とされる崩壊系列の
> 核種には、イットリウム、ストロンチウム、ジルコニウム、およびニオブの同位体が含まれます。

39-Yを親とする場合、その系列は2つに大別できる。

1つ目は{39-Y, 38-Sr}。

|39-Y     |38-Sr         |
|---------|--------------|
|Blood1   |Blood         |
|Blood2   |              |
|C-bone-S |C-bone-S      |
|T-bone-S |T-bone-S      |
|C-bone-V |              |
|T-bone-V |              |
|         |Exch-C-bone-V |
|         |Exch-T-bone-V |
|         |Noch-C-bone-V |
|         |Noch-T-bone-V |
|Liver0   |              |
|Liver1   |              |
|         |Liver   *1    |
|Kidneys  |Kidneys *1    |

*1 子孫核種の場合のみ追加される

線源領域Otherは、異なる元素のモデル間では同一と識別できないものとして扱う。

|39-Y     |38-Sr         |
|---------|--------------|
|ST0      |ST0           |
|ST1      |ST1           |
|         |ST2           |


2つ目は{39-Y, 40-Zr, 41-Nb}。

|39-Y     |40-Zr    |41-Nb    |
|---------|---------|---------|
|Blood1   |Blood1   |Blood1   |
|Blood2   |Blood2   |Blood2   |
|C-bone-S |C-bone-S |C-bone-S |
|T-bone-S |T-bone-S |T-bone-S |
|C-bone-V |C-bone-V |C-bone-V |
|T-bone-V |T-bone-V |T-bone-V |
|         |         |         |
|         |         |         |
|         |         |         |
|         |         |         |
|Liver0   |Liver0   |Liver0   |
|Liver1   |Liver1   |Liver1   |
|         |         |         |
|Kidneys  |Kidneys  |Kidneys  |
|ST0      |ST0      |ST0      |
|ST1      |ST1      |ST1      |

線源領域Otherは、一般的には異なる元素のモデル間では同一と識別できないものとして扱うが、ここでは同一と識別できるものとして扱う。

# 翻訳メモ

ICRP Publ.134 p.254 Para.529
> (529) Chain members addressed in the derivation of dose coefficients for internally
> deposited yttrium isotopes include isotopes of yttrium, strontium, zirconium, and
> niobium. An yttrium isotope produced in the body after uptake of an yttrium parent
> is assumed to have the same systemic biokinetics as the parent. Isotopes of zirconium
> and niobium produced in systemic compartments after intake of an yttrium parent
> are assigned the characteristic systemic models for zirconium and niobium, respectively,
> described elsewhere in this publication. The characteristic systemic models for
> yttrium, zirconium, and niobium all have the same model structure. A zirconium or
> niobium atom produced in a compartment by radioactive decay is assumed to
> behave as if it had entered that compartment as a parent radionuclide. This includes
> subcompartments of ‘other soft tissues’.
> ICRP Publ.134 p.254 Para.529
> (529) 体内に取り込まれたイットリウム同位体の線量係数の導出において対象とされる崩壊系列の
> 核種には、イットリウム、ストロンチウム、ジルコニウム、およびニオブの同位体が含まれます。
> イットリウム親核種の摂取後に体内で生成されるイットリウム同位体は、その親核種と同じ全身の
> 生体内動態を示すものと仮定されます。イットリウム親核種の摂取後に全身コンパートメント内で
> 生成されるジルコニウムおよびニオブの同位体には、本報告書の他の箇所で記述されている、
> それぞれジルコニウムおよびニオブに固有の全身モデルが適用されます。イットリウム、
> ジルコニウム、およびニオブに固有の全身モデルは、いずれも同じモデル構造を有しています。
> 放射性崩壊によってコンパートメント内で生成されたジルコニウムまたはニオブの原子は、
> 親放射性核種としてそのコンパートメントに流入した場合と同様に挙動すると仮定されます。
> これには、「その他の軟部組織」のサブコンパートメントも含まれます。

ICRP Publ.134 p.254 Para.530
> (530) The model for strontium produced in systemic compartments after intake of
> an yttrium parent is an extension of the characteristic model for strontium described
> elsewhere in this publication. That model is extended for application to strontium as
> a progeny of yttrium by adding individual compartments representing liver and
> kidneys, which are represented explicitly in the model for yttrium. Each of these
> compartments is assumed to exchange strontium with blood. Parameter values
> describing rates of uptake and removal of strontium by liver and kidneys are set
> for reasonable agreement with postmortem measurements from human subjects
> injected with 85Sr during late stages of various terminal illnesses (Schulert et al.,
> 1959). The transfer coefficients from blood to liver and kidneys are both set at
> 0.05 d⁻¹. The transfer coefficient from blood to the intermediate-term soft tissue
> compartment in the characteristic model for strontium is reduced from 1.5 d⁻¹ to
> 1.4 d⁻¹ to leave the total outflow rate from blood unchanged. The removal half-times
> from liver and kidneys to blood are set at 6 d and 2 d, respectively.
> (530) イットリウム親化合物の摂取後に全身コンパートメントで生成されるストロンチウムのモデルは、
> 本書の他の箇所で説明されているストロンチウムの特性モデルの拡張です。このモデルは、
> イットリウムのモデルで明示的に表現されている肝臓と腎臓を表す個々のコンパートメントを
> 追加することにより、イットリウムの子孫としてのストロンチウムへの適用のために拡張されています。
> これらのコンパートメントはそれぞれ、血液とストロンチウムを交換すると仮定されます。
> 肝臓と腎臓によるストロンチウムの吸収および除去速度を表すパラメータ値は、様々な末期疾患の
> 後期に85Srを注入された被験者の死後測定値と妥当に一致するように設定されています（Schulert et al., 1959）。
> 血液から肝臓および腎臓への移行係数は、いずれも0.05 d⁻¹に設定されています。ストロンチウムの
> 特性モデルにおける血液から中間期軟部組織コンパートメントへの移行係数は、血液からの総流出速度を
> 変えずに、1.5 d⁻¹から1.4 d⁻¹に減少します。肝臓および腎臓から血液への除去半減期は、
> それぞれ6日および2日と設定されます。

ICRP Publ.134 p.255 Para.531
> (531) The blood compartment of the strontium model (named Blood) is identified
> with the compartment Blood 1 of the yttrium model (Fig. 11.1). Thus, strontium
> produced in Blood 1 by decay of yttrium is assumed to be produced in Blood in the
> strontium model. Strontium produced by radioactive decay in compartments of the
> yttrium model that are not identifiable with compartments of the strontium model is
> treated as follows. Strontium produced in Blood 2 of the yttrium model is assumed to
> transfer to Blood in the strontium model at a rate of 1000 d⁻¹ (t½1 min).
> Strontium produced in either of the two liver compartments of the yttrium model
> is assumed to transfer to Blood in the strontium model with a half-time of 6 d, which
> is the removal half-time of strontium from the liver in the strontium model described
> above. Strontium produced in either of the two compartments of other soft tissues in
> the yttrium model is assumed to transfer to Blood in the strontium model at a rate of
> 2.5 d⁻¹, which is the shortest removal half-time from the soft tissue compartments in
> the characteristic model for strontium. Strontium reaching Blood in the strontium
> model subsequently follows the model for strontium described above. The single
> kidney compartment in the model for strontium as a progeny of yttrium is identified
> with the single kidney compartment in the model for yttrium. Strontium produced in
> that compartment by decay of yttrium is assumed to behave as if entering the compartment
> as a parent radionuclide.
> (531) ストロンチウムモデルの血液コンパートメント（血液）は、イットリウムモデルの
> 血液1コンパートメントと同一視されます（図11.1）。したがって、イットリウムの崩壊によって
> 血液1で生成されたストロンチウムは、ストロンチウムモデルの血液でも生成されると仮定されます。
> イットリウムモデルのコンパートメントと同一視できないコンパートメントにおける放射性崩壊によって
> 生成されたストロンチウムは、以下のように扱われます。イットリウムモデルの血液2で生成された
> ストロンチウムは、ストロンチウムモデルの血液に1000 d⁻¹（t½～1分）の速度で移行すると仮定されます。
> イットリウムモデルの肝臓の2つのコンパートメントのいずれかで生成されたストロンチウムは、
> ストロンチウムモデルにおいて半減期6日で血液に移行すると仮定されます。これは、上記の
> ストロンチウムモデルにおける肝臓からのストロンチウムの半減期です。イットリウムモデルの
> 他の軟部組織の2つのコンパートメントのいずれかで生成されたストロンチウムは、
> ストロンチウムモデルにおいて2.5 d⁻¹の速度で血液に移行すると仮定されます。これは、
> ストロンチウムの特性モデルにおける軟部組織コンパートメントからの最短の半減期です。
> ストロンチウムモデルにおいて血液に到達するストロンチウムは、その後、上記の
> ストロンチウムのモデルに従います。イットリウムの子孫としてのストロンチウムのモデルにおける
> 単一の腎臓コンパートメントは、イットリウムのモデルにおける単一の腎臓コンパートメントと
> 同一です。イットリウムの崩壊によってその区画内で生成されたストロンチウムは、
> 親放射性核種としてその区画内に入るかのように振舞うと想定されます。
