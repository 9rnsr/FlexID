[{Ba}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   St-wall               St-wall
  acc   SI-wall               SI-wall
  acc   RC-wall               RC-wall
  acc   LC-wall               LC-wall
  acc   RS-wall               RS-wall
  acc   Ht-wall               Ht-wall
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   SkeletalMuscle        Muscle
  acc   Spleen                Spleen
  acc   Pancreas              Pancreas
  acc   Brain                 Brain
  acc   R-marrow              R-marrow
  acc   Skin                  Skin
  acc   Lung-Tis              Lung-Tis
  acc   Adipose               Adipose
  acc   Cartilage             Cartilage
  acc   Other                 Other


[{Ba}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

  {Cs}/St-conRe           St-con                    ---

  {Cs}/Plasma             Plasma                    ---
  {Cs}/C-bone-S           C-bone-S                  ---
  {Cs}/T-bone-S           T-bone-S                  ---

# ICRP Publ.137 p.152-154 Para.317
  # > Barium produced in skeletal muscle and red blood cells transfers to plasma at the
  #   rate 1000 d⁻¹, the default value for extremely rapid transfer between systemic compartments.
  # > 骨格筋と赤血球で生成されたバリウムは、全身コンパートメント間の極めて迅速な移行の
  #   デフォルト値である 1000 d⁻¹ の速度で血漿に移行します。
  {Cs}/RBC                Plasma                 1000
  {Cs}/SkeletalMuscle     Plasma                 1000

  # > Barium produced in all other soft tissue compartments transfers to
  #   plasma at the rate 200 d⁻¹ (t½=5 min), chosen to yield, at most, a moderate deficiency
  #   of 137mBa in these tissues compared with equilibrium values.
  # > その他のすべての軟部組織コンパートメントで生成されたバリウムは、平衡値と比較してこれらの
  #   組織で最大でも中程度の 137mBa の欠乏をもたらすように選択された 200 d⁻¹ (t½=5 分) の速度で
  #   血漿に移行します。
  {Cs}/St-wall            Plasma                  200
  {Cs}/SI-wall            Plasma                  200
  {Cs}/RC-wall            Plasma                  200
  {Cs}/LC-wall            Plasma                  200
  {Cs}/RS-wall            Plasma                  200
  {Cs}/Liver              Plasma                  200
  {Cs}/Kidneys            Plasma                  200
  {Cs}/Spleen             Plasma                  200
  {Cs}/Pancreas           Plasma                  200
  {Cs}/Skin               Plasma                  200
  {Cs}/Adipose            Plasma                  200
  {Cs}/Brain              Plasma                  200
  {Cs}/Ht-wall            Plasma                  200
  {Cs}/Lung-Tis           Plasma                  200
  {Cs}/R-marrow           Plasma                  200
  {Cs}/Cartilage          Plasma                  200
  {Cs}/Other1             Plasma                  200
  {Cs}/Other2             Plasma                  200

  # > Barium produced
  #   in bone decays at its site of production. Barium transfers from plasma to trabecular
  #   bone surfaces at the rate 19.4 d⁻¹, cortical bone surfaces at the rate 15.6 d⁻¹, right
  #   colon contents at the rate 40.3 d⁻¹, urinary bladder contents at the rate 4.48 d⁻¹, and
  #   a set of compartments representing extracellular fluids of each of the soft tissues
  #   addressed in the model for cesium (Fig. 6.1) at a total rate of 184 d⁻¹.
  # > 骨で生成されたバリウムは、生成された場所で崩壊します。バリウムは、血漿から海綿骨表面へは
  #   19.4 d⁻¹ の速度で、皮質骨表面へは 15.6 d⁻¹ の速度で、右結腸内容物へは 40.3 d⁻¹ の
  #   速度で、膀胱内容物へは 4.48 d⁻¹ の速度で、～移動します。(Fig. 6.1)
  #
  # > The transfer coefficients from plasma to bone surface compartments and excretion pathways
  #   are two times the corresponding values given in the model for barium as a parent.
  # > 血漿から骨表面コンパートメントおよび排泄経路への移行係数は、親バリウムのモデルで示されている
  #   対応する値の 2 倍です。
  Plasma                  T-bone-S                 19.4         # ≒ 2 * 9.72/d
  Plasma                  C-bone-S                 15.6         # ≒ 2 * 7.78/d
  Plasma                  RC-con                   40.3         # ≒ 2 * 20.16
  Plasma                  UB-con                    4.48        # ≒ 2 * 2.24

  # > and a set of compartments representing extracellular fluids of each of the soft tissues
  #   addressed in the model for cesium (Fig. 6.1) at a total rate of 184 d⁻¹.
  # > そしてセシウムのモデルで扱われている各軟組織の細胞外液を表すコンパートメントのセットへは
  #   合計 184 /d の速度で移動します (Fig.6.1)
  #
  # > Transfer rates from blood to
  #   individual soft tissue pools are based on the reference arterial distribution of cardiac
  #   output in the adult male given in Publication 89 (2002, p. 142). For example, the
  #   transfer rate from blood to spleen is calculated as 0.03 × 184 d⁻¹ = 5.52 d⁻¹, where
  #   0.03 is the reference fraction of cardiac output going to the spleen.
  # > 血液から個々の軟部組織プールへの移行率は、Publication 89 (2002、p. 142)に記載されている
  #   成人男性の心拍出量の基準動脈分布に基づいています。たとえば、血液から脾臓への移行率は
  #   0.03 × 184 d⁻¹ = 5.52 d⁻¹ と計算されます。ここで、0.03 は脾臓に送られる心拍出量の基準割合です。
  #
  # Publ.89 p.142 Para.318
  Plasma                  Ht-wall                   7.36        # = 263.79/d *  2.7901%     # = 184/d * 0.04  (?)
  Plasma                  Liver                    11.96        # = 263.79/d *  4.5339%     # = 184/d *  6.5% (arterial)
  Plasma                  Kidneys                  34.96        # = 263.79/d * 13.253%      # = 184/d * 19%
  Plasma                  SkeletalMuscle           31.28        # = 263.79/d * 11.8579%     # = 184/d * 17%
  Plasma                  St-wall                   1.84        # = 263.79/d *  0.6975%     # = 184/d *  1.0%
  Plasma                  SI-wall                  18.4         # = 263.79/d *  6.9752%     # = 184/d * 10%
  Plasma                  RC-wall                   3           # = 263.79/d *  1.1335%     # = 184/d * a (a+b+c=0.04) → 3/dに近似(?)
  Plasma                  LC-wall                   3           # = 263.79/d *  1.1335%     # = 184/d * b (a+b+c=0.04) → 3/dに近似(?)
  Plasma                  RS-wall                   1.36        # = 263.79/d *  0.5269%     # = 184/d * c (a+b+c=0.04) → 1.36/dに近似(?)
  Plasma                  Spleen                    5.52        # = 263.79/d *  2.0926%     # = 184/d *  3.0%
  Plasma                  Pancreas                  1.84        # = 263.79/d *  0.6975%     # = 184/d *  1.0%
  Plasma                  Brain                    22.08        # = 263.79/d *  8.3703%     # = 184/d * 12%
  Plasma                  R-marrow                  5.52        # = 263.79/d *  2.0926%     # = 184/d *  3.0%
  Plasma                  Skin                      9.2         # = 263.79/d *  3.4876%     # = 184/d *  5.0%
  Plasma                  Lung-Tis                  4.6         # = 263.79/d *  1.7438%     # = 184/d * 0.025
  Plasma                  Adipose                   9.2         # = 263.79/d *  3.4876%     # = 184/d *  5.0% (Fat) # その他の軟組織(other soft tissues)?
  Plasma                  Other                    12.88        # = 263.79/d *  4.8827%     # = 184/d * (?)         # その他の軟組織(other soft tissues)?
  Plasma                  Cartilage                 1E-20       # = 263.79/d *  3.791E-21%  # = 184/d * (?)         # その他の軟組織(other soft tissues)?

  # > The transfer coefficient from the soft tissue compartment back to plasma is 61.4 d⁻¹.
  # > 軟組織コンパートメントから血漿へ戻る際の移動係数は 61.4 /d です。
  Ht-wall                 Plasma                   61.4
  Liver                   Plasma                   61.4
  Kidneys                 Plasma                   61.4
  SkeletalMuscle          Plasma                   61.4
  St-wall                 Plasma                   61.4
  SI-wall                 Plasma                   61.4
  RC-wall                 Plasma                   61.4
  LC-wall                 Plasma                   61.4
  RS-wall                 Plasma                   61.4
  Spleen                  Plasma                   61.4
  Pancreas                Plasma                   61.4
  Brain                   Plasma                   61.4
  R-marrow                Plasma                   61.4
  Skin                    Plasma                   61.4
  Lung-Tis                Plasma                   61.4
  Adipose                 Plasma                   61.4         # その他の軟組織(other soft tissues)?
  Other                   Plasma                   61.4         # その他の軟組織(other soft tissues)?
  Cartilage               Plasma                   61.4         # その他の軟組織(other soft tissues)?

# Publ.137 p.175 Table 7.3 (?)
  T-bone-S                Plasma                    0.578       #  83.2853% of 0.694/d
  T-bone-S                Exch-T-bone-V             0.116       #  16.7147% of 0.694/d
  C-bone-S                Plasma                    0.578       #  83.2853% of 0.694/d
  C-bone-S                Exch-C-bone-V             0.116       #  16.7147% of 0.694/d
  Exch-T-bone-V           T-bone-S                  0.097       #  69.7842% of 0.139/d  # 0.0097 = 69.7842% of 0.0139の間違いか？
  Exch-T-bone-V           Noch-T-bone-V             0.042       #  30.2158% of 0.139/d  # 0.0042 = 30.2158% of 0.0139の間違いか？
  Exch-C-bone-V           C-bone-S                  0.097       #  69.7842% of 0.139/d  # 0.0097 = 69.7842% of 0.0139の間違いか？
  Exch-C-bone-V           Noch-C-bone-V             0.042       #  30.2158% of 0.139/d  # 0.0042 = 30.2158% of 0.0139の間違いか？
  Noch-C-bone-V           Plasma                    4.93E-4     # 100.0%                # 0.0000821 の間違いか？
  Noch-T-bone-V           Plasma                    8.21E-5     # 100.0%                # 0.000493 の間違いか？
