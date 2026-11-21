[{Ca}:compartment]
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
  acc   RedMarrow             R-marrow      # for progeny of K
  acc   Liver                 Liver         # for progeny of K
  acc   Kidneys               Kidneys       # for progeny of K
  acc   Muscle                Muscle        # for progeny of K
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Ca}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.121 Table 6.3
  Blood                   UB-con                    0.60
  Blood                   RC-con                    0.45
  Blood                   T-bone-S                  2.08
  Blood                   C-bone-S                  1.67
  Blood                   ST0                       8.70
# Blood                   ST1                       1.50        # fix
  Blood                   ST2                       0.00075
  T-bone-S                Blood                     0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Blood                     0.578
  C-bone-S                Exch-C-bone-V             0.116
  ST0                     Blood                     2.9
  ST1                     Blood                     0.1733
  ST2                     Blood                     0.00038
  Exch-T-bone-V           T-bone-S                  0.002773
  Exch-T-bone-V           Noch-T-bone-V             0.004159
  Exch-C-bone-V           C-bone-S                  0.002773
  Exch-C-bone-V           Noch-C-bone-V             0.004159
  Noch-C-bone-V           Blood                     0.0000821
  Noch-T-bone-V           Blood                     0.000493

# ICRP Publ.151 p.83 Para.146
# > (146) The only progeny of a potassium parent addressed in this publication is 45Ca
# > produced by decay of 45K. For application to 45Ca produced in systemic compartments,
# > the characteristic model for calcium (ICRP, 2016) was expanded to address
# > explicitly each of the tissues and individual compartments addressed explicitly in the
# > model for potassium. The following transfer coefficients from added tissue compartments
# > to the central blood compartment of the calcium model were assigned: RBC,
# > 1000 d⁻¹ (default value); kidneys, 0.1733 d⁻¹; liver, 0.1733 d⁻¹; muscle, 0.1733 d⁻¹;
# > red marrow, 0.1733 d⁻¹; and other, 2.9 d⁻¹. The following transfer coefficients from
# > blood to tissues were also added to the calcium model: kidneys, 0.00766 d⁻¹; liver,
# > 0.0445 d⁻¹; muscle, 0.716 d⁻¹; and red marrow, 0.0289 d⁻¹. The transfer coefficient
# > from blood to the intermediate-term soft tissue compartments of the calcium model
# > was reduced from 1.5 d⁻¹ to 0.703 d⁻¹ to leave the total outflow rate of calcium from
# > blood at 15 d⁻¹.
# > (146) 本出版物において扱うカリウムを親核種とする唯一の娘核種は、45Kの崩壊によって
# > 生成される45Caです。生体内の各区画で生成される45Caに適用するため、カルシウムに関する
# > 標準的なモデル（ICRP, 2016）を拡張し、カリウムモデルで明示的に扱われている各組織および
# > 個別の区画を、カルシウムモデルにおいても明示的に取り扱うようにしました。カルシウムモデルに
# > おいて、追加された組織区画から中心血液区画への移行係数は、赤血球（RBC）1000 d⁻¹
# > （デフォルト値）、腎臓0.1733 d⁻¹、肝臓0.1733 d⁻¹、筋肉0.1733 d⁻¹、赤色骨髄0.1733 d⁻¹、
# > その他2.9 d⁻¹と設定されました。また、血液から各組織への移行係数として、腎臓0.00766 d⁻¹、
# > 肝臓0.0445 d⁻¹、筋肉0.716 d⁻¹、赤色骨髄0.0289 d⁻¹をカルシウムモデルに追加しました。
# > さらに、血液からカルシウムモデルの中期軟部組織区画への移行係数を1.5 d⁻¹から0.703 d⁻¹へと
# > 低減させることで、血液から流出するカルシウムの総速度が15 d⁻¹となるように調整されました。

  {K}/Plasma              Blood                     ---
  {K}/RBC                 Blood                  1000

  {K}/C-bone-S            C-bone-S                  ---
  {K}/T-bone-S            T-bone-S                  ---

  {K}/RedMarrow           RedMarrow                 ---
  {K}/Liver               Liver                     ---
  {K}/Kidneys             Kidneys                   ---
  {K}/Muscle              Muscle                    ---

  {K}/Other               Blood                     2.9

  RedMarrow               Blood                     0.1733
  Liver                   Blood                     0.1733
  Kidneys                 Blood                     0.1733
  Muscle                  Blood                     0.1733
  Blood                   RedMarrow                 0.0289
  Blood                   Liver                     0.0445
  Blood                   Kidneys                   0.00766
  Blood                   Muscle                    0.716
  Blood                   ST1                       0.703
