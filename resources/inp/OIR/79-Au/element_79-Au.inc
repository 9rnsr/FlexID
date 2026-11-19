[{Au}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other1                Other
  acc   Other2                Other


[{Au}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.321 Table 35.3
  Blood1                  Blood2                    0.1
  Blood1                  Kidneys                   0.1
  Blood1                  Liver                     0.1
  Blood1                  Other1                    0.18
  Blood1                  Other2                    0.1
  Blood1                  UB-con                    0.3
  Blood1                  RC-con                    0.1
  Blood1                  T-bone-S                  0.01
  Blood1                  C-bone-S                  0.01
  Blood2                  Blood1                    0.139
  Kidneys                 UB-con                    0.0693
  Liver                   Blood1                    0.0693
  Other1                  Blood1                    0.0693
  Other2                  Blood1                    0.0139
  T-bone-S                Blood1                    0.0693
  C-bone-S                Blood1                    0.0693

$if progeny

# ICRP Publ.151 p.320 Para.651
# > (651) Progeny of gold addressed in this publication are radioisotopes of gold,
# > rhenium, osmium, iridium, and platinum. The model for gold as a parent is applied
# > to gold as a progeny of gold. The models for rhenium, osmium, iridium, and platinum
# > as progeny of gold are expansions of the characteristic models for these elements
# > with added compartments and associated transfer coefficients needed to solve
# > the linked biokinetic models for chains headed by gold (see Annex B). If produced in
# > an ambiguous compartment (i.e. a compartment not explicitly named in the progeny’s
# > model), the progeny is assumed to transfer at a specified rate to the central
# > blood compartment of its characteristic biokinetic model, and to follow that model
# > thereafter. The following transfer rates to the central blood compartment are
# > assigned to rhenium, osmium, iridium, and platinum produced in an ambiguous
# > compartment: 1000 d⁻¹ if produced in a blood compartment; and at the following
# > element-specific rates if produced in a tissue compartment: rhenium, 0.462 d⁻¹;
# > osmium or platinum, 0.09902 d⁻¹; and iridium, 0.0693 d⁻¹.
# > (651) 本刊行物で扱う金の子孫元素は、金、レニウム、オスミウム、イリジウム、および
# > 白金の放射性同位元素です。親元素としての金のモデルは、金の子孫元素としての金に
# > 適用されます。金の子孫元素としてのレニウム、オスミウム、イリジウム、および白金のモデルは、
# > これらの元素の特性モデルの拡張版であり、金を先頭とする連鎖の連結生体動態モデルを
# > 解くために必要なコンパートメントと関連する移動係数が追加されています（付録B参照）。
# > あいまいなコンパートメント（すなわち、子孫元素のモデルで明示的に名前が付けられていない
# > コンパートメント）で生成された場合、子孫元素は、その特性生体動態モデルの
# > 中心血液コンパートメントに特定の速度で移行し、その後はそのモデルに従うと仮定されます。
# > 不明瞭なコンパートメントで生成されたレニウム、オスミウム、イリジウム、および白金の
# > 中心血液コンパートメントへの移行速度は、血液コンパートメントで生成された場合は1000 d⁻¹、
# > 組織コンパートメントで生成された場合は以下の元素固有の移行速度となります：
# > レニウム：0.462 d⁻¹、オスミウムまたは白金：0.09902 d⁻¹、イリジウム：0.0693 d⁻¹。

  {Au}/Blood1             Blood1                    ---
  {Au}/Blood2             Blood2                    ---

  {Au}/C-bone-S           C-bone-S                  ---
  {Au}/T-bone-S           T-bone-S                  ---

  {Au}/Liver              Liver                     ---
  {Au}/Kidneys            Kidneys                   ---

  {Au}/Other1             Other1                    ---
  {Au}/Other2             Other2                    ---

$endif
