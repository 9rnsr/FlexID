[{Os}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissue
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Os}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.301 Table 33.3
  Blood1                  SI-conRe                  3.0
  Blood1                  UB-con                   23
  Blood1                  Liver1                   12
  Blood1                  Kidneys1                 10.67
  Blood1                  Kidneys2                  0.33
  Blood1                  Blood2                   27
  Blood1                  ST0                      15
  Blood1                  ST1                       2.5
  Blood1                  ST2                       2.5
  Blood1                  C-bone-S                  1.0
  Blood1                  T-bone-S                  3.0
  Blood2                  Blood1                    0.6931
  Liver1                  Blood1                    0.09704
  Liver1                  SI-conRe                  0.03466
  Liver1                  Liver2                    0.006931
  Liver2                  Blood1                    0.003798
  Kidneys1                UB-con                    0.1386
  Kidneys2                Blood1                    0.003798
  ST0                     Blood1                    0.09902
  ST1                     Blood1                    0.0231
  ST2                     Blood1                    0.0009495
  C-bone-S                Blood1                    0.07922
  T-bone-S                Blood1                    0.07922
  C-bone-S                C-bone-V                  0.0198
  T-bone-S                T-bone-V                  0.0198
  C-bone-V                Blood1                    0.0000821
  T-bone-V                Blood1                    0.000493

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

  {Re}/Blood              Blood1                 1000

  {Re}/C-bone-S           C-bone-S                  ---
  {Re}/T-bone-S           T-bone-S                  ---
  {Re}/C-bone-V           C-bone-V                  ---
  {Re}/T-bone-V           T-bone-V                  ---

  {Re}/S-glands           Blood1                    0.09902
  {Re}/OralcavityRe       Oralcavity                ---
  {Re}/Oesophagus-sRe     Oesophagus-s              ---
  {Re}/St-conRe           St-con                    ---

  {Re}/St-wall            Blood1                    0.09902
  {Re}/RC-wall            Blood1                    0.09902
  {Re}/Thyroid1           Blood1                    0.09902
  {Re}/Thyroid2           Blood1                    0.09902
  {Re}/Liver1             Liver1                    ---
  {Re}/Liver2             Liver2                    ---
  {Re}/Kidneys1           Kidneys1                  ---
  {Re}/Kidneys2           Kidneys2                  ---

  {Re}/ST0                Blood1                    0.09902
  {Re}/ST1                Blood1                    0.09902
  {Re}/ST2                Blood1                    0.09902
