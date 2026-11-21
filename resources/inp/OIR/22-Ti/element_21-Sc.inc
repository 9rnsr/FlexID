[{Sc}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Spleen                Spleen
  acc   ST1                   Other
  acc   ST2                   Other


[{Sc}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.92 Table 10.3
  Blood1                  Blood2                    0.45
  Blood1                  UB-con                    0.054
  Blood1                  Liver1                    0.60
  Blood1                  Kidneys                   0.09
  Blood1                  Spleen                    0.06
  Blood1                  T-marrow                  0.15
  Blood1                  C-marrow                  0.15
  Blood1                  T-bone-S                  0.15
  Blood1                  C-bone-S                  0.15
  Blood1                  ST1                       0.60
  Blood1                  ST2                       0.546
  Blood2                  Blood1                    0.462
  Liver1                  SI-conRe                  0.0578
  Liver1                  Liver2                    0.0578
  Liver1                  Blood1                    0.116
  Liver2                  Blood1                    0.00693
  Kidneys                 Blood1                    0.0347
  Spleen                  Blood1                    0.0019
  T-marrow                Blood1                    0.00693
  C-marrow                Blood1                    0.00693
  ST1                     Blood1                    0.231
  ST2                     Blood1                    0.00693
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

# ICRP Publ.151 p.100 Para.178
# > (178) The only progeny of titanium addressed in this publication is 44Sc, produced
# > by decay of 44Ti. The characteristic model for scandium is applied to 44Sc as progeny
# > of 44Ti, with added compartments and associated transfer coefficients needed to solve
# > the linked biokinetic models of titanium and scandium. The following transfer rates
# > from compartments appearing in the titanium model to scandium’s central blood
# > compartment are added to the characteristic model for scandium: 1000 d⁻¹ if 44Si is
# > produced in a blood compartment not identified in the scandium model, and at
# > 0.231 d⁻¹ if 44Si is produced in a compartment in titanium’s ‘Other’ not identified
# > in the scandium model.
# > (178) 本書で取り上げるチタンの娘核種は、44Tiの崩壊によって生成される44Scのみである。
# > スカンジウムに関する標準的なモデルを、44Tiの娘核種である44Scに適用する。その際、
# > チタンとスカンジウムの連動した生物学的動態モデルを解くために必要なコンパートメント
# > およびそれに関連する移行係数を追加する。チタンモデルに含まれるコンパートメントから
# > スカンジウムの血中コンパートメント（中心コンパートメント）への移行速度として、
# > 以下の値をスカンジウムの標準モデルに追加する。すなわち、スカンジウムモデル上で
# > 特定されていない血中コンパートメント内で44Scが生成される場合は1000 d⁻¹とし、
# > チタンモデルにおける「その他（Other）」のコンパートメントのうち、スカンジウムモデル上で
# > 特定されていないコンパートメント内で44Scが生成される場合は0.231 d⁻¹とする。

  {Ti}/Blood1             Blood1                    ---
  {Ti}/Blood2             Blood1                 1000

  {Ti}/C-bone-S           C-bone-S                  ---
  {Ti}/T-bone-S           T-bone-S                  ---
  {Ti}/C-bone-V           C-bone-V                  ---
  {Ti}/T-bone-V           T-bone-V                  ---

  {Ti}/Liver0             Liver1                    ---
  {Ti}/Liver1             Liver2                    ---
  {Ti}/Kidneys            Kidneys                   ---

  {Ti}/ST0                Blood1                    0.231
  {Ti}/ST1                Blood1                    0.231
