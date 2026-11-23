[{Sn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Other1                Other
  acc   Other2                Other


[{Sn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.255 Table 27.3
  Blood                   UB-con                    1.8
  Blood                   RC-con                    0.2
  Blood                   T-bone-S                  0.6
  Blood                   C-bone-S                  0.6
  Blood                   Other1                    0.6
  Blood                   Other2                    1.0
  Blood                   Liver1                    0.075
  Blood                   Liver2                    0.025
  Blood                   Kidneys1                  0.05
  Blood                   Kidneys2                  0.05
  T-bone-S                Blood                     0.035
  C-bone-S                Blood                     0.035
  T-bone-S                T-bone-V                  0.035
  C-bone-S                C-bone-V                  0.035
  T-bone-V                Blood                     0.0035
  C-bone-V                Blood                     0.0035
  Liver1                  Blood                     0.0116
  Liver2                  Blood                     0.00077
  Kidneys1                UB-con                    0.139
  Kidneys2                Blood                     0.0116
  Other1                  Blood                     0.139
  Other2                  Blood                     0.0035

$if progeny

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

  {Sn}/Blood              Blood                     ---

  {Sn}/C-bone-S           C-bone-S                  ---
  {Sn}/T-bone-S           T-bone-S                  ---
  {Sn}/C-bone-V           C-bone-V                  ---
  {Sn}/T-bone-V           T-bone-V                  ---

  {Sn}/Liver1             Liver1                    ---
  {Sn}/Liver2             Liver2                    ---
  {Sn}/Kidneys1           Kidneys1                  ---
  {Sn}/Kidneys2           Kidneys2                  ---

  {Sn}/Other1             Other1                    ---
  {Sn}/Other2             Other2                    ---

$endif
