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

$if progeny

# ICRP Publ.151 p.289 Para.577
# > (577) Progeny of rhenium isotopes addressed in this publication are radioisotopes
# > of tungsten, tantalum, rhenium, and osmium. The model for rhenium as a parent is
# > applied to rhenium produced by decay of another rhenium isotope. The models for
# > tungsten, tantalum, and osmium as rhenium progeny are their characteristic models
# > with added compartments and associated transfer coefficients needed to solve the
# > linked biokinetic models for chains headed by rhenium (see Annex B). If produced in
# > an ambiguous compartment (i.e. a compartment contained in the model for a preceding
# > chain member but not contained in the progeny’s characteristic model), the
# > progeny is assumed to transfer at a specified rate to the central blood compartment
# > of its characteristic biokinetic model, and to follow that model thereafter. The following
# > transfer rates to the central blood compartment are assigned to tungsten,
# > tantalum, or osmium produced in an ambiguous compartment: 1000 d1 if produced
# > in a blood compartment; and at the following element-specific rates if produced in
# > any other ambiguous compartment: tungsten, 8.32 d1; tantalum, 1.39 d1; and
# > osmium, 0.09902 d1.
# > (577) 本書で扱うレニウム同位体の娘核種は、タングステン、タンタル、レニウム、および
# > オスミウムの放射性同位体です。親核種がレニウムである場合のモデルは、別のレニウム
# > 同位体の崩壊によって生成されるレニウムに対しても適用されます。レニウムの娘核種である
# > タングステン、タンタル、オスミウムのモデルは、それぞれの固有のモデルに、レニウムを
# > 起点とする崩壊系列の連鎖的な生物学的動態モデルを解くために必要なコンパートメント
# > および関連する移行係数を追加したものとなっています（付録Bを参照）。娘核種が
# > 「曖昧なコンパートメント」（すなわち、崩壊系列における先行核種のモデルには含まれて
# > いるが、その娘核種固有のモデルには含まれていないコンパートメント）で生成された場合、
# > その娘核種は、所定の移行速度で、自身固有の生物学的動態モデルにおける中心血液
# > コンパートメントへ移行し、その後はそのモデルに従うものと仮定されます。曖昧な
# > コンパートメントで生成されたタングステン、タンタル、またはオスミウムが中心血液
# > コンパートメントへ移行する速度は、次のように設定されます：血液コンパートメントで
# > 生成された場合は1000 d⁻¹とし、それ以外の曖昧なコンパートメントで生成された場合は、
# > 元素ごとに以下の移行速度を適用します：タングステンは8.32 d⁻¹、タンタルは1.39 d⁻¹、
# > オスミウムは0.09902 d⁻¹です。

  {Re}/Blood              Blood                     ---

  {Re}/C-bone-S           C-bone-S                  ---
  {Re}/T-bone-S           T-bone-S                  ---
  {Re}/C-bone-V           C-bone-V                  ---
  {Re}/T-bone-V           T-bone-V                  ---

  {Re}/S-glands           S-glands                  ---
  {Re}/OralcavityRe       OralcavityRe              ---
  {Re}/Oesophagus-sRe     Oesophagus-sRe            ---
  {Re}/St-conRe           St-conRe                  ---
  {Re}/St-wall            St-wall                   ---
  {Re}/RC-wall            RC-wall                   ---
  {Re}/Thyroid1           Thyroid1                  ---
  {Re}/Thyroid2           Thyroid2                  ---
  {Re}/Liver1             Liver1                    ---
  {Re}/Liver2             Liver2                    ---
  {Re}/Kidneys1           Kidneys1                  ---
  {Re}/Kidneys2           Kidneys2                  ---

  {Re}/ST0                ST0                       ---
  {Re}/ST1                ST1                       ---
  {Re}/ST2                ST2                       ---

$endif
