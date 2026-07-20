#    ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのポロニウムは、ラジウム子孫元素としてのモデルを用いる
# → ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのポロニウムには、鉛子孫元素としてのモデルを修正して用いる
# → ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > (484) The systemic model for polonium as a member of a lead chain is the same as
# > the systemic model applied in this publication to polonium as a parent radionuclide
# > (Section 11), except that no distinction is made in the progeny model between
# > polonium entering blood from the respiratory and alimentary tracts. ...
# > (484) 鉛鎖の一員としてのポロニウムの全身モデルは、本書で親核種としてのポロニウムに
# > 適用されている全身モデル(第11節)と同一である。ただし、子孫核種モデルでは、呼吸器系と
# > 消化器系から血液に入るポロニウムを区別しない点が異なる。 ...

[{Po}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   Plasma3               Blood
  acc   RBC                   Blood
  mix   BoneSurface           ---           # Publ.137 Para.605
  acc   C-bone-S              C-bone-S      # Publ.137 Para.605
  acc   T-bone-S              T-bone-S      # Publ.137 Para.605
 #acc   RedMarrow             R-marrow      # Publ.137 Para.712
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen
  acc   Other                 Other
  exc   Excreta               ---           # hair,sweat,skin

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのポロニウム
# > (712) ... For each of these four elements, the red marrow compartment
# > is replaced by a compartment with the same kinetics (i.e. the same paths and rates of
# > inflow and outflow) named ‘trabecular marrow’ for consistency with the models for
# > other radium progeny; a compartment is added to represent cortical marrow, which is
# > assumed to exchange activity with plasma; ...
# > (712) ... これら4元素それぞれに
# > ついて、赤色骨髄コンパートメントは、他のラジウム子孫元素のモデルとの整合性を図るため、
# > 同じ動態（すなわち、同じ経路および流入・流出速度）を持つ「骨梁骨髄」と呼ばれる
# > コンパートメントに置き換えられる。皮質骨髄を表すコンパートメントが追加され、この
# > コンパートメントは血漿と放射能を交換すると仮定される。...

  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow

[{Po}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > (484) ... polonium as a progeny transferred to blood from either the respiratory
# > or alimentary tract is assigned to Plasma 2. ...
# > (484) ... 呼吸器系または消化器系から血液に移行する子孫核種としてのポロニウムは、
# > 血漿2に割り当てられる。 ...

# ICRP Publ.137 p.285 Table 11.3
  Plasma2                 Plasma1                 800
  Plasma2                 Kidneys1                200
  Plasma1                 Plasma3                   4
  Plasma1                 RBC                       6
  Plasma1                 Liver1                   17.5
  Plasma1                 Liver2                   17.5
  Plasma1                 Kidneys1                  5
  Plasma1                 Kidneys2                  5
  Plasma1                 Skin                      5
 #Plasma1                 RedMarrow                 4           # fix
  Plasma1                 BoneSurface               1.5
  Plasma1                 Spleen                    2
  Plasma1                 Testes                    0.1
  Plasma1                 Ovaries                   0.05
 #Plasma1                 Other                    32.35        # fix
  Plasma3                 Plasma1                   0.099
  RBC                     Plasma1                   0.099
  Liver1                  SI-conRe                  0.139
  Liver2                  Plasma1                   0.099
  Kidneys1                UB-con                    0.173
  Kidneys2                Plasma1                   0.099
  Skin                    Plasma1                   0.00693
  Skin                    Excreta                   0.00693
 #RedMarrow               Plasma1                   0.099       # fix
  BoneSurface             C-bone-S                 50%          # Publ.137 Para.605
  BoneSurface             T-bone-S                 50%          # Publ.137 Para.605
  C-bone-S                Plasma1                   0.0231      # Publ.137 Para.605
  T-bone-S                Plasma1                   0.0231      # Publ.137 Para.605
  Spleen                  Plasma1                   0.099
  Testes                  Plasma1                   0.0139
  Ovaries                 Plasma1                   0.0139
  Other                   Plasma1                   0.099

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのポロニウム
# > (712) ... the transfer coefficient from plasma to
# > cortical marrow is one-third of the transfer coefficient from plasma to trabecular
# > marrow; the removal half-time from cortical marrow is the same as that for trabecular
# > marrow; and the transfer coefficient from plasma to ST1 is reduced to maintain the
# > original total outflow rate from plasma to all destinations.
# > (712) ... 血漿から皮質骨髄への移行係数は、
# > 血漿から骨梁骨髄への移行係数の3分の1である。皮質骨髄からの除去半減期は、骨梁骨髄の場合と
# > 同じである。血漿からST1への移行係数は、血漿からすべての目的地への元の総流出速度を
# > 維持するために低減される。

  Plasma1                 T-marrow                  4           # RedMarrowをT-marrowに置換
  Plasma1                 C-marrow                  1.33        # Plasma1→T-marrowの移行速度の1/3
  Plasma1                 Other                    31.02        # Plasma1→C-marrowの速度を差し引く
  T-marrow                Plasma1                   0.099       # RedMarrowをT-marrowに置換
  C-marrow                Plasma1                   0.099       # T-marrow→Plasma1の移行速度と同じ

# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > (484) ... Polonium produced in a blood compartment
# > that is not identifiable with a blood compartment of the polonium model is
# > assumed to transfer to Plasma 2 in the polonium model (Fig. 11.1) at the rate
# > 1000 d⁻¹. Polonium produced in a soft tissue compartment not identifiable with a
# > compartment of the polonium model is assumed to move to Plasma 2 with a halftime
# > of 7 d. Polonium produced in a compartment of cortical or trabecular bone
# > volume is assumed to transfer to Plasma 2 at the reference rate of turnover of that
# > bone type.
# > (484) ... ポロニウムモデルの血液コンパートメントと
# > 識別できない血液コンパートメントで生成されたポロニウムは、ポロニウムモデル(図11.1)において、
# > 1000 d⁻¹の速度で血漿2に移行すると仮定される。ポロニウムモデルのコンパートメントに
# > 識別できない軟部組織コンパートメントで生成されたポロニウムは、7日の半減期で血漿2に
# > 移動するものと仮定されます。皮質骨または海綿骨の体積コンパートメントで生成された
# > ポロニウムは、その骨の種類に応じた基準代謝速度で血漿2に移行するものと仮定されます。

  {*}/Plasma              Plasma2                1000
  {Po}/Plasma1            Plasma1                   ---
  {Po}/Plasma2            Plasma2                   ---
  {Po}/Plasma3            Plasma3                   ---
  {*}/RBC                 RBC                       ---
  {Pu}/Blood0             Plasma2                1000
  {Pu}/Blood1             Plasma2                1000
  {Pu}/Blood2             Plasma2                1000
  {*}/Blood               Plasma2                1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/C-bone-V            Plasma2               $(0.03 / 365)
  {*}/T-bone-V            Plasma2               $(0.18 / 365)
  {*}/Noch-C-bone-V       Plasma2               $(0.03 / 365)
  {*}/Noch-T-bone-V       Plasma2               $(0.18 / 365)
  {*}/Exch-C-bone-V       Plasma2               $(0.03 / 365)
  {*}/Exch-T-bone-V       Plasma2               $(0.18 / 365)

  {*}/C-marrow            C-marrow                  ---
  {*}/T-marrow            T-marrow                  ---
  {Pu}/Liver0             Plasma2               $(0.693 / 7)    # 半減期7日
  {Pu}/Liver1             Plasma2               $(0.693 / 7)    # 半減期7日
  {Pu}/Liver2             Plasma2               $(0.693 / 7)    # 半減期7日
  {^Pu}/Liver1            Liver1                    ---
  {^Pu}/Liver2            Liver2                    ---
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {*}/ST0                 Plasma2               $(0.693 / 7)    # 半減期7日
  {*}/ST1                 Plasma2               $(0.693 / 7)    # 半減期7日
  {*}/ST2                 Plasma2               $(0.693 / 7)    # 半減期7日
  {Po}/Other              Other                     ---
  {^Po}/Other             Plasma2               $(0.693 / 7)    # 半減期7日
