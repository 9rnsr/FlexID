[{Zn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   RedMarrow             R-marrow      # for progeny of Ga
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Spleen                Spleen        # for progeny of Ga
  acc   Muscle                Muscle
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other
  exc   Excreta               ---


[{Zn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.199 Table 9.4
  Plasma                  Liver1                   60
  Plasma                  Kidneys                   4
  Plasma                  Pancreas                  3
  Plasma                  Muscle                    2
  Plasma                  RBC                       1.5
  Plasma                  ST0                      40
  Plasma                  ST1                      30
  Plasma                  ST2                       0.4
  Plasma                  UB-con                    0.13
  Plasma                  Excreta                   0.13
  Plasma                  SI-conRe                  0.2
  Plasma                  T-bone-S                  0.15
  Plasma                  C-bone-S                  0.3
  Liver1                  Plasma                   10
  Liver1                  SI-conRe                  0.067
  Liver1                  Liver2                   10
  Liver2                  Plasma                    0.6
  Kidneys                 Plasma                    0.7
  Pancreas                Plasma                    1.5
  Pancreas                SI-conRe                  1.0
  Muscle                  Plasma                    0.005
  RBC                     Plasma                    0.14
  ST0                     Plasma                   10
  ST1                     Plasma                    3
  ST2                     Plasma                    0.01
  T-bone-S                Plasma                    0.01
  C-bone-S                Plasma                    0.01
  T-bone-S                T-bone-V                  0.00053
  C-bone-S                C-bone-V                  0.00053
  T-bone-V                Plasma                    0.000493
  C-bone-V                Plasma                    0.0000821

# ICRP Publ.151 p.165 Para.330
# > (330) The only progeny of gallium addressed in this publication is 65Zn, produced
# > by decay of 65Ga. The model for systemic zinc as a progeny of gallium is an expansion
# > of the characteristic model for zinc with added compartments and associated
# > transfer coefficients needed to solve the linked biokinetic models for gallium and zinc
# > (see Annex B). Compartments representing the spleen and red marrow were added to
# > the model for zinc to address all tissues considered explicitly in the model for gallium.
# > The following transfer coefficients were added to the characteristic model for zinc:
# > blood to spleen, 3.0 d⁻¹; blood to red marrow, 3.0 d⁻¹; spleen to blood, 2.5 d⁻¹; red
# > marrow to blood, 2.5 d⁻¹; and other to blood, 10 d⁻¹.
# > (330) 本報告書において取り上げるガリウムの唯一の娘核種は、65Gaの崩壊によって生成される
# > 65Znです。ガリウムの娘核種としての亜鉛の体内動態モデルは、亜鉛に関する標準的なモデルを
# > 拡張したものであり、ガリウムと亜鉛の連動した生物学的動態モデルを解くために必要な
# > コンパートメントおよびそれに関連する移行係数が追加されています（付録Bを参照）。
# > ガリウムのモデルにおいて明示的に考慮されているすべての組織を網羅するため、亜鉛のモデルには
# > 脾臓および赤色骨髄を表すコンパートメントが追加されました。亜鉛の標準的なモデルには、
# > 以下の移行係数が追加されました：血液から脾臓へ 3.0 d⁻¹、血液から赤色骨髄へ 3.0 d⁻¹、
# > 脾臓から血液へ 2.5 d⁻¹、赤色骨髄から血液へ 2.5 d⁻¹、および「その他」から血液へ 10 d⁻¹。

  Plasma                  Spleen                    3.0
  Plasma                  RedMarrow                 3.0
  Spleen                  Plasma                    2.5
  RedMarrow               Plasma                    2.5

  {Ga}/Blood              Plasma                   10.0

  {Ga}/C-bone-S           C-bone-S                  ---
  {Ga}/T-bone-S           T-bone-S                  ---

  {Ga}/RedMarrow          RedMarrow                 ---
  {Ga}/Liver              Plasma                   10.0
  {Ga}/Kidneys            Kidneys                   ---
  {Ga}/Pancreas           Pancreas                  ---
  {Ga}/Spleen             Spleen                    ---
  {Ga}/Muscle             Muscle                    ---

  {Ga}/ST0                Plasma                   10.0
  {Ga}/ST1                Plasma                   10.0
