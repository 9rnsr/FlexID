[{Re}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   S-glands              S-glands
  acc   OralcavityRe          O-cavity
  acc   Oesophagus-sRe        Oesophag-s
  acc   St-conRe              St-cont
  acc   St-wall               St-wall
  acc   RC-wall               RC-wall
  acc   Thyroid1              Thyroid
  acc   Thyroid2              Thyroid
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other tissue
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Re}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4
# ICRP Publ.130 p.77 Para.143
# ICRP Publ.151 p.289 Fig.32.1
  OralcavityRe            Oesophagus-sRe          720
  Oesophagus-sRe          St-conRe               2160
  St-conRe                SI-conRe                 20.57

# ICRP Publ.151 p.290 Table 32.3
  Blood                   Thyroid1                  7.0
  Blood                   ST0                      71.88
  Blood                   ST1                       3.0
  Blood                   ST2                       0.18
  Blood                   UB-con                    1.7
  Blood                   S-glands                  2.6
  Blood                   St-wall                   4.3
  Blood                   Kidneys1                  0.7
  Blood                   Kidneys2                  0.04
  Blood                   Liver1                    4.5
  Blood                   RC-wall                   3.4
  Blood                   T-bone-S                  0.35
  Blood                   C-bone-S                  0.35
  Thyroid1                Blood                   100
  Thyroid1                Thyroid2                  1.0
  Thyroid2                Blood                     1.0
  ST0                     Blood                    50
  ST1                     Blood                     0.462
  ST2                     Blood                     0.0347
  S-glands                OralcavityRe             50
  St-wall                 St-conRe                 50
  Kidneys1                UB-con                    8.32
  Kidneys2                Blood                     0.0347
  Liver1                  Blood                     8.234
  Liver1                  Liver2                    0.0832
  Liver2                  Blood                     0.0347
  RC-wall                 RC-con                    1.39
  T-bone-S                Blood                     0.457
  T-bone-S                T-bone-V                  0.00462
  C-bone-S                Blood                     0.457
  C-bone-S                C-bone-V                  0.00462
  T-bone-V                Blood                     0.000493
  C-bone-V                Blood                     0.0000821

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

  {W}/Plasma              Blood                     ---
  {W}/RBC                 Blood                  1000

  {W}/C-bone-S            C-bone-S                  ---
  {W}/T-bone-S            T-bone-S                  ---
  {W}/Exch-C-bone-V       Blood                 $( 3% / 365)
  {W}/Noch-C-bone-V       Blood                 $( 3% / 365)
  {W}/Exch-T-bone-V       Blood                 $(18% / 365)
  {W}/Noch-T-bone-V       Blood                 $(18% / 365)

  {W}/Liver1              Liver1                    ---
  {W}/Liver2              Liver2                    ---
  {W}/Spleen              Blood                     0.462
  {W}/Kidneys1            Kidneys1                  ---
  {W}/Kidneys2            Kidneys2                  ---

  {W}/Other0              Blood                     0.462
  {W}/Other1              Blood                     0.462
  {W}/Other2              Blood                     0.462
