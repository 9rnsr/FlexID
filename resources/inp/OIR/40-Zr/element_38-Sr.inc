# ICRP Publ.134 p.275 Para.577
# > (577) ... The model for strontium
# > produced in systemic compartments after intake of a zirconium parent is the same as
# > the model for strontium produced after intake of an yttrium parent, as described in
# > the section on yttrium (Section 11.2.3.).
# > (577) ... ジルコニウム親核種の摂取後に全身コンパートメント内で生成されるストロンチウムのモデルは、
# > イットリウムの項（11.2.3項）で説明したイットリウム親核種の摂取後に生成される
# > ストロンチウムのモデルと同じです。

# ICRP Publ.134 p.254 Para.530
# > (530) The model for strontium produced in systemic compartments after intake of
# > an yttrium parent is an extension of the characteristic model for strontium described
# > elsewhere in this publication. That model is extended for application to strontium as
# > a progeny of yttrium by adding individual compartments representing liver and
# > kidneys, which are represented explicitly in the model for yttrium. ...
# > (530) イットリウム親化合物の摂取後に全身コンパートメントで生成されるストロンチウムのモデルは、
# > 本書の他の箇所で説明されているストロンチウムの特性モデルの拡張です。このモデルは、
# > イットリウムのモデルで明示的に表現されている肝臓と腎臓を表す個々のコンパートメントを
# > 追加することにより、イットリウムの子孫としてのストロンチウムへの適用のために拡張されています。 ...

[{Sr}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Noch-T-bone-V         T-bone-V
$if progeny
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
$endif
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Sr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.220 Table 10.3
  Blood                   UB-con                    1.73
  Blood                   RC-con                    0.525
  Blood                   T-bone-S                  2.08
  Blood                   C-bone-S                  1.67
  Blood                   ST0                       7.5
$if parent
  Blood                   ST1                       1.5
$endif
  Blood                   ST2                       0.003
  T-bone-S                Blood                     0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Blood                     0.578
  C-bone-S                Exch-C-bone-V             0.116
  ST0                     Blood                     2.50
  ST1                     Blood                     0.116
  ST2                     Blood                     0.00038
  Exch-T-bone-V           T-bone-S                  0.0043
  Exch-T-bone-V           Noch-T-bone-V             0.0043
  Exch-C-bone-V           C-bone-S                  0.0043
  Exch-C-bone-V           Noch-C-bone-V             0.0043
  Noch-C-bone-V           Blood                     0.0000821
  Noch-T-bone-V           Blood                     0.000493

$if progeny

# ICRP Publ.134 p.254 Para.530
# > (530) ... Each of these
# > compartments is assumed to exchange strontium with blood. Parameter values
# > describing rates of uptake and removal of strontium by liver and kidneys are set
# > for reasonable agreement with postmortem measurements from human subjects
# > injected with 85Sr during late stages of various terminal illnesses (Schulert et al.,
# > 1959). The transfer coefficients from blood to liver and kidneys are both set at
# > 0.05 d⁻¹. The transfer coefficient from blood to the intermediate-term soft tissue
# > compartment in the characteristic model for strontium is reduced from 1.5 d⁻¹ to
# > 1.4 d⁻¹ to leave the total outflow rate from blood unchanged. The removal half-times
# > from liver and kidneys to blood are set at 6 d and 2 d, respectively.
# > (530) ... これらのコンパートメントはそれぞれ、血液とストロンチウムを交換すると仮定されます。
# > 肝臓と腎臓によるストロンチウムの吸収および除去速度を表すパラメータ値は、様々な末期疾患の
# > 後期に85Srを注入された被験者の死後測定値と妥当に一致するように設定されています（Schulert et al., 1959）。
# > 血液から肝臓および腎臓への移行係数は、いずれも0.05 d⁻¹に設定されています。ストロンチウムの
# > 特性モデルにおける血液から中間期軟部組織コンパートメントへの移行係数は、血液からの総流出速度を
# > 変えずに、1.5 d⁻¹から1.4 d⁻¹に減少します。肝臓および腎臓から血液への除去半減期は、
# > それぞれ6日および2日と設定されます。

  Blood                   Liver                     0.05
  Blood                   Kidneys                   0.05
  Blood                   ST1                       1.4     # fixed

  Liver                   Blood                 $(0.693 / 6)
  Kidneys                 Blood                 $(0.693 / 2)

# ICRP Publ.134 p.254 Para.531
# > (531) The blood compartment of the strontium model (named Blood) is identified
# > with the compartment Blood 1 of the yttrium model (Fig. 11.1). Thus, strontium
# > produced in Blood 1 by decay of yttrium is assumed to be produced in Blood in the
# > strontium model. Strontium produced by radioactive decay in compartments of the
# > yttrium model that are not identifiable with compartments of the strontium model is
# > treated as follows. Strontium produced in Blood 2 of the yttrium model is assumed to
# > transfer to Blood in the strontium model at a rate of 1000 d⁻¹ (t½～1 min).
# > Strontium produced in either of the two liver compartments of the yttrium model
# > is assumed to transfer to Blood in the strontium model with a half-time of 6 d, which
# > is the removal half-time of strontium from the liver in the strontium model described
# > above. Strontium produced in either of the two compartments of other soft tissues in
# > the yttrium model is assumed to transfer to Blood in the strontium model at a rate of
# > 2.5 d⁻¹, which is the shortest removal half-time from the soft tissue compartments in
# > the characteristic model for strontium. Strontium reaching Blood in the strontium
# > model subsequently follows the model for strontium described above. The single
# > kidney compartment in the model for strontium as a progeny of yttrium is identified
# > with the single kidney compartment in the model for yttrium. Strontium produced in
# > that compartment by decay of yttrium is assumed to behave as if entering the compartment
# > as a parent radionuclide.
# > (531) ストロンチウムモデルの血液コンパートメント（血液）は、イットリウムモデルの
# > 血液1コンパートメントと同一視されます（図11.1）。したがって、イットリウムの崩壊によって
# > 血液1で生成されたストロンチウムは、ストロンチウムモデルの血液でも生成されると仮定されます。
# > イットリウムモデルのコンパートメントと同一視できないコンパートメントにおける放射性崩壊によって
# > 生成されたストロンチウムは、以下のように扱われます。イットリウムモデルの血液2で生成された
# > ストロンチウムは、ストロンチウムモデルの血液に1000 d⁻¹（t½～1分）の速度で移行すると仮定されます。
# > イットリウムモデルの肝臓の2つのコンパートメントのいずれかで生成されたストロンチウムは、
# > ストロンチウムモデルにおいて半減期6日で血液に移行すると仮定されます。これは、上記の
# > ストロンチウムモデルにおける肝臓からのストロンチウムの半減期です。イットリウムモデルの
# > 他の軟部組織の2つのコンパートメントのいずれかで生成されたストロンチウムは、
# > ストロンチウムモデルにおいて2.5 d⁻¹の速度で血液に移行すると仮定されます。これは、
# > ストロンチウムの特性モデルにおける軟部組織コンパートメントからの最短の半減期です。
# > ストロンチウムモデルにおいて血液に到達するストロンチウムは、その後、上記の
# > ストロンチウムのモデルに従います。イットリウムの子孫としてのストロンチウムのモデルにおける
# > 単一の腎臓コンパートメントは、イットリウムのモデルにおける単一の腎臓コンパートメントと
# > 同一です。イットリウムの崩壊によってその区画内で生成されたストロンチウムは、
# > 親放射性核種としてその区画内に入るかのように振舞うと想定されます。

  {Zr}/Blood1             Blood                     ---
  {Zr}/Blood2             Blood                  1000

  {Zr}/C-bone-S           C-bone-S                  ---             # 明示されていないため、同一区画と判断
  {Zr}/T-bone-S           T-bone-S                  ---             # 明示されていないため、同一区画と判断
  {Zr}/C-bone-V           Blood                 $( 3% / 365)        # 明示されていないため、骨代謝回転率と判断
  {Zr}/T-bone-V           Blood                 $(18% / 365)        # 明示されていないため、骨代謝回転率と判断

  {Zr}/Liver0             Blood                     6
  {Zr}/Liver1             Blood                     6
  {Zr}/Kidneys            Kidneys                   ---

  {Zr}/ST0                Blood                     2.5
  {Zr}/ST1                Blood                     2.5

$endif
