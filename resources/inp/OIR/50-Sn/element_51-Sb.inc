[{Sb}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Thyroid               Thyroid
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Sb}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.60 Table 3.3
  Plasma                  Thyroid                   0.3
  Plasma                  UB-con                    9.0
  Plasma                  Liver1                    4.0
  Plasma                  Kidneys                   0.25
  Plasma                  RBC                       3.0
  Plasma                  ST0                      74.91
  Plasma                  ST1                       4.5
  Plasma                  ST2                       0.04
  Plasma                  C-bone-S                  2.0
  Plasma                  T-bone-S                  2.0
  Thyroid                 Plasma                    0.139
  RBC                     Plasma                    0.693
  Liver1                  Plasma                    0.437
  Liver1                  SI-conRe                  0.243
  Liver1                  Liver2                    0.0139
  Liver2                  Plasma                    0.000693
  Kidneys                 Plasma                    0.231
  ST0                     Plasma                    1.39
  ST1                     Plasma                    0.0139
  ST2                     Plasma                    0.000693
  C-bone-S                Plasma                    0.343
  T-bone-S                Plasma                    0.343
  C-bone-S                C-bone-V                  0.00347
  T-bone-S                T-bone-V                  0.00347
  C-bone-V                Plasma                    0.0000821
  T-bone-V                Plasma                    0.000493

# ICRP Publ.151 p.263 Para.533
# > (533) Progeny of tin addressed in this publication are isotopes of tin, indium,
# > cadmium, tellurium, and antimony. The model for tin as a parent is applied to tin
# > produced by decay of another isotope of tin. The models for indium, cadmium,
# > tellurium, and antimony as progeny of tin are expansions of their characteristic
# > models with added compartments and associated transfer coefficients needed to
# > solve the linked biokinetic models of chains headed by tin. Muscle and pancreas
# > were added to the explicitly identified tissues in the characteristic model for
# > indium. The following rates of transfer of indium between blood compartments in
# > the characteristic model for indium and the added tissues were assigned: transferrin to
# > pancreas, 0.001 d⁻¹; transferrin to muscle, 0.2 d⁻¹; pancreas to plasma, 2.37 d⁻¹; and
# > muscle to plasma, 2.37 d⁻¹. Indium, cadmium, tellurium, or antimony produced in a
# > compartment of the model for a preceding chain that is not a compartment in the
# > model for that progeny (an ambiguous compartment) is assumed to transfer to the
# > central blood compartment of the progeny’s model, and to follow that model thereafter.
# > The following transfer coefficients are assigned to progeny produced in ambiguous
# > compartments: 1000 d⁻¹ if produced in a blood compartment; at the rate of bone
# > turnover for the indicated bone type if produced in a bone volume compartment; and
# > at the following element-specific rates if produced in any other compartment: indium,
# > 2.37 d⁻¹; cadmium, 0.5 d⁻¹; tellurium, 0.0693 d⁻¹; and antimony, 1.39 d⁻¹.
# > (533) 本出版物で扱うスズの娘核種は、スズ、インジウム、カドミウム、テルル、およびアンチモンの
# > 同位体です。親核種としてのスズに関するモデルは、別のスズ同位体の崩壊によって生成される
# > スズに対しても適用されます。スズの娘核種であるインジウム、カドミウム、テルル、および
# > アンチモンに関するモデルは、それぞれの固有モデルを拡張したものです。これには、スズを
# > 起点とする崩壊系列の連鎖的な生物動態モデルを解くために必要な、コンパートメントの
# > 追加およびそれに関連する移行係数が含まれます。インジウムの固有モデルにおいて明示的に
# > 特定されている組織には、筋肉および膵臓が追加されます。インジウムの固有モデルにおける
# > 血液コンパートメントと、追加された組織との間におけるインジウムの移行速度は、
# > 以下のように設定されました：トランスフェリンから膵臓へ 0.001 d⁻¹、トランスフェリンから
# > 筋肉へ 0.2 d⁻¹、膵臓から血漿へ 2.37 d⁻¹、および筋肉から血漿へ 2.37 d⁻¹。崩壊系列の先行する
# > モデルのコンパートメントで生成されたインジウム、カドミウム、テルル、またはアンチモンが、
# > その娘核種のモデルには存在しないコンパートメント（「曖昧なコンパートメント」と呼ぶ）で
# > 生成された場合、それらは娘核種のモデルにおける中心的な血液コンパートメントへ移行し、
# > その後はそのモデルに従うものと仮定されます。曖昧なコンパートメントで生成された娘核種には、
# > 以下の移行係数が割り当てられます：血液コンパートメントで生成された場合は 1000 d⁻¹です。
# > 骨体積コンパートメントで生成された場合は、当該骨の種類に応じた骨代謝回転速度を適用します。
# > その他のコンパートメントで生成された場合は、元素ごとに以下の速度を適用します：
# > インジウム 2.37 d⁻¹、カドミウム 0.5 d⁻¹、テルル 0.0693 d⁻¹、およびアンチモン 1.39 d⁻¹。

  {Sn}/Blood              Plasma                    ---
  {Sb}/Plasma             Plasma                    ---
  {Sb}/RBC                RBC                       ---

  {Sn,Sb}/C-bone-S        C-bone-S                  ---
  {Sn,Sb}/T-bone-S        T-bone-S                  ---
  {Sn,Sb}/C-bone-V        C-bone-V                  ---
  {Sn,Sb}/T-bone-V        T-bone-V                  ---

  {Sb}/Thyroid            Thyroid                   ---
  {Sn,Sb}/Liver1          Liver1                    ---
  {Sn,Sb}/Liver2          Liver2                    ---
  {Sn}/Kidneys1           Plasma                    1.39
  {Sn}/Kidneys2           Plasma                    1.39
  {Sb}/Kidneys            Kidneys                   ---

  {Sn}/Other1             Plasma                    1.39
  {Sn}/Other2             Plasma                    1.39
  {Sb}/ST0                ST0                       ---
  {Sb}/ST1                ST1                       ---
  {Sb}/ST2                ST2                       ---
