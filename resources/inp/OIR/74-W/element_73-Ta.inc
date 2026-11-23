[{Ta}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Ta}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.276 Table 30.3
  Blood1                  Blood2                    3.2
  Blood1                  Liver0                    0.24
  Blood1                  Kidneys                   0.04
  Blood1                  ST0                       3.2
  Blood1                  ST1                       0.12
  Blood1                  UB-con                    0.88
  Blood1                  SI-conRe                  0.08
  Blood1                  T-bone-S                  0.12
  Blood1                  C-bone-S                  0.12
  Blood2                  Blood1                    1.39
  Liver0                  SI-conRe                  0.0578
  Liver0                  Blood1                    0.0578
  Liver0                  Liver1                    0.231
  Liver1                  Blood1                    0.005
  Kidneys                 Blood1                    0.005
  ST0                     Blood1                    1.39
  ST1                     Blood1                    0.01
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

# ICRP Publ.151 p.285 Para.570
# > (570) Progeny of tungsten addressed in this publication are isotopes of tantalum and
# > rhenium. The characteristic models for tantalum and rhenium are applied to these
# > elements as members of chains headed by tungsten, with added transfer coefficients
# > needed to solve the linked biokinetic models for chains headed by tungsten. Tantalum
# > or rhenium produced in an ambiguous compartment (i.e. a compartment of the model
# > for a preceding chain that is not a compartment in the model for the progeny) is
# > assumed to transfer to the central blood compartment of the progeny’s characteristic
# > model, and to follow that model thereafter. The following transfer rates are assigned to
# > tantalum or rhenium produced in ambiguous compartments: 1000 d⁻¹ if produced in a
# > blood compartment; at the reference rate of bone turnover for the indicated bone type
# > if produced in a bone volume compartment; and at the following element-specific rates
# > if produced in any other compartment: tantalum, 1.39 d⁻¹; and rhenium, 0.462 d⁻¹.
# > (570) 本書で扱うタングステンの娘核種は、タンタルおよびレニウムの同位体です。
# > タンタルおよびレニウムの特性モデルは、タングステンを起点とする崩壊系列の構成要素として
# > 適用されます。この際、タングステンを起点とする崩壊系列の連鎖的な生物動態モデルを解くために
# > 必要な移行係数が追加されます。不明確なコンパートメント（すなわち、先行する崩壊系列の
# > モデルには存在するが、当該娘核種のモデルには存在しないコンパートメント）で生成された
# > タンタルまたはレニウムは、その娘核種の特性モデルにおける中心的な血液コンパートメントへ
# > 移行し、その後はそのモデルに従うものと仮定されます。不明確なコンパートメントで生成された
# > タンタルまたはレニウムには、以下の移行速度が割り当てられます：血液コンパートメントで
# > 生成された場合は1000 d⁻¹、骨体積コンパートメントで生成された場合は当該骨種における
# > 骨代謝回転の基準速度、その他のコンパートメントで生成された場合は、元素ごとに以下の
# > 速度とします：タンタル：1.39 d⁻¹、レニウム：0.462 d⁻¹。

  {W}/Plasma              Blood1                 1000
  {W}/RBC                 Blood1                 1000

  {W}/C-bone-S            C-bone-S                  ---
  {W}/T-bone-S            T-bone-S                  ---
  {W}/Exch-C-bone-V       Blood1                $( 3% / 365)
  {W}/Noch-C-bone-V       Blood1                $( 3% / 365)
  {W}/Exch-T-bone-V       Blood1                $(18% / 365)
  {W}/Noch-T-bone-V       Blood1                $(18% / 365)

  {W}/Liver1              Liver0                    ---     # 確認書による
  {W}/Liver2              Liver1                    ---     # 確認書による
  {W}/Spleen              Blood1                    1.39
  {W}/Kidneys1            Blood1                    1.39
  {W}/Kidneys2            Blood1                    1.39

  {W}/Other0              Blood1                    1.39
  {W}/Other1              Blood1                    1.39
  {W}/Other2              Blood1                    1.39
