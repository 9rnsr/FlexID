[{Te}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Thyroid               Thyroid
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   ST                    Other


[{Te}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.73 Table 4.7
  Blood1                  UB-con                    0.751
  Blood1                  Kidneys                   0.0404
  Blood1                  Liver                     0.1213
  Blood1                  Blood2                    0.1011
  Blood1                  ST                        0.0768
  Blood1                  C-bone-S                  0.0202
  Blood1                  T-bone-S                  0.0404
  Blood1                  Thyroid                   0.0040
  Blood2                  Blood1                    0.0693
  Liver                   SI-conRe                  0.0693
  Thyroid                 Blood1                    0.0693
  Kidneys                 Blood1                    0.0693
  ST                      Blood1                    0.0693
  C-bone-S                Blood1                    0.0116
  T-bone-S                Blood1                    0.0116
  C-bone-S                C-bone-V                  0.000693
  T-bone-S                T-bone-V                  0.000693
  C-bone-V                Blood1                    0.000082
  T-bone-V                Blood1                    0.000493

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

  {Sn}/Blood              Blood1                    ---
  {Sb}/Plasma             Blood1                    ---
  {Sb}/RBC                Blood1                 1000
  {Te}/Blood1             Blood1                    ---
  {Te}/Blood2             Blood2                    ---

  {Sn,Sb,Te}/C-bone-S     C-bone-S                  ---
  {Sn,Sb,Te}/T-bone-S     T-bone-S                  ---
  {Sn,Sb,Te}/C-bone-V     C-bone-V                  ---
  {Sn,Sb,Te}/T-bone-V     T-bone-V                  ---

  {Sb,Te}/Thyroid         Thyroid                   ---
  {Sn,Sb}/Liver1          Blood1                    0.0693
  {Sn,Sb}/Liver2          Blood1                    0.0693
  {Te}/Liver              Liver                     ---
  {Sn}/Kidneys1           Blood1                    0.0693
  {Sn}/Kidneys2           Blood1                    0.0693
  {Sb,Te}/Kidneys         Kidneys                   ---

  {Sn}/Other1             Blood1                    0.0693
  {Sn}/Other2             Blood1                    0.0693
  {Sb}/ST0                Blood1                    0.0693
  {Sb}/ST1                Blood1                    0.0693
  {Sb}/ST2                Blood1                    0.0693
  {Te}/ST                 ST                        ---
